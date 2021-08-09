using System;

namespace PriceCheck
{
    /// <inheritdoc />
    public class PluginUIBase : IDisposable
    {
        /// <summary>
        /// Overlay window.
        /// </summary>
        public OverlayWindow OverlayWindow = null!;

        /// <summary>
        /// Settings window.
        /// </summary>
        public SettingsWindow SettingsWindow = null!;

        private readonly PriceCheckPlugin plugin;

        /// <summary>
        /// Initializes a new instance of the <see cref="PluginUIBase"/> class.
        /// </summary>
        /// <param name="plugin">price check plugin.</param>
        protected PluginUIBase(PriceCheckPlugin plugin)
        {
            this.plugin = plugin;
            this.BuildWindows();
            this.SetWindowVisibility();
            this.AddEventHandlers();
        }

        /// <inheritdoc />
        public void Dispose()
        {
        }

        /// <summary>
        /// Draw windows.
        /// </summary>
        public void Draw()
        {
            this.OverlayWindow.DrawView();
            this.SettingsWindow.DrawView();
        }

        private void BuildWindows()
        {
            this.OverlayWindow = new OverlayWindow(this.plugin);
            this.SettingsWindow = new SettingsWindow(this.plugin);
        }

        private void SetWindowVisibility()
        {
            this.OverlayWindow.IsVisible = this.plugin.Configuration.ShowOverlay;
            this.SettingsWindow.IsVisible = false;
        }

        private void AddEventHandlers()
        {
            this.SettingsWindow.OnOverlayVisibilityUpdated += this.UpdateOverlayVisibility;
        }

        private void UpdateOverlayVisibility(object sender, bool e)
        {
            this.OverlayWindow.IsVisible = e;
        }
    }
}
