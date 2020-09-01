namespace PriceCheck
{
	public interface IUniversalisClient
	{
		MarketBoardData GetMarketBoard(uint? worldId, ulong itemId);
		void Dispose();
	}
}