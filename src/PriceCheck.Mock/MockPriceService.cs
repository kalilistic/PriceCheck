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
					AveragePrice = 1000,
					Result = "1000"
				},
				new PricedItem
				{
					ItemName = "Carrot",
					AveragePrice = 200,
					Result = "200"
				},
				new PricedItem
				{
					ItemName = "Orange",
					AveragePrice = 900,
					Result = "900"
				}
			};
		}
	}
}