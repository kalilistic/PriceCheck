using System.Collections.Generic;
using System.Linq;

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
        // ReSharper disable once CollectionNeverQueried.Global
        public static readonly List<string> PriceModeNames = new ();

        /// <summary>
        /// Price mode: historical average.
        /// </summary>
        public static readonly PriceMode HistoricalAverage = new PriceMode(0, "Historical Average");

        /// <summary>
        /// Price mode: current average.
        /// </summary>
        public static readonly PriceMode CurrentAverage = new PriceMode(1, "Current Average");

        /// <summary>
        /// Price mode: minimum price.
        /// </summary>
        public static readonly PriceMode MinimumPrice = new PriceMode(2, "Historical Minimum");

        /// <summary>
        /// Price mode: maximum price.
        /// </summary>
        public static readonly PriceMode MaximumPrice = new PriceMode(3, "Historical Maximum");

        /// <summary>
        /// Initializes a new instance of the <see cref="PriceMode"/> class.
        /// </summary>
        public PriceMode()
        {
        }

        private PriceMode(int index, string name)
        {
            this.Index = index;
            this.Name = name;
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
