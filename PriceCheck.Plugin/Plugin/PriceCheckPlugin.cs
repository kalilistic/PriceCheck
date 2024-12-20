﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using CheapLoc;
using Dalamud.Configuration;
using Dalamud.DrunkenToad.Extensions;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.Text;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Game.Text.SeStringHandling.Payloads;
using Dalamud.IoC;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using PriceCheck.Localization;

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

        private LegacyLoc localization = null!;

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
                    this.localization = new LegacyLoc(PluginInterface, CommandManager);
                    this.PriceService = new PriceService(this);
                    this.PluginCommandManager = new PluginCommandManager(this);
                    this.UniversalisClient = new UniversalisClient(this);
                    this.HoveredItemManager = new HoveredItemManager(this);
                    ClientState.Login += this.Login;
                    this.LoadUI();
                }
                catch (Exception ex)
                {
                    PluginLog.Error(ex, "Failed to initialize plugin.");
                }
            });
        }

        /// <summary>
        /// Gets pluginInterface.
        /// </summary>
        [PluginService]
        public static IDalamudPluginInterface PluginInterface { get; private set; } = null!;

        /// <summary>
        /// Gets pluginInterface.
        /// </summary>
        [PluginService]
        public static IPluginLog PluginLog { get; private set; } = null!;

        /// <summary>
        /// Gets client state.
        /// </summary>
        [PluginService]
        public static IClientState ClientState { get; private set; } = null!;

        /// <summary>
        /// Gets chat gui.
        /// </summary>
        [PluginService]
        public static IChatGui Chat { get; private set; } = null!;

        /// <summary>
        /// Gets command manager.
        /// </summary>
        [PluginService]
        public static ICommandManager CommandManager { get; private set; } = null!;

        /// <summary>
        /// Gets toast gui.
        /// </summary>
        [PluginService]
        public static IToastGui Toast { get; private set; } = null!;

        /// <summary>
        /// Gets toast gui.
        /// </summary>
        [PluginService]
        public static IKeyState KeyState { get; private set; } = null!;

        /// <summary>
        /// Gets data manager.
        /// </summary>
        [PluginService]
        public static IDataManager DataManager { get; private set; } = null!;

        /// <summary>
        /// Gets condition.
        /// </summary>
        [PluginService]
        public static ICondition Condition { get; private set; } = null!;

        /// <summary>
        /// Gets game gui.
        /// </summary>
        [PluginService]
        public static IGameGui GameGui { get; private set; } = null!;

        [PluginService]
        public static IContextMenu ContextMenu { get; private set; } = null!;

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
        /// Gets or sets hovered item manager to handle hover item events.
        /// </summary>
        public HoveredItemManager HoveredItemManager { get; set; } = null!;

        /// <summary>
        /// Gets plugin configuration.
        /// </summary>
        public static void PrintHelpMessage()
        {
            Chat.Print(Loc.Localize(
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
                Chat.Print(new XivChatEntry
                {
                    Message = BuildSeString(PluginInterface.InternalName, payloadList),
                });
            }
            else
            {
                Chat.Print(new XivChatEntry
                {
                    Message = BuildSeString(PluginInterface.InternalName, payloadList),
                    Type = this.Configuration.ChatChannel,
                });
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
                this.HoveredItemManager.Dispose();
                this.ItemCancellationTokenSource?.Dispose();
                this.UniversalisClient.Dispose();
                this.localization.Dispose();
            }
            catch (Exception ex)
            {
                PluginLog.Error(ex, "Failed to dispose plugin properly.");
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
                ClientState.LocalPlayer?.HomeWorld != null &&
                !(this.Configuration.RestrictInCombat && Condition[ConditionFlag.InCombat]) &&
                !(this.Configuration.RestrictInContent && DataManager.InContent(ClientState.TerritoryType)))
            {
                return true;
            }

            this.ItemCancellationTokenSource = null;
            return false;
        }

        private static SeString BuildSeString(string? pluginName, string message)
        {
            var basePayloads = BuildBasePayloads(pluginName);
            var customPayloads = new List<Payload> { new TextPayload(message) };

            return new SeString(basePayloads.Concat(customPayloads).ToList());
        }

        private static SeString BuildSeString(string? pluginName, IEnumerable<Payload> payloads)
        {
            var basePayloads = BuildBasePayloads(pluginName);
            return new SeString(basePayloads.Concat(payloads).ToList());
        }

        private static IEnumerable<Payload> BuildBasePayloads(string? pluginName) => new List<Payload>
        {
            new UIForegroundPayload(0), new TextPayload($"[{pluginName}] "), new UIForegroundPayload(548),
        };

        private void Login()
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
                Chat.Print(Loc.Localize("InstallThankYou", "Thank you for installing PriceCheck!"));
                PrintHelpMessage();
                this.Configuration.FreshInstall = false;
                this.Configuration.ShowToast = true;
                this.Configuration.RestrictInCombat = true;
                this.Configuration.RestrictInContent = true;
                this.SaveConfig();
            }
            catch (Exception ex)
            {
                PluginLog.Error(ex, "Failed fresh install.");
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
                PluginLog.Error("Failed to load config so creating new one.", ex);
                this.Configuration = new PluginConfig();
                this.SaveConfig();
            }
        }
    }
}
