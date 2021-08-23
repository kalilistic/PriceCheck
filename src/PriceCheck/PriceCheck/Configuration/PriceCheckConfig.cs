using System.Collections.Generic;

using Dalamud.DrunkenToad;
using Dalamud.Game.Text;

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
        public int RequestTimeout { get; set; } = 10000;

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
        /// Gets or sets a value indicating whether to allow keybind trigger after item hover.
        /// </summary>
        public bool AllowKeybindAfterHover { get; set; } = true;

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
        /// Gets or sets a value indicating whether to show overlay on login.
        /// </summary>
        public bool ShowOverlayOnLogin { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether to show overlay on login.
        /// </summary>
        public bool ShowOverlayByKeybind { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to show keybind in title bar.
        /// </summary>
        public bool ShowKeybindInTitleBar { get; set; }

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
        public bool ShowToast { get; set; }

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
        public ModifierKey.Enum ModifierKey { get; set; } = Dalamud.DrunkenToad.ModifierKey.Enum.VkShift;

        /// <summary>
        /// Gets or sets the primary key for key bind.
        /// </summary>
        public PrimaryKey.Enum PrimaryKey { get; set; } = Dalamud.DrunkenToad.PrimaryKey.Enum.VkZ;

        /// <summary>
        /// Gets or sets the minimum price to show.
        /// </summary>
        public int MinPrice { get; set; } = 1000;

        /// <summary>
        /// Gets or sets the maximum number of upload days.
        /// </summary>
        public int MaxUploadDays { get; set; } = 60;

        /// <summary>
        /// Gets or sets the number of milliseconds to hide overlay after use.
        /// </summary>
        public int HideOverlayElapsed { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether context menu items should be enabled.
        /// </summary>
        public bool ShowContextMenu { get; set; } = true;

        /// <summary>
        /// Gets or sets list of items to show above.
        /// </summary>
        public List<byte> ShowContextAboveThis { get; set; } = new ();

        /// <summary>
        /// Gets or sets list of items to show below.
        /// </summary>
        public List<byte> ShowContextBelowThis { get; set; } = new ();

        /// <summary>
        /// Gets or sets chat channel to use for price check messages.
        /// </summary>
        public XivChatType ChatChannel { get; set; } = XivChatType.None;

        /// <summary>
        /// Gets or sets a value indicating whether to show Success in overlay.
        /// </summary>
        public bool ShowSuccessInOverlay { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether to show FailedToProcess in overlay.
        /// </summary>
        public bool ShowFailedToProcessInOverlay { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether to show FailedToGetData in overlay.
        /// </summary>
        public bool ShowFailedToGetDataInOverlay { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether to show NoDataAvailable in overlay.
        /// </summary>
        public bool ShowNoDataAvailableInOverlay { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether to show RecentDataAvailable in overlay.
        /// </summary>
        public bool ShowNoRecentDataAvailableInOverlay { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether to show BelowVendor in overlay.
        /// </summary>
        public bool ShowBelowVendorInOverlay { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether to show BelowMinimum in overlay.
        /// </summary>
        public bool ShowBelowMinimumInOverlay { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether to show Unmarketable in overlay.
        /// </summary>
        public bool ShowUnmarketableInOverlay { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to show Success in chat.
        /// </summary>
        public bool ShowSuccessInChat { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether to show FailedToProcess in chat.
        /// </summary>
        public bool ShowFailedToProcessInChat { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether to show FailedToGetData in chat.
        /// </summary>
        public bool ShowFailedToGetDataInChat { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether to show NoDataAvailable in chat.
        /// </summary>
        public bool ShowNoDataAvailableInChat { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether to show RecentDataAvailable in chat.
        /// </summary>
        public bool ShowNoRecentDataAvailableInChat { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether to show BelowVendor in chat.
        /// </summary>
        public bool ShowBelowVendorInChat { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether to show BelowMinimum in chat.
        /// </summary>
        public bool ShowBelowMinimumInChat { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether to show Unmarketable in chat.
        /// </summary>
        public bool ShowUnmarketableInChat { get; set; }
    }
}
