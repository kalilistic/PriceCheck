using System;

namespace PriceCheck
{
	public class PricedItem
	{
		public ulong RawItemId { get; set; }
		public uint ItemId { get; set; }
		public bool IsHQ { get; set; }
		public string ItemName { get; set; }
		public uint VendorPrice { get; set; }
		public long? LastUpdated { get; set; }
		public uint AveragePrice { get; set; }
		public string Result { get; set; }

		public void SetAveragePrice(double? averagePrice)
		{
			if (averagePrice == null)
				AveragePrice = 0;
			else
				AveragePrice = (uint) Math.Round((double) averagePrice);
		}
	}
}