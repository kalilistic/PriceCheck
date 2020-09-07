namespace PriceCheck
{
	public class Result
	{
		public static Result Success = new Result("Sell on marketboard");
		public static Result FailedToGetData = new Result("Failed to get data");
		public static Result NoDataAvailable = new Result("No data available");
		public static Result NoRecentDataAvailable = new Result("No recent data");
		public static Result BelowVendor = new Result("Sell to vendor");
		public static Result BelowMinimum = new Result("Below minimum price");
		private readonly string _typeKeyWord;

		private Result(string typeKeyWord)
		{
			_typeKeyWord = typeKeyWord;
		}

		public override string ToString()
		{
			return _typeKeyWord;
		}
	}
}