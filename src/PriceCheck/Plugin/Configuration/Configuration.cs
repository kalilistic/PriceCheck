namespace PriceCheck
{
	public abstract class Configuration
	{
		public int Version { get; set; } = 0;
		public bool Enabled { get; set; } = true;
		public bool ShowOverlay { get; set; } = true;
		public bool ShowInChat { get; set; } = true;
		public int MinPrice { get; set; } = 1000;
		public int MaxUploadDays { get; set; } = 60;
		public int MaxItemsInOverlay { get; set; } = 10;
		public int RequestTimeout { get; set; } = 5000;
		public ModifierKey.Enum ModifierKey { get; set; } = PriceCheck.ModifierKey.Enum.VkShift;
		public PrimaryKey.Enum PrimaryKey { get; set; } = PriceCheck.PrimaryKey.Enum.VkZ;

		public abstract void Save();
	}
}