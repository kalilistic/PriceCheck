// ReSharper disable DelegateSubtraction

using System;
using System.Collections.Generic;
using System.Globalization;

namespace PriceCheck
{
	public class PriceService : IPriceService
	{
		private readonly IPluginWrapper _plugin;
		private readonly List<PricedItem> _pricedItems;
		private readonly IUniversalisClient _universalisClient;

		public PriceService(IPluginWrapper plugin, IUniversalisClient universalisClient)
		{
			_plugin = plugin;
			_pricedItems = new List<PricedItem>();
			_universalisClient = universalisClient;
			_plugin.ItemDetected += ProcessItem;
		}

		public List<PricedItem> GetItems()
		{
			return _pricedItems;
		}

		internal void ProcessItem(object sender, ulong itemId)
		{
			// create item from id
			var pricedItem = new PricedItem {RawItemId = itemId};
			if (pricedItem.RawItemId >= 1000000)
			{
				pricedItem.ItemId = Convert.ToUInt32(pricedItem.RawItemId - 1000000);
				pricedItem.IsHQ = true;
			}
			else
			{
				pricedItem.ItemId = Convert.ToUInt32(pricedItem.RawItemId);
				pricedItem.IsHQ = false;
			}

			// enrich with data from excel
			var excelItem = _plugin.GetItems().Find(item => item.RowId == pricedItem.ItemId);
			if (excelItem == null) return;
			var isMarketable = excelItem.ItemSearchCategory.Row != 0;
			if (!isMarketable) return;
			pricedItem.ItemName = excelItem.Name;
			pricedItem.VendorPrice = excelItem.PriceLow;
			if (pricedItem.IsHQ) pricedItem.ItemName += " " + _plugin.GetHQIcon();

			// get marketboard data
			var marketBoard = _universalisClient.GetMarketBoard(_plugin.GetLocalPlayerHomeWorld(), pricedItem.ItemId);
			if (marketBoard != null)
			{
				pricedItem.LastUpdated = marketBoard.LastUploadTime;

				// set average by quality
				pricedItem.SetAveragePrice(pricedItem.IsHQ ? marketBoard.AveragePriceHQ : marketBoard.AveragePriceNQ);

				// check if price is zero
				if (pricedItem.AveragePrice == 0) pricedItem.Result = "No data available";

				// check for old data
				if (pricedItem.Result == null && pricedItem.LastUpdated != null)
				{
					var currentTime = (long) (DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalMilliseconds;
					var diffInSeconds = currentTime - pricedItem.LastUpdated;
					var diffInDays = diffInSeconds / 86400000;
					if (diffInDays > _plugin.GetConfig().MaxUploadDays) pricedItem.Result = "No recent data";
				}

				// check vendor price
				if (pricedItem.Result == null && pricedItem.VendorPrice >= pricedItem.AveragePrice)
					pricedItem.Result = "Sell to vendor";

				// check price min
				if (pricedItem.Result == null && pricedItem.AveragePrice < _plugin.GetConfig().MinPrice)
					pricedItem.Result = "Below minimum price";

				// set result to price otherwise
				if (pricedItem.Result == null)
					pricedItem.Result = pricedItem.AveragePrice.ToString(CultureInfo.InvariantCulture);
			}
			else
			{
				pricedItem.Result = "Failed to get price";
			}

			// remove existing record for this id
			for (var i = 0; i < _pricedItems.Count; i++)
			{
				if (_pricedItems[i].ItemId != pricedItem.ItemId) continue;
				_pricedItems.RemoveAt(i);
				break;
			}

			// remove oldest item over max
			while (_pricedItems.Count >= _plugin.GetConfig().MaxItemsInOverlay)
				_pricedItems.RemoveAt(_pricedItems.Count - 1);

			// add item to list
			_pricedItems.Insert(0, pricedItem);

			// send echo
			if (_plugin.GetConfig().ShowInChat) _plugin.SendEcho("[" + pricedItem.ItemName + "] " + pricedItem.Result);
		}

		public void Dispose()
		{
			_plugin.ItemDetected -= ProcessItem;
		}
	}
}