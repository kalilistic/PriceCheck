using System.Collections.Generic;

namespace PriceCheck
{
    /// <summary>
    /// Pricing service.
    /// </summary>
    public interface IPriceService
    {
        /// <summary>
        /// Get priced items.
        /// </summary>
        /// <returns>list of priced items.</returns>
        IEnumerable<PricedItem> GetItems();

        /// <summary>
        /// Dispose service.
        /// </summary>
        void Dispose();
    }
}
