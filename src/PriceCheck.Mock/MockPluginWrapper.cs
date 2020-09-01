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
						null, 1, Language.English)
				},
				new Item
				{
					RowId = 2,
					Name = "Carrot",
					ItemSearchCategory = new LazyRow<ItemSearchCategory>(
						null, 2, Language.English)
				},
				new Item
				{
					RowId = 3,
					Name = "Kiwi",
					ItemSearchCategory = new LazyRow<ItemSearchCategory>(
						null, 0, Language.English)
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
			return new MockConfig();
		}

		public void SendEcho(string message)
		{
			Log(message);
		}

		public string GetHQIcon()
		{
			return "(HQ)";
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