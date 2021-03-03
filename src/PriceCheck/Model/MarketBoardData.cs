namespace PriceCheck
{
    public class MarketBoardData
    {
        public long LastCheckTime { get; set; }
        public long LastUploadTime { get; set; }
        public double? AveragePriceNQ { get; set; }
        public double? AveragePriceHQ { get; set; }
        public double? CurrentAveragePriceNQ { get; set; }
        public double? CurrentAveragePriceHQ { get; set; }
        public double? MinimumPriceNQ { get; set; }
        public double? MinimumPriceHQ { get; set; }
        public double? MaximumPriceNQ { get; set; }
        public double? MaximumPriceHQ { get; set; }
    }
}