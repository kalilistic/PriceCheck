using System;
using System.Linq;
using System.Numerics;

using CheapLoc;
using Dalamud.DrunkenToad;
using Dalamud.Game.Text;
using Dalamud.Interface.Colors;
using Dalamud.Interface.Components;
using ImGuiNET;

namespace PriceCheck
{
    /// <summary>
    /// Config window for the plugin.
    /// </summary>
    public class ConfigWindow : PluginWindow
    {
        private readonly PriceCheckPlugin plugin;
        private Tab currentTab = Tab.General;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigWindow"/> class.
        /// </summary>
        /// <param name="plugin">PriceCheck plugin.</param>
        public ConfigWindow(PriceCheckPlugin plugin)
            : base(plugin, "PriceCheck Config")
        {
            this.plugin = plugin;
            this.Size = new Vector2(600f, 600f);
            this.SizeCondition = ImGuiCond.Appearing;
        }

        private enum Tab
        {
            General,
            Overlay,
            Chat,
            Toast,
            Keybind,
            Filters,
            Thresholds,
            ContextMenu,
        }

        /// <inheritdoc/>
        public override void Draw()
        {
            this.DrawTabs();
            switch (this.currentTab)
            {
                case Tab.General:
                {
                    this.DrawGeneral();
                    break;
                }

                case Tab.Overlay:
                {
                    this.DrawOverlay();
                    break;
                }

                case Tab.Chat:
                {
                    this.DrawChat();
                    break;
                }

                case Tab.Toast:
                {
                    this.DrawToast();
                    break;
                }

                case Tab.Keybind:
                {
                    this.DrawKeybind();
                    break;
                }

                case Tab.Filters:
                {
                    this.DrawFilters();
                    break;
                }

                case Tab.Thresholds:
                {
                    this.DrawThresholds();
                    break;
                }

                case Tab.ContextMenu:
                {
                    this.DrawContextMenu();
                    break;
                }

                default:
                    this.DrawGeneral();
                    break;
            }
        }

        private void DrawTabs()
        {
            if (ImGui.BeginTabBar("PriceCheckSettingsTabBar", ImGuiTabBarFlags.NoTooltip))
            {
                if (ImGui.BeginTabItem(Loc.Localize("General", "General") + "###PriceCheck_General_Tab"))
                {
                    this.currentTab = Tab.General;
                    ImGui.EndTabItem();
                }

                if (ImGui.BeginTabItem(Loc.Localize("Overlay", "Overlay") + "###PriceCheck_Overlay_Tab"))
                {
                    this.currentTab = Tab.Overlay;
                    ImGui.EndTabItem();
                }

                if (ImGui.BeginTabItem(Loc.Localize("Chat", "Chat") + "###PriceCheck_Chat_Tab"))
                {
                    this.currentTab = Tab.Chat;
                    ImGui.EndTabItem();
                }

                if (ImGui.BeginTabItem(Loc.Localize("Toast", "Toast") + "###PriceCheck_Toast_Tab"))
                {
                    this.currentTab = Tab.Toast;
                    ImGui.EndTabItem();
                }

                if (ImGui.BeginTabItem(Loc.Localize("Keybind", "Keybind") + "###PriceCheck_Keybind_Tab"))
                {
                    this.currentTab = Tab.Keybind;
                    ImGui.EndTabItem();
                }

                if (ImGui.BeginTabItem(Loc.Localize("Filters", "Filters") + "###PriceCheck_Filters_Tab"))
                {
                    this.currentTab = Tab.Filters;
                    ImGui.EndTabItem();
                }

                if (ImGui.BeginTabItem(Loc.Localize("Thresholds", "Thresholds") + "###PriceCheck_Thresholds_Tab"))
                {
                    this.currentTab = Tab.Thresholds;
                    ImGui.EndTabItem();
                }

                if (ImGui.BeginTabItem(Loc.Localize("ContextMenu", "Context Menu") + "###PriceCheck_ContextMenu_Tab"))
                {
                    this.currentTab = Tab.ContextMenu;
                    ImGui.EndTabItem();
                }

                ImGui.EndTabBar();
                ImGui.Spacing();
            }
        }

