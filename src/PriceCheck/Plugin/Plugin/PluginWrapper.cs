// ReSharper disable InvertIf
// ReSharper disable DelegateSubtraction

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Dalamud.Game.Chat;
using Dalamud.Plugin;
using Lumina.Excel.GeneratedSheets;

namespace PriceCheck
{
	public class PluginWrapper : IPluginWrapper
	{
		private readonly PluginConfiguration _configuration;
		private readonly DalamudPluginInterface _pluginInterface;

		public PluginWrapper(DalamudPluginInterface pluginInterface)
		{
			_pluginInterface = pluginInterface;
			_configuration = _pluginInterface.GetPluginConfig() as PluginConfiguration ?? new PluginConfiguration();
			_configuration.Initialize(_pluginInterface);
			_pluginInterface.Framework.Gui.HoveredItemChanged += HoveredItemChanged;
		}

		public event EventHandler<ulong> ItemDetected;

		public Configuration GetConfig()
		{
			return _configuration;
		}

		public void SendEcho(string message)
		{
			_pluginInterface.Framework.Gui.Chat.Print("[PriceCheck] " + message);
		}

		public string GetHQIcon()
		{
			return Convert.ToChar(SeIconChar.HighQuality, CultureInfo.InvariantCulture)
				.ToString(CultureInfo.InvariantCulture);
		}

		public bool IsLocalPlayerReady()
		{
			if (_pluginInterface.ClientState.LocalPlayer == null)
			{
				LogInfo("Local player is not available.");
				return false;
			}

			return true;
		}

		public List<Item> GetItems()
		{
			return _pluginInterface.Data.Excel.GetSheet<Item>().ToList();
		}

		public uint? GetLocalPlayerHomeWorld()
		{
			if (_pluginInterface.ClientState.LocalPlayer.HomeWorld == null ||
			    _pluginInterface.ClientState.LocalPlayer.HomeWorld.Id == 0)
			{
				LogInfo("Local player home world is not available.");
				return null;
			}

			return _pluginInterface.ClientState.LocalPlayer.HomeWorld.Id;
		}

		public bool IsKeyBindPressed()
		{
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