using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

using CheapLoc;
using Dalamud.Game.Command;
using Dalamud.Game.Text;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Game.Text.SeStringHandling.Payloads;
using Dalamud.Plugin;
using DalamudPluginCommon;
using Lumina.Excel.GeneratedSheets;
using XivCommon;
using XivCommon.Functions.ContextMenu;
using XivCommon.Functions.ContextMenu.Inventory;

namespace PriceCheck
{
    /// <summary>
    /// PriceCheck plugin.
    /// </summary>
    public class PriceCheckPlugin : PluginBase, IPriceCheckPlugin
    {
        private readonly XivCommonBase common;
        private readonly DalamudPluginInterface pluginInterface;
        private CancellationTokenSource? itemCancellationTokenSource;
        private PluginUI pluginUI = null!;
        private UniversalisClient universalisClient = null!;

        /// <summary>
        /// Initializes a new instance of the <see cref="PriceCheckPlugin"/> class.
        /// </summary>
        /// <param name="pluginName">plugin name.</param>
        /// <param name="pluginInterface">plugin interface.</param>
        public PriceCheckPlugin(string pluginName, DalamudPluginInterface pluginInterface)
            : base(pluginName, pluginInterface, Assembly.GetExecutingAssembly())
        {
            this.pluginInterface = pluginInterface;
            this.LoadConfig();
            this.LoadServices();
            this.SetupCommands();
            this.LoadUI();
            this.HandleFreshInstall();
            Result.UpdateLanguage();
            this.common = new XivCommonBase(pluginInterface, Hooks.ContextMenu);
            this.PluginInterface.Framework.Gui.HoveredItemChanged += this.HoveredItemChanged;
            this.PluginInterface.OnLanguageChanged += OnLanguageChanged;
            this.common.Functions.ContextMenu.OpenInventoryContextMenu += this.OnOpenInventoryContextMenu;
        }

        /// <inheritdoc/>
        public event EventHandler<DetectedItem> OnItemDetected = null!;

        /// <inheritdoc />
        public long LastPriceCheck { get; set; }

        /// <inheritdoc/>
        public IPriceService PriceService { get; private set; } = null!;

        /// <summary>
        /// Gets or sets plugin configuration.
        /// </summary>
        public PriceCheckConfig Configuration { get; set; } = null!;

        /// <inheritdoc/>
        public void PrintHelpMessage()
        {
            this.Chat.PrintNotice(Loc.Localize(
                "HelpMessage",
                "To check prices, hold your keybind and then hover over an item in your inventory or linked " +
                "in chat. You can set your keybind (or disable it) in the PriceCheck settings. The prices are " +
                "averages from Universalis and will not match any specific listings you see on the market board. " +
                "You can use /pcheck to open the overlay and /pcheckconfig to open settings. If you need help, " +
                "reach out on discord or open an issue on GitHub. If you want to help add translations, you can " +
                "submit updates on Crowdin. Links to both GitHub and Crowdin are available in settings."));
        }

        /// <inheritdoc />
        public Item? GetItemById(uint itemId)
        {
            return this.GameData.Item(itemId);
        }

        /// <inheritdoc />
        public uint GetHomeWorldId()
        {
            return this.ClientState.LocalPlayer.HomeWorldId();
        }

        /// <inheritdoc />
        public bool IsLoggedIn()
        {
            return this.ClientState.IsLoggedIn();
        }

        /// <inheritdoc />
        public void SendToast(PricedItem pricedItem)
        {
            this.PluginInterface.Framework.Gui.Toast.ShowNormal(
                $"{pricedItem.DisplayName} {(char)SeIconChar.ArrowRight} {pricedItem.Message}");
        }

        /// <inheritdoc/>
        public void PrintItemMessage(PricedItem pricedItem)
        {
            var payloadList = new List<Payload>
            {
                new UIForegroundPayload(this.PluginInterface.Data, 0),
                new TextPayload($"[{this.PluginName}] "),
            };
            if (this.Configuration.UseChatColors)
            {
                if (pricedItem.Result != null)
                    payloadList.Add(new UIForegroundPayload(this.PluginInterface.Data, pricedItem.Result.ChatColor()));
            }

            if (this.Configuration.UseItemLinks)
            {
                payloadList.Add(new ItemPayload(this.PluginInterface.Data, pricedItem.ItemId, pricedItem.IsHQ));
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
                payloadList.Add(new UIForegroundPayload(this.PluginInterface.Data, 0));
            this.Chat.Print(payloadList);
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
                return this.ClientState.KeyState.IsKeyPressed(this.Configuration.ModifierKey);
            }

            return this.ClientState.KeyState.IsKeyPressed(this.Configuration.ModifierKey) &&
                   this.ClientState.KeyState.IsKeyPressed(this.Configuration.PrimaryKey);
        }