        private void DrawGeneral()
        {
            var enabled = this.plugin.Configuration.Enabled;
            if (ImGui.Checkbox(
                Loc.Localize("PluginEnabled", "Plugin enabled") + "###PriceCheck_PluginEnabled_Checkbox",
                ref enabled))
            {
                this.plugin.Configuration.Enabled = enabled;
                this.plugin.SaveConfig();
            }

            ImGuiComponents.HelpMarker(Loc.Localize(
                "PluginEnabled_HelpMarker",
                "toggle the plugin on/off"));

            var showPrices = this.plugin.Configuration.ShowPrices;
            if (ImGui.Checkbox(
                Loc.Localize("ShowPrices", "Show prices") + "###PriceCheck_ShowPrices_Checkbox",
                ref showPrices))
            {
                this.plugin.Configuration.ShowPrices = showPrices;
                this.plugin.SaveConfig();
            }

            ImGuiComponents.HelpMarker(Loc.Localize(
                "ShowPrices_HelpMarker",
                "show price or just show advice"));

            var allowKeybindAfterHover = this.plugin.Configuration.AllowKeybindAfterHover;
            if (ImGui.Checkbox(
                Loc.Localize("AllowKeybindAfterHover", "Allow keybind to be pressed after hovering over item") + "###PriceCheck_AllowKeybindAfterHover_Checkbox",
                ref allowKeybindAfterHover))
            {
                this.plugin.Configuration.AllowKeybindAfterHover = allowKeybindAfterHover;
                this.plugin.SaveConfig();
            }

            ImGuiComponents.HelpMarker(Loc.Localize(
                                           "AllowKeybindAfterHover_HelpMarker",
                                           "allows you to hold the keybind after the item tooltip has been opened (disable for legacy mode)"));

            ImGui.Spacing();
            ImGui.Text(Loc.Localize("HoverDelay", "Hover delay"));
            ImGuiComponents.HelpMarker(Loc.Localize(
                "HoverDelay_HelpMarker",
                "delay (in seconds) before processing after hovering"));
            var hoverDelay = this.plugin.Configuration.HoverDelay;
            if (ImGui.SliderInt("###PriceCheck_HoverDelay_Slider", ref hoverDelay, 0, 10))
            {
                this.plugin.Configuration.HoverDelay = hoverDelay;
                this.plugin.SaveConfig();
            }

            ImGui.Spacing();
            ImGui.Text(Loc.Localize("PriceMode", "Price mode"));
            ImGuiComponents.HelpMarker(Loc.Localize(
                "PriceMode_HelpMarker",
                "select price calculation to use"));
            var priceMode = this.plugin.Configuration.PriceMode;
            if (ImGui.Combo(
                "###PriceCheck_PriceMode_Combo",
                ref priceMode,
                PriceMode.PriceModeNames.ToArray(),
                PriceMode.PriceModeNames.Count))
            {
                this.plugin.Configuration.PriceMode = priceMode;
                this.plugin.SaveConfig();
            }

            ImGui.PushStyleColor(ImGuiCol.Text, ImGuiColors.HealerGreen);
            ImGui.TextWrapped(PriceMode.GetPriceModeByIndex(priceMode)?.Description);
            ImGui.PopStyleColor();

            ImGui.Spacing();
        }

