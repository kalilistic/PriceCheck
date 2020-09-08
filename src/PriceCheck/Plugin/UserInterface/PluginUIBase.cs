using System;

namespace PriceCheck
{
	public class PluginUIBase : IDisposable
	{
		public Configuration Configuration;
		public OverlayWindow OverlayWindow;
		public IPriceService PriceService;
		public SettingsWindow SettingsWindow;

		public PluginUIBase(Configuration configuration, IPriceService priceService)
		{
			Configuration = configuration;
			PriceService = priceService;
			OverlayWindow = new OverlayWindow(configuration, priceService);
			SettingsWindow = new SettingsWindow(configuration);
			OverlayWindow.IsVisible = configuration.ShowOverlay;
			SettingsWindow.OverlayVisibilityUpdated += UpdateOverlayVisibility;
		}

		public void Dispose()
		{
		}

		private void UpdateOverlayVisibility(object sender, bool e)
		{
			OverlayWindow.IsVisible = e;
		}

		public void Draw()
		{
			OverlayWindow.DrawWindow();
			SettingsWindow.DrawWindow();
		}
	}
}