        /// <summary>
        /// Dispose plugin.
        /// </summary>
        public new void Dispose()
        {
            base.Dispose();
            this.RemoveCommands();
            this.common.Functions.ContextMenu.OpenInventoryContextMenu -= this.OnOpenInventoryContextMenu;
            this.common.Dispose();
            this.PluginInterface.Framework.Gui.HoveredItemChanged -= this.HoveredItemChanged;
            this.pluginInterface.UiBuilder.OnBuildUi -= this.DrawUI;
            this.itemCancellationTokenSource?.Dispose();
            this.PriceService.Dispose();
            this.universalisClient.Dispose();
            this.pluginInterface.Dispose();
        }

        /// <inheritdoc />
        public void SaveConfig()
        {
            this.SaveConfig(this.Configuration);
        }

        /// <summary>
        /// Setup commands.
        /// </summary>
        public void SetupCommands()
        {
            this.pluginInterface.CommandManager.AddHandler("/pcheck", new CommandInfo(this.TogglePriceCheck)
            {
                HelpMessage = "Show price check.",
                ShowInHelp = true,
            });
            this.pluginInterface.CommandManager.AddHandler("/pricecheck", new CommandInfo(this.TogglePriceCheck)
            {
                ShowInHelp = false,
            });
            this.pluginInterface.CommandManager.AddHandler("/pcheckconfig", new CommandInfo(this.ToggleConfig)
            {
                HelpMessage = "Show price check config.",
                ShowInHelp = true,
            });
            this.pluginInterface.CommandManager.AddHandler("/pricecheckconfig", new CommandInfo(this.ToggleConfig)
            {
                ShowInHelp = false,
            });
        }

        /// <summary>
        /// Remove Commands.
        /// </summary>
        public void RemoveCommands()
        {
            this.pluginInterface.CommandManager.RemoveHandler("/pcheck");
            this.pluginInterface.CommandManager.RemoveHandler("/pricecheck");
            this.pluginInterface.CommandManager.RemoveHandler("/pcheckconfig");
            this.pluginInterface.CommandManager.RemoveHandler("/pricecheckconfig");
        }

        /// <summary>
        /// Toggle price check.
        /// </summary>
        /// <param name="command">command.</param>
        /// <param name="args">args.</param>
        public void TogglePriceCheck(string command, string args)
        {
            Logger.LogInfo("Running command {0} with args {1}", command, args);
            this.Configuration.ShowOverlay = !this.Configuration.ShowOverlay;
            this.pluginUI.OverlayWindow.IsVisible = !this.pluginUI.OverlayWindow.IsVisible;
        }

        /// <summary>
        /// Toggle config.
        /// </summary>
        /// <param name="command">command.</param>
        /// <param name="args">args.</param>
        public void ToggleConfig(string command, string args)
        {
            Logger.LogInfo("Running command {0} with args {1}", command, args);
            this.pluginUI.SettingsWindow.IsVisible = !this.pluginUI.SettingsWindow.IsVisible;
        }

        private static void OnLanguageChanged(string langCode)
        {
            Result.UpdateLanguage();
        }

        private static int? GetActionIndex(ICollection<byte> configActionIds, IReadOnlyList<byte> currentActionIds)
        {
            for (var i = 0; i < currentActionIds.Count; i++)
            {
                if (configActionIds.Contains(currentActionIds[i]))
                {
                    return i;
                }
            }

            return null;
        }

        private void ContextItemChanged(InventoryContextMenuItemSelectedArgs args)
        {
            try
            {
                if (!this.ShouldPriceCheck()) return;
                if (this.itemCancellationTokenSource != null)
                {
                    if (!this.itemCancellationTokenSource.IsCancellationRequested)
                        this.itemCancellationTokenSource.Cancel();
                    this.itemCancellationTokenSource.Dispose();
                }

                if (args.ItemId == 0)
                {
                    this.itemCancellationTokenSource = null;
                    return;
                }

                this.itemCancellationTokenSource = new CancellationTokenSource(this.Configuration.RequestTimeout * 2);
                Task.Run(async () =>
                {
                    await Task.Delay(this.Configuration.HoverDelay * 1000, this.itemCancellationTokenSource!.Token)
                              .ConfigureAwait(false);
                    this.OnItemDetected.Invoke(this, new DetectedItem(args.ItemId, args.ItemHq));
                });
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Failed to price check item via context menu.");
            }
        }

