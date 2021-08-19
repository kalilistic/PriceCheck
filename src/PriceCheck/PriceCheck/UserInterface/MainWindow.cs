using System.Linq;
using System.Numerics;

using CheapLoc;
using Dalamud.DrunkenToad;
using ImGuiNET;

namespace PriceCheck
{
    /// <summary>
    /// Config window for the plugin.
    /// </summary>
    public class MainWindow : PluginWindow
    {
        private readonly PriceCheckPlugin plugin;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow"/> class.
        /// </summary>
        /// <param name="plugin">PriceCheck plugin.</param>
        public MainWindow(PriceCheckPlugin plugin)
            : base(plugin, "PriceCheck")
        {
            this.plugin = plugin;
            this.Size = new Vector2(300f, 150f);
            this.SizeCondition = ImGuiCond.Appearing;
        }

        /// <inheritdoc />
        public override void Draw()
        {
            if (this.plugin.Configuration.HideOverlayElapsed != 0 &&
                DateUtil.CurrentTime() - this.plugin.PriceService.LastPriceCheck >
                this.plugin.Configuration.HideOverlayElapsed) return;
            if (!this.plugin.Configuration.Enabled)
            {
                ImGui.Text(Loc.Localize("PluginDisabled", "PriceCheckPlugin is disabled."));
            }
            else
            {
                try
                {
                    var items = this.plugin.PriceService.GetItems().ToList();
                    if (items is { Count: > 0 })
                    {
                        ImGui.Columns(2);
                        foreach (var item in items)
                        {
                            if (this.plugin.Configuration.UseOverlayColors)
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
    }
}
