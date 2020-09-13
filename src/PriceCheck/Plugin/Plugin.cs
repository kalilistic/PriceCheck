// ReSharper disable UnusedMember.Global
// ReSharper disable DelegateSubtraction

using System;
using Dalamud.Plugin;

namespace PriceCheck
{
	public class Plugin : IDalamudPlugin
	{
		private PluginCommandManager<Plugin> _commandManager;
		private PluginWrapper _plugin;

		private DalamudPluginInterface _pluginInterface;
		private PluginUI _pluginUI;
		private PriceService _priceService;
		private UniversalisClient _universalisClient;

		public string Name => "PriceCheck";

		public void Initialize(DalamudPluginInterface pluginInterface)
		{
			_pluginInterface = pluginInterface;
			_plugin = new PluginWrapper(pluginInterface);
			_universalisClient = new UniversalisClient(_plugin);
			_priceService = new PriceService(_plugin, _universalisClient);
			_pluginUI = new PluginUI(_plugin, _priceService);
			_commandManager = new PluginCommandManager<Plugin>(this, _pluginInterface);
			_pluginInterface.UiBuilder.OnBuildUi += DrawUI;
			_pluginInterface.UiBuilder.OnOpenConfigUi += (sender, args) => DrawConfigUI();
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!disposing) return;
			_pluginInterface.UiBuilder.OnOpenConfigUi -= (sender, args) => DrawConfigUI();
			_pluginInterface.UiBuilder.OnBuildUi -= DrawUI;
			_plugin.Dispose();
			_priceService.Dispose();
			_universalisClient.Dispose();
			_commandManager.Dispose();
			_pluginInterface.Dispose();
		}

		private void DrawUI()
		{
			_pluginUI.Draw();
		}

		private void DrawConfigUI()
		{
			_pluginUI.SettingsWindow.IsVisible = true;
		}

		[Command("/pricecheck")]
		[Aliases("/pcheck")]
		[HelpMessage("Show price check overlay.")]
		public void ToggleOverlay(string command, string args)
		{
			_plugin.LogInfo("Running command {0} with args {1}", command, args);
			_plugin.GetConfig().ShowOverlay = true;
			_pluginUI.OverlayWindow.IsVisible = !_pluginUI.OverlayWindow.IsVisible;
		}

		[Command("/pricecheckconfig")]
		[Aliases("/pcheckconfig")]
		[HelpMessage("Show price check config.")]
		public void ToggleConfig(string command, string args)
		{
			_plugin.LogInfo("Running command {0} with args {1}", command, args);
			_pluginUI.SettingsWindow.IsVisible = !_pluginUI.SettingsWindow.IsVisible;
		}

		[Command("/pricecheckexportlocalizable")]
		[Aliases("/pcheckexloc")]
		[DoNotShowInHelp]
		public void ExportLocalizable(string command, string args)
		{
			_plugin.LogInfo("Running command {0} with args {1}", command, args);
			_plugin.ExportLocalizable();
		}
	}
}