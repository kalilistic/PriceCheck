using System.Linq;
using System.Numerics;
using CheapLoc;
using ImGuiNET;

namespace PriceCheck
{
	public class OverlayWindow : WindowBase
	{
		private readonly IPriceCheckPlugin _priceCheckPlugin;
		private float _uiScale;

		public OverlayWindow(IPriceCheckPlugin priceCheckPlugin)
		{
			_priceCheckPlugin = priceCheckPlugin;
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
				if (!_priceCheckPlugin.Configuration.Enabled)
					ImGui.Text(Loc.Localize("PluginDisabled", "PriceCheckPlugin is disabled."));
				else
					try
					{
						var items = _priceCheckPlugin.PriceService.GetItems()?.ToList();
						if (items != null && items.Count > 0)
						{
							ImGui.Columns(2);
							foreach (var item in items)
							{
								if (_priceCheckPlugin.Configuration.UseOverlayColors)
								{
									ImGui.TextColored(item.Result.OverlayColor(), item.DisplayName);
									ImGui.NextColumn();
									ImGui.TextColored(item.Result.OverlayColor(), item.Message);
								}
								else
								{
									ImGui.Text(item.DisplayName);
									ImGui.NextColumn();
									ImGui.Text(item.Message);
								}

								ImGui.NextColumn();
								ImGui.Separator();
							}
						}
						else
						{
							ImGui.Text(Loc.Localize("WaitingForItems", "Waiting for items."));
						}
					}
					catch
					{
						// ignored
					}
			}

			ImGui.End();
		}
	}
}