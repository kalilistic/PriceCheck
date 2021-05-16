namespace PriceCheck
{
    /// <summary>
    /// Universalis client.
    /// </summary>
    public interface IUniversalisClient
    {
        /// <summary>
        /// Get market board data.
        /// </summary>
        /// <param name="worldId">world id.</param>
        /// <param name="itemId">item id.</param>
        /// <returns>market board data.</returns>
        MarketBoardData? GetMarketBoard(uint worldId, ulong itemId);

        /// <summary>
        /// Dispose client.
        /// </summary>
        void Dispose();
    }
}