        private void DrawOverlay()
        {
            ImGui.TextColored(ImGuiColors.DalamudViolet, Loc.Localize("DisplayHeading", "Display"));
            ImGui.Spacing();

            var showOverlay = this.plugin.Configuration.ShowOverlay;
            if (ImGui.Checkbox(
                Loc.Localize("ShowOverlay", "Show overlay") + "###PriceCheck_ShowOverlay_Checkbox",
                ref showOverlay))
            {
                this.plugin.Configuration.ShowOverlay = showOverlay;
                this.plugin.WindowManager.MainWindow!.IsOpen = showOverlay;
                this.plugin.SaveConfig();
            }

            ImGuiComponents.HelpMarker(Loc.Localize(
                "ShowOverlay_HelpMarker",
                "show price check results in overlay window"));

            var showOverlayOnLogin = this.plugin.Configuration.ShowOverlayOnLogin;
            if (ImGui.Checkbox(
                Loc.Localize("ShowOverlayOnLogin", "Show overlay on login") + "###PriceCheck_ShowOverlayOnLogin_Checkbox",
                ref showOverlayOnLogin))
            {
                this.plugin.Configuration.ShowOverlayOnLogin = showOverlayOnLogin;
                this.plugin.SaveConfig();
            }

            ImGuiComponents.HelpMarker(Loc.Localize(
                                           "ShowOverlayOnLogin_HelpMarker",
                                           "show overlay window on login"));

            var showOverlayByKeybind = this.plugin.Configuration.ShowOverlayByKeybind;
            if (ImGui.Checkbox(
                Loc.Localize("ShowOverlayByKeybind", "Show overlay by keybind") + "###PriceCheck_ShowOverlayByKeybind_Checkbox",
                ref showOverlayByKeybind))
            {
                this.plugin.Configuration.ShowOverlayByKeybind = showOverlayByKeybind;
                this.plugin.SaveConfig();
            }

            ImGuiComponents.HelpMarker(Loc.Localize(
                                           "ShowOverlayByKeybind_HelpMarker",
                                           "show overlay window when keybind is being held"));

            ImGui.TextColored(ImGuiColors.DalamudViolet, Loc.Localize("StyleHeading", "Style"));
            ImGui.Spacing();

            var useOverlayColors = this.plugin.Configuration.UseOverlayColors;
            if (ImGui.Checkbox(
                Loc.Localize("UseOverlayColors", "Use overlay colors") + "###PriceCheck_UseOverlayColors_Checkbox",
                ref useOverlayColors))
            {
                this.plugin.Configuration.UseOverlayColors = useOverlayColors;
                this.plugin.SaveConfig();
            }

            ImGuiComponents.HelpMarker(Loc.Localize(
                "UseOverlayColors_HelpMarker",
                "use different colors for overlay based on result"));

            ImGui.Spacing();
            ImGui.Text(Loc.Localize("MaxItems", "Max items"));
            ImGuiComponents.HelpMarker(Loc.Localize(
                "MaxItems_HelpMarker",
                "set max number of items in overlay at a time"));
            var maxItemsInOverlay = this.plugin.Configuration.MaxItemsInOverlay;
            if (ImGui.SliderInt("###PriceCheck_MaxItems_Slider", ref maxItemsInOverlay, 1, 30))
            {
                this.plugin.Configuration.MaxItemsInOverlay = maxItemsInOverlay;
                this.plugin.SaveConfig();
            }

            ImGui.Spacing();
            ImGui.Text(Loc.Localize("HideOverlayTimer", "Hide overlay timer"));
            ImGuiComponents.HelpMarker(Loc.Localize(
                                           "HideOverlayTimer_HelpMarker",
                                           "hide overlay after x amount of seconds since last price check - you can this by setting to zero."));
            var hideOverlayTimer = this.plugin.Configuration.HideOverlayElapsed.FromMillisecondsToSeconds();
            if (ImGui.SliderInt("###PriceCheck_HideOverlay_Slider", ref hideOverlayTimer, 0, 300))
            {
                this.plugin.Configuration.HideOverlayElapsed = hideOverlayTimer.FromSecondsToMilliseconds();
                this.plugin.SaveConfig();
            }

            ImGui.Spacing();
            ImGui.TextColored(ImGuiColors.DalamudViolet, Loc.Localize("FiltersHeading", "Filters"));
            ImGui.Spacing();
            var showSuccessInOverlay = this.plugin.Configuration.ShowSuccessInOverlay;
            if (ImGui.Checkbox(
                Loc.Localize("ShowSuccessInOverlay", "Show successful price check") + "###PriceCheck_ShowSuccessInOverlay_Checkbox",
                ref showSuccessInOverlay))
            {
                this.plugin.Configuration.ShowSuccessInOverlay = showSuccessInOverlay;
                this.plugin.SaveConfig();
            }

            ImGuiComponents.HelpMarker(Loc.Localize(
                                           "ShowSuccessInOverlay_HelpMarker",
                                           "show successful price check"));

            var showFailedToProcessInOverlay = this.plugin.Configuration.ShowFailedToProcessInOverlay;
            if (ImGui.Checkbox(
                Loc.Localize("ShowFailedToProcessInOverlay", "Show failed to process error") + "###PriceCheck_ShowFailedToProcessInOverlay_Checkbox",
                ref showFailedToProcessInOverlay))
            {
                this.plugin.Configuration.ShowFailedToProcessInOverlay = showFailedToProcessInOverlay;
                this.plugin.SaveConfig();
            }

            ImGuiComponents.HelpMarker(Loc.Localize(
                                           "ShowFailedToProcessInOverlay_HelpMarker",
                                           "show error where something went wrong unexpectedly"));

            var showFailedToGetDataInOverlay = this.plugin.Configuration.ShowFailedToGetDataInOverlay;
            if (ImGui.Checkbox(
                Loc.Localize("ShowFailedToGetDataInOverlay", "Show failed to get data error") + "###PriceCheck_ShowFailedToGetDataInOverlay_Checkbox",
                ref showFailedToGetDataInOverlay))
            {
                this.plugin.Configuration.ShowFailedToGetDataInOverlay = showFailedToGetDataInOverlay;
                this.plugin.SaveConfig();
            }

            ImGuiComponents.HelpMarker(Loc.Localize(
                                           "ShowFailedToGetDataInOverlay_HelpMarker",
                                           "show error where the plugin couldn't connect to universalis to get the data - usually a problem with your connection or universalis is down"));

            var showNoDataAvailableInOverlay = this.plugin.Configuration.ShowNoDataAvailableInOverlay;
            if (ImGui.Checkbox(
                Loc.Localize("ShowNoDataAvailableInOverlay", "Show no data available warning") + "###PriceCheck_ShowNoDataAvailableInOverlay_Checkbox",
                ref showNoDataAvailableInOverlay))
            {
                this.plugin.Configuration.ShowNoDataAvailableInOverlay = showNoDataAvailableInOverlay;
                this.plugin.SaveConfig();
            }

            ImGuiComponents.HelpMarker(Loc.Localize(
                                           "ShowNoDataAvailableInOverlay_HelpMarker",
                                           "show warning where there was no data from universalis available for the item"));

            var showNoRecentDataAvailableInOverlay = this.plugin.Configuration.ShowNoRecentDataAvailableInOverlay;
            if (ImGui.Checkbox(
                Loc.Localize("ShowNoRecentDataAvailableInOverlay", "Show no recent data available warning") + "###PriceCheck_ShowNoRecentDataAvailableInOverlay_Checkbox",
                ref showNoRecentDataAvailableInOverlay))
            {
                this.plugin.Configuration.ShowNoRecentDataAvailableInOverlay = showNoRecentDataAvailableInOverlay;
                this.plugin.SaveConfig();
            }

            ImGuiComponents.HelpMarker(Loc.Localize(
                                           "ShowNoRecentDataAvailableInOverlay_HelpMarker",
                                           "show warning where there was no recent data from universalis available for the item within your threshold"));

            var showBelowVendorInOverlay = this.plugin.Configuration.ShowBelowVendorInOverlay;
            if (ImGui.Checkbox(
                Loc.Localize("ShowBelowVendorInOverlay", "Show cheaper than vendor price warning") + "###PriceCheck_ShowBelowVendorInOverlay_Checkbox",
                ref showBelowVendorInOverlay))
            {
                this.plugin.Configuration.ShowBelowVendorInOverlay = showBelowVendorInOverlay;
                this.plugin.SaveConfig();
            }

            ImGuiComponents.HelpMarker(Loc.Localize(
                                           "ShowBelowVendorInOverlay_HelpMarker",
                                           "show warning that the market price is cheaper than what you can sell it to a vendor for"));

            var showBelowMinimumInOverlay = this.plugin.Configuration.ShowBelowMinimumInOverlay;
            if (ImGui.Checkbox(
                Loc.Localize("ShowBelowMinimumInOverlay", "Show cheaper than minimum threshold warning") + "###PriceCheck_ShowBelowMinimumInOverlay_Checkbox",
                ref showBelowMinimumInOverlay))
            {
                this.plugin.Configuration.ShowBelowMinimumInOverlay = showBelowMinimumInOverlay;
                this.plugin.SaveConfig();
            }

            ImGuiComponents.HelpMarker(Loc.Localize(
                                           "ShowBelowMinimumInOverlay_HelpMarker",
                                           "show warning the price is below your minimum threshold"));

            var showUnmarketableInOverlay = this.plugin.Configuration.ShowUnmarketableInOverlay;
            if (ImGui.Checkbox(
                Loc.Localize("ShowUnmarketableInOverlay", "Show unmarketable warning") + "###PriceCheck_ShowUnmarketableInOverlay_Checkbox",
                ref showUnmarketableInOverlay))
            {
                this.plugin.Configuration.ShowUnmarketableInOverlay = showUnmarketableInOverlay;
                this.plugin.SaveConfig();
            }

            ImGuiComponents.HelpMarker(Loc.Localize(
                                           "ShowUnmarketableInOverlay_HelpMarker",
                                           "show warning that the item can't be sold on the market board"));
        }

