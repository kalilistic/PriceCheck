namespace PriceCheck
{
    /// <summary>
    /// Item data used by plugin.
    /// </summary>
    public class PricedItem
    {
        /// <summary>
        /// Gets or sets item id parsed from event.
        /// </summary>
        public ulong RawItemId { get; set; }

        /// <summary>
        /// Gets or sets parsed item id.
        /// </summary>
        public uint ItemId { get; set; }

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
        /// Gets or sets a value indicating whether item is marketable.
        /// </summary>
        public long? LastUpdated { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether item is marketable.
        /// </summary>
        public uint MarketPrice { get; set; }

        /// <summary>
        /// Gets or sets the item name for display.
        /// </summary>
        public string DisplayName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the message to show in chat.
        /// </summary>
        public string Message { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the price check result.
        /// </summary>
        public Result? Result { get; set; }
    }
}
