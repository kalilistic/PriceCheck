using System;
using Lumina.Excel.GeneratedSheets;

namespace PriceCheck
{
	public interface IPriceCheckPlugin
	{
		IPriceService PriceService { get; }
		PriceCheckConfig Configuration { get; set; }
		Localization Localization { get; }
		string PluginName { get; }
		event EventHandler<ulong> ItemDetected;
		void PrintHelpMessage();
		Item GetItemById(uint itemId);
		void PrintItemMessage(PricedItem pricedItem);
		bool IsKeyBindPressed();
		void Dispose();
		void SaveConfig();
		void SetupCommands();
		void RemoveCommands();
		void TogglePriceCheck(string command, string args);
		void ToggleConfig(string command, string args);
		void ExportLocalizable(string command, string args);
		void PrintMessage(string message);
		string GetSeIcon(SeIconChar seIconChar);
		uint? GetLocalPlayerHomeWorld();
		void LogInfo(string messageTemplate);
		void LogInfo(string messageTemplate, params object[] values);
		void LogError(string messageTemplate);
		void LogError(string messageTemplate, params object[] values);
		void LogError(Exception exception, string messageTemplate, params object[] values);
		bool IsKeyPressed(ModifierKey.Enum key);
		bool IsKeyPressed(PrimaryKey.Enum key);
		void SaveConfig(dynamic config);
		void UpdateResources();
	}
}