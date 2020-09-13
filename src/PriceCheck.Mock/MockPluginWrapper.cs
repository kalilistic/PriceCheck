using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Lumina.Data;
using Lumina.Excel;
using Lumina.Excel.GeneratedSheets;

namespace PriceCheck.Mock
{
	public class MockPluginWrapper : IPluginWrapper
	{
		[SuppressMessage("CodeQuality", "IDE0052:Remove unread private members", Justification = "<Pending>")]
		private readonly Localization _localization;

		public MockPluginWrapper()
		{
			_localization = new Localization(this);
		}

		public MockConfig Config { get; set; } = new MockConfig();
		public event EventHandler<ulong> ItemDetected;

		public bool IsLocalPlayerReady()
		{
			return true;
		}

		public uint? GetLocalPlayerHomeWorld()
		{
			return 63;
		}

		public bool IsKeyBindPressed()
		{
			return true;
		}

		public void Dispose()
		{
		}

		public List<Item> GetItems()
		{
			return new List<Item>
			{
				new Item
				{
					RowId = 1,
					Name = "Potato",
					ItemSearchCategory = new LazyRow<ItemSearchCategory>(
						null, 1, Language.English),
					PriceLow = 100
				},
				new Item
				{
					RowId = 2,
					Name = "Mango",
					ItemSearchCategory = new LazyRow<ItemSearchCategory>(
						null, 0, Language.English),
					PriceLow = 90000
				},
				new Item
				{
					RowId = 3,
					Name = "Strawberry",
					ItemSearchCategory = new LazyRow<ItemSearchCategory>(
						null, 0, Language.English),
					PriceLow = 25
				},
				new Item
				{
					RowId = 4,
					Name = "Blueberry",
					ItemSearchCategory = new LazyRow<ItemSearchCategory>(
						null, 0, Language.English),
					PriceLow = 300
				},
				new Item
				{
					RowId = 5,
					Name = "Clementine",
					ItemSearchCategory = new LazyRow<ItemSearchCategory>(
						null, 0, Language.English),
					PriceLow = 300
				}
			};
		}

		public void LogInfo(string messageTemplate)
		{
			Log(messageTemplate);
		}

		public void LogInfo(string messageTemplate, params object[] values)
		{
			Log(messageTemplate);
		}

		public void LogError(string messageTemplate)
		{
			Log(messageTemplate);
		}

		public void LogError(string messageTemplate, params object[] values)
		{
			Log(messageTemplate);
		}

		public void LogError(Exception exception, string messageTemplate, params object[] values)
		{
			Log(messageTemplate);
		}

		public Configuration GetConfig()
		{
			return Config;
		}

		public void SendEcho(PricedItem pricedItem)
		{
			Log(pricedItem.DisplayName);
		}

		public string GetHQIcon()
		{
			return " (HQ)";
		}

		public void ExportLocalizable()
		{
			throw new NotImplementedException();
		}

		public Localization GetLoc()
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