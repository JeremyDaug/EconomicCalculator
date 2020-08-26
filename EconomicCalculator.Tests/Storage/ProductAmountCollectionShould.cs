using EconomicCalculator.Storage;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EconomicCalculator.Tests.Storage
{
    [TestFixture]
    public class ProductAmountCollectionShould
    {
        private ProductAmountCollection sut;

        private Mock<IProduct> ProductMock1;
        private Mock<IProduct> ProductMock2;

        private Guid TestId1;
        private Guid TestId2;

        private const double TestValue1 = 1;
        private const double TestValue2 = 10;

        [SetUp]
        public void Setup()
        {
            ProductMock1 = new Mock<IProduct>();
            ProductMock2 = new Mock<IProduct>();

            TestId1 = Guid.NewGuid();
            TestId2 = Guid.NewGuid();

            ProductMock1.SetupGet(x => x.Id).Returns(TestId1);
            ProductMock2.SetupGet(x => x.Id).Returns(TestId2);

            sut = new ProductAmountCollection();
        }

        #region AddProducts

        [Test]
        public void ThrowArgumentNullFromAddProducts()
        {
            Assert.Throws(typeof(ArgumentNullException),
                () => sut.AddProducts(null, TestValue1));
        }

        [Test]
        public void ThrowArgumentOutOfRangeExceptionFromAddProducts()
        {
            Assert.Throws(typeof(ArgumentOutOfRangeException),
                () => sut.AddProducts(ProductMock1.Object, -1));
        }

        [Test]
        public void AddProductToCollectionWithValue()
        {
            // Ensure it's not there.
            Assert.That(sut.Products.Contains(ProductMock1.Object), Is.False);
            Assert.That(sut.ProductDict.ContainsKey(ProductMock1.Object.Id), Is.False);

            // Add it in
            sut.AddProducts(ProductMock1.Object, TestValue1);

            // check it's in.
            Assert.That(sut.ProductDict.ContainsKey(ProductMock1.Object.Id), Is.True);
            Assert.That(sut.Products.Any(x => x.Id == ProductMock1.Object.Id), Is.True);
            Assert.That(sut.ProductDict[ProductMock1.Object.Id], Is.EqualTo(TestValue1));
        }

        [Test]
        public void AddProductToCollectionWithExistingValue()
        {
            var more = 10;

            // Check it's empty.
            Assert.That(sut.Products.Contains(ProductMock1.Object), Is.False);
            Assert.That(sut.ProductDict.ContainsKey(ProductMock1.Object.Id), Is.False);

            // add it.
            sut.AddProducts(ProductMock1.Object, TestValue1);

            // Check it's been added.
            Assert.That(sut.ProductDict.ContainsKey(ProductMock1.Object.Id), Is.True);
            Assert.That(sut.Products.Any(x => x.Id == ProductMock1.Object.Id), Is.True);
            Assert.That(sut.ProductDict[ProductMock1.Object.Id], Is.EqualTo(TestValue1));

            // Add to it.
            sut.AddProducts(ProductMock1.Object, more);

            // check the addition has been applied.
            Assert.That(sut.ProductDict.ContainsKey(ProductMock1.Object.Id), Is.True);
            Assert.That(sut.Products.Any(x => x.Id == ProductMock1.Object.Id), Is.True);
            Assert.That(sut.ProductDict[ProductMock1.Object.Id], Is.EqualTo(TestValue1+more));
        }

        #endregion AddProducts

        #region DeleteProduct

        [Test]
        public void ThrowArgumentNullFromDeleteProducts()
        {
            Assert.Throws(typeof(ArgumentNullException),
                () => sut.DeleteProduct(null));
        }

        [Test]
        public void ThrowKeyNotFoundFromDeleteProudcts()
        {
            Assert.Throws<KeyNotFoundException>(() => sut.DeleteProduct(ProductMock1.Object));
        }

        [Test]
        public void DeleteProductFromCollection()
        {
            // Add it in
            sut.AddProducts(ProductMock1.Object, TestValue1);

            // Check it's in there
            Assert.That(sut.ProductDict.ContainsKey(ProductMock1.Object.Id), Is.True);
            Assert.That(sut.Products.Any(x => x.Id == ProductMock1.Object.Id), Is.True);
            Assert.That(sut.ProductDict[ProductMock1.Object.Id], Is.EqualTo(TestValue1));

            // Delete it
            sut.DeleteProduct(ProductMock1.Object);

            // check it has been deleted.
            Assert.That(sut.ProductDict.ContainsKey(ProductMock1.Object.Id), Is.False);
            Assert.That(sut.Products.Any(x => x.Id == ProductMock1.Object.Id), Is.False);
        }

        #endregion DeleteProduct

        #region Multiply

        [Test]
        public void MultiplyAndReturnNewProductCollection()
        {
            var scalar = 2;
            // Add to sut
            sut.AddProducts(ProductMock1.Object, TestValue1);
            sut.AddProducts(ProductMock2.Object, TestValue2);

            // Mulitply.
            var result = sut.Multiply(scalar);

            Assert.That(result.GetProductAmount(ProductMock1.Object),
                Is.EqualTo(TestValue1 * scalar));
            Assert.That(result.GetProductAmount(ProductMock2.Object),
                Is.EqualTo(TestValue2 * scalar));
        }

        #endregion Multiply
    }
}
