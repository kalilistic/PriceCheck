using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Lumina.Data;
using Lumina.Excel;
using Lumina.Excel.GeneratedSheets;
using Lumina.Text;

namespace PriceCheck.Mock
{
	public class MockPriceCheckPlugin : IPriceCheckPlugin, IPluginBase
	{
		public MockPriceCheckPlugin()
		{
			PluginName = "PriceCheck";
			Localization = new Localization(this);
			Configuration = new MockConfig();
			PriceService = new MockPriceService();
		}

		public dynamic LoadConfig()
		{
			throw new NotImplementedException();
		}

		void IPluginBase.Dispose()
		{
			throw new NotImplementedException();
		}

		public string PluginFolder()
		{
			throw new NotImplementedException();
		}

		void IPluginBase.UpdateResources()
		{
			throw new NotImplementedException();
		}

		public string PluginVersion()
		{
			throw new NotImplementedException();
		}

		public bool IsLoggedIn()
		{
			throw new NotImplementedException();
		}

		public double ConvertHeightToInches(int raceId, int tribeId, int genderId, int sliderHeight)
		{
			throw new NotImplementedException();
		}

		void IPluginBase.PrintMessage(string message)
		{
			throw new NotImplementedException();
		}

		string IPluginBase.GetSeIcon(SeIconChar seIconChar)
		{
			return " (HQ)";
		}

		uint? IPluginBase.GetLocalPlayerHomeWorld()
		{
			return 63;
		}

		void IPluginBase.LogInfo(string messageTemplate)
		{
			Log(messageTemplate);
		}

		void IPluginBase.LogInfo(string messageTemplate, params object[] values)
		{
			Log(messageTemplate);
		}

		void IPluginBase.LogError(string messageTemplate)
		{
			Log(messageTemplate);
		}

		void IPluginBase.LogError(string messageTemplate, params object[] values)
		{
			Log(messageTemplate);
		}

		void IPluginBase.LogError(Exception exception, string messageTemplate, params object[] values)
		{
			Log(messageTemplate);
		}

		bool IPluginBase.IsKeyPressed(ModifierKey.Enum key)
		{
			throw new NotImplementedException();
		}

		bool IPluginBase.IsKeyPressed(PrimaryKey.Enum key)
		{
			throw new NotImplementedException();
		}

		void IPluginBase.SaveConfig(dynamic config)
		{
			throw new NotImplementedException();
		}

		public IPriceService PriceService { get; }
		public PriceCheckConfig Configuration { get; set; }
		public Localization Localization { get; }
		public string PluginName { get; }
		public event EventHandler<ulong> ItemDetected;

		public void PrintHelpMessage()
		{
			throw new NotImplementedException();
		}

		public Item GetItemById(uint itemId)
		{
			var items = new List<Item>
			{
				new Item
				{
					RowId = 1,
					Name = new SeString(Encoding.ASCII.GetBytes("Potato")),
					ItemSearchCategory = new LazyRow<ItemSearchCategory>(
						null, 1, Language.English),
					PriceLow = 100
				},
				new Item
				{
					RowId = 2,
					Name = new SeString(Encoding.ASCII.GetBytes("Mango")),
					ItemSearchCategory = new LazyRow<ItemSearchCategory>(
						null, 1, Language.English),
					PriceLow = 90000
				},
				new Item
				{
					RowId = 3,
					Name = new SeString(Encoding.ASCII.GetBytes("Strawberry")),
					ItemSearchCategory = new LazyRow<ItemSearchCategory>(
						null, 1, Language.English),
					PriceLow = 25
				},
				new Item
				{
					RowId = 4,
					Name = new SeString(Encoding.ASCII.GetBytes("Blueberry")),
					ItemSearchCategory = new LazyRow<ItemSearchCategory>(
						null, 1, Language.English),
					PriceLow = 300
				},
				new Item
				{
					RowId = 5,
					Name = new SeString(Encoding.ASCII.GetBytes("Clementine")),
					ItemSearchCategory = new LazyRow<ItemSearchCategory>(
						null, 1, Language.English),
					PriceLow = 300
				}
			};
			return items.Find(item => item.RowId == itemId);
		}

		public void PrintItemMessage(PricedItem pricedItem)
		{
			Log(pricedItem.ToString());
		}

		public bool IsKeyBindPressed()
		{
			throw new NotImplementedException();
		}

		void IPriceCheckPlugin.Dispose()
		{
			throw new NotImplementedException();
		}

		public void SaveConfig()
		{
		}

		public void SetupCommands()
		{
			throw new NotImplementedException();
		}

		public void RemoveCommands()
		{
			throw new NotImplementedException();
		}

		public void TogglePriceCheck(string command, string args)
		{
			throw new NotImplementedException();
		}

		public void ToggleConfig(string command, string args)
		{
			throw new NotImplementedException();
		}

		public void ExportLocalizable(string command, string args)
		{
			throw new NotImplementedException();
		}

		void IPriceCheckPlugin.PrintMessage(string message)
		{
			throw new NotImplementedException();
		}

		string IPriceCheckPlugin.GetSeIcon(SeIconChar seIconChar)
		{
			return " (HQ)";
		}

		uint? IPriceCheckPlugin.GetLocalPlayerHomeWorld()
		{
			return 63;
		}

		void IPriceCheckPlugin.LogInfo(string messageTemplate)
		{
			Log(messageTemplate);
		}

		void IPriceCheckPlugin.LogInfo(string messageTemplate, params object[] values)
		{
			Log(messageTemplate);
		}

		void IPriceCheckPlugin.LogError(string messageTemplate)
		{
			Log(messageTemplate);
		}

		void IPriceCheckPlugin.LogError(string messageTemplate, params object[] values)
		{
			Log(messageTemplate);
		}

		void IPriceCheckPlugin.LogError(Exception exception, string messageTemplate, params object[] values)
		{
			Log(messageTemplate);
		}

		bool IPriceCheckPlugin.IsKeyPressed(ModifierKey.Enum key)
		{
			throw new NotImplementedException();
		}

		bool IPriceCheckPlugin.IsKeyPressed(PrimaryKey.Enum key)
		{
			throw new NotImplementedException();
		}

		void IPriceCheckPlugin.SaveConfig(dynamic config)
		{
			throw new NotImplementedException();
		}

		void IPriceCheckPlugin.UpdateResources()
		{
			throw new NotImplementedException();
		}

		private static void Log(string messageTemplate)
		{
			Trace.WriteLine("LOG: " + messageTemplate);
		}

		protected virtual void OnItemDetected(ulong e)
		{
			ItemDetected?.Invoke(this, e);
		}
	}
}