using System;

namespace PriceCheck.Mock
{
	public class MockUniversalis : IUniversalisClient
	{
		public MarketBoardData GetMarketBoard(uint? worldId, ulong itemId)
		{
			if (itemId == 1)
				return new MarketBoardData
				{
					AveragePriceHQ = 300, AveragePriceNQ = 200, LastUploadTime = 1599272449630
				};
			if (itemId == 2)
				return new MarketBoardData
				{
					AveragePriceHQ = 300, AveragePriceNQ = 200, LastUploadTime = 1599272449630
				};
			if (itemId == 3)
				return new MarketBoardData
				{
					LastUploadTime = 1568300583687, AveragePriceHQ = 200, AveragePriceNQ = 150
				};
			if (itemId == 4)
				return new MarketBoardData
				{
					LastUploadTime = 0
				};
			return null;
		}

		public void Dispose()
		{
			throw new NotImplementedException();
		}
	}
}