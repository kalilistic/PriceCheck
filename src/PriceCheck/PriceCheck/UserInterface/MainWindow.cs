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
            this.UpdateWindowTitle();
            if (this.plugin.PluginService.ClientState.IsLoggedIn())
            {
                this.OpenOnLogin();
            }
        }

        /// <summary>
        /// Open window on login depending on config.
        /// </summary>
        public void OpenOnLogin()
        {
            if (this.plugin.Configuration.ShowOverlay && this.plugin.Configuration.ShowOverlayOnLogin)
            {
                this.IsOpen = true;
            }
        }

        /// <summary>
        /// Update window title.
        /// </summary>
        public void UpdateWindowTitle()
        {
            if (this.plugin.Configuration.ShowKeybindInTitleBar && this.plugin.Configuration.KeybindEnabled)
            {
                if (this.plugin.Configuration.PrimaryKey.Equals(PrimaryKey.Enum.VkNone))
                {
                    this.WindowName = string.Format(
                        Loc.Localize("TitleBarWithKeybind1", "PriceCheck ({0})"),
                        ModifierKey.Names[ModifierKey.EnumToIndex(this.plugin.Configuration.ModifierKey)]);
                }
                else
                {
                    this.WindowName = string.Format(
                        Loc.Localize("TitleBarWithKeybind2", "PriceCheck ({0}+{1})"),
                        ModifierKey.Names[ModifierKey.EnumToIndex(this.plugin.Configuration.ModifierKey)],
                        PrimaryKey.Names[PrimaryKey.EnumToIndex(this.plugin.Configuration.PrimaryKey)]);
                }
            }
            else
            {
                this.WindowName = "PriceCheck";
            }
        }

        /// <inheritdoc />
        public override void Draw()
        {
            if (!this.plugin.WindowManager.IsOpenByKeybind && this.plugin.Configuration.HideOverlayElapsed != 0 &&
                DateUtil.CurrentTime() - this.plugin.PriceService.LastPriceCheck >
                this.plugin.Configuration.HideOverlayElapsed)
            {
                this.IsOpen = false;
                return;
            }

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
                        ImGui.BeginGroup();
                        ImGui.Columns(2);
                        foreach (var item in items)
                        {
                            if (this.plugin.Configuration.UseOverlayColors)
                            {
                                ImGui.TextColored(item.OverlayColor, item.ItemName);
                                ImGui.NextColumn();
                                ImGui.TextColored(item.OverlayColor, item.Message);
                            }
                            else
                            {
                                ImGui.Text(item.ItemName);
                                ImGui.NextColumn();
                                ImGui.Text(item.Message);
                            }

                            ImGui.NextColumn();
                            ImGui.Separator();
                        }

                        ImGui.EndGroup();
                        if (ImGui.IsItemClicked(ImGuiMouseButton.Right))
                        {
                            ImGui.OpenPopup("###PriceCheck_Overlay_Popup");
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

                if (ImGui.BeginPopup("###PriceCheck_Overlay_Popup"))
                {
                    if (ImGui.MenuItem(
                        Loc.Localize("ClearPriceHistory", "Clear History")))
                    {
                        this.plugin.PriceService.ClearItems();
                    }

                    ImGui.EndPopup();
                }
            }
        }
    }
}