        private void OnOpenInventoryContextMenu(InventoryContextMenuOpenArgs args)
        {
            if (!this.Configuration.ShowContextMenu) return;

            // setup
            var index = 0;
            var actionIds = args.Items.Select(baseContextMenuItem => ((NativeContextMenuItem)baseContextMenuItem).InternalAction).ToList();
            var contextMenuItem = new InventoryContextMenuItem(
                Loc.Localize("ContextMenuItem", "Check Marketboard Price"), this.ContextItemChanged);

            // default
            if (this.Configuration.ShowContextAboveThis.Count == 0 &&
                this.Configuration.ShowContextBelowThis.Count == 0)
            {
                args.Items.Add(contextMenuItem);
                return;
            }

            // get show above index
            var relativeIndex = GetActionIndex(this.Configuration.ShowContextAboveThis, actionIds);
            if (relativeIndex != null)
            {
                index = (int)relativeIndex;
                actionIds.RemoveRange(index, actionIds.Count - index);
            }

            // get show below index
            relativeIndex = GetActionIndex(this.Configuration.ShowContextBelowThis, actionIds);
            if (relativeIndex != null)
            {
                index = (int)relativeIndex + 1;
            }

            // insert price check menu item
            args.Items.Insert(index, contextMenuItem);
        }

        private void LoadServices()
        {
            this.universalisClient = new UniversalisClient(this);
            this.PriceService = new PriceService(this, this.universalisClient);
        }

        private void LoadUI()
        {
            this.pluginUI = new PluginUI(this);
            this.pluginInterface.UiBuilder.OnBuildUi += this.DrawUI;
            this.pluginInterface.UiBuilder.OnOpenConfigUi += (_, _) => this.DrawConfigUI();
        }

        private bool ShouldPriceCheck()
        {
            if (this.Configuration.Enabled &&
                this.PluginInterface.Data.IsDataReady &&
                this.PluginInterface.ClientState?.LocalPlayer?.HomeWorld != null &&
                !(this.Configuration.RestrictInCombat && this.ClientState.Condition.InCombat()) &&
                !(this.Configuration.RestrictInContent && this.InContent()))
            {
                return true;
            }

            this.itemCancellationTokenSource = null;
            return false;
        }

        private void HoveredItemChanged(object sender, ulong itemId)
        {
            try
            {
                if (!this.ShouldPriceCheck()) return;
                if (!this.IsKeyBindPressed()) return;
                if (this.itemCancellationTokenSource != null)
                {
                    if (!this.itemCancellationTokenSource.IsCancellationRequested)
                        this.itemCancellationTokenSource.Cancel();
                    this.itemCancellationTokenSource.Dispose();
                }

                if (itemId == 0)
                {
                    this.itemCancellationTokenSource = null;
                    return;
                }

                this.itemCancellationTokenSource = new CancellationTokenSource(this.Configuration.RequestTimeout * 2);
                Task.Run(async () =>
                {
                    await Task.Delay(this.Configuration.HoverDelay * 1000, this.itemCancellationTokenSource!.Token)
                              .ConfigureAwait(false);
                    this.OnItemDetected.Invoke(this, new DetectedItem(itemId));
                });
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Failed to price check.");
                this.itemCancellationTokenSource = null;
            }
        }

        private void HandleFreshInstall()
        {
            if (!this.Configuration.FreshInstall) return;
            this.Chat.PrintNotice(Loc.Localize("InstallThankYou", "Thank you for installing PriceCheck!"));
            this.PrintHelpMessage();
            this.Configuration.FreshInstall = false;
            this.Configuration.ShowToast = true;
            this.Configuration.RestrictInCombat = true;
            this.Configuration.RestrictInContent = true;
            this.SaveConfig();
            this.pluginUI.SettingsWindow.IsVisible = true;
        }

        private void DrawUI()
        {
            this.pluginUI.Draw();
        }

        private void DrawConfigUI()
        {
            this.pluginUI.SettingsWindow.IsVisible = true;
        }

        private new void LoadConfig()
        {
            try
            {
                this.Configuration = base.LoadConfig() as PluginConfig ?? new PluginConfig();
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
