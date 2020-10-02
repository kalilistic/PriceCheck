// ReSharper disable InvertIf
// ReSharper disable DelegateSubtraction
// ReSharper disable ConditionIsAlwaysTrueOrFalse

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CheapLoc;
using Dalamud.Game.Chat.SeStringHandling.Payloads;
using Dalamud.Game.Command;
using Dalamud.Plugin;
using Lumina.Excel.GeneratedSheets;

namespace PriceCheck
{
	public sealed class PriceCheckPlugin : PluginBase, IPriceCheckPlugin
	{
		private List<Item> _items;
		private DalamudPluginInterface _pluginInterface;
		private PluginUI _pluginUI;
		private UniversalisClient _universalisClient;

		public PriceCheckPlugin(string pluginName, DalamudPluginInterface pluginInterface) : base(pluginName,
			pluginInterface)
		{
			Task.Run(() =>
			{
				_pluginInterface = pluginInterface;

				// config
				LoadConfig();

				// services
				_universalisClient = new UniversalisClient(this);
				PriceService = new PriceService(this, _universalisClient);

				// data
				_items = PluginInterface.Data.Excel.GetSheet<Item>()
					.Where(item => item.ItemSearchCategory.Row != 0 && !string.IsNullOrEmpty(item.Name)).ToList();

				// commands
				SetupCommands();

				// localization
				Localization.SetLanguage(Configuration.PluginLanguage);

				// ui
				_pluginUI = new PluginUI(this);
				_pluginInterface.UiBuilder.OnBuildUi += DrawUI;
				_pluginInterface.UiBuilder.OnOpenConfigUi += (sender, args) => DrawConfigUI();

				// finalize
				HandleFreshInstall();
				PluginInterface.Framework.Gui.HoveredItemChanged += HoveredItemChanged;
			});
		}

		public IPriceService PriceService { get; set; }

		public PriceCheckConfig Configuration { get; set; }


		public event EventHandler<ulong> ItemDetected;

		public void PrintHelpMessage()
		{
			PrintMessage(Loc.Localize("HelpMessage",
				"To check prices, hold your keybind and then hover over an item in your inventory or linked in chat. " +
				"You can set your keybind (or disable it) in the PriceCheck settings. The prices are averages from Universalis " +
				"and will not match any specific listings you see on the market board. You can use /pcheck to open the overlay and " +
				"/pcheckconfig to open settings. If you need help, reach out on discord or open an issue on GitHub. If you want to " +
				"help add translations, you can submit updates on Crowdin. Links to both GitHub and Crowdin are available in settings."));
		}

		public List<Item> GetItems()
		{
			return _items;
		}

		public void PrintItemMessage(PricedItem pricedItem)
		{
			var payloadList = BuildMessagePayload();
			if (Configuration.UseChatColors)
				payloadList.Add(new UIForegroundPayload(PluginInterface.Data, pricedItem.Result.ColorKey()));
			payloadList.Add(new ItemPayload(PluginInterface.Data, pricedItem.ItemId, pricedItem.IsHQ));
			payloadList.Add(new TextPayload($"{(char) SeIconChar.LinkMarker}"));
			payloadList.Add(new TextPayload(" " + pricedItem.DisplayName));
			payloadList.Add(RawPayload.LinkTerminator);
			payloadList.Add(new TextPayload(" " + GetSeIcon(SeIconChar.ArrowRight) + " " + pricedItem.Message));
			if (Configuration.UseChatColors)
				payloadList.Add(new UIForegroundPayload(PluginInterface.Data, 0));
			SendMessagePayload(payloadList);
		}

		public bool IsKeyBindPressed()
		{
			if (!Configuration.KeybindEnabled) return true;
			if (Configuration.PrimaryKey == PrimaryKey.Enum.VkNone)
				return IsKeyPressed(Configuration.ModifierKey);
			return IsKeyPressed(Configuration.ModifierKey) &&
			       IsKeyPressed(Configuration.PrimaryKey);
		}

