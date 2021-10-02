using EconomicCalculator.Refactor.Storage;
using EconomicCalculator.Refactor.Storage.Organizations;
using EconomicCalculator.Refactor.Storage.Products;
using EconomicCalculator.Storage;
using EconomicCalculator.Storage.Products;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace EconomicCalculator.Tests.Storage.Organizations
{
    [TestFixture]
    public class TerritoryShould
    {
        private Territory sut;

        private Mock<IPopulationGroup> buyerMock;
        private Guid buyerId = Guid.NewGuid();

        private Mock<IPopulationGroup> sellerMock;
        private Guid sellerId = Guid.NewGuid();

        private Mock<IPlot> plotMock;
        private Guid plotId = Guid.NewGuid();

        [SetUp]
        public void Setup()
        {
            buyerMock = new Mock<IPopulationGroup>();
            buyerMock.Setup(x => x.Id).Returns(buyerId);

            sellerMock = new Mock<IPopulationGroup>();
            sellerMock.Setup(x => x.Id).Returns(sellerId);

            plotMock = new Mock<IPlot>();
            plotMock.Setup(x => x.Id).Returns(plotId);

            sut = new Territory
            {
                Extent = 100,
                Plot = plotMock.Object
            };
        }

        private void AssertCollectionContains(IReadOnlyProductAmountCollection result, Mock<IPlot> plot, double val)
        {
            Assert.That(result.GetProductValue(plot.Object), Is.EqualTo(val));
        }

        #region ConstructorTests

        [Test]
        public void NotThrowAnythingFromConstructor()
        {
            Assert.DoesNotThrow(() => new Territory());
        }

        #endregion ConstructorTests

        #region WaterLevel

        [Test]
        [TestCase(-1)]
        public void ThrowIfWaterLevelIsLessThan0(double val)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => sut.WaterCoverage = val);
        }

        #endregion WaterLevel

        #region Humidity

        [Test]
        [TestCase(-1)]
        public void ThrowIfHumidityIsLessThan0(int val)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => sut.Humidity = val);
        }

        #endregion Humidity

        #region Roughness

        [Test]
        [TestCase(-1)]
        public void ThrowIfRoughnessIsLessThan0(int val)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => sut.Roughness = val);
        }

        #endregion Roughness

        #region Infrastructure

        [Test]
        [TestCase(-1)]
        public void ThrowIfInfrastructureIsLessThan0(double val)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => sut.InfrastructureLevel = val);
        }

        #endregion Infrastructure

        #region BuyLand

        [Test]
        [TestCase(0)]
        [TestCase(-1)]
        public void ThrowArgumentOutOfRangeFromBuyLandWhenAmountIsNotPositive(double val)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => sut.BuyLand(val, buyerMock.Object));
        }

        [Test] 
        public void ThrowArgumentNullFromBuyLandWhenBuyerIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => sut.BuyLand(1, null));
        }

        [Test]
        public void ReturnBoughtLandFromBuyLand()
        {
            // get result
            var result = sut.BuyLand(1, buyerMock.Object);

            // ensure it has the the land in the result.
            AssertCollectionContains(result, plotMock, 1);
        }

        [Test]
        public void ReturnEmptyCollectionWhenNoLandAvailable()
        {
            // Update Available Land
            sut.Ownership.Add(Guid.NewGuid(), 100);

            // get result
            var result = sut.BuyLand(1, buyerMock.Object);

            // ensure it has the the land in the result.
            Assert.That(result.Contains(plotMock.Object), Is.False);
        }

        [Test]
        public void ReturnPartialLandWhenNotEnoughLandAvailable()
        {
            // Update Ownership
            sut.Ownership.Add(Guid.NewGuid(), 99);

            // get result
            var result = sut.BuyLand(2, buyerMock.Object);

            // ensure it has the the land in the result.
            AssertCollectionContains(result, plotMock, 1);
        }

        [Test]
        public void RoundToNearest0point1Acres()
        {
            // get result just below 0.1.
            var result = sut.BuyLand(0.06, buyerMock.Object);

            // Assert 0.1 is in there.
            AssertCollectionContains(result, plotMock, 0.1);
        }

        #endregion BuyLand

        #region UpdateOwnership

        [Test]
        public void ThrowArgumentUNllWhenBuyerIsNullOnUpdateOwnership()
        {
            Assert.Throws<ArgumentNullException>(() => sut.UpdateOwnership(null, 1, sellerMock.Object));
        }

        [Test]
        public void ThrowArgumentNullWhenSellIsNullOnUpdateOwnership()
        {
            Assert.Throws<ArgumentNullException>(() => sut.UpdateOwnership(buyerMock.Object, 1, null));
        }

        [Test]
        [TestCase(0)]
        [TestCase(-1)]
        public void ThrowsArgumentOutOfRangeWhenAcresIsNotPositiveFromUpdateOwnership(double val)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => sut.UpdateOwnership(buyerMock.Object, val, sellerMock.Object));
        }

        [Test]
        public void ThrowsArgumentOutOfRangeWhenAcresIsGreaterThanAvailableLandsOfSeller()
        {
            sut.Ownership[sellerId] = 1;

            Assert.Throws<ArgumentOutOfRangeException>(() => sut.UpdateOwnership(buyerMock.Object, 2, sellerMock.Object));
        }

        [Test]
        public void ThrowsKeyNotFoundWhenSellerDoesNotOwnLand()
        {
            Assert.Throws<KeyNotFoundException>(() => sut.UpdateOwnership(buyerMock.Object, 1, sellerMock.Object));
        }

        [Test]
        public void UpdateOwnershipOfLandFRomSellerToBuyer()
        {
            // Set ownership from seller up.
            sut.Ownership[sellerId] = 10;

            // commence trade
            sut.UpdateOwnership(buyerMock.Object, 5, sellerMock.Object);

            // ensure the trade went through.
            Assert.That(sut.Ownership[buyerId], Is.EqualTo(5));
            Assert.That(sut.Ownership[sellerId], Is.EqualTo(5));
        }

        [Test]
        public void UpdateOwnershipOfLandFRomSellerToBuyerWhenBuyerAlreadyOwns()
        {
            // Set ownership from seller up.
            sut.Ownership[sellerId] = 10;
            sut.Ownership[buyerId] = 10;

            // commence trade
            sut.UpdateOwnership(buyerMock.Object, 5, sellerMock.Object);

            // ensure the trade went through.
            Assert.That(sut.Ownership[buyerId], Is.EqualTo(15));
            Assert.That(sut.Ownership[sellerId], Is.EqualTo(5));
        }

        #endregion UpdateOwnership
    }
}
