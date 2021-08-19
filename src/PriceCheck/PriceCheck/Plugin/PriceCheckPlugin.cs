using System;
using System.Collections.Generic;
using System.Threading;

using CheapLoc;
using Dalamud.DrunkenToad;
using Dalamud.Game.Text;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Game.Text.SeStringHandling.Payloads;
using Dalamud.Plugin;
using XivCommon;

namespace PriceCheck
{
    /// <summary>
    /// PriceCheck plugin.
    /// </summary>
    public class PriceCheckPlugin
    {
        /// <summary>
        /// Plugin service.
        /// </summary>
        public readonly PluginService PluginService;

        /// <summary>
        /// XivCommon library instance.
        /// </summary>
        public XivCommonBase XivCommon;

        /// <summary>
        /// Cancellation token to terminate request if interrupted.
        /// </summary>
        public CancellationTokenSource? ItemCancellationTokenSource;

        private readonly DalamudPluginInterface pluginInterface;

        private UniversalisClient universalisClient = null!;

        /// <summary>
        /// Initializes a new instance of the <see cref="PriceCheckPlugin"/> class.
        /// </summary>
        /// <param name="pluginName">plugin name.</param>
        /// <param name="pluginInterface">plugin interface.</param>
        public PriceCheckPlugin(string pluginName, DalamudPluginInterface pluginInterface)
        {
            this.pluginInterface = pluginInterface;
            this.PluginService = new PluginService(pluginName, pluginInterface);
            this.LoadConfig();
            this.LoadServices();
            this.CommandManager = new CommandManager(this);
            this.LoadUI();
            this.HandleFreshInstall();
            Result.UpdateLanguage();
            this.XivCommon = new XivCommonBase(pluginInterface, Hooks.ContextMenu);
            this.ContextMenuManager = new ContextMenuManager(this);
            this.HoveredItemManager = new HoveredItemManager(this);
            this.PluginService.PluginInterface.OnLanguageChanged += OnLanguageChanged;
        }

        /// <summary>
        /// Item detected event handler.
        /// </summary>
        public event EventHandler<DetectedItem> OnItemDetected = null!;

        /// <summary>
        /// Gets or sets command manager to handle user commands.
        /// </summary>
        public CommandManager CommandManager { get; set; }

        /// <summary>
        /// Gets price service.
        /// </summary>
        public PriceService PriceService { get; private set; } = null!;

        /// <summary>
        /// Gets or sets plugin configuration.
        /// </summary>
        public PriceCheckConfig Configuration { get; set; } = null!;

        /// <summary>
        /// Gets or sets window manager.
        /// </summary>
        public WindowManager WindowManager { get; set; } = null!;

        /// <summary>
        /// Gets or sets context Menu manager to handle item context menu.
        /// </summary>
        public ContextMenuManager ContextMenuManager { get; set; }

        /// <summary>
        /// Gets or sets hovered item manager to handle hover item events.
        /// </summary>
        public HoveredItemManager HoveredItemManager { get; set; }

        /// <summary>
        /// Gets plugin configuration.
        /// </summary>
        public void PrintHelpMessage()
        {
            this.PluginService.Chat.PrintNotice(Loc.Localize(
                                                    "HelpMessage",
                                                    "To check prices, hold your keybind and then hover over an item in your inventory or linked " +
                                                    "in chat. You can set your keybind (or disable it) in the PriceCheck settings. The prices are " +
                                                    "averages from Universalis and will not match any specific listings you see on the market board. " +
                                                    "You can use /pcheck to open the overlay and /pcheckconfig to open settings. If you need help, " +
                                                    "reach out on discord or open an issue on GitHub. If you want to help add translations, you can " +
                                                    "submit updates on Crowdin. Links to both GitHub and Crowdin are available in settings."));
        }

        /// <summary>
        /// Get player home world id.
        /// </summary>
        /// <returns>home world id.</returns>
        public uint GetHomeWorldId()
        {
            return this.PluginService.ClientState.LocalPlayer.HomeWorldId();
        }

        /// <summary>
        /// Send toast.
        /// </summary>
        /// <param name="pricedItem">priced Item.</param>
        public void SendToast(PricedItem pricedItem)
        {
            this.PluginService.PluginInterface.Framework.Gui.Toast.ShowNormal(
                $"{pricedItem.DisplayName} {(char)SeIconChar.ArrowRight} {pricedItem.Message}");
        }

