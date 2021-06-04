namespace PriceCheck
{
    /// <summary>
    /// Item data used by plugin.
    /// </summary>
    public class DetectedItem
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DetectedItem"/> class.
        /// </summary>
        /// <param name="itemId">ItemId.</param>
        /// <param name="isHQ">isHQ.</param>
        public DetectedItem(ulong itemId, bool? isHQ)
        {
            this.ItemId = itemId;
            this.IsHQ = isHQ;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DetectedItem"/> class.
        /// </summary>
        /// <param name="itemId">ItemId.</param>>
        public DetectedItem(ulong itemId)
        {
            this.ItemId = itemId;
        }

        /// <summary>
        /// Gets or sets parsed item id.
        /// </summary>
        public ulong ItemId { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether item is HQ.
        /// </summary>
        public bool? IsHQ { get; set; }
    }
}