        private void DrawChat()
        {
            ImGui.TextColored(ImGuiColors.DalamudViolet, Loc.Localize("DisplayHeading", "Display"));
            ImGui.Spacing();

            var showInChat = this.plugin.Configuration.ShowInChat;
            if (ImGui.Checkbox(
                Loc.Localize("ShowInChat", "Show in chat") + "###PriceCheck_ShowInChat_Checkbox",
                ref showInChat))
            {
                this.plugin.Configuration.ShowInChat = showInChat;
                this.plugin.SaveConfig();
            }

            ImGuiComponents.HelpMarker(Loc.Localize(
                "ShowInChat_HelpMarker",
                "show price check results in chat"));

            ImGui.Text(Loc.Localize("ChatChannel", "Chat channel"));
            ImGuiComponents.HelpMarker(Loc.Localize(
                                           "ChatChannel_HelpMarker",
                                           "set the chat channel to send messages"));
            var chatChannel = this.plugin.Configuration.ChatChannel;
            ImGui.SetNextItemWidth(ImGui.GetWindowSize().X / 3);
            if (ImGui.BeginCombo("###PriceCheck_ChatChannel_Combo", chatChannel.ToString()))
            {
                foreach (var type in Enum.GetValues(typeof(XivChatType)).Cast<XivChatType>())
                {
                    if (ImGui.Selectable(type.ToString(), type == chatChannel))
                    {
                        this.plugin.Configuration.ChatChannel = type;
                        this.plugin.SaveConfig();
                    }
                }

                ImGui.EndCombo();
            }

            ImGui.TextColored(ImGuiColors.DalamudViolet, Loc.Localize("StyleHeading", "Style"));
            ImGui.Spacing();

            var useChatColors = this.plugin.Configuration.UseChatColors;
            if (ImGui.Checkbox(
                Loc.Localize("UseChatColors", "Use chat colors") + "###PriceCheck_UseChatColors_Checkbox",
                ref useChatColors))
            {
                this.plugin.Configuration.UseChatColors = useChatColors;
                this.plugin.SaveConfig();
            }

            ImGuiComponents.HelpMarker(Loc.Localize(
                "UseChatColors_HelpMarker",
                "use different colors for chat based on result"));

            var useItemLinks = this.plugin.Configuration.UseItemLinks;
            if (ImGui.Checkbox(
                Loc.Localize("UseItemLinks", "Use item links") + "###PriceCheck_UseItemLinks_Checkbox",
                ref useItemLinks))
            {
                this.plugin.Configuration.UseItemLinks = useItemLinks;
                this.plugin.SaveConfig();
            }

            ImGuiComponents.HelpMarker(Loc.Localize(
                "UseItemLinks_HelpMarker",
                "use item links in chat results"));

            ImGui.Spacing();
            ImGui.TextColored(ImGuiColors.DalamudViolet, Loc.Localize("FiltersHeading", "Filters"));
            var showSuccessInChat = this.plugin.Configuration.ShowSuccessInChat;
            if (ImGui.Checkbox(
                Loc.Localize("ShowSuccessInChat", "Show successful price check") + "###PriceCheck_ShowSuccessInChat_Checkbox",
                ref showSuccessInChat))
            {
                this.plugin.Configuration.ShowSuccessInChat = showSuccessInChat;
                this.plugin.SaveConfig();
            }

            ImGuiComponents.HelpMarker(Loc.Localize(
                                           "ShowSuccessInChat_HelpMarker",
                                           "show successful price check"));

            var showFailedToProcessInChat = this.plugin.Configuration.ShowFailedToProcessInChat;
            if (ImGui.Checkbox(
                Loc.Localize("ShowFailedToProcessInChat", "Show failed to process error") + "###PriceCheck_ShowFailedToProcessInChat_Checkbox",
                ref showFailedToProcessInChat))
            {
                this.plugin.Configuration.ShowFailedToProcessInChat = showFailedToProcessInChat;
                this.plugin.SaveConfig();
            }

            ImGuiComponents.HelpMarker(Loc.Localize(
                                           "ShowFailedToProcessInChat_HelpMarker",
                                           "show error where something went wrong unexpectedly"));

            var showFailedToGetDataInChat = this.plugin.Configuration.ShowFailedToGetDataInChat;
            if (ImGui.Checkbox(
                Loc.Localize("ShowFailedToGetDataInChat", "Show failed to get data error") + "###PriceCheck_ShowFailedToGetDataInChat_Checkbox",
                ref showFailedToGetDataInChat))
            {
                this.plugin.Configuration.ShowFailedToGetDataInChat = showFailedToGetDataInChat;
                this.plugin.SaveConfig();
            }

            ImGuiComponents.HelpMarker(Loc.Localize(
                                           "ShowFailedToGetDataInChat_HelpMarker",
                                           "show error where the plugin couldn't connect to universalis to get the data - usually a problem with your connection or universalis is down"));

            var showNoDataAvailableInChat = this.plugin.Configuration.ShowNoDataAvailableInChat;
            if (ImGui.Checkbox(
                Loc.Localize("ShowNoDataAvailableInChat", "Show no data available warning") + "###PriceCheck_ShowNoDataAvailableInChat_Checkbox",
                ref showNoDataAvailableInChat))
            {
                this.plugin.Configuration.ShowNoDataAvailableInChat = showNoDataAvailableInChat;
                this.plugin.SaveConfig();
            }

            ImGuiComponents.HelpMarker(Loc.Localize(
                                           "ShowNoDataAvailableInChat_HelpMarker",
                                           "show warning where there was no data from universalis available for the item"));

            var showNoRecentDataAvailableInChat = this.plugin.Configuration.ShowNoRecentDataAvailableInChat;
            if (ImGui.Checkbox(
                Loc.Localize("ShowNoRecentDataAvailableInChat", "Show no recent data available warning") + "###PriceCheck_ShowNoRecentDataAvailableInChat_Checkbox",
                ref showNoRecentDataAvailableInChat))
            {
                this.plugin.Configuration.ShowNoRecentDataAvailableInChat = showNoRecentDataAvailableInChat;
                this.plugin.SaveConfig();
            }

            ImGuiComponents.HelpMarker(Loc.Localize(
                                           "ShowNoRecentDataAvailableInChat_HelpMarker",
                                           "show warning where there was no recent data from universalis available for the item within your threshold"));

            var showBelowVendorInChat = this.plugin.Configuration.ShowBelowVendorInChat;
            if (ImGui.Checkbox(
                Loc.Localize("ShowBelowVendorInChat", "Show cheaper than vendor price warning") + "###PriceCheck_ShowBelowVendorInChat_Checkbox",
                ref showBelowVendorInChat))
            {
                this.plugin.Configuration.ShowBelowVendorInChat = showBelowVendorInChat;
                this.plugin.SaveConfig();
            }

            ImGuiComponents.HelpMarker(Loc.Localize(
                                           "ShowBelowVendorInChat_HelpMarker",
                                           "show warning that the market price is cheaper than what you can sell it to a vendor for"));

            var showBelowMinimumInChat = this.plugin.Configuration.ShowBelowMinimumInChat;
            if (ImGui.Checkbox(
                Loc.Localize("ShowBelowMinimumInChat", "Show cheaper than minimum threshold warning") + "###PriceCheck_ShowBelowMinimumInChat_Checkbox",
                ref showBelowMinimumInChat))
            {
                this.plugin.Configuration.ShowBelowMinimumInChat = showBelowMinimumInChat;
                this.plugin.SaveConfig();
            }

            ImGuiComponents.HelpMarker(Loc.Localize(
                                           "ShowBelowMinimumInChat_HelpMarker",
                                           "show warning the price is below your minimum threshold"));

            var showUnmarketableInChat = this.plugin.Configuration.ShowUnmarketableInChat;
            if (ImGui.Checkbox(
                Loc.Localize("ShowUnmarketableInChat", "Show unmarketable warning") + "###PriceCheck_ShowUnmarketableInChat_Checkbox",
                ref showUnmarketableInChat))
            {
                this.plugin.Configuration.ShowUnmarketableInChat = showUnmarketableInChat;
                this.plugin.SaveConfig();
            }

            ImGuiComponents.HelpMarker(Loc.Localize(
                                           "ShowUnmarketableInChat_HelpMarker",
                                           "show warning that the item can't be sold on the market board"));
        }

