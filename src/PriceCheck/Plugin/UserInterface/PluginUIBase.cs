using System;

namespace PriceCheck
{
	public class PluginUIBase : IDisposable
	{
		public MainWindow MainWindow;
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
			MainWindow = new MainWindow(Plugin);
			OverlayWindow = new OverlayWindow(Plugin, PriceService);
			SettingsWindow = new SettingsWindow(Plugin);
		}

		private void SetWindowVisibility()
		{
			OverlayWindow.IsVisible = Plugin.GetConfig().ShowOverlay;
			MainWindow.IsVisible = false;
			SettingsWindow.IsVisible = false;
		}

		private void AddEventHandlers()
		{
			MainWindow.OverlayVisibilityUpdated += UpdateOverlayVisibility;
			MainWindow.SettingsVisibilityUpdated += UpdateSettingsVisibility;
			SettingsWindow.OverlayVisibilityUpdated += UpdateOverlayVisibility;
		}

		private void UpdateOverlayVisibility(object sender, bool e)
		{
			OverlayWindow.IsVisible = e;
		}

		private void UpdateSettingsVisibility(object sender, bool e)
		{
			SettingsWindow.IsVisible = !SettingsWindow.IsVisible;
		}

		public void Draw()
		{
			MainWindow.DrawWindow();
			OverlayWindow.DrawWindow();
			SettingsWindow.DrawWindow();
		}
	}
}