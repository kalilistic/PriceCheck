using System.Numerics;
using ImGuiNET;

namespace PriceCheck
{
	public class OverlayWindow : WindowBase
	{
		private readonly Configuration _configuration;
		private readonly IPriceService _priceService;

		public OverlayWindow(Configuration configuration, IPriceService priceService)
		{
			_configuration = configuration;
			_priceService = priceService;
		}

		public void DrawWindow()
		{
			if (!IsVisible) return;

			ImGui.SetNextWindowSize(new Vector2(375, 330), ImGuiCond.FirstUseEver);
			if (ImGui.Begin("PriceCheck Overlay", ref IsVisible,
				ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse))
			{
				if (!_configuration.Enabled)
				{
					ImGui.Text("Plugin is disabled.");
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
							ImGui.Text(item.Result);
							ImGui.NextColumn();
							ImGui.Separator();
						}
					}
					else
					{
						ImGui.Text("Waiting for items.");
					}
				}
			}

			ImGui.End();
		}
	}
}