using DalamudPluginCommon;

namespace PriceCheck
{
    /// <summary>
    /// Sample plugin configuration.
    /// </summary>
    public abstract class PriceCheckConfig
    {
        /// <summary>
        /// Gets or sets the request timeout to universalis.
        /// </summary>
        public int RequestTimeout { get; set; } = 5000;

        /// <summary>
        /// Gets or sets a value indicating whether this is a fresh install of the plugin.
        /// </summary>
        public bool FreshInstall { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether plugin is enabled.
        /// </summary>
        public bool Enabled { get; set; } = true;

        /// <summary>
        /// Gets or sets the price mode to use for calculation.
        /// </summary>
        public int PriceMode { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to show prices.
        /// </summary>
        public bool ShowPrices { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether to show unmarketable items.
        /// </summary>
        public bool ShowUnmarketable { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to prevent price checks in combat.
        /// </summary>
        public bool RestrictInCombat { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to prevent price checks in content.
        /// </summary>
        public bool RestrictInContent { get; set; }

        /// <summary>
        /// Gets or sets a value for the hover delay before price check is triggered.
        /// </summary>
        public int HoverDelay { get; set; } = 1;

        /// <summary>
        /// Gets or sets a value indicating whether to show overlay.
        /// </summary>
        public bool ShowOverlay { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether to use overlay colors.
        /// </summary>
        public bool UseOverlayColors { get; set; } = true;

        /// <summary>
        /// Gets or sets the max number of items to show in overlay.
        /// </summary>
        public int MaxItemsInOverlay { get; set; } = 10;

        /// <summary>
        /// Gets or sets a value indicating whether to show price checks in toasts.
        /// </summary>
        public bool ShowToast { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether to show price checks in chat.
        /// </summary>
        public bool ShowInChat { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether to use colors in chat.
        /// </summary>
        public bool UseChatColors { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether to use item links in chat.
        /// </summary>
        public bool UseItemLinks { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether to require key bind to trigger price check.
        /// </summary>
        public bool KeybindEnabled { get; set; } = true;

        /// <summary>
        /// Gets or sets the modifier key for key bind.
        /// </summary>
        public ModifierKey.Enum ModifierKey { get; set; } = DalamudPluginCommon.ModifierKey.Enum.VkShift;

        /// <summary>
        /// Gets or sets the primary key for key bind.
        /// </summary>
        public PrimaryKey.Enum PrimaryKey { get; set; } = DalamudPluginCommon.PrimaryKey.Enum.VkZ;

        /// <summary>
        /// Gets or sets the minimum price to show.
        /// </summary>
        public int MinPrice { get; set; } = 1000;

        /// <summary>
        /// Gets or sets the maximum number of upload days.
        /// </summary>
        public int MaxUploadDays { get; set; } = 60;
    }
}
