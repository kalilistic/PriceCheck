using System.Collections.Generic;

namespace PriceCheck
{
	public interface IPriceService
	{
		List<PricedItem> GetItems();
		void Dispose();
	}
}