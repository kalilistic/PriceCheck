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

		[TearDown]
		public void TearDown()
		{
			_priceService.Dispose();
		}

		private PriceService _priceService;
		private IUniversalisClient _universalisClient;
		private MockPluginWrapper _plugin;

		[Test]
		public void BuildPricedItemFromId_NQ_ReturnsPricedItem()
		{
			var pricedItem = _priceService.BuildPricedItemFromId(32000);
			Assert.AreEqual(32000, pricedItem.ItemId);
			Assert.AreEqual(32000, pricedItem.RawItemId);
			Assert.AreEqual(false, pricedItem.IsHQ);
		}

		[Test]
		public void BuildPricedItemFromId_HQ_ReturnsPricedItem()
		{
			var pricedItem = _priceService.BuildPricedItemFromId(1000001);
			Assert.AreEqual(1, pricedItem.ItemId);
			Assert.AreEqual(1000001, pricedItem.RawItemId);
			Assert.AreEqual(true, pricedItem.IsHQ);
		}

		[Test]
		public void EnrichWithExcelData_ValidItemId_ReturnsFalse()
		{
			var pricedItem = new PricedItem {ItemId = 1};
			var result = _priceService.EnrichWithExcelData(pricedItem);
			Assert.AreEqual(false, result);
			Assert.AreEqual("Potato", pricedItem.ItemName);
			Assert.AreEqual(100, pricedItem.VendorPrice);
		}

		[Test]
		public void EnrichWithExcelData_InvalidItemId_ReturnsTrue()
		{
			var pricedItem = new PricedItem {ItemId = 4534};
			var result = _priceService.EnrichWithExcelData(pricedItem);
			Assert.AreEqual(true, result);
		}

		[Test]
		public void EnrichWithMarketBoardData_ValidResponseForHQ_ReturnsFalse()
		{
			var pricedItem = new PricedItem {ItemId = 1, IsHQ = true};
			var result = _priceService.EnrichWithMarketBoardData(pricedItem);
			Assert.AreEqual(false, result);
			Assert.AreEqual(1599272449630, pricedItem.LastUpdated);
			Assert.AreEqual(300, pricedItem.AveragePrice);
		}

		[Test]
		public void EnrichWithMarketBoardData_ValidResponseForNQ_ReturnsFalse()
		{
			var pricedItem = new PricedItem {ItemId = 1, IsHQ = false};
			var result = _priceService.EnrichWithMarketBoardData(pricedItem);
			Assert.AreEqual(false, result);
			Assert.AreEqual(1599272449630, pricedItem.LastUpdated);
			Assert.AreEqual(200, pricedItem.AveragePrice);
		}

		[Test]
		public void EnrichWithMarketBoardData_InvalidResponse_ReturnsTrue()
		{
			var pricedItem = new PricedItem {ItemId = 5, IsHQ = false};
			var result = _priceService.EnrichWithMarketBoardData(pricedItem);
			Assert.AreEqual(true, result);
		}

		[Test]
		public void ValidateMarketBoardData_Valid_ReturnsFalse()
		{
			var pricedItem = new PricedItem {ItemId = 1, LastUpdated = 1, AveragePrice = 1};
			var result = _priceService.ValidateMarketBoardData(pricedItem);
			Assert.AreEqual(false, result);
		}

		[Test]
		public void ValidateMarketBoardData_ZeroPrice_ReturnsTrue()
		{
			var pricedItem = new PricedItem {ItemId = 98};
			var result = _priceService.ValidateMarketBoardData(pricedItem);
			Assert.AreEqual(true, result);
		}

		[Test]
		public void ValidateMarketBoardData_ZeroUpload_ReturnsTrue()
		{
			var pricedItem = new PricedItem {ItemId = 97};
			var result = _priceService.ValidateMarketBoardData(pricedItem);
			Assert.AreEqual(true, result);
		}

		[Test]
		public void EvaluateDataAge_IsRecentData_ReturnsFalse()
		{
			var pricedItem = new PricedItem {LastUpdated = 1599272449630};
			_plugin.GetConfig().MaxUploadDays = 360;
			var result = _priceService.EvaluateDataAge(pricedItem);
			Assert.AreEqual(false, result);
		}

		[Test]
		public void EvaluateDataAge_IsOldData_ReturnsTrue()
		{
			var pricedItem = new PricedItem {LastUpdated = 1568300583687};
			_plugin.GetConfig().MaxUploadDays = 30;
			var result = _priceService.EvaluateDataAge(pricedItem);
			Assert.AreEqual(true, result);
		}

		[Test]
		public void CompareVendorPrice_IsCheaperOnVendor_ReturnsFalse()
		{
			var pricedItem = new PricedItem {AveragePrice = 100, VendorPrice = 99};
			var result = _priceService.CompareVendorPrice(pricedItem);
			Assert.AreEqual(false, result);
		}

		[Test]
		public void CompareVendorPrice_IsHigherOnVendor_ReturnsTrue()
		{
			var pricedItem = new PricedItem {AveragePrice = 99, VendorPrice = 100};
			var result = _priceService.CompareVendorPrice(pricedItem);
			Assert.AreEqual(true, result);
		}

		[Test]
		public void CompareVendorPrice_SameOnVendor_ReturnsTrue()
		{
			var pricedItem = new PricedItem {AveragePrice = 100, VendorPrice = 100};
			var result = _priceService.CompareVendorPrice(pricedItem);
			Assert.AreEqual(true, result);
		}

		[Test]
		public void CompareMinPrice_BelowMin_ReturnsTrue()
		{
			var pricedItem = new PricedItem {AveragePrice = 100};
			_plugin.GetConfig().MinPrice = 101;
			var result = _priceService.CompareMinPrice(pricedItem);
			Assert.AreEqual(true, result);
		}

		[Test]
		public void CompareMinPrice_MatchMin_ReturnsFalse()
		{
			var pricedItem = new PricedItem {AveragePrice = 100};
			_plugin.GetConfig().MinPrice = 100;
			var result = _priceService.CompareMinPrice(pricedItem);
			Assert.AreEqual(false, result);
		}

		[Test]
		public void CompareMinPrice_AboveMin_ReturnsFalse()
		{
			var pricedItem = new PricedItem {AveragePrice = 101};
			_plugin.GetConfig().MinPrice = 100;
			var result = _priceService.CompareMinPrice(pricedItem);
			Assert.AreEqual(false, result);
		}

		[Test]
		public void RemoveExistingRecord_NoItems_RemainsZero()
		{
			var pricedItem = new PricedItem {ItemId = 1};
			_priceService.RemoveExistingRecord(pricedItem);
			Assert.AreEqual(0, _priceService.GetItems().Count);
		}

		[Test]
		public void RemoveExistingRecord_ItemExists_SetToZero()
		{
			var pricedItem = new PricedItem {ItemId = 1};
			_priceService.GetItems().Add(pricedItem);
			_priceService.RemoveExistingRecord(pricedItem);
			Assert.AreEqual(0, _priceService.GetItems().Count);
		}

		[Test]
		public void RemoveExistingRecord_ItemNew_RemainsOne()
		{
			var pricedItem = new PricedItem {ItemId = 1};
			_priceService.GetItems().Add(pricedItem);
			var pricedItem2 = new PricedItem {ItemId = 2};
			_priceService.RemoveExistingRecord(pricedItem2);
			Assert.AreEqual(1, _priceService.GetItems().Count);
		}

		[Test]
		public void RemoveItemsOverMax_OverMax_ReducedToMax()
		{
			_plugin.GetConfig().MaxItemsInOverlay = 1;
			var pricedItem = new PricedItem {ItemId = 1};
			var pricedItem2 = new PricedItem {ItemId = 2};
			_priceService.GetItems().Add(pricedItem);
			_priceService.GetItems().Add(pricedItem2);
			_priceService.RemoveItemsOverMax();
			Assert.AreEqual(0, _priceService.GetItems().Count);
		}

		[Test]
		public void DetermineDisplayName_HQ_NameWithHQ()
		{
			var pricedItem = new PricedItem {ItemId = 2, ItemName = "ItemName", IsHQ = true};
			_priceService.SetDisplayName(pricedItem);
			Assert.IsTrue(pricedItem.DisplayName.Contains("HQ"));
		}

		[Test]
		public void ProcessItem_NewItem_AddedToList()
		{
			_plugin.Config.MinPrice = 1;
			_priceService.ProcessItem(this, 1);
			Assert.AreEqual(1, _priceService.GetItems().Count);
		}

		[Test]
		public void ProcessItem_BadItemId_NotAddedToList()
		{
			_priceService.ProcessItem(this, 3424);
			Assert.AreEqual(0, _priceService.GetItems().Count);
		}

		[Test]
		public void ProcessItem_FailedToGetData_FailedToGetData()
		{
			_priceService.ProcessItem(this, 5);
			Assert.AreEqual(Result.FailedToGetData, _priceService.GetItems()[0].Result);
		}

		[Test]
		public void ProcessItem_InvalidMarketData_NoDataAvailable()
		{
			_priceService.ProcessItem(this, 4);
			Assert.AreEqual(Result.NoDataAvailable, _priceService.GetItems()[0].Result);
		}

		[Test]
		public void ProcessItem_OldData_NoRecentDataAvailable()
		{
			_priceService.ProcessItem(this, 3);
			Assert.AreEqual(Result.NoRecentDataAvailable, _priceService.GetItems()[0].Result);
		}

		[Test]
		public void ProcessItem_VendorMore_SellVendorMessage()
		{
			_priceService.ProcessItem(this, 2);
			Assert.AreEqual(Result.BelowVendor, _priceService.GetItems()[0].Result);
		}

		[Test]
		public void ProcessItem_BelowMin_BelowMinimum()
		{
			_plugin.Config.MinPrice = 99999999;
			_priceService.ProcessItem(this, 1);
			Assert.AreEqual(Result.BelowMinimum, _priceService.GetItems()[0].Result);
		}
	}
}