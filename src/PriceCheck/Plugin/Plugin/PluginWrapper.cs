// ReSharper disable InvertIf
// ReSharper disable DelegateSubtraction
// ReSharper disable ConditionIsAlwaysTrueOrFalse

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using CheapLoc;
using Dalamud.Game.Chat;
using Dalamud.Game.Chat.SeStringHandling;
using Dalamud.Game.Chat.SeStringHandling.Payloads;
using Dalamud.Plugin;
using Lumina.Excel.GeneratedSheets;

namespace PriceCheck
{
	public class PluginWrapper : IPluginWrapper
	{
		private readonly List<Item> _items;
		private readonly Localization _localization;
		private readonly DalamudPluginInterface _pluginInterface;
		private PluginConfiguration _configuration;

		public PluginWrapper(DalamudPluginInterface pluginInterface)
		{
			_pluginInterface = pluginInterface;
			LoadConfig();
			_localization = new Localization(this);
			_items = _pluginInterface.Data.Excel.GetSheet<Item>()
				.Where(item => item.ItemSearchCategory.Row != 0 && !string.IsNullOrEmpty(item.Name)).ToList();
			_pluginInterface.Framework.Gui.HoveredItemChanged += HoveredItemChanged;
		}

		public event EventHandler<ulong> ItemDetected;

		public void PrintHelpMessage()
		{
			PrintMessage(Loc.Localize("HelpMessage",
				"To check prices, hold your keybind and then hover over an item in your inventory or linked in chat. You can set your keybind (or disable it) in the PriceCheck settings. The prices are averages from Universalis and will not match any specific listings you see on the market board. You can use /pcheck to open the overlay and /pcheckconfig to open settings. If you need help, reach out on discord or open an issue on GitHub. If you want to help add translations, you can submit updates on Crowdin. Links to both GitHub and Crowdin are available in settings."));
		}

		public Localization GetLoc()
		{
			return _localization;
		}

		public Configuration GetConfig()
		{
			return _configuration;
		}

		public void ExportLocalizable()
		{
			_localization.ExportLocalizable();
		}

		public void PrintMessage(string message)
		{
			var payloadList = BuildMessagePayload();
			payloadList.Add(new UIForegroundPayload(_pluginInterface.Data, 566));
			payloadList.Add(new TextPayload(message));
			payloadList.Add(new UIForegroundPayload(_pluginInterface.Data, 0));
			SendMessagePayload(payloadList);
		}

		public void PrintItemMessage(PricedItem pricedItem)
		{
			var payloadList = BuildMessagePayload();
			if (_configuration.UseChatColors)
				payloadList.Add(new UIForegroundPayload(_pluginInterface.Data, pricedItem.Result.ColorKey()));
			payloadList.Add(new ItemPayload(_pluginInterface.Data, pricedItem.ItemId, pricedItem.IsHQ));
			payloadList.Add(new TextPayload($"{(char) SeIconChar.LinkMarker}"));
			payloadList.Add(new TextPayload(" " + pricedItem.DisplayName));
			payloadList.Add(RawPayload.LinkTerminator);
			payloadList.Add(new TextPayload(" " + GetRightArrowIcon() + " " + pricedItem.Message));
			if (_configuration.UseChatColors) payloadList.Add(new UIForegroundPayload(_pluginInterface.Data, 0));
			SendMessagePayload(payloadList);
		}

		public string GetHQIcon()
		{
			return GetSeIcon(SeIconChar.HighQuality);
		}

		public List<Item> GetItems()
		{
			return _items;
		}

		public uint? GetLocalPlayerHomeWorld()
		{
			return _pluginInterface.ClientState.LocalPlayer.HomeWorld.Id;
		}

		public bool IsKeyBindPressed()
		{
			if (!_configuration.KeybindEnabled) return true;
			if (_configuration.PrimaryKey == PrimaryKey.Enum.VkNone)
				return _pluginInterface.ClientState.KeyState[(byte) _configuration.ModifierKey];
			return _pluginInterface.ClientState.KeyState[(byte) _configuration.ModifierKey] &&
			       _pluginInterface.ClientState.KeyState[(byte) _configuration.PrimaryKey];
		}

		public void LogInfo(string messageTemplate)
		{
			PluginLog.Log(messageTemplate);
		}

		public void LogInfo(string messageTemplate, params object[] values)
		{
			PluginLog.Log(messageTemplate, values);
		}

		public void LogError(string messageTemplate)
		{
			PluginLog.LogError(messageTemplate);
		}

		public void LogError(string messageTemplate, params object[] values)
		{
			PluginLog.LogError(messageTemplate, values);
		}

		public void LogError(Exception exception, string messageTemplate, params object[] values)
		{
			PluginLog.LogError(exception, messageTemplate, values);
		}

		public void Dispose()
		{
			_pluginInterface.Framework.Gui.HoveredItemChanged -= HoveredItemChanged;
		}

		private List<Payload> BuildMessagePayload()
		{
			return new List<Payload>
			{
				new UIForegroundPayload(_pluginInterface.Data, 0),
				new TextPayload("[PriceCheck] ")
			};
		}

		private void SendMessagePayload(List<Payload> payloadList)
		{
			var payload = new SeString(payloadList);
			_pluginInterface.Framework.Gui.Chat.PrintChat(new XivChatEntry
			{
				MessageBytes = payload.Encode()
			});
		}

		private void LoadConfig()
		{
			try
			{
				_configuration = _pluginInterface.GetPluginConfig() as PluginConfiguration ?? new PluginConfiguration();
			}
			catch (Exception)
			{
				LogError("Failed to load config so creating new one.");
				_configuration = new PluginConfiguration();
				_configuration.Save();
			}

			_configuration.Initialize(_pluginInterface);
		}

		public string GetRightArrowIcon()
		{
			return GetSeIcon(SeIconChar.ArrowRight);
		}

		private static string GetSeIcon(SeIconChar seIconChar)
		{
			return Convert.ToChar(seIconChar, CultureInfo.InvariantCulture)
				.ToString(CultureInfo.InvariantCulture);
		}

		private void HoveredItemChanged(object sender, ulong itemId)
		{
			if (itemId == 0) return;
			if (!_configuration.Enabled) return;
			if (!_pluginInterface.Data.IsDataReady) return;
			if (_pluginInterface?.ClientState?.LocalPlayer?.HomeWorld == null) return;
			if (!IsKeyBindPressed()) return;
			Task.Run(() => { ItemDetected?.Invoke(this, itemId); });
		}
	}
}