		public override void Dispose()
		{
			RemoveCommands();
			PluginInterface.Framework.Gui.HoveredItemChanged -= HoveredItemChanged;
			_pluginInterface.UiBuilder.OnOpenConfigUi -= (sender, args) => DrawConfigUI();
			_pluginInterface.UiBuilder.OnBuildUi -= DrawUI;
			PriceService.Dispose();
			_universalisClient.Dispose();
			_pluginInterface.Dispose();
		}

		public void SaveConfig()
		{
			SaveConfig(Configuration);
		}


		public void SetupCommands()
		{
			_pluginInterface.CommandManager.AddHandler("/pcheck", new CommandInfo(TogglePriceCheck)
			{
				HelpMessage = "Show price check.",
				ShowInHelp = true
			});
			_pluginInterface.CommandManager.AddHandler("/pricecheck", new CommandInfo(TogglePriceCheck)
			{
				ShowInHelp = false
			});
			_pluginInterface.CommandManager.AddHandler("/pcheckconfig", new CommandInfo(ToggleConfig)
			{
				HelpMessage = "Show price check config.",
				ShowInHelp = true
			});
			_pluginInterface.CommandManager.AddHandler("/pricecheckconfig", new CommandInfo(ToggleConfig)
			{
				ShowInHelp = false
			});
			_pluginInterface.CommandManager.AddHandler("/pcheckexloc", new CommandInfo(ExportLocalizable)
			{
				ShowInHelp = false
			});
		}

		public void RemoveCommands()
		{
			_pluginInterface.CommandManager.RemoveHandler("/pcheck");
			_pluginInterface.CommandManager.RemoveHandler("/pricecheck");
			_pluginInterface.CommandManager.RemoveHandler("/pcheckconfig");
			_pluginInterface.CommandManager.RemoveHandler("/pricecheckconfig");
			_pluginInterface.CommandManager.RemoveHandler("/pcheckexloc");
		}

		public void TogglePriceCheck(string command, string args)
		{
			LogInfo("Running command {0} with args {1}", command, args);
			Configuration.ShowOverlay = !Configuration.ShowOverlay;
			_pluginUI.OverlayWindow.IsVisible = !_pluginUI.OverlayWindow.IsVisible;
		}

		public void ToggleConfig(string command, string args)
		{
			LogInfo("Running command {0} with args {1}", command, args);
			_pluginUI.SettingsWindow.IsVisible = !_pluginUI.SettingsWindow.IsVisible;
		}

		public void ExportLocalizable(string command, string args)
		{
			LogInfo("Running command {0} with args {1}", command, args);
			Localization.ExportLocalizable();
		}

		private void HoveredItemChanged(object sender, ulong itemId)
		{
			if (itemId == 0) return;
			if (!Configuration.Enabled) return;
			if (!PluginInterface.Data.IsDataReady) return;
			if (PluginInterface?.ClientState?.LocalPlayer?.HomeWorld == null) return;
			if (!IsKeyBindPressed()) return;
			Task.Run(() => { ItemDetected?.Invoke(this, itemId); });
		}

		private void HandleFreshInstall()
		{
			if (!Configuration.FreshInstall) return;
			PrintMessage(Loc.Localize("InstallThankYou", "Thank you for installing PriceCheck!"));
			PrintHelpMessage();
			Configuration.FreshInstall = false;
			SaveConfig();
			_pluginUI.SettingsWindow.IsVisible = true;
		}

		private void DrawUI()
		{
			_pluginUI.Draw();
		}

		private void DrawConfigUI()
		{
			_pluginUI.SettingsWindow.IsVisible = true;
		}

		public void LoadConfig()
		{
			try
			{
				Configuration = base.LoadConfig() as PluginConfig ?? new PluginConfig();
			}
			catch (Exception ex)
			{
				LogError("Failed to load config so creating new one.", ex);
				Configuration = new PluginConfig();
				SaveConfig();
			}
		}
	}
}