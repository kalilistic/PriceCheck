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
		}

		public void Dispose()
		{
		}

		public void Draw()
		{
			OverlayWindow.DrawWindow();
			SettingsWindow.DrawWindow();
		}
	}
}