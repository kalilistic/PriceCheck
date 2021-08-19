using System;
using System.Collections.Generic;
using System.Globalization;

using Dalamud.DrunkenToad;
using Dalamud.Game.Text;
using Lumina.Excel.GeneratedSheets;

namespace PriceCheck
{
    /// <summary>
    /// Pricing service.
    /// </summary>
    public class PriceService
    {
        private readonly PriceCheckPlugin priceCheckPlugin;
        private readonly List<PricedItem> pricedItems;
        private readonly UniversalisClient universalisClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="PriceService"/> class.
        /// </summary>
        /// <param name="priceCheckPlugin">price check plugin.</param>
        /// <param name="universalisClient">universalis client.</param>
        public PriceService(PriceCheckPlugin priceCheckPlugin, UniversalisClient universalisClient)
        {
            this.priceCheckPlugin = priceCheckPlugin;
            this.pricedItems = new List<PricedItem>();
            this.universalisClient = universalisClient;
            this.priceCheckPlugin.OnItemDetected += this.ProcessItem;
        }

        /// <summary>
        /// Gets or sets last price check conducted in unix timestamp.
        /// </summary>
        public long LastPriceCheck { get; set; }

        /// <summary>
        /// Get priced items.
        /// </summary>
        /// <returns>list of priced items.</returns>
        public IEnumerable<PricedItem> GetItems()
        {
            return this.pricedItems;
        }

        /// <summary>
        /// Dispose service.
        /// </summary>
        public void Dispose()
        {
            this.priceCheckPlugin.OnItemDetected -= this.ProcessItem;
        }

