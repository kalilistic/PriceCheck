namespace PriceCheck
{
    public class PricedItem
    {
        public ulong RawItemId { get; set; }
        public uint ItemId { get; set; }
        public bool IsHQ { get; set; }
        public bool IsMarketable { get; set; }
        public string ItemName { get; set; }
        public uint VendorPrice { get; set; }
        public long? LastUpdated { get; set; }
        public uint MarketPrice { get; set; }
        public string DisplayName { get; set; }
        public string Message { get; set; }
        public Result Result { get; set; }
    }
}