        private void DrawToast()
        {
            var showToast = this.plugin.Configuration.ShowToast;
            if (ImGui.Checkbox(
                Loc.Localize("ShowToast", "Show toast") + "###PriceCheck_ShowToast_Checkbox",
                ref showToast))
            {
                this.plugin.Configuration.ShowToast = showToast;
                this.plugin.SaveConfig();
            }

            ImGuiComponents.HelpMarker(Loc.Localize(
                                           "ShowToast_HelpMarker",
                                           "show price check results in toasts"));
        }

        private void DrawKeybind()
        {
            var keybindEnabled = this.plugin.Configuration.KeybindEnabled;
            if (ImGui.Checkbox(
                Loc.Localize("KeybindEnabled", "Enable keybind") + "###PriceCheck_KeybindEnabled_Checkbox",
                ref keybindEnabled))
            {
                this.plugin.Configuration.KeybindEnabled = keybindEnabled;
                this.plugin.SaveConfig();
            }

            ImGuiComponents.HelpMarker(Loc.Localize(
                "KeybindEnabled_HelpMarker",
                "toggle if keybind is used or just hover"));

            var showKeybindInTitleBar = this.plugin.Configuration.ShowKeybindInTitleBar;
            if (ImGui.Checkbox(
                Loc.Localize("ShowKeybindInTitleBar", "Show keybind in titlebar") + "###PriceCheck_ShowKeybindInTitleBar_Checkbox",
                ref showKeybindInTitleBar))
            {
                this.plugin.Configuration.ShowKeybindInTitleBar = showKeybindInTitleBar;
                this.plugin.SaveConfig();
                this.plugin.WindowManager.MainWindow?.UpdateWindowTitle();
            }

            ImGuiComponents.HelpMarker(Loc.Localize(
                                           "ShowKeybindInTitleBar_HelpMarker",
                                           "toggle if keybind is displayed in titlebar"));

            ImGui.Spacing();
            ImGui.Text(Loc.Localize("ModifierKeybind", "Modifier"));
            ImGuiComponents.HelpMarker(Loc.Localize(
                "ModifierKeybind_HelpMarker",
                "set your modifier key (e.g. shift)"));
            var modifierKey =
                ModifierKey.EnumToIndex(this.plugin.Configuration.ModifierKey);
            if (ImGui.Combo(
                "###PriceCheck_ModifierKey_Combo",
                ref modifierKey,
                ModifierKey.Names.ToArray(),
                ModifierKey.Names.Length))
            {
                this.plugin.Configuration.ModifierKey =
                    ModifierKey.IndexToEnum(modifierKey);
                this.plugin.SaveConfig();
                this.plugin.WindowManager.MainWindow?.UpdateWindowTitle();
            }

            ImGui.Spacing();
            ImGui.Text(Loc.Localize("PrimaryKeybind", "Primary"));
            ImGuiComponents.HelpMarker(Loc.Localize(
                "PrimaryKeybind_HelpMarker",
                "set your primary key (e.g. None, Z)"));
            var primaryKey = PrimaryKey.EnumToIndex(this.plugin.Configuration.PrimaryKey);
            if (ImGui.Combo(
                "###PriceCheck_PrimaryKey_Combo",
                ref primaryKey,
                PrimaryKey.Names.ToArray(),
                PrimaryKey.Names.Length))
            {
                this.plugin.Configuration.PrimaryKey = PrimaryKey.IndexToEnum(primaryKey);
                this.plugin.SaveConfig();
                this.plugin.WindowManager.MainWindow?.UpdateWindowTitle();
            }
        }

