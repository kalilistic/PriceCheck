using System.Numerics;

using Dalamud.Interface.Colors;

namespace PriceCheck
{
    /// <summary>
    /// Item with pricing data.
    /// </summary>
    public class PricedItem
    {
        /// <summary>
        /// Gets or sets parsed item id.
        /// </summary>
        public uint ItemId { get; set; }

        /// <summary>
        /// Gets or sets a value indicating price check time.
        /// </summary>
        public long? LastUpdated { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether item is HQ.
        /// </summary>
        public bool IsHQ { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether item is marketable.
        /// </summary>
        public bool IsMarketable { get; set; }

        /// <summary>
        /// Gets or sets the item name.
        /// </summary>
        public string ItemName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the vendor price.
        /// </summary>
        public uint VendorPrice { get; set; }

        /// <summary>
        /// Gets or sets the price check result.
        /// </summary>
        public ItemResult Result { get; set; }

        /// <summary>
        /// Gets or sets a value for market price.
        /// </summary>
        public uint MarketPrice { get; set; }

        /// <summary>
        /// Gets or sets the message to show in chat.
        /// </summary>
        public string Message { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the color for chat messages.
        /// </summary>
        public ushort ChatColor { get; set; } = 0;

        /// <summary>
        /// Gets or sets the color for overlay messages.
        /// </summary>
        public Vector4 OverlayColor { get; set; } = ImGuiColors.DalamudWhite;
    }
}
