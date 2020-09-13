namespace PriceCheck
{
	public class Result
	{
		public static Result Success = new Result("Sell on marketboard", 45);
		public static Result FailedToGetData = new Result("Failed to get data", 17);
		public static Result NoDataAvailable = new Result("No data available", 17);
		public static Result NoRecentDataAvailable = new Result("No recent data", 17);
		public static Result BelowVendor = new Result("Sell to vendor", 25);
		public static Result BelowMinimum = new Result("Below minimum price", 25);
		private readonly ushort _colorKey;
		private readonly string _description;

		private Result(string resultDesc, ushort colorKey)
		{
			_description = resultDesc;
			_colorKey = colorKey;
		}

		public override string ToString()
		{
			return _description;
		}

		public ushort ColorKey()
		{
			return _colorKey;
		}
	}
}