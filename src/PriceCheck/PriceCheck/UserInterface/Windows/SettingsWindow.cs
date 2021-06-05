using System;
using System.Diagnostics;
using System.Linq;
using System.Numerics;

using CheapLoc;
using Dalamud.Interface.Components;
using DalamudPluginCommon;
using ImGuiNET;

namespace PriceCheck
{
    /// <summary>
    /// Settings window.
    /// </summary>
    public class SettingsWindow : WindowBase
    {
        private readonly IPriceCheckPlugin plugin;
        private Tab currentTab = Tab.General;
        private float uiScale;

        /// <summary>
        /// Initializes a new instance of the <see cref="SettingsWindow"/> class.
        /// </summary>
        /// <param name="plugin">price check plugin.</param>
        public SettingsWindow(IPriceCheckPlugin plugin)
        {
            this.plugin = plugin;
        }

        /// <summary>
        /// Overlay visibility changed event.
        /// </summary>
        public event EventHandler<bool> OnOverlayVisibilityUpdated = null!;

        private enum Tab
        {
            General,
            Overlay,
            Chat,
            Toast,
            Keybind,
            Filters,
            Thresholds,
            Other,
        }

        /// <inheritdoc />
        public override void DrawView()
        {
            if (!this.plugin.IsLoggedIn()) return;
            if (!this.IsVisible) return;
            var isVisible = this.IsVisible;
            this.uiScale = ImGui.GetIO().FontGlobalScale;
            ImGui.SetNextWindowSize(new Vector2(450 * this.uiScale, 240 * this.uiScale), ImGuiCond.Appearing);
            if (ImGui.Begin(
                Loc.Localize("SettingsWindow", "PriceCheck Settings") + "###PriceCheck_Settings_Window",
                ref isVisible,
                ImGuiWindowFlags.NoScrollWithMouse | ImGuiWindowFlags.NoScrollbar))
            {
                this.IsVisible = isVisible;
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

                    case Tab.Other:
                    {
                        this.DrawOther();
                        break;
                    }

                    default:
                        this.DrawGeneral();
                        break;
                }
            }

            ImGui.End();
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

                if (ImGui.BeginTabItem(Loc.Localize("Other", "Other") + "###PriceCheck_Other_Tab"))
                {
                    this.currentTab = Tab.Other;
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
                Loc.Localize("PluginEnabled", "Plugin Enabled") + "###PriceCheck_PluginEnabled_Checkbox",
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
                Loc.Localize("ShowPrices", "Show Prices") + "###PriceCheck_ShowPrices_Checkbox",
                ref showPrices))
            {
                this.plugin.Configuration.ShowPrices = showPrices;
                this.plugin.SaveConfig();
            }

            ImGuiComponents.HelpMarker(Loc.Localize(
                "ShowPrices_HelpMarker",
                "show price or just show advice"));

            ImGui.Spacing();
            ImGui.Text(Loc.Localize("HoverDelay", "Hover Delay"));
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
            ImGui.Text(Loc.Localize("PriceMode", "Price Mode"));
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

