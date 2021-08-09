using System.Linq;
using System.Numerics;

using CheapLoc;
using Dalamud.DrunkenToad;
using ImGuiNET;

namespace PriceCheck
{
    /// <summary>
    /// Overlay window.
    /// </summary>
    public class OverlayWindow : WindowBase
    {
        private readonly PriceCheckPlugin priceCheckPlugin;
        private float uiScale;

        /// <summary>
        /// Initializes a new instance of the <see cref="OverlayWindow"/> class.
        /// </summary>
        /// <param name="priceCheckPlugin">price check plugin.</param>
        public OverlayWindow(PriceCheckPlugin priceCheckPlugin)
        {
            this.priceCheckPlugin = priceCheckPlugin;
        }

        /// <inheritdoc/>
        public override void DrawView()
        {
            if (!this.priceCheckPlugin.IsLoggedIn()) return;
            if (!this.IsVisible) return;
            if (this.priceCheckPlugin.Configuration.HideOverlayElapsed != 0 &&
                DateUtil.CurrentTime() - this.priceCheckPlugin.LastPriceCheck >
                this.priceCheckPlugin.Configuration.HideOverlayElapsed) return;
            var isVisible = this.IsVisible;
            this.uiScale = ImGui.GetIO().FontGlobalScale;
            ImGui.SetNextWindowSize(new Vector2(300 * this.uiScale, 150 * this.uiScale), ImGuiCond.FirstUseEver);
            if (ImGui.Begin(
                Loc.Localize("OverlayWindow", "PriceCheck") + "###PriceCheck_Overlay_Window",
                ref isVisible,
                ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse))
            {
                this.IsVisible = isVisible;
                if (!this.priceCheckPlugin.Configuration.Enabled)
                {
                    ImGui.Text(Loc.Localize("PluginDisabled", "PriceCheckPlugin is disabled."));
                }
                else
                {
                    try
                    {
                        var items = this.priceCheckPlugin.PriceService.GetItems().ToList();
                        if (items is { Count: > 0 })
                        {
                            ImGui.Columns(2);
                            foreach (var item in items)
                            {
                                if (this.priceCheckPlugin.Configuration.UseOverlayColors)
                                {
                                    if (item.Result != null)
                                    {
                                        ImGui.TextColored(item.Result.OverlayColor(), item.DisplayName);
                                        ImGui.NextColumn();
                                        ImGui.TextColored(item.Result.OverlayColor(), item.Message);
                                    }
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
            }

            ImGui.End();
        }
    }
}
