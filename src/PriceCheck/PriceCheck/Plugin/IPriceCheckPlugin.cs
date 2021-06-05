using System;

using Lumina.Excel.GeneratedSheets;

namespace PriceCheck
{
    /// <summary>
    /// PriceCheck plugin.
    /// </summary>
    public interface IPriceCheckPlugin
    {
        /// <summary>
        /// Item detected event handler.
        /// </summary>
        event EventHandler<DetectedItem> OnItemDetected;

        /// <summary>
        /// Gets price service.
        /// </summary>
        IPriceService PriceService { get; }

        /// <summary>
        /// Gets plugin configuration.
        /// </summary>
        PriceCheckConfig Configuration { get; }

        /// <summary>
        /// Gets or sets last price check conducted in unix timestamp.
        /// </summary>
        long LastPriceCheck { get; set; }

        /// <summary>
        /// Send toast.
        /// </summary>
        /// <param name="pricedItem">priced Item.</param>
        void SendToast(PricedItem pricedItem);

        /// <summary>
        /// Print help message.
        /// </summary>
        void PrintHelpMessage();

        /// <summary>
        /// Get player home world id.
        /// </summary>
        /// <returns>home world id.</returns>
        uint GetHomeWorldId();

        /// <summary>
        /// Indicator if logged in.
        /// </summary>
        /// <returns>logged in indicator.</returns>
        bool IsLoggedIn();

        /// <summary>
        /// Find item by id.
        /// </summary>
        /// <param name="itemId">item id.</param>
        /// <returns>item.</returns>
        Item? GetItemById(uint itemId);

        /// <summary>
        /// Print item message.
        /// </summary>
        /// <param name="pricedItem">priced item.</param>
        void PrintItemMessage(PricedItem pricedItem);

        /// <summary>
        /// Save plugin configuration.
        /// </summary>
        void SaveConfig();
    }
}
