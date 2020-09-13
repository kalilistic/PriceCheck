// ReSharper disable InvertIf
// ReSharper disable DelegateSubtraction
// ReSharper disable ConditionIsAlwaysTrueOrFalse

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Dalamud.Game.Chat;
using Dalamud.Game.Chat.SeStringHandling;
using Dalamud.Game.Chat.SeStringHandling.Payloads;
using Dalamud.Plugin;
using Lumina.Excel.GeneratedSheets;

namespace PriceCheck
{
	public class PluginWrapper : IPluginWrapper
	{
		private readonly Localization _localization;
		private readonly DalamudPluginInterface _pluginInterface;
		private PluginConfiguration _configuration;
		public uint HomeWorldId;

		public PluginWrapper(DalamudPluginInterface pluginInterface)
		{
			_pluginInterface = pluginInterface;
			LoadConfig();
			_localization = new Localization(this);
			_pluginInterface.Framework.Gui.HoveredItemChanged += HoveredItemChanged;
		}

		public event EventHandler<ulong> ItemDetected;


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

		public void SendEcho(PricedItem pricedItem)
		{
			var payloadList = new List<Payload>
			{
				new TextPayload("[PriceCheck] "),
				new ItemPayload(_pluginInterface.Data, pricedItem.ItemId, pricedItem.IsHQ),
				new TextPayload($"{(char) SeIconChar.LinkMarker}"),
				new TextPayload(" " + pricedItem.DisplayName),
				RawPayload.LinkTerminator,
				new TextPayload(" " + GetRightArrowIcon() + " " + pricedItem.Message)
			};

			var payload = new SeString(payloadList);

			_pluginInterface.Framework.Gui.Chat.PrintChat(new XivChatEntry
			{
				MessageBytes = payload.Encode()
			});
		}

		public string GetHQIcon()
		{
			return GetSeIcon(SeIconChar.HighQuality);
		}

		public bool IsLocalPlayerReady()
		{
			if (_pluginInterface.ClientState?.LocalPlayer == null)
			{
				LogInfo("Local player is not available.");
				return false;
			}

			return true;
		}

		public List<Item> GetItems()
		{
			return _pluginInterface.Data.Excel.GetSheet<Item>().Where(item => item.ItemSearchCategory.Row != 0)
				.ToList();
		}

		public uint? GetLocalPlayerHomeWorld()
		{
			if (HomeWorldId != 0) return HomeWorldId;
			if (!IsLocalPlayerReady()) return null;
			if (_pluginInterface.ClientState.LocalPlayer.HomeWorld == null ||
			    _pluginInterface.ClientState.LocalPlayer.HomeWorld.Id == 0)
			{
				LogInfo("Local player home world is not available.");
				return null;
			}

			HomeWorldId = _pluginInterface.ClientState.LocalPlayer.HomeWorld.Id;
			return HomeWorldId;
		}

		public bool IsKeyBindPressed()
		{
			if (!_configuration.KeybindEnabled) return true;
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
			if (!_configuration.Enabled) return;
			if (itemId == 0) return;
			if (!IsLocalPlayerReady()) return;
			if (!IsKeyBindPressed()) return;
			Task.Run(() => { ItemDetected?.Invoke(this, itemId); });
		}
	}
}