using System;

namespace PriceCheck
{
	public class PluginUIBase : IDisposable
	{
		public OverlayWindow OverlayWindow;
		public IPluginWrapper Plugin;
		public IPriceService PriceService;
		public SettingsWindow SettingsWindow;

		public PluginUIBase(IPluginWrapper plugin, IPriceService priceService)
		{
			Plugin = plugin;
			PriceService = priceService;
			OverlayWindow = new OverlayWindow(Plugin, PriceService);
			SettingsWindow = new SettingsWindow(Plugin);
			OverlayWindow.IsVisible = Plugin.GetConfig().ShowOverlay;
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