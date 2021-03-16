// ReSharper disable DelegateSubtraction
// ReSharper disable RedundantJumpStatement
// ReSharper disable ConvertIfStatementToReturnStatement

using System;
using System.Collections.Generic;
using System.Globalization;

namespace PriceCheck
{
    public class PriceService : IPriceService
    {
        private readonly IPriceCheckPlugin _priceCheckPlugin;
        private readonly List<PricedItem> _pricedItems;
        private readonly IUniversalisClient _universalisClient;

        public PriceService(IPriceCheckPlugin priceCheckPlugin, IUniversalisClient universalisClient)
        {
            _priceCheckPlugin = priceCheckPlugin;
            _pricedItems = new List<PricedItem>();
            _universalisClient = universalisClient;
            _priceCheckPlugin.ItemDetected += ProcessItem;
        }

        public List<PricedItem> GetItems()
        {
            return _pricedItems;
        }

        public void Dispose()
        {
            _priceCheckPlugin.ItemDetected -= ProcessItem;
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
            var excelItem = _priceCheckPlugin.GetItemById(pricedItem.ItemId);
            if (excelItem == null) return true;
            if (excelItem.ItemSearchCategory.Row == 0)
            {
                if (_priceCheckPlugin.Configuration.ShowUnmarketable)
                    pricedItem.IsMarketable = false;
                else
                    return true;
            }
            else
            {
                pricedItem.IsMarketable = true;
            }

            pricedItem.ItemName = excelItem.Name;
            pricedItem.VendorPrice = excelItem.PriceLow;
            return false;
        }

        internal bool EnrichWithMarketBoardData(PricedItem pricedItem)
        {
            if (!pricedItem.IsMarketable)
            {
                pricedItem.Result = Result.Unmarketable;
                return true;
            }

            var worldId = _priceCheckPlugin.GetLocalPlayerHomeWorld();
            if (worldId == null)
            {
                pricedItem.Result = Result.FailedToGetData;
                return true;
            }

            var marketBoard =
                _universalisClient.GetMarketBoard(worldId, pricedItem.ItemId);
            if (marketBoard == null)
            {
                pricedItem.Result = Result.FailedToGetData;
                return true;
            }

            pricedItem.LastUpdated = marketBoard.LastUploadTime;

            double? marketPrice = null;
            if (_priceCheckPlugin.Configuration.PriceMode == PriceMode.HistoricalAverage.Index)
                marketPrice = pricedItem.IsHQ ? marketBoard.AveragePriceHQ : marketBoard.AveragePriceNQ;
            else if (_priceCheckPlugin.Configuration.PriceMode == PriceMode.CurrentAverage.Index)
                marketPrice = pricedItem.IsHQ ? marketBoard.CurrentAveragePriceHQ : marketBoard.CurrentAveragePriceNQ;
            else if (_priceCheckPlugin.Configuration.PriceMode == PriceMode.MinimumPrice.Index)
                marketPrice = pricedItem.IsHQ ? marketBoard.MinimumPriceHQ : marketBoard.MinimumPriceNQ;
            else if (_priceCheckPlugin.Configuration.PriceMode == PriceMode.MaximumPrice.Index)
                marketPrice = pricedItem.IsHQ ? marketBoard.MaximumPriceHQ : marketBoard.MaximumPriceNQ;
            if (marketPrice == null)
                marketPrice = 0;
            else
                marketPrice = Math.Round((double) marketPrice);
            pricedItem.MarketPrice = (uint) marketPrice;
            return false;
        }

        internal bool ValidateMarketBoardData(PricedItem pricedItem)
        {
            if (pricedItem.LastUpdated != 0 && pricedItem.MarketPrice != 0) return false;
            pricedItem.Result = Result.NoDataAvailable;
            return true;
        }

        internal bool EvaluateDataAge(PricedItem pricedItem)
        {
            var currentTime = (long) (DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalMilliseconds;
            var diffInSeconds = currentTime - pricedItem.LastUpdated;
            var diffInDays = diffInSeconds / 86400000;
            if (!(diffInDays > _priceCheckPlugin.Configuration.MaxUploadDays)) return false;
            pricedItem.Result = Result.NoRecentDataAvailable;
            return true;
        }

        internal bool CompareVendorPrice(PricedItem pricedItem)
        {
            if (pricedItem.VendorPrice < pricedItem.MarketPrice) return false;
            pricedItem.Result = Result.BelowVendor;
            return true;
        }

        internal bool CompareMinPrice(PricedItem pricedItem)
        {
            if (pricedItem.MarketPrice >= _priceCheckPlugin.Configuration.MinPrice)
                return false;
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
            while (_pricedItems.Count >= _priceCheckPlugin.Configuration.MaxItemsInOverlay)
                _pricedItems.RemoveAt(_pricedItems.Count - 1);
        }

        private void AddItemToList(PricedItem pricedItem)
        {
            _pricedItems.Insert(0, pricedItem);
        }

        private void SendEcho(PricedItem pricedItem)
        {
            if (_priceCheckPlugin.Configuration.ShowInChat)
                _priceCheckPlugin.PrintItemMessage(pricedItem);
        }

        internal void SetDisplayName(PricedItem pricedItem)
        {
            if (pricedItem.IsHQ)
                pricedItem.DisplayName =
                    pricedItem.ItemName + " " + _priceCheckPlugin.GetSeIcon(SeIconChar.HighQuality);
            else
                pricedItem.DisplayName = pricedItem.ItemName;
        }

        private void SetMessage(PricedItem pricedItem)
        {
            if (pricedItem.Result == null)
            {
                pricedItem.Result = Result.Success;
                pricedItem.Message = _priceCheckPlugin.Configuration.ShowPrices
                    ? pricedItem.MarketPrice.ToString("N0", CultureInfo.InvariantCulture)
                    : pricedItem.Result.ToString();
            }
            else
            {
                pricedItem.Message = pricedItem.Result.ToString();
            }
        }

        private void ProcessItem(PricedItem pricedItem)
        {
            var failedToGetMarketBoardData = EnrichWithMarketBoardData(pricedItem);
            if (failedToGetMarketBoardData) return;
            var invalidMarketBoardData = ValidateMarketBoardData(pricedItem);
            if (invalidMarketBoardData) return;
            var isOldData = EvaluateDataAge(pricedItem);
            if (isOldData) return;
            var hasHigherPriceOnVendor = CompareVendorPrice(pricedItem);
            if (hasHigherPriceOnVendor) return;
            var belowMinPrice = CompareMinPrice(pricedItem);
            if (belowMinPrice) return;
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
    }
}