        /// <summary>
        /// Build priced item from item id.
        /// </summary>
        /// <param name="itemId">item id.</param>
        /// <returns>priced item.</returns>
        internal static PricedItem BuildPricedItemFromId(ulong itemId)
        {
            var pricedItem = new PricedItem { RawItemId = itemId };
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

        /// <summary>
        /// Validate market board data from universalis.
        /// </summary>
        /// <param name="pricedItem">priced item.</param>
        /// <returns>indicator if successful.</returns>
        internal static bool ValidateMarketBoardData(PricedItem pricedItem)
        {
            if (pricedItem.LastUpdated != 0 && pricedItem.MarketPrice != 0) return false;
            pricedItem.Result = Result.NoDataAvailable !;
            return true;
        }

        /// <summary>
        /// Compare market price with vendor price.
        /// </summary>
        /// <param name="pricedItem">priced item.</param>
        /// <returns>indicator if successful.</returns>
        internal static bool CompareVendorPrice(PricedItem pricedItem)
        {
            if (pricedItem.VendorPrice < pricedItem.MarketPrice) return false;
            pricedItem.Result = Result.BelowVendor !;
            return true;
        }

        /// <summary>
        /// Remove previous price record.
        /// </summary>
        /// <param name="pricedItem">priced item.</param>
        internal static void SetDisplayName(PricedItem pricedItem)
        {
            if (pricedItem.IsHQ)
            {
                pricedItem.DisplayName =
                    pricedItem.ItemName + " " + (char)SeIconChar.HighQuality;
            }
            else
            {
                pricedItem.DisplayName = pricedItem.ItemName;
            }
        }

        private Item? GetItemById(uint itemId)
        {
            return this.priceCheckPlugin.PluginService.GameData.Item(itemId);
        }

        private bool EnrichWithExcelData(PricedItem pricedItem)
        {
            var excelItem = this.GetItemById(pricedItem.ItemId);
            if (excelItem == null) return true;
            if (excelItem.ItemSearchCategory.Row == 0)
            {
                if (this.priceCheckPlugin.Configuration.ShowUnmarketable)
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

        private bool EnrichWithMarketBoardData(PricedItem pricedItem)
        {
            if (!pricedItem.IsMarketable)
            {
                pricedItem.Result = Result.Unmarketable !;
                return true;
            }

            var worldId = this.priceCheckPlugin.GetHomeWorldId();
            if (worldId == 0)
            {
                pricedItem.Result = Result.FailedToGetData !;
                return true;
            }

            var marketBoard =
                this.universalisClient.GetMarketBoard(worldId, pricedItem.ItemId);
            if (marketBoard == null)
            {
                pricedItem.Result = Result.FailedToGetData !;
                return true;
            }

            pricedItem.LastUpdated = marketBoard.LastUploadTime;

            double? marketPrice = null;
            if (this.priceCheckPlugin.Configuration.PriceMode == PriceMode.HistoricalAverage.Index)
                marketPrice = pricedItem.IsHQ ? marketBoard.AveragePriceHQ : marketBoard.AveragePriceNQ;
            else if (this.priceCheckPlugin.Configuration.PriceMode == PriceMode.CurrentAverage.Index)
                marketPrice = pricedItem.IsHQ ? marketBoard.CurrentAveragePriceHQ : marketBoard.CurrentAveragePriceNQ;
            else if (this.priceCheckPlugin.Configuration.PriceMode == PriceMode.HistoricalMinimumPrice.Index)
                marketPrice = pricedItem.IsHQ ? marketBoard.MinimumPriceHQ : marketBoard.MinimumPriceNQ;
            else if (this.priceCheckPlugin.Configuration.PriceMode == PriceMode.HistoricalMaximumPrice.Index)
                marketPrice = pricedItem.IsHQ ? marketBoard.MaximumPriceHQ : marketBoard.MaximumPriceNQ;
            else if (this.priceCheckPlugin.Configuration.PriceMode == PriceMode.CurrentMinimumPrice.Index)
                marketPrice = marketBoard.CurrentMinimumPrice;
            if (marketPrice == null)
                marketPrice = 0;
            else
                marketPrice = Math.Round((double)marketPrice);
            pricedItem.MarketPrice = (uint)marketPrice;
            return false;
        }

        private bool EvaluateDataAge(PricedItem pricedItem)
        {
            var currentTime = (long)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalMilliseconds;
            var diffInSeconds = currentTime - pricedItem.LastUpdated;
            var diffInDays = diffInSeconds / 86400000;
            if (!(diffInDays > this.priceCheckPlugin.Configuration.MaxUploadDays)) return false;
            pricedItem.Result = Result.NoRecentDataAvailable !;
            return true;
        }

        private void CompareMinPrice(PricedItem pricedItem)
        {
            if (pricedItem.MarketPrice >= this.priceCheckPlugin.Configuration.MinPrice) return;
            pricedItem.Result = Result.BelowMinimum !;
        }

        private void RemoveExistingRecord(PricedItem pricedItem)
        {
            for (var i = 0; i < this.pricedItems.Count; i++)
            {
                if (this.pricedItems[i].ItemId != pricedItem.ItemId) continue;
                this.pricedItems.RemoveAt(i);
                break;
            }
        }

        private void RemoveItemsOverMax()
        {
            while (this.pricedItems.Count >= this.priceCheckPlugin.Configuration.MaxItemsInOverlay)
                this.pricedItems.RemoveAt(this.pricedItems.Count - 1);
        }

        private void ProcessItem(object sender, DetectedItem detectedItem)
        {
            this.priceCheckPlugin.PriceService.LastPriceCheck = DateUtil.CurrentTime();
            PricedItem pricedItem;
            if (detectedItem.IsHQ == null)
            {
                pricedItem = BuildPricedItemFromId(detectedItem.ItemId);
            }
            else
            {
                pricedItem = new PricedItem
                {
                    RawItemId = detectedItem.ItemId,
                    ItemId = Convert.ToUInt32(detectedItem.ItemId),
                    IsHQ = (bool)detectedItem.IsHQ,
                };
            }

            var failedToEnrichItem = this.EnrichWithExcelData(pricedItem);
            if (failedToEnrichItem) return;
            this.ProcessItem(pricedItem);
            SetDisplayName(pricedItem);
            this.SetMessage(pricedItem);
            this.RemoveExistingRecord(pricedItem);
            this.RemoveItemsOverMax();
            this.AddItemToList(pricedItem);
            this.SendChatMessage(pricedItem);
            this.SendToast(pricedItem);
        }

        private void AddItemToList(PricedItem pricedItem)
        {
            this.pricedItems.Insert(0, pricedItem);
        }

        private void SendChatMessage(PricedItem pricedItem)
        {
            if (this.priceCheckPlugin.Configuration.ShowInChat)
                this.priceCheckPlugin.PrintItemMessage(pricedItem);
        }

        private void SendToast(PricedItem pricedItem)
        {
            if (this.priceCheckPlugin.Configuration.ShowToast) this.priceCheckPlugin.SendToast(pricedItem);
        }

        private void SetMessage(PricedItem pricedItem)
        {
            if (pricedItem.Result == null)
            {
                pricedItem.Result = Result.Success !;
                pricedItem.Message = (this.priceCheckPlugin.Configuration.ShowPrices
                                          ? pricedItem.MarketPrice.ToString("N0", CultureInfo.InvariantCulture)
                                          : pricedItem.Result?.ToString()) ?? string.Empty;
            }
            else
            {
                pricedItem.Message = pricedItem.Result.ToString();
            }
        }

        private void ProcessItem(PricedItem pricedItem)
        {
            var failedToGetMarketBoardData = this.EnrichWithMarketBoardData(pricedItem);
            if (failedToGetMarketBoardData) return;
            var invalidMarketBoardData = ValidateMarketBoardData(pricedItem);
            if (invalidMarketBoardData) return;
            var isOldData = this.EvaluateDataAge(pricedItem);
            if (isOldData) return;
            var hasHigherPriceOnVendor = CompareVendorPrice(pricedItem);
            if (hasHigherPriceOnVendor) return;
            this.CompareMinPrice(pricedItem);
        }
    }
}