            ImGui.Spacing();
        }

        private void DrawOverlay()
        {
            var showOverlay = this.plugin.Configuration.ShowOverlay;
            if (ImGui.Checkbox(
                Loc.Localize("ShowOverlay", "Show Overlay") + "###PriceCheck_ShowOverlay_Checkbox",
                ref showOverlay))
            {
                this.plugin.Configuration.ShowOverlay = showOverlay;
                this.OnOverlayVisibilityUpdated(this, showOverlay);
                this.plugin.SaveConfig();
            }

            ImGuiComponents.HelpMarker(Loc.Localize(
                "ShowOverlay_HelpMarker",
                "show price check results in overlay window"));

            var useOverlayColors = this.plugin.Configuration.UseOverlayColors;
            if (ImGui.Checkbox(
                Loc.Localize("UseOverlayColors", "Use Overlay Colors") + "###PriceCheck_UseOverlayColors_Checkbox",
                ref useOverlayColors))
            {
                this.plugin.Configuration.UseOverlayColors = useOverlayColors;
                this.plugin.SaveConfig();
            }

            ImGuiComponents.HelpMarker(Loc.Localize(
                "UseOverlayColors_HelpMarker",
                "use different colors for overlay based on result"));

            ImGui.Spacing();
            ImGui.Text(Loc.Localize("MaxItems", "Max Items"));
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
            ImGui.Text(Loc.Localize("HideOverlayTimer", "Hide Overlay Timer"));
            ImGuiComponents.HelpMarker(Loc.Localize(
                                           "HideOverlayTimer_HelpMarker",
                                           "hide overlay after x amount of seconds since last price check - you can this by setting to zero."));
            var hideOverlayTimer = this.plugin.Configuration.HideOverlayElapsed.FromMillisecondsToSeconds();
            if (ImGui.SliderInt("###PriceCheck_HideOverlay_Slider", ref hideOverlayTimer, 0, 300))
            {
                this.plugin.Configuration.HideOverlayElapsed = hideOverlayTimer.FromSecondsToMilliseconds();
                this.plugin.SaveConfig();
            }
        }

        private void DrawChat()
        {
            var showInChat = this.plugin.Configuration.ShowInChat;
            if (ImGui.Checkbox(
                Loc.Localize("ShowInChat", "Show in Chat") + "###PriceCheck_ShowInChat_Checkbox",
                ref showInChat))
            {
                this.plugin.Configuration.ShowInChat = showInChat;
                this.plugin.SaveConfig();
            }

            ImGuiComponents.HelpMarker(Loc.Localize(
                "ShowInChat_HelpMarker",
                "show price check results in chat"));

            var useChatColors = this.plugin.Configuration.UseChatColors;
            if (ImGui.Checkbox(
                Loc.Localize("UseChatColors", "Use Chat Colors") + "###PriceCheck_UseChatColors_Checkbox",
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
                Loc.Localize("UseItemLinks", "Use Item Links") + "###PriceCheck_UseItemLinks_Checkbox",
                ref useItemLinks))
            {
                this.plugin.Configuration.UseItemLinks = useItemLinks;
                this.plugin.SaveConfig();
            }

            ImGuiComponents.HelpMarker(Loc.Localize(
                "UseItemLinks_HelpMarker",
                "use item links in chat results"));
        }

        private void DrawToast()
        {
            var showToast = this.plugin.Configuration.ShowToast;
            if (ImGui.Checkbox(
                Loc.Localize("ShowToast", "Show Toast") + "###PriceCheck_ShowToast_Checkbox",
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
                Loc.Localize("KeybindEnabled", "Use Keybind") + "###PriceCheck_KeybindEnabled_Checkbox",
                ref keybindEnabled))
            {
                this.plugin.Configuration.KeybindEnabled = keybindEnabled;
                this.plugin.SaveConfig();
            }

            ImGuiComponents.HelpMarker(Loc.Localize(
                "KeybindEnabled_HelpMarker",
                "toggle if keybind is used or just hover"));

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

            var showUnmarketable = this.plugin.Configuration.ShowUnmarketable;
            if (ImGui.Checkbox(
                Loc.Localize("ShowUnmarketable", "Show Unmarketable") + "###PriceCheck_ShowUnmarketable_Checkbox",
                ref showUnmarketable))
            {
                this.plugin.Configuration.ShowUnmarketable = showUnmarketable;
                this.plugin.SaveConfig();
            }

            ImGuiComponents.HelpMarker(Loc.Localize(
                "ShowUnmarketable_HelpMarker",
                "toggle whether to show items unmarketable items"));
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

        private void DrawOther()
        {
            var buttonSize = new Vector2(160f * this.uiScale, 25f * this.uiScale);
            var heightOffset = 100f * this.uiScale;
            ImGui.Spacing();
            if (ImGui.Button(Loc.Localize("OpenGithub", "Open Github") + "###PriceCheck_OpenGithub_Button", buttonSize))
                Process.Start("https://github.com/kalilistic/PriceCheck");
            if (ImGui.Button(Loc.Localize("PrintHelp", "Print Help") + "###PriceCheck_PrintHelp_Button", buttonSize))
                this.plugin.PrintHelpMessage();
            ImGui.SetCursorPosY(heightOffset);
            if (ImGui.Button(
                Loc.Localize("ImproveTranslate", "Improve Translations") + "###PriceCheck_ImproveTranslate_Button",
                buttonSize))
                Process.Start("https://crowdin.com/project/pricecheck");
        }
    }
}
