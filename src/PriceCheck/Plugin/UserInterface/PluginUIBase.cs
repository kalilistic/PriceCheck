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
			BuildWindows();
			SetWindowVisibility();
			AddEventHandlers();
		}

		public void Dispose()
		{
		}

		private void BuildWindows()
		{
			OverlayWindow = new OverlayWindow(Plugin, PriceService);
			SettingsWindow = new SettingsWindow(Plugin);
		}

		private void SetWindowVisibility()
		{
			OverlayWindow.IsVisible = Plugin.GetConfig().ShowOverlay;
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
			OverlayWindow.DrawWindow();
			SettingsWindow.DrawWindow();
		}
	}
}