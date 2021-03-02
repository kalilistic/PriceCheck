using System;

namespace PriceCheck
{
    public class PluginUIBase : IDisposable
    {
        public OverlayWindow OverlayWindow;
        public IPriceCheckPlugin PriceCheckPlugin;
        public SettingsWindow SettingsWindow;

        public PluginUIBase(IPriceCheckPlugin priceCheckPlugin)
        {
            PriceCheckPlugin = priceCheckPlugin;
            BuildWindows();
            SetWindowVisibility();
            AddEventHandlers();
        }

        public void Dispose()
        {
        }

        private void BuildWindows()
        {
            OverlayWindow = new OverlayWindow(PriceCheckPlugin);
            SettingsWindow = new SettingsWindow(PriceCheckPlugin);
        }

        private void SetWindowVisibility()
        {
            OverlayWindow.IsVisible = PriceCheckPlugin.Configuration.ShowOverlay;
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