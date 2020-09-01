using System.Linq;
using System.Numerics;
using ImGuiNET;

namespace PriceCheck
{
	public class SettingsWindow : WindowBase
	{
		private readonly Configuration _configuration;

		public SettingsWindow(Configuration configuration)
		{
			_configuration = configuration;
		}

		public void DrawWindow()
		{
			if (!IsVisible) return;

			ImGui.SetNextWindowSizeConstraints(new Vector2(140, 200), new Vector2(float.MaxValue, float.MaxValue));
			if (ImGui.Begin("PriceCheck Settings", ref IsVisible,
				ImGuiWindowFlags.AlwaysAutoResize))
			{
				var enabled = _configuration.Enabled;
				if (ImGui.Checkbox("Enabled##PriceCheckEnabled", ref enabled))
				{
					_configuration.Enabled = enabled;
					_configuration.Save();
				}

				var showOverlay = _configuration.ShowOverlay;
				if (ImGui.Checkbox("Show Overlay##PriceCheckShowOverlay", ref showOverlay))
				{
					_configuration.ShowOverlay = showOverlay;
					_configuration.Save();
				}

				var showInChat = _configuration.ShowInChat;
				if (ImGui.Checkbox("Show in Chat##PriceCheckShowInChat", ref showInChat))
				{
					_configuration.ShowInChat = showInChat;
					_configuration.Save();
				}

				ImGui.Spacing();
				ImGui.Text("Keybind");
				var modifierKey = ModifierKey.EnumToIndex(_configuration.ModifierKey);
				if (ImGui.Combo("##PriceCheckModifierKey", ref modifierKey, ModifierKey.Names.ToArray(),
					ModifierKey.Names.Length))
				{
					_configuration.ModifierKey = ModifierKey.IndexToEnum(modifierKey);
					_configuration.Save();
				}

				var primaryKey = PrimaryKey.EnumToIndex(_configuration.PrimaryKey);
				if (ImGui.Combo("##PriceCheckPrimaryKey", ref primaryKey, PrimaryKey.Names.ToArray(),
					PrimaryKey.Names.Length))
				{
					_configuration.PrimaryKey = PrimaryKey.IndexToEnum(primaryKey);
					_configuration.Save();
				}

				ImGui.Spacing();
				ImGui.Text("Minimum Price");
				var minPrice = _configuration.MinPrice;
				if (ImGui.SliderInt("##PriceCheckMinimumPrice", ref minPrice, 0, 20000))
				{
					_configuration.MinPrice = minPrice;
					_configuration.Save();
				}

				ImGui.Spacing();
				ImGui.Text("Max Upload Days");
				var maxUploadDays = _configuration.MaxUploadDays;
				if (ImGui.SliderInt("##PriceCheckMaxUploadDays", ref maxUploadDays, 0, 365))
				{
					_configuration.MaxUploadDays = maxUploadDays;
					_configuration.Save();
				}

				ImGui.Spacing();
				ImGui.Text("Max Items in Overlay");
				var maxItemsInOverlay = _configuration.MaxItemsInOverlay;
				if (ImGui.SliderInt("##PriceCheckMaxItemsInOverlay", ref maxItemsInOverlay, 0, 30))
				{
					_configuration.MaxItemsInOverlay = maxItemsInOverlay;
					_configuration.Save();
				}

				ImGui.Spacing();
			}

			ImGui.End();
		}
	}
}