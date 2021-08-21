#pragma warning disable 1591
namespace PriceCheck
{
    /// <summary>
    /// Result of conducting price check on item.
    /// </summary>
    public enum ItemResult
    {
        None,
        Success,
        FailedToProcess,
        FailedToGetData,
        NoDataAvailable,
        NoRecentDataAvailable,
        BelowVendor,
        BelowMinimum,
        Unmarketable,
    }
}
