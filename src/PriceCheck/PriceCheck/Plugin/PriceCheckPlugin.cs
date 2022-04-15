using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using CheapLoc;
using Dalamud.Configuration;
using Dalamud.Data;
using Dalamud.DrunkenToad;
using Dalamud.Game.ClientState;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.ClientState.Keys;
using Dalamud.Game.Command;
using Dalamud.Game.Gui;
using Dalamud.Game.Gui.ContextMenus;
using Dalamud.Game.Gui.Toast;
using Dalamud.Game.Text;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Game.Text.SeStringHandling.Payloads;
using Dalamud.IoC;
using Dalamud.Plugin;

// ReSharper disable MemberInitializerValueIgnored
namespace PriceCheck
{
    /// <summary>
    /// PriceCheck plugin.
    /// </summary>
    public class PriceCheckPlugin : IDalamudPlugin
    {
        /// <summary>
        /// Cancellation token to terminate request if interrupted.
        /// </summary>
        public CancellationTokenSource? ItemCancellationTokenSource;

        private Localization localization = null!;

        /// <summary>
        /// Initializes a new instance of the <see cref="PriceCheckPlugin"/> class.
        /// </summary>
        public PriceCheckPlugin()
        {
            Task.Run(() =>
            {
                try
                {
                    this.LoadConfig();
                    this.HandleFreshInstall();
                    this.localization = new Localization(PluginInterface, CommandManager);
                    this.PriceService = new PriceService(this);
                    this.PluginCommandManager = new PluginCommandManager(this);
                    this.UniversalisClient = new UniversalisClient(this);
                    this.ContextMenuManager = new ContextMenuManager(this);
                    this.HoveredItemManager = new HoveredItemManager(this);
                    ClientState.Login += this.Login;
                    this.LoadUI();
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex, "Failed to initialize plugin.");
                }
            });
        }

        /// <summary>
        /// Gets pluginInterface.
        /// </summary>
        [PluginService]
        [RequiredVersion("1.0")]
        public static DalamudPluginInterface PluginInterface { get; private set; } = null!;

        /// <summary>
        /// Gets client state.
        /// </summary>
        [PluginService]
        [RequiredVersion("1.0")]
        public static ClientState ClientState { get; private set; } = null!;

        /// <summary>
        /// Gets chat gui.
        /// </summary>
        [PluginService]
        [RequiredVersion("1.0")]
        public static ChatGui Chat { get; private set; } = null!;

        /// <summary>
        /// Gets command manager.
        /// </summary>
        [PluginService]
        [RequiredVersion("1.0")]
        public static CommandManager CommandManager { get; private set; } = null!;

        /// <summary>
        /// Gets toast gui.
        /// </summary>
        [PluginService]
        [RequiredVersion("1.0")]
        public static ToastGui Toast { get; private set; } = null!;

        /// <summary>
        /// Gets toast gui.
        /// </summary>
        [PluginService]
        [RequiredVersion("1.0")]
        public static KeyState KeyState { get; private set; } = null!;

        /// <summary>
        /// Gets data manager.
        /// </summary>
        [PluginService]
        [RequiredVersion("1.0")]
        public static DataManager DataManager { get; private set; } = null!;

        /// <summary>
        /// Gets condition.
        /// </summary>
        [PluginService]
        [RequiredVersion("1.0")]
        public static Condition Condition { get; private set; } = null!;

        /// <summary>
        /// Gets game gui.
        /// </summary>
        [PluginService]
        [RequiredVersion("1.0")]
        public static GameGui GameGui { get; private set; } = null!;

        /// <summary>
        /// Gets context menu.
        /// </summary>
        [PluginService]
        [RequiredVersion("1.0")]
        public static ContextMenu ContextMenu { get; private set; } = null!;

        /// <inheritdoc />
        public string Name => "PriceCheck";

        /// <summary>
        /// Gets or sets command manager to handle user commands.
        /// </summary>
        public PluginCommandManager PluginCommandManager { get; set; } = null!;

        /// <summary>
        /// Gets price service.
        /// </summary>
        public PriceService PriceService { get; private set; } = null!;

        /// <summary>
        /// Gets universalis client.
        /// </summary>
        public UniversalisClient UniversalisClient { get; private set; } = null!;

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
        public ContextMenuManager ContextMenuManager { get; set; } = null!;

        /// <summary>
        /// Gets or sets hovered item manager to handle hover item events.
        /// </summary>
        public HoveredItemManager HoveredItemManager { get; set; } = null!;

