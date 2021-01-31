using System;
using System.Collections.Generic;

namespace PriceCheck.Mock
{
	public class MockPriceService : IPriceService
	{
		public List<PricedItem> GetItems()
		{
			return new List<PricedItem>
			{
				new PricedItem
				{
					ItemName = "Potato",
					MarketPrice = 1000,
					Message = "1000"
				},
				new PricedItem
				{
					ItemName = "Carrot",
					MarketPrice = 200,
					Message = "200"
				},
				new PricedItem
				{
					ItemName = "Orange",
					MarketPrice = 900,
					Message = "900"
				}
			};
		}

		public void Dispose()
		{
			throw new NotImplementedException();
		}
	}
}