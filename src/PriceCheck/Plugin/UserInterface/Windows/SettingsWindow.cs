// ReSharper disable InvertIf

using System;
using System.Diagnostics;
using System.Numerics;
using CheapLoc;
using ImGuiNET;

namespace PriceCheck
{
	public class SettingsWindow : WindowBase
	{
		private readonly IPriceCheckPlugin _priceCheckPlugin;
		private Tab _currentTab = Tab.General;
		private float _uiScale;

		public SettingsWindow(IPriceCheckPlugin priceCheckPlugin)
		{
			_priceCheckPlugin = priceCheckPlugin;
		}

		public event EventHandler<bool> OverlayVisibilityUpdated;

		public void DrawWindow()
		{
			if (!IsVisible) return;
			_uiScale = ImGui.GetIO().FontGlobalScale;
			ImGui.SetNextWindowSize(new Vector2(310 * _uiScale, 210 * _uiScale));
			ImGui.Begin(Loc.Localize("SettingsWindow", "PriceCheck Settings") + "###PriceCheck_Settings_Window",
				ref IsVisible,
				ImGuiWindowFlags.NoScrollWithMouse | ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.AlwaysAutoResize);

			DrawTabs();
			switch (_currentTab)
			{
				case Tab.General:
				{
					DrawGeneral();
					break;
				}
				case Tab.Overlay:
				{
					DrawOverlay();
					break;
				}
				case Tab.Chat:
				{
					DrawChat();
					break;
				}
				case Tab.Keybind:
				{
					DrawKeybind();
					break;
				}
				case Tab.Thresholds:
				{
					DrawThresholds();
					break;
				}
				default:
					DrawGeneral();
					break;
			}

			ImGui.End();
		}

		public void DrawTabs()
		{
			if (ImGui.BeginTabBar("PriceCheckSettingsTabBar", ImGuiTabBarFlags.NoTooltip))
			{
				if (ImGui.BeginTabItem(Loc.Localize("General", "General") + "###PriceCheck_General_Tab"))
				{
					_currentTab = Tab.General;
					ImGui.EndTabItem();
				}

				if (ImGui.BeginTabItem(Loc.Localize("Overlay", "Overlay") + "###PriceCheck_Overlay_Tab"))
				{
					_currentTab = Tab.Overlay;
					ImGui.EndTabItem();
				}

				if (ImGui.BeginTabItem(Loc.Localize("Chat", "Chat") + "###PriceCheck_Chat_Tab"))
				{
					_currentTab = Tab.Chat;
					ImGui.EndTabItem();
				}

				if (ImGui.BeginTabItem(Loc.Localize("Keybind", "Keybind") + "###PriceCheck_Keybind_Tab"))
				{
					_currentTab = Tab.Keybind;
					ImGui.EndTabItem();
				}

				if (ImGui.BeginTabItem(Loc.Localize("Thresholds", "Thresholds") + "###PriceCheck_Thresholds_Tab"))
				{
					_currentTab = Tab.Thresholds;
					ImGui.EndTabItem();
				}

				ImGui.EndTabBar();
				ImGui.Spacing();
			}
		}

		public void DrawGeneral()
		{
			var enabled = _priceCheckPlugin.Configuration.Enabled;
			if (ImGui.Checkbox(
				Loc.Localize("PluginEnabled", "PriceCheckPlugin Enabled") + "###PriceCheck_PluginEnabled_Checkbox",
				ref enabled))
			{
				_priceCheckPlugin.Configuration.Enabled = enabled;
				_priceCheckPlugin.SaveConfig();
			}

			var showPrices = _priceCheckPlugin.Configuration.ShowPrices;
			if (ImGui.Checkbox(Loc.Localize("ShowPrices", "Show Prices") + "###PriceCheck_ShowPrices_Checkbox",
				ref showPrices))
			{
				_priceCheckPlugin.Configuration.ShowPrices = showPrices;
				_priceCheckPlugin.SaveConfig();
			}

			ImGui.Spacing();
			ImGui.Text(Loc.Localize("Language", "Language"));
			var pluginLanguage = _priceCheckPlugin.Configuration.PluginLanguage;
			if (ImGui.Combo("###PriceCheck_Language_Combo", ref pluginLanguage,
				PluginLanguage.LanguageNames.ToArray(),
				PluginLanguage.LanguageNames.Count))
			{
				_priceCheckPlugin.Configuration.PluginLanguage = pluginLanguage;
				_priceCheckPlugin.SaveConfig();
				_priceCheckPlugin.Localization.SetLanguage(pluginLanguage);
			}

			ImGui.Spacing();
			ImGui.Spacing();
			if (ImGui.SmallButton(Loc.Localize("OpenTranslate", "Translate") + "###PriceCheck_Translate_Button"))
				Process.Start("https://crowdin.com/project/pricecheck");
			ImGui.SameLine(75f * _uiScale);
			if (ImGui.SmallButton(Loc.Localize("OpenGithub", "Github") + "###PriceCheck_Github_Button"))
				Process.Start("https://github.com/kalilistic/PriceCheck");
			ImGui.SameLine(128f * _uiScale);
			if (ImGui.SmallButton(Loc.Localize("PrintHelp", "Help") + "###PriceCheck_Help_Button"))
				_priceCheckPlugin.PrintHelpMessage();
		}