        /// <summary>
        /// Print item message.
        /// </summary>
        /// <param name="pricedItem">priced item.</param>
        public void PrintItemMessage(PricedItem pricedItem)
        {
            var payloadList = new List<Payload>
            {
                new UIForegroundPayload(this.PluginService.PluginInterface.Data, 0),
                new TextPayload($"[{this.PluginService.PluginName}] "),
            };
            if (this.Configuration.UseChatColors)
            {
                if (pricedItem.Result != null)
                    payloadList.Add(new UIForegroundPayload(this.PluginService.PluginInterface.Data, pricedItem.Result.ChatColor()));
            }

            if (this.Configuration.UseItemLinks)
            {
                payloadList.Add(new ItemPayload(this.PluginService.PluginInterface.Data, pricedItem.ItemId, pricedItem.IsHQ));
                payloadList.Add(new TextPayload($"{(char)SeIconChar.LinkMarker}"));
                payloadList.Add(new TextPayload(" " + pricedItem.DisplayName));
                payloadList.Add(RawPayload.LinkTerminator);
            }
            else
            {
                payloadList.Add(new TextPayload(pricedItem.DisplayName));
            }

            payloadList.Add(new TextPayload(" " + (char)SeIconChar.ArrowRight + " " + pricedItem.Message));
            if (this.Configuration.UseChatColors)
                payloadList.Add(new UIForegroundPayload(this.PluginService.PluginInterface.Data, 0));
            this.PluginService.Chat.Print(payloadList);
        }

        /// <summary>
        /// Check if keybind is pressed.
        /// </summary>
        /// <returns>indicator if keybind is pressed.</returns>
        public bool IsKeyBindPressed()
        {
            if (!this.Configuration.KeybindEnabled) return true;
            if (this.Configuration.PrimaryKey == PrimaryKey.Enum.VkNone)
            {
                return this.PluginService.ClientState.KeyState.IsKeyPressed(this.Configuration.ModifierKey);
            }

            return this.PluginService.ClientState.KeyState.IsKeyPressed(this.Configuration.ModifierKey) &&
                   this.PluginService.ClientState.KeyState.IsKeyPressed(this.Configuration.PrimaryKey);
        }

        /// <summary>
        /// Dispose plugin.
        /// </summary>
        public void Dispose()
        {
            try
            {
                this.WindowManager.Dispose();
                this.PluginService.Dispose();
                this.CommandManager.Dispose();
                this.ContextMenuManager.Dispose();
                this.HoveredItemManager.Dispose();
                this.ItemCancellationTokenSource?.Dispose();
                this.PriceService.Dispose();
                this.universalisClient.Dispose();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Failed to dispose plugin properly.");
            }

            this.pluginInterface.Dispose();
        }

        /// <summary>
        /// Save plugin configuration.
        /// </summary>
        public void SaveConfig()
        {
            this.PluginService.SaveConfig(this.Configuration);
        }

        /// <summary>
        /// Trigger item detection handling.
        /// </summary>
        /// <param name="item">detected item.</param>
        public void ItemDetected(DetectedItem item)
        {
            this.OnItemDetected.Invoke(this, item);
        }

        /// <summary>
        /// Check state and configuration to determine if price check should be made.
        /// </summary>
        /// <returns>indicator whether or not price check should be made.</returns>
        public bool ShouldPriceCheck()
        {
            if (this.Configuration.Enabled &&
                this.PluginService.PluginInterface.Data.IsDataReady &&
                this.PluginService.PluginInterface.ClientState?.LocalPlayer?.HomeWorld != null &&
                !(this.Configuration.RestrictInCombat && this.PluginService.ClientState.Condition.InCombat()) &&
                !(this.Configuration.RestrictInContent && this.PluginService.InContent()))
            {
                return true;
            }

            this.ItemCancellationTokenSource = null;
            return false;
        }

        private static void OnLanguageChanged(string langCode)
        {
            Result.UpdateLanguage();
        }

        private void LoadServices()
        {
            this.universalisClient = new UniversalisClient(this);
            this.PriceService = new PriceService(this, this.universalisClient);
        }

        private void LoadUI()
        {
            this.WindowManager = new WindowManager(this);
        }

        private void HandleFreshInstall()
        {
            if (!this.Configuration.FreshInstall) return;
            this.PluginService.Chat.PrintNotice(Loc.Localize("InstallThankYou", "Thank you for installing PriceCheck!"));
            this.PrintHelpMessage();
            this.Configuration.FreshInstall = false;
            this.Configuration.ShowToast = true;
            this.Configuration.RestrictInCombat = true;
            this.Configuration.RestrictInContent = true;
            this.SaveConfig();
            this.WindowManager.ConfigWindow!.IsOpen = true;
        }

        private void LoadConfig()
        {
            try
            {
                this.Configuration = this.PluginService.LoadConfig() as PluginConfig ?? new PluginConfig();
            }
            catch (Exception ex)
            {
                Logger.LogError("Failed to load config so creating new one.", ex);
                this.Configuration = new PluginConfig();
                this.SaveConfig();
            }
        }
    }
}
