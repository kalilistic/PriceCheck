namespace PriceCheck
{
    /// <summary>
    /// Market board data for a given item.
    /// </summary>
    public class MarketBoardData
    {
        /// <summary>
        /// Gets or sets last time price was checked.
        /// </summary>
        public long LastCheckTime { get; set; }

        /// <summary>
        /// Gets or sets last time price was checked.
        /// </summary>
        public long LastUploadTime { get; set; }

        /// <summary>
        /// Gets or sets the average price (NQ).
        /// </summary>
        public double? AveragePriceNQ { get; set; }

        /// <summary>
        /// Gets or sets the average price (HQ).
        /// </summary>
        public double? AveragePriceHQ { get; set; }

        /// <summary>
        /// Gets or sets the current average price (NQ).
        /// </summary>
        public double? CurrentAveragePriceNQ { get; set; }

        /// <summary>
        /// Gets or sets the current average price (HQ).
        /// </summary>
        public double? CurrentAveragePriceHQ { get; set; }

        /// <summary>
        /// Gets or sets the minimum price (NQ).
        /// </summary>
        public double? MinimumPriceNQ { get; set; }

        /// <summary>
        /// Gets or sets the minimum price (HQ).
        /// </summary>
        public double? MinimumPriceHQ { get; set; }

        /// <summary>
        /// Gets or sets the maximum price (NQ).
        /// </summary>
        public double? MaximumPriceNQ { get; set; }

        /// <summary>
        /// Gets or sets the maximum price (HQ).
        /// </summary>
        public double? MaximumPriceHQ { get; set; }

        /// <summary>
        /// Gets or sets the current minimum price.
        /// </summary>
        public double? CurrentMinimumPrice { get; set; }
    }
}
