using System.Numerics;

using CheapLoc;

namespace PriceCheck
{
    /// <summary>
    /// Price check result.
    /// </summary>
    public class Result
    {
        /// <summary>
        /// Success.
        /// </summary>
        public static Result? Success;

        /// <summary>
        /// Failed to get data from universalis.
        /// </summary>
        public static Result? FailedToGetData;

        /// <summary>
        /// No data is available on universalis.
        /// </summary>
        public static Result? NoDataAvailable;

        /// <summary>
        /// No recent data is available on universalis.
        /// </summary>
        public static Result? NoRecentDataAvailable;

        /// <summary>
        /// Price is below vendor price.
        /// </summary>
        public static Result? BelowVendor;

        /// <summary>
        /// Price is below minimum threshold.
        /// </summary>
        public static Result? BelowMinimum;

        /// <summary>
        /// Item can't be sold on market board.
        /// </summary>
        public static Result? Unmarketable;

        private readonly ushort chatColor;
        private readonly string description;
        private readonly Vector4 overlayColor;

        private Result(string resultDesc, ushort chatColor, Vector4 overlayColor)
        {
            this.description = resultDesc;
            this.chatColor = chatColor;
            this.overlayColor = overlayColor;
        }

        /// <summary>
        /// Update language.
        /// </summary>
        public static void UpdateLanguage()
        {
            Success = new Result(Loc.Localize("SellOnMarketboard", "Sell on marketboard"), 45, new Vector4(0f, .8f, .133f, 1));
            FailedToGetData = new Result(Loc.Localize("FailedToGetData", "Failed to get data - universalis may be down"), 17, new Vector4(.863f, 0, 0, 1));
            NoDataAvailable = new Result(Loc.Localize("NoDataAvailable", "No data available"), 17, new Vector4(.863f, 0, 0, 1));
            NoRecentDataAvailable = new Result(Loc.Localize("NoRecentDataAvailable", "No recent data"), 17, new Vector4(.863f, 0, 0, 1));
            BelowVendor = new Result(Loc.Localize("BelowVendor", "Sell to vendor"), 25, new Vector4(1f, 1f, .4f, 1f));
            BelowMinimum = new Result(Loc.Localize("BelowMinimum", "Below minimum price"), 25, new Vector4(1f, 1f, .4f, 1f));
            Unmarketable = new Result(Loc.Localize("Unmarketable", "Can't sell on marketboard"), 25, new Vector4(1f, 1f, .4f, 1f));
        }

        /// <summary>
        /// Result description.
        /// </summary>
        /// <returns>description.</returns>
        public override string ToString()
        {
            return this.description;
        }

        /// <summary>
        /// Chat color.
        /// </summary>
        /// <returns>chat color.</returns>
        public ushort ChatColor()
        {
            return this.chatColor;
        }

        /// <summary>
        /// Overlay color.
        /// </summary>
        /// <returns>overlay color.</returns>
        public Vector4 OverlayColor()
        {
            return this.overlayColor;
        }
    }
}
