namespace PriceCheck
{
	public abstract class PriceCheckConfig
	{
		public int RequestTimeout { get; set; } = 5000;
		public bool FreshInstall { get; set; } = true;

		// General
		public bool Enabled { get; set; } = true;
		public bool ShowPrices { get; set; } = true;
		public int HoverDelay { get; set; } = 1;
		public int PluginLanguage { get; set; } = 0;

		// Overlay
		public bool ShowOverlay { get; set; } = true;
		public int MaxItemsInOverlay { get; set; } = 10;

		// Chat
		public bool ShowInChat { get; set; } = true;
		public bool UseChatColors { get; set; } = true;
		public bool UseItemLinks { get; set; } = true;

		// Keybind
		public bool KeybindEnabled { get; set; } = true;
		public ModifierKey.Enum ModifierKey { get; set; } = PriceCheck.ModifierKey.Enum.VkShift;
		public PrimaryKey.Enum PrimaryKey { get; set; } = PriceCheck.PrimaryKey.Enum.VkZ;

		// Thresholds
		public int MinPrice { get; set; } = 1000;
		public int MaxUploadDays { get; set; } = 60;
	}
}