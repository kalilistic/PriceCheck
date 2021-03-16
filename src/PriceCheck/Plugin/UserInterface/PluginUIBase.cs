using System;

namespace PriceCheck
{
    public class PluginUIBase : IDisposable
    {
        public OverlayWindow OverlayWindow;
        private readonly IPriceCheckPlugin _priceCheckPlugin;
        public SettingsWindow SettingsWindow;

        protected PluginUIBase(IPriceCheckPlugin priceCheckPlugin)
        {
            _priceCheckPlugin = priceCheckPlugin;
            BuildWindows();
            SetWindowVisibility();
            AddEventHandlers();
        }

        public void Dispose()
        {
        }

        private void BuildWindows()
        {
            OverlayWindow = new OverlayWindow(_priceCheckPlugin);
            SettingsWindow = new SettingsWindow(_priceCheckPlugin);
        }

        private void SetWindowVisibility()
        {
            OverlayWindow.IsVisible = _priceCheckPlugin.Configuration.ShowOverlay;
            SettingsWindow.IsVisible = false;
        }

        private void AddEventHandlers()
        {
            SettingsWindow.OverlayVisibilityUpdated += UpdateOverlayVisibility;
        }

        private void UpdateOverlayVisibility(object sender, bool e)
        {
            OverlayWindow.IsVisible = e;
        }

        public void Draw()
        {
            OverlayWindow.DrawView();
            SettingsWindow.DrawView();
        }
    }
}