        /// <summary>
        /// Gets plugin configuration.
        /// </summary>
        public static void PrintHelpMessage()
        {
            Chat.PluginPrintNotice(Loc.Localize(
                                                    "HelpMessage",
                                                    "To check prices, hold your keybind and then hover over an item in your inventory or linked " +
                                                    "in chat. You can set your keybind (or disable it) in the PriceCheck settings. The prices are " +
                                                    "averages from Universalis and will not match any specific listings you see on the market board. " +
                                                    "You can use /pcheck to open the overlay and /pcheckconfig to open settings. If you need help, " +
                                                    "reach out on discord or open an issue on GitHub. If you want to help add translations, you can " +
                                                    "submit updates on Crowdin."));
        }

        /// <summary>
        /// Send toast.
        /// </summary>
        /// <param name="pricedItem">priced Item.</param>
        public static void SendToast(PricedItem pricedItem)
        {
            Toast.ShowNormal(
                $"{pricedItem.ItemName} {(char)SeIconChar.ArrowRight} {pricedItem.Message}");
        }

        /// <summary>
        /// Print item message.
        /// </summary>
        /// <param name="pricedItem">priced item.</param>
        public void PrintItemMessage(PricedItem pricedItem)
        {
            var payloadList = new List<Payload>();
            if (this.Configuration.UseChatColors)
            {
                payloadList.Add(new UIForegroundPayload(pricedItem.ChatColor));
            }

            if (this.Configuration.UseItemLinks)
            {
                payloadList.Add(new ItemPayload(pricedItem.ItemId, pricedItem.IsHQ));
                payloadList.Add(new TextPayload($"{(char)SeIconChar.LinkMarker}"));
                payloadList.Add(new TextPayload(" " + pricedItem.ItemName));
                payloadList.Add(RawPayload.LinkTerminator);
            }
            else
            {
                payloadList.Add(new TextPayload(pricedItem.ItemName));
            }

            payloadList.Add(new TextPayload(" " + (char)SeIconChar.ArrowRight + " " + pricedItem.Message));
            if (this.Configuration.UseChatColors)
                payloadList.Add(new UIForegroundPayload(0));
            if (this.Configuration.ChatChannel == XivChatType.None)
            {
                Chat.PluginPrint(payloadList);
            }
            else
            {
                Chat.PluginPrint(payloadList, this.Configuration.ChatChannel);
            }
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
                return KeyState[(byte)this.Configuration.ModifierKey];
            }

            return KeyState[(byte)this.Configuration.ModifierKey] && KeyState[(byte)this.Configuration.PrimaryKey];
        }

        /// <summary>
        /// Dispose plugin.
        /// </summary>
        public void Dispose()
        {
            try
            {
                ClientState.Login -= this.Login;
                this.WindowManager.Dispose();
                PluginCommandManager.Dispose();
                this.ContextMenuManager.Dispose();
                this.HoveredItemManager.Dispose();
                this.ItemCancellationTokenSource?.Dispose();
                this.UniversalisClient.Dispose();
                this.localization.Dispose();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Failed to dispose plugin properly.");
            }

            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Save plugin configuration.
        /// </summary>
        public void SaveConfig()
        {
            PluginInterface.SavePluginConfig((IPluginConfiguration)this.Configuration);
        }

        /// <summary>
        /// Check state and configuration to determine if price check should be made.
        /// </summary>
        /// <returns>indicator whether or not price check should be made.</returns>
        public bool ShouldPriceCheck()
        {
            if (this.Configuration.Enabled &&
                DataManager.IsDataReady &&
                ClientState.LocalPlayer?.HomeWorld != null &&
                !(this.Configuration.RestrictInCombat && Condition[ConditionFlag.InCombat]) &&
                !(this.Configuration.RestrictInContent && DataManager.InContent(ClientState.TerritoryType)))
            {
                return true;
            }

            this.ItemCancellationTokenSource = null;
            return false;
        }

        private void Login(object? sender, EventArgs e)
        {
            this.WindowManager.MainWindow?.OpenOnLogin();
        }

        private void LoadUI()
        {
            this.WindowManager = new WindowManager(this);
        }

        private void HandleFreshInstall()
        {
            try
            {
                if (!this.Configuration.FreshInstall) return;
                Chat.PluginPrintNotice(Loc.Localize("InstallThankYou", "Thank you for installing PriceCheck!"));
                PrintHelpMessage();
                this.Configuration.FreshInstall = false;
                this.Configuration.ShowToast = true;
                this.Configuration.RestrictInCombat = true;
                this.Configuration.RestrictInContent = true;
                this.SaveConfig();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Failed fresh install.");
            }
        }

        private void LoadConfig()
        {
            try
            {
                this.Configuration = PluginInterface.GetPluginConfig() as PluginConfig ?? new PluginConfig();
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
