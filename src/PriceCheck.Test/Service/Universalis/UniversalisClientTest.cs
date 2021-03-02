using NUnit.Framework;
using PriceCheck.Mock;

namespace PriceCheck.Test
{
    [TestFixture]
    public class UniversalisTest
    {
        [SetUp]
        public void Setup()
        {
            _client = new UniversalisClient(new MockPriceCheckPlugin());
        }

        [TearDown]
        public void TearDown()
        {
            _client?.Dispose();
        }

        private IUniversalisClient _client;


        [Test]
        public void GetSummary_IT_ReturnsSummary()
        {
            var result = _client.GetMarketBoard(63, 29436);
            Assert.IsNotNull(result);
        }

        [Test]
        public void GetSummary_IT_ReturnsSummaryEmptyItem()
        {
            var result = _client.GetMarketBoard(63, 6760);
            Assert.NotNull(result.LastCheckTime);
        }
    }
}