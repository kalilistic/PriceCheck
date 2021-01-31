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
	}
}