using System.Collections.Generic;
using System.Linq;

namespace PriceCheck
{
    public class PriceMode
    {
        public static readonly List<PriceMode> PriceModes = new List<PriceMode>();
        public static readonly List<string> PriceModeNames = new List<string>();

        public static readonly PriceMode HistoricalAverage = new PriceMode(0, "Historical Average");
        public static readonly PriceMode CurrentAverage = new PriceMode(1, "Current Average");

        public PriceMode()
        {
        }

        private PriceMode(int index, string name)
        {
            Index = index;
            Name = name;
            PriceModes.Add(this);
            PriceModeNames.Add(name);
        }

        public int Index { get; set; }
        public string Name { get; set; }

        public static PriceMode GetPriceModeByIndex(int index)
        {
            return PriceModes.FirstOrDefault(priceMode => priceMode.Index == index);
        }

        public override string ToString()
        {
            return Name;
        }
    }
}