using System.Collections.Generic;
using System.Linq;

using CheapLoc;

namespace PriceCheck
{
    /// <summary>
    /// PriceMode enum.
    /// </summary>
    public class PriceMode
    {
        /// <summary>
        /// List of price modes.
        /// </summary>
        public static readonly List<PriceMode> PriceModes = new ();

        /// <summary>
        /// List of price mode names.
        /// </summary>
        public static readonly List<string> PriceModeNames = new ();

        /// <summary>
        /// Price mode: historical average.
        /// </summary>
        public static readonly PriceMode AveragePrice = new (
            0,
            Loc.Localize("AveragePrice", "Average Price"),
            Loc.Localize("AveragePriceDesc", "The average sale price in recent sale history with outliers removed beyond three standard deviations of the mean."));

        /// <summary>
        /// Price mode: current average.
        /// </summary>
        public static readonly PriceMode CurrentAveragePrice = new (
        1,
        Loc.Localize("CurrentAveragePrice", "Current Average Price"),
        Loc.Localize("CurrentAveragePriceDesc", "The average sale price in current listings with outliers removed beyond three standard deviations of the mean."));

        /// <summary>
        /// Price mode: minimum price.
        /// </summary>
        public static readonly PriceMode MinimumPrice = new (
            2,
            Loc.Localize("MinimumPrice", "Minimum Price"),
            Loc.Localize("MinimumPriceDesc", "The minimum price in current listings."));

        /// <summary>
        /// Price mode: maximum price.
        /// </summary>
        public static readonly PriceMode MaximumPrice = new (
            3,
            Loc.Localize("MaximumPrice", "Maximum Price"),
            Loc.Localize("MaximumPriceDesc", "The maximum price in current listings."));

        /// <summary>
        /// Price mode: current minimum price.
        /// </summary>
        public static readonly PriceMode CurrentMinimumPrice = new (
            4,
            Loc.Localize("CurrentMinimumPrice", "Current Minimum Price"),
            Loc.Localize("CurrentMinimumPriceDesc", "The minimum price in current listings across both normal and high quality items."));

        /// <summary>
        /// Initializes a new instance of the <see cref="PriceMode"/> class.
        /// </summary>
        public PriceMode()
        {
        }

        private PriceMode(int index, string name, string description)
        {
            this.Index = index;
            this.Name = name;
            this.Description = description;
            PriceModes.Add(this);
            PriceModeNames.Add(name);
        }

        /// <summary>
        /// Gets or sets price mode index.
        /// </summary>
        public int Index { get; set; }

        /// <summary>
        /// Gets or sets price mode name.
        /// </summary>
        public string Name { get; set; } = null!;

        /// <summary>
        /// Gets or sets price mode description.
        /// </summary>
        public string Description { get; set; } = null!;

        /// <summary>
        /// Find price mode by index.
        /// </summary>
        /// <param name="index">price mode index.</param>
        /// <returns>price mode.</returns>
        public static PriceMode? GetPriceModeByIndex(int index)
        {
            return PriceModes.FirstOrDefault(priceMode => priceMode.Index == index);
        }

        /// <summary>
        /// Gets item name.
        /// </summary>
        /// <returns>price mode name.</returns>
        public override string ToString()
        {
            return this.Name;
        }
    }
}
