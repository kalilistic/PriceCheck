using NUnit.Framework;
using PriceCheck.Mock;

namespace PriceCheck.Test
{
	[TestFixture]
	public class PriceServiceTest
	{
		[SetUp]
		public void Setup()
		{
			_plugin = new MockPluginWrapper();
			_universalisClient = new MockUniversalis();
			_priceService = new PriceService(_plugin, _universalisClient);
		}

		private IPriceService _priceService;
		private IUniversalisClient _universalisClient;
		private MockPluginWrapper _plugin;

		[Test]
		public void AddNewItem_AddsDupe_RemoveOriginal()
		{
			_plugin.OnItemDetected(1);
			_plugin.OnItemDetected(1);
			Assert.AreEqual(1, _plugin.GetItems()[0].RowId);
		}

		[Test]
		public void AddNewItem_AddsItem()
		{
			_plugin.OnItemDetected(1);
			Assert.AreEqual(1, _plugin.GetItems()[0].RowId);
		}

		[Test]
		public void AddNewItem_NonMarketable_NotAdded()
		{
			_plugin.OnItemDetected(3);
			Assert.AreEqual(0, _priceService.GetItems().Count);
		}

		[Test]
		public void AddNewItem_OverMax_RemoveItem()
		{
			for (var i = 0; i < 10; i++) _plugin.OnItemDetected(1);
			var initialCount = _plugin.GetItems().Count;
			_plugin.OnItemDetected(2);
			var finalCount = _plugin.GetItems().Count;
			Assert.AreEqual(initialCount, finalCount);
		}
	}
}