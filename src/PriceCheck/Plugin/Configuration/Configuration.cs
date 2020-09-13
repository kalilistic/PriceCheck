namespace PriceCheck
{
	public abstract class Configuration
	{
		public int Version { get; set; } = 0;
		public int RequestTimeout { get; set; } = 5000;
		public bool ShowMain { get; set; } = true;

		// General
		public bool Enabled { get; set; } = true;
		public bool ShowOverlay { get; set; } = true;
		public bool ShowInChat { get; set; } = true;
		public bool ShowPrices { get; set; } = true;
		public int PluginLanguage { get; set; } = 0;

		// Keybind
		public bool KeybindEnabled { get; set; } = true;
		public ModifierKey.Enum ModifierKey { get; set; } = PriceCheck.ModifierKey.Enum.VkShift;
		public PrimaryKey.Enum PrimaryKey { get; set; } = PriceCheck.PrimaryKey.Enum.VkZ;

		// Thresholds
		public int MinPrice { get; set; } = 1000;
		public int MaxUploadDays { get; set; } = 60;
		public int MaxItemsInOverlay { get; set; } = 10;

		public abstract void Save();
	}
}