using System.Numerics;
using CheapLoc;
using ImGuiNET;

namespace PriceCheck
{
	public class OverlayWindow : WindowBase
	{
		private readonly IPluginWrapper _plugin;
		private readonly IPriceService _priceService;
		private float _uiScale;

		public OverlayWindow(IPluginWrapper plugin, IPriceService priceService)
		{
			_plugin = plugin;
			_priceService = priceService;
		}

		public void DrawWindow()
		{
			if (!IsVisible) return;
			_uiScale = ImGui.GetIO().FontGlobalScale;
			ImGui.SetNextWindowSize(new Vector2(300 * _uiScale, 150 * _uiScale), ImGuiCond.FirstUseEver);
			if (ImGui.Begin(Loc.Localize("OverlayWindow", "PriceCheck Overlay") + "###PriceCheck_Overlay_Window",
				ref IsVisible,
				ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse))
			{
				if (!_plugin.GetConfig().Enabled)
				{
					ImGui.Text(Loc.Localize("PluginDisabled", "Plugin is disabled."));
				}
				else
				{
					var items = _priceService.GetItems();
					if (items != null && items.Count > 0)
					{
						ImGui.Columns(2);
						foreach (var item in items)
						{
							ImGui.Text(item.ItemName);
							ImGui.NextColumn();
							ImGui.Text(item.Message);
							ImGui.NextColumn();
							ImGui.Separator();
						}
					}
					else
					{
						ImGui.Text(Loc.Localize("WaitingForItems", "Waiting for items."));
					}
				}
			}

			ImGui.End();
		}
	}
}