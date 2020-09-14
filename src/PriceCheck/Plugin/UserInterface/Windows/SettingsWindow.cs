using System;
using System.Linq;
using System.Numerics;
using CheapLoc;
using ImGuiNET;

namespace PriceCheck
{
	public class SettingsWindow : WindowBase
	{
		private readonly IPluginWrapper _plugin;

		public SettingsWindow(IPluginWrapper plugin)
		{
			_plugin = plugin;
		}

		public event EventHandler<bool> OverlayVisibilityUpdated;

		public void DrawWindow()
		{
			if (!IsVisible) return;

			ImGui.SetNextWindowSizeConstraints(new Vector2(140, 200), new Vector2(float.MaxValue, float.MaxValue));
			if (ImGui.Begin(Loc.Localize("SettingsWindow", "PriceCheck Settings") + "###PriceCheck_Settings_Window",
				ref IsVisible,
				ImGuiWindowFlags.AlwaysAutoResize))
			{
				ImGui.Separator();
				ImGui.Text(Loc.Localize("General", "General"));
				ImGui.Separator();
				ImGui.Spacing();

				var enabled = _plugin.GetConfig().Enabled;
				if (ImGui.Checkbox(Loc.Localize("PluginEnabled", "Enabled") + "###PriceCheck_PluginEnabled_Checkbox",
					ref enabled))
				{
					_plugin.GetConfig().Enabled = enabled;
					_plugin.GetConfig().Save();
				}

				var showOverlay = _plugin.GetConfig().ShowOverlay;
				if (ImGui.Checkbox(Loc.Localize("ShowOverlay", "Show Overlay") + "###PriceCheck_ShowOverlay_Checkbox",
					ref showOverlay))
				{
					_plugin.GetConfig().ShowOverlay = showOverlay;
					OverlayVisibilityUpdated?.Invoke(this, showOverlay);
					_plugin.GetConfig().Save();
				}

				var showInChat = _plugin.GetConfig().ShowInChat;
				if (ImGui.Checkbox(Loc.Localize("ShowInChat", "Show in Chat") + "###PriceCheck_ShowInChat_Checkbox",
					ref showInChat))
				{
					_plugin.GetConfig().ShowInChat = showInChat;
					_plugin.GetConfig().Save();
				}

				var useChatColors = _plugin.GetConfig().UseChatColors;
				if (ImGui.Checkbox(
					Loc.Localize("UseChatColors", "Use Chat Colors") + "###PriceCheck_UseChatColors_Checkbox",
					ref useChatColors))
				{
					_plugin.GetConfig().UseChatColors = useChatColors;
					_plugin.GetConfig().Save();
				}

				var showPrices = _plugin.GetConfig().ShowPrices;
				if (ImGui.Checkbox(Loc.Localize("ShowPrices", "Show Prices") + "###PriceCheck_ShowPrices_Checkbox",
					ref showPrices))
				{
					_plugin.GetConfig().ShowPrices = showPrices;
					_plugin.GetConfig().Save();
				}

				ImGui.Spacing();
				ImGui.Text(Loc.Localize("Language", "Language"));
				var pluginLanguage = _plugin.GetConfig().PluginLanguage;
				if (ImGui.Combo("###PriceCheck_Language_Combo", ref pluginLanguage,
					PluginLanguage.LanguageNames.ToArray(),
					PluginLanguage.LanguageNames.Count))
				{
					_plugin.GetConfig().PluginLanguage = pluginLanguage;
					_plugin.GetConfig().Save();
					_plugin.GetLoc().SetLanguage();
					Result.UpdateLanguage();
				}

				ImGui.Spacing();
				ImGui.Separator();
				ImGui.Text(Loc.Localize("Keybind", "Keybind"));
				ImGui.Separator();
				ImGui.Spacing();

				var keybindEnabled = _plugin.GetConfig().KeybindEnabled;
				if (ImGui.Checkbox(Loc.Localize("KeybindEnabled", "Enabled") + "###PriceCheck_KeybindEnabled_Checkbox",
					ref keybindEnabled))
				{
					_plugin.GetConfig().KeybindEnabled = keybindEnabled;
					_plugin.GetConfig().Save();
				}

				ImGui.Spacing();
				ImGui.Text(Loc.Localize("ModifierKeybind", "Modifier"));
				var modifierKey = ModifierKey.EnumToIndex(_plugin.GetConfig().ModifierKey);
				if (ImGui.Combo("###PriceCheck_ModifierKey_Combo", ref modifierKey, ModifierKey.Names.ToArray(),
					ModifierKey.Names.Length))
				{
					_plugin.GetConfig().ModifierKey = ModifierKey.IndexToEnum(modifierKey);
					_plugin.GetConfig().Save();
				}

				ImGui.Spacing();
				ImGui.Text(Loc.Localize("PrimaryKeybind", "Primary"));
				var primaryKey = PrimaryKey.EnumToIndex(_plugin.GetConfig().PrimaryKey);
				if (ImGui.Combo("###PriceCheck_PrimaryKey_Combo", ref primaryKey, PrimaryKey.Names.ToArray(),
					PrimaryKey.Names.Length))
				{
					_plugin.GetConfig().PrimaryKey = PrimaryKey.IndexToEnum(primaryKey);
					_plugin.GetConfig().Save();
				}

				ImGui.Spacing();
				ImGui.Separator();
				ImGui.Text(Loc.Localize("Thresholds", "Thresholds"));
				ImGui.Separator();

				ImGui.Spacing();
				ImGui.Text(Loc.Localize("MinimumPrice", "Minimum Price"));
				var minPrice = _plugin.GetConfig().MinPrice;
				if (ImGui.SliderInt("###PriceCheck_MinPrice_Slider", ref minPrice, 0, 20000))
				{
					_plugin.GetConfig().MinPrice = minPrice;
					_plugin.GetConfig().Save();
				}

				ImGui.Spacing();
				ImGui.Text(Loc.Localize("MaxUploadDays", "Max Upload Days"));
				var maxUploadDays = _plugin.GetConfig().MaxUploadDays;
				if (ImGui.SliderInt("###PriceCheck_MaxUploadDays_Slider", ref maxUploadDays, 0, 365))
				{
					_plugin.GetConfig().MaxUploadDays = maxUploadDays;
					_plugin.GetConfig().Save();
				}

				ImGui.Spacing();
				ImGui.Text(Loc.Localize("MaxItems", "Max Items in Overlay"));
				var maxItemsInOverlay = _plugin.GetConfig().MaxItemsInOverlay;
				if (ImGui.SliderInt("###PriceCheck_MaxItems_Slider", ref maxItemsInOverlay, 0, 30))
				{
					_plugin.GetConfig().MaxItemsInOverlay = maxItemsInOverlay;
					_plugin.GetConfig().Save();
				}

				ImGui.Spacing();
			}

			ImGui.End();
		}
	}
}