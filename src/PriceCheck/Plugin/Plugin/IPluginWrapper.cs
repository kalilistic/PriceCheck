using System;
using System.Collections.Generic;
using Lumina.Excel.GeneratedSheets;

namespace PriceCheck
{
	public interface IPluginWrapper
	{
		event EventHandler<ulong> ItemDetected;
		bool IsLocalPlayerReady();
		uint? GetLocalPlayerHomeWorld();
		bool IsKeyBindPressed();
		void Dispose();
		List<Item> GetItems();
		void LogInfo(string messageTemplate);
		void LogInfo(string messageTemplate, params object[] values);
		void LogError(string messageTemplate);
		void LogError(string messageTemplate, params object[] values);
		void LogError(Exception exception, string messageTemplate, params object[] values);
		Configuration GetConfig();
		void PrintItemMessage(PricedItem pricedItem);
		void PrintMessage(string message);
		string GetHQIcon();
		void ExportLocalizable();
		Localization GetLoc();
		void PrintHelpMessage();
	}
}