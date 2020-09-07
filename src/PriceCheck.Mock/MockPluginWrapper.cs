using System;
using System.Collections.Generic;
using System.Diagnostics;
using Lumina.Data;
using Lumina.Excel;
using Lumina.Excel.GeneratedSheets;

namespace PriceCheck.Mock
{
	public class MockPluginWrapper : IPluginWrapper
	{
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

		public void OnItemDetected(ulong itemId)
		{
			ItemDetected?.Invoke(this, itemId);
		}

		private static void Log(string messageTemplate)
		{
			Trace.WriteLine("LOG: " + messageTemplate);
		}
	}
}