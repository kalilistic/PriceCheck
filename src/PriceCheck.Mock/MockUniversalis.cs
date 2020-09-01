using System;

namespace PriceCheck.Mock
{
	public class MockUniversalis : IUniversalisClient
	{
		public MarketBoardData GetMarketBoard(uint? worldId, ulong itemId)
		{
			return new MarketBoardData();
		}

		public void Dispose()
		{
			throw new NotImplementedException();
		}
	}
}