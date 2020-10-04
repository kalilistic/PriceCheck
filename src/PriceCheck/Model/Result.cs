using System.Numerics;
using CheapLoc;

namespace PriceCheck
{
	public class Result
	{
		public static Result Success;
		public static Result FailedToGetData;
		public static Result NoDataAvailable;
		public static Result NoRecentDataAvailable;
		public static Result BelowVendor;
		public static Result BelowMinimum;
		private readonly ushort _chatColor;
		private readonly string _description;
		private readonly Vector4 _overlayColor;

		private Result(string resultDesc, ushort chatColor, Vector4 overlayColor)
		{
			_description = resultDesc;
			_chatColor = chatColor;
			_overlayColor = overlayColor;
		}

		public override string ToString()
		{
			return _description;
		}

		public ushort ChatColor()
		{
			return _chatColor;
		}

		public Vector4 OverlayColor()
		{
			return _overlayColor;
		}

		public static void UpdateLanguage()
		{
			Success = new Result(Loc.Localize("SellOnMarketboard", "Sell on marketboard"), 45,
				new Vector4(0f, .8f, .133f, 1));
			FailedToGetData = new Result(Loc.Localize("FailedToGetData", "Failed to get data"), 17,
				new Vector4(.863f, 0, 0, 1));
			NoDataAvailable = new Result(Loc.Localize("NoDataAvailable", "No data available"), 17,
				new Vector4(.863f, 0, 0, 1));
			NoRecentDataAvailable = new Result(Loc.Localize("NoRecentDataAvailable", "No recent data"), 17,
				new Vector4(.863f, 0, 0, 1));
			BelowVendor = new Result(Loc.Localize("BelowVendor", "Sell to vendor"), 25, new Vector4(1f, 1f, .4f, 1f));
			BelowMinimum = new Result(Loc.Localize("BelowMinimum", "Below minimum price"), 25,
				new Vector4(1f, 1f, .4f, 1f));
		}
	}
}