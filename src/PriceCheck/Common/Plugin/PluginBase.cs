// ReSharper disable UnusedMemberInSuper.Global

using System;
using System.Collections.Generic;
using System.Globalization;
using Dalamud.Configuration;
using Dalamud.Game.Chat;
using Dalamud.Game.Chat.SeStringHandling;
using Dalamud.Game.Chat.SeStringHandling.Payloads;
using Dalamud.Plugin;

namespace PriceCheck
{
	public abstract class PluginBase : IPluginBase
	{
		public readonly DalamudPluginInterface PluginInterface;

		protected PluginBase(string pluginName, DalamudPluginInterface pluginInterface)
		{
			PluginName = pluginName;
			PluginInterface = pluginInterface;
			Localization = new Localization(this);
		}

		public Localization Localization { get; }
		public string PluginName { get; }

		public void SetLanguage(PluginLanguage language)
		{
			Localization.SetLanguage(language);
		}

		public void PrintMessage(string message)
		{
			var payloadList = BuildMessagePayload();
			payloadList.Add(new UIForegroundPayload(PluginInterface.Data, 566));
			payloadList.Add(new TextPayload(message));
			payloadList.Add(new UIForegroundPayload(PluginInterface.Data, 0));
			SendMessagePayload(payloadList);
		}

		public string GetSeIcon(SeIconChar seIconChar)
		{
			return Convert.ToChar(seIconChar, CultureInfo.InvariantCulture)
				.ToString(CultureInfo.InvariantCulture);
		}

		public uint? GetLocalPlayerHomeWorld()
		{
			return PluginInterface.ClientState.LocalPlayer.HomeWorld.Id;
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

		public bool IsKeyPressed(ModifierKey.Enum key)
		{
			return PluginInterface.ClientState.KeyState[(byte) key];
		}

		public bool IsKeyPressed(PrimaryKey.Enum key)
		{
			return PluginInterface.ClientState.KeyState[(byte) key];
		}

		public void SaveConfig(dynamic config)
		{
			PluginInterface.SavePluginConfig((IPluginConfiguration) config);
		}

		public dynamic LoadConfig()
		{
			return PluginInterface.GetPluginConfig();
		}

		public abstract void Dispose();

		protected List<Payload> BuildMessagePayload()
		{
			return new List<Payload>
			{
				new UIForegroundPayload(PluginInterface.Data, 0),
				new TextPayload($"[{PluginName}] ")
			};
		}

		protected void SendMessagePayload(List<Payload> payloadList)
		{
			var payload = new SeString(payloadList);
			PluginInterface.Framework.Gui.Chat.PrintChat(new XivChatEntry
			{
				MessageBytes = payload.Encode()
			});
		}
	}
}