        private void DrawFilters()
        {
            var restrictInCombat = this.plugin.Configuration.RestrictInCombat;
            if (ImGui.Checkbox(
                Loc.Localize("RestrictInCombat", "Don't process in combat") + "###PriceCheck_RestrictInCombat_Checkbox",
                ref restrictInCombat))
            {
                this.plugin.Configuration.RestrictInCombat = restrictInCombat;
                this.plugin.SaveConfig();
            }

            ImGuiComponents.HelpMarker(Loc.Localize(
                "RestrictInCombat_HelpMarker",
                "don't process price checks while in combat"));

            var restrictInContent = this.plugin.Configuration.RestrictInContent;
            if (ImGui.Checkbox(
                Loc.Localize("RestrictInContent", "Don't process in content") + "###PriceCheck_RestrictInContent_Checkbox",
                ref restrictInContent))
            {
                this.plugin.Configuration.RestrictInContent = restrictInContent;
                this.plugin.SaveConfig();
            }

            ImGuiComponents.HelpMarker(Loc.Localize(
                "RestrictInContent_HelpMarker",
                "don't process price checks while in content"));
        }

        private void DrawThresholds()
        {
            ImGui.Text(Loc.Localize("MinimumPrice", "Minimum Price"));
            ImGuiComponents.HelpMarker(Loc.Localize(
                "MinimumPrice_HelpMarker",
                "set minimum price at which actual average will be displayed"));
            var minPrice = this.plugin.Configuration.MinPrice;
            if (ImGui.InputInt("###PriceCheck_MinPrice_Slider", ref minPrice, 500, 500))
            {
                this.plugin.Configuration.MinPrice = Math.Abs(minPrice);
                this.plugin.SaveConfig();
            }

            ImGui.Spacing();
            ImGui.Text(Loc.Localize("MaxUploadDays", "Max Upload Days"));
            ImGuiComponents.HelpMarker(Loc.Localize(
                "MaxUploadDays_HelpMarker",
                "set maximum age to avoid using old data"));
            var maxUploadDays = this.plugin.Configuration.MaxUploadDays;
            if (ImGui.InputInt("###PriceCheck_MaxUploadDays_Slider", ref maxUploadDays, 5, 5))
            {
                this.plugin.Configuration.MaxUploadDays = Math.Abs(maxUploadDays);
                this.plugin.SaveConfig();
            }
        }

        private void DrawContextMenu()
        {
            var showContextMenu = this.plugin.Configuration.ShowContextMenu;
            if (ImGui.Checkbox(
                Loc.Localize("ShowContextMenu", "Show context menu") +
                "###PriceCheck_ShowContextMenu_Checkbox",
                ref showContextMenu))
            {
                this.plugin.Configuration.ShowContextMenu = showContextMenu;
                this.plugin.SaveConfig();
            }
        }
    }
}
