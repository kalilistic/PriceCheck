// ReSharper disable DelegateSubtraction

using System;
using System.Collections.Generic;
using System.Globalization;

// ReSharper disable ConvertIfStatementToReturnStatement

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

		internal PricedItem BuildPricedItemFromId(ulong itemId)
		{
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

			return pricedItem;
		}

		internal bool EnrichWithExcelData(PricedItem pricedItem)
		{
			var excelItem = _plugin.GetItems().Find(item => item.RowId == pricedItem.ItemId);
			if (excelItem == null) return true;
			pricedItem.ItemName = excelItem.Name;
			pricedItem.VendorPrice = excelItem.PriceLow;
			return false;
		}

		internal bool EnrichWithMarketBoardData(PricedItem pricedItem)
		{
			var marketBoard = _universalisClient.GetMarketBoard(_plugin.GetLocalPlayerHomeWorld(), pricedItem.ItemId);
			if (marketBoard == null)
			{
				pricedItem.Result = Result.FailedToGetData;
				return true;
			}

			pricedItem.LastUpdated = marketBoard.LastUploadTime;
			var averagePrice = pricedItem.IsHQ ? marketBoard.AveragePriceHQ : marketBoard.AveragePriceNQ;
			if (averagePrice == null)
				averagePrice = 0;
			else
				averagePrice = Math.Round((double) averagePrice);
			pricedItem.AveragePrice = (uint) averagePrice;
			return false;
		}

		internal bool ValidateMarketBoardData(PricedItem pricedItem)
		{
			if (pricedItem.LastUpdated != 0 && pricedItem.AveragePrice != 0) return false;
			pricedItem.Result = Result.NoDataAvailable;
			return true;
		}

		internal bool EvaluateDataAge(PricedItem pricedItem)
		{
			var currentTime = (long) (DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalMilliseconds;
			var diffInSeconds = currentTime - pricedItem.LastUpdated;
			var diffInDays = diffInSeconds / 86400000;
			if (!(diffInDays > _plugin.GetConfig().MaxUploadDays)) return false;
			pricedItem.Result = Result.NoRecentDataAvailable;
			return true;
		}

		internal bool CompareVendorPrice(PricedItem pricedItem)
		{
			if (pricedItem.VendorPrice < pricedItem.AveragePrice) return false;
			pricedItem.Result = Result.BelowVendor;
			return true;
		}

		internal bool CompareMinPrice(PricedItem pricedItem)
		{
			if (pricedItem.AveragePrice >= _plugin.GetConfig().MinPrice) return false;
			pricedItem.Result = Result.BelowMinimum;
			return true;
		}

		internal void RemoveExistingRecord(PricedItem pricedItem)
		{
			for (var i = 0; i < _pricedItems.Count; i++)
			{
				if (_pricedItems[i].ItemId != pricedItem.ItemId) continue;
				_pricedItems.RemoveAt(i);
				break;
			}
		}

		internal void RemoveItemsOverMax()
		{
			while (_pricedItems.Count >= _plugin.GetConfig().MaxItemsInOverlay)
				_pricedItems.RemoveAt(_pricedItems.Count - 1);
		}

		internal void AddItemToList(PricedItem pricedItem)
		{
			_pricedItems.Insert(0, pricedItem);
		}

		internal void SendEcho(PricedItem pricedItem)
		{
			if (_plugin.GetConfig().ShowInChat) _plugin.SendEcho(pricedItem);
		}

		internal void SetDisplayName(PricedItem pricedItem)
		{
			if (pricedItem.IsHQ)
				pricedItem.DisplayName = pricedItem.ItemName + " " + _plugin.GetHQIcon();
			else
				pricedItem.DisplayName = pricedItem.ItemName;
		}

		internal void SetMessage(PricedItem pricedItem)
		{
			if (pricedItem.Result == null)
			{
				pricedItem.Result = Result.Success;
				pricedItem.Message = _plugin.GetConfig().ShowPrices
					? pricedItem.AveragePrice.ToString("N0", CultureInfo.InvariantCulture)
					: pricedItem.Result.ToString();
			}
			else
			{
				pricedItem.Message = pricedItem.Result.ToString();
			}
		}

		internal bool ProcessItem(PricedItem pricedItem)
		{
			var failedToGetMarketBoardData = EnrichWithMarketBoardData(pricedItem);
			if (failedToGetMarketBoardData) return true;
			var invalidMarketBoardData = ValidateMarketBoardData(pricedItem);
			if (invalidMarketBoardData) return true;
			var isOldData = EvaluateDataAge(pricedItem);
			if (isOldData) return true;
			var hasHigherPriceOnVendor = CompareVendorPrice(pricedItem);
			if (hasHigherPriceOnVendor) return true;
			var belowMinPrice = CompareMinPrice(pricedItem);
			if (belowMinPrice) return true;
			return false;
		}

		internal void ProcessItem(object sender, ulong itemId)
		{
			var pricedItem = BuildPricedItemFromId(itemId);
			var failedToEnrichItem = EnrichWithExcelData(pricedItem);
			if (failedToEnrichItem) return;
			ProcessItem(pricedItem);
			SetDisplayName(pricedItem);
			SetMessage(pricedItem);
			RemoveExistingRecord(pricedItem);
			RemoveItemsOverMax();
			AddItemToList(pricedItem);
			SendEcho(pricedItem);
		}

		public void Dispose()
		{
			_plugin.ItemDetected -= ProcessItem;
		}
	}
}