		public void DrawOverlay()
		{
			var showOverlay = _priceCheckPlugin.Configuration.ShowOverlay;
			if (ImGui.Checkbox(Loc.Localize("ShowOverlay", "Show Overlay") + "###PriceCheck_ShowOverlay_Checkbox",
				ref showOverlay))
			{
				_priceCheckPlugin.Configuration.ShowOverlay = showOverlay;
				OverlayVisibilityUpdated?.Invoke(this, showOverlay);
				_priceCheckPlugin.SaveConfig();
			}

			ImGui.Spacing();
			ImGui.Text(Loc.Localize("MaxItems", "Max Items"));
			var maxItemsInOverlay = _priceCheckPlugin.Configuration.MaxItemsInOverlay;
			if (ImGui.SliderInt("###PriceCheck_MaxItems_Slider", ref maxItemsInOverlay, 0, 30))
			{
				_priceCheckPlugin.Configuration.MaxItemsInOverlay = maxItemsInOverlay;
				_priceCheckPlugin.SaveConfig();
			}
		}

		public void DrawChat()
		{
			var showInChat = _priceCheckPlugin.Configuration.ShowInChat;
			if (ImGui.Checkbox(Loc.Localize("ShowInChat", "Show in Chat") + "###PriceCheck_ShowInChat_Checkbox",
				ref showInChat))
			{
				_priceCheckPlugin.Configuration.ShowInChat = showInChat;
				_priceCheckPlugin.SaveConfig();
			}

			var useChatColors = _priceCheckPlugin.Configuration.UseChatColors;
			if (ImGui.Checkbox(
				Loc.Localize("UseChatColors", "Use Chat Colors") + "###PriceCheck_UseChatColors_Checkbox",
				ref useChatColors))
			{
				_priceCheckPlugin.Configuration.UseChatColors = useChatColors;
				_priceCheckPlugin.SaveConfig();
			}
		}

		public void DrawKeybind()
		{
			var keybindEnabled = _priceCheckPlugin.Configuration.KeybindEnabled;
			if (ImGui.Checkbox(Loc.Localize("KeybindEnabled", "Use Keybind") + "###PriceCheck_KeybindEnabled_Checkbox",
				ref keybindEnabled))
			{
				_priceCheckPlugin.Configuration.KeybindEnabled = keybindEnabled;
				_priceCheckPlugin.SaveConfig();
			}

			ImGui.Spacing();
			ImGui.Text(Loc.Localize("ModifierKeybind", "Modifier"));
			var modifierKey =
				ModifierKey.EnumToIndex(_priceCheckPlugin.Configuration.ModifierKey);
			if (ImGui.Combo("###PriceCheck_ModifierKey_Combo", ref modifierKey, ModifierKey.Names.ToArray(),
				ModifierKey.Names.Length))
			{
				_priceCheckPlugin.Configuration.ModifierKey =
					ModifierKey.IndexToEnum(modifierKey);
				_priceCheckPlugin.SaveConfig();
			}

			ImGui.Spacing();
			ImGui.Text(Loc.Localize("PrimaryKeybind", "Primary"));
			var primaryKey = PrimaryKey.EnumToIndex(_priceCheckPlugin.Configuration.PrimaryKey);
			if (ImGui.Combo("###PriceCheck_PrimaryKey_Combo", ref primaryKey, PrimaryKey.Names.ToArray(),
				PrimaryKey.Names.Length))
			{
				_priceCheckPlugin.Configuration.PrimaryKey = PrimaryKey.IndexToEnum(primaryKey);
				_priceCheckPlugin.SaveConfig();
			}
		}

		public void DrawThresholds()
		{
			ImGui.Text(Loc.Localize("MinimumPrice", "Minimum Price"));
			var minPrice = _priceCheckPlugin.Configuration.MinPrice;
			if (ImGui.SliderInt("###PriceCheck_MinPrice_Slider", ref minPrice, 0, 20000))
			{
				_priceCheckPlugin.Configuration.MinPrice = minPrice;
				_priceCheckPlugin.SaveConfig();
			}

			ImGui.Spacing();
			ImGui.Text(Loc.Localize("MaxUploadDays", "Max Upload Days"));
			var maxUploadDays = _priceCheckPlugin.Configuration.MaxUploadDays;
			if (ImGui.SliderInt("###PriceCheck_MaxUploadDays_Slider", ref maxUploadDays, 0, 365))
			{
				_priceCheckPlugin.Configuration.MaxUploadDays = maxUploadDays;
				_priceCheckPlugin.SaveConfig();
			}
		}


		private enum Tab
		{
			General,
			Overlay,
			Chat,
			Keybind,
			Thresholds
		}
	}
}