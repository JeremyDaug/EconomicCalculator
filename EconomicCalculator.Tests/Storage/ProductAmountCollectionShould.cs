﻿using EconomicCalculator.Storage;
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

        #region SetProductAmount

        [Test]
        public void ThrowNullArgumentIfProductISNull()
        {
            Assert.Throws<ArgumentNullException>(() => sut.SetProductAmount(null, 100));
        }

        [Test]
        public void SetProductAmountInCollection()
        {
            // Set it
            sut.SetProductAmount(ProductMock1.Object, 10);

            // Check it
            Assert.That(sut.GetProductAmount(ProductMock1.Object), Is.EqualTo(10));
        }

        [Test]
        public void SetProductAmountThatAlreadyExists()
        {
            // Add it to the collection
            sut.AddProducts(ProductMock1.Object, 0);

            // CHeck it's in there.
            Assert.That(sut.Products.Any(x => x.Id == ProductMock1.Object.Id), Is.True);
            Assert.That(sut.ProductDict.ContainsKey(ProductMock1.Object.Id), Is.True);
            Assert.That(sut.GetProductAmount(ProductMock1.Object), Is.EqualTo(0));

            // Set it
            sut.SetProductAmount(ProductMock1.Object, 100);

            // Check it updated.
            Assert.That(sut.Products.Any(x => x.Id == ProductMock1.Object.Id), Is.True);
            Assert.That(sut.ProductDict.ContainsKey(ProductMock1.Object.Id), Is.True);
            Assert.That(sut.GetProductAmount(ProductMock1.Object), Is.EqualTo(100));
        }

        #endregion SetProductAmount

        #region AddProducts

        [Test]
        public void ThrowArgumentNullFromAddProducts()
        {
            Assert.Throws(typeof(ArgumentNullException),
                () => sut.AddProducts(null, TestValue1));
        }

        [Test]
        public void ThrowArgumentNullFromAddProductsWhenProductsListIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => sut.AddProducts(null));
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

        [Test]
        public void AddProductsFromAnotherProductAmountCollection()
        {
            // Build the one to add for later use.
            var diff = 10;
            var src = new ProductAmountCollection();
            src.AddProducts(ProductMock1.Object, diff);
            src.AddProducts(ProductMock2.Object, diff);

            // Check that the sut is empty.
            Assert.That(sut.Products.Count, Is.EqualTo(0));
            Assert.That(sut.ProductDict.Count, Is.EqualTo(0));

            // Add the src to the sut
            sut.AddProducts(src);

            // CHeck that it was added properly
            Assert.That(sut.Products.Any(x => x.Id == ProductMock1.Object.Id), Is.True);
            Assert.That(sut.ProductDict.ContainsKey(ProductMock1.Object.Id), Is.True);
            Assert.That(sut.GetProductAmount(ProductMock1.Object), Is.EqualTo(diff));
            Assert.That(sut.GetProductAmount(ProductMock2.Object), Is.EqualTo(diff));

            // add it again
            sut.AddProducts(src);

            // check it again to see it was added twice.
            Assert.That(sut.GetProductAmount(ProductMock1.Object), Is.EqualTo(diff*2));
            Assert.That(sut.GetProductAmount(ProductMock2.Object), Is.EqualTo(diff*2));
        }

        #endregion AddProducts

        #region SubtractProducts

        [Test]
        public void ThrowArgumentNullFromRemoveProducts()
        {
            Assert.Throws(typeof(ArgumentNullException),
                () => sut.AddProducts(null, TestValue1));
        }

        [Test]
        public void RemoveProductToCollectionWithValue()
        {
            // Ensure it's not there.
            Assert.That(sut.Products.Contains(ProductMock1.Object), Is.False);
            Assert.That(sut.ProductDict.ContainsKey(ProductMock1.Object.Id), Is.False);

            // Add it in
            sut.SubtractProducts(ProductMock1.Object, TestValue1);

            // check it's in.
            Assert.That(sut.ProductDict.ContainsKey(ProductMock1.Object.Id), Is.True);
            Assert.That(sut.Products.Any(x => x.Id == ProductMock1.Object.Id), Is.True);
            Assert.That(sut.ProductDict[ProductMock1.Object.Id], Is.EqualTo(-TestValue1));
        }

        [Test]
        public void RemoveProductToCollectionWithExistingValue()
        {
            var more = 10;

            // Check it's empty.
            Assert.That(sut.Products.Contains(ProductMock1.Object), Is.False);
            Assert.That(sut.ProductDict.ContainsKey(ProductMock1.Object.Id), Is.False);

            // add it.
            sut.SubtractProducts(ProductMock1.Object, TestValue1);

            // Check it's been added.
            Assert.That(sut.ProductDict.ContainsKey(ProductMock1.Object.Id), Is.True);
            Assert.That(sut.Products.Any(x => x.Id == ProductMock1.Object.Id), Is.True);
            Assert.That(sut.ProductDict[ProductMock1.Object.Id], Is.EqualTo(-TestValue1));

            // Add to it.
            sut.SubtractProducts(ProductMock1.Object, more);

            // check the addition has been applied.
            Assert.That(sut.ProductDict.ContainsKey(ProductMock1.Object.Id), Is.True);
            Assert.That(sut.Products.Any(x => x.Id == ProductMock1.Object.Id), Is.True);
            Assert.That(sut.ProductDict[ProductMock1.Object.Id], Is.EqualTo(-TestValue1 - more));
        }

        #endregion SubtractProducts

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

        #region GetProductionAmount

        [Test]
        public void ThrowArgumentNullFromGetProductAmountWhenProductIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => sut.GetProductAmount(null));
        }

        [Test]
        public void ThrowsKeyNotFoundFromGetProductAmountWhenProductIsNotInTheCollection()
        {
            Assert.Throws<KeyNotFoundException>(() => sut.GetProductAmount(ProductMock1.Object));
        }

        [Test]
        [TestCase(100)]
        [TestCase(1)]
        [TestCase(50.005)]
        public void GetProductAmountFromCollection(double value)
        {
            sut.AddProducts(ProductMock1.Object, value);

            Assert.That(sut.GetProductAmount(ProductMock1.Object), Is.EqualTo(value));
        }

        #endregion GetProductionAmount

        #region IncludeProduct

        [Test]
        public void ThrowArgumentNullFromIncludeProductWhenProductIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => sut.IncludeProduct(null));
        }

        [Test]
        public void IncludeProductToCollectionWith0Amount()
        {
            // ENsure it wasn't there first
            Assert.That(sut.Products.Any(x => x.Id == ProductMock1.Object.Id), Is.False);
            Assert.That(sut.ProductDict.ContainsKey(ProductMock1.Object.Id), Is.False);

            // include it
            sut.IncludeProduct(ProductMock1.Object);

            // Insure it's there now
            Assert.That(sut.Products.Any(x => x.Id == ProductMock1.Object.Id), Is.True);
            Assert.That(sut.ProductDict.ContainsKey(ProductMock1.Object.Id), Is.True);
            // And it has a value of 0
            Assert.That(sut.GetProductAmount(ProductMock1.Object), Is.EqualTo(0));
        }

        [Test]
        public void NotOverrideExistingItemWhenIncludingItemAlreadyInCollection()
        {
            // Put the product in the collection
            sut.SetProductAmount(ProductMock1.Object, 100);

            // Ensure it's in there correctly.
            Assert.That(sut.Products.Any(x => x.Id == ProductMock1.Object.Id), Is.True);
            Assert.That(sut.ProductDict.ContainsKey(ProductMock1.Object.Id), Is.True);
            Assert.That(sut.GetProductAmount(ProductMock1.Object), Is.EqualTo(100));

            // Include the product
            sut.IncludeProduct(ProductMock1.Object);

            // Ensure it hasn't changed.
            Assert.That(sut.Products.Any(x => x.Id == ProductMock1.Object.Id), Is.True);
            Assert.That(sut.ProductDict.ContainsKey(ProductMock1.Object.Id), Is.True);
            Assert.That(sut.GetProductAmount(ProductMock1.Object), Is.EqualTo(100));

            // Ensure it isn't double booked.
            Assert.That(sut.Products.Count, Is.EqualTo(1));
            Assert.That(sut.ProductDict.Count, Is.EqualTo(1));
        }

        #endregion IncludeProduct

        #region IncludeProducts

        [Test]
        public void ThrowArgumentNullFromIncludeProductsWhenProductsIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => sut.IncludeProducts(null));
        }

        [Test]
        public void AddMultipleProductsToTheListWithValue0()
        {
            // ensure the list is empty.
            Assert.That(sut.Products.Count, Is.EqualTo(0));
            Assert.That(sut.ProductDict.Count, Is.EqualTo(0));

            // make list
            var products = new List<IProduct>
            {
                ProductMock1.Object,
                ProductMock2.Object
            };

            // Add items to list
            sut.IncludeProducts(products);

            // Check that items are in list
            Assert.That(sut.Products.Count, Is.EqualTo(2));
            Assert.That(sut.ProductDict.Count, Is.EqualTo(2));
            Assert.That(sut.Products.Any(x => x.Id == ProductMock1.Object.Id), Is.True);
            Assert.That(sut.ProductDict.ContainsKey(ProductMock1.Object.Id), Is.True);
            Assert.That(sut.Products.Any(x => x.Id == ProductMock2.Object.Id), Is.True);
            Assert.That(sut.ProductDict.ContainsKey(ProductMock2.Object.Id), Is.True);

            // with value 0
            Assert.That(sut.GetProductAmount(ProductMock1.Object), Is.EqualTo(0));
            Assert.That(sut.GetProductAmount(ProductMock2.Object), Is.EqualTo(0));
        }

        [Test]
        public void IncluedEverythingButNotOverwriteExistingValuesInTheCollection()
        {
            var value = 100;
            sut.SetProductAmount(ProductMock1.Object, value);

            // ensure the list is empty.
            Assert.That(sut.Products.Count, Is.EqualTo(1));
            Assert.That(sut.ProductDict.Count, Is.EqualTo(1));
            Assert.That(sut.GetProductAmount(ProductMock1.Object), Is.EqualTo(value));

            // make list
            var products = new List<IProduct>
            {
                ProductMock1.Object,
                ProductMock2.Object
            };

            // Add items to list
            sut.IncludeProducts(products);

            // Check that items are in list
            Assert.That(sut.Products.Count, Is.EqualTo(2));
            Assert.That(sut.ProductDict.Count, Is.EqualTo(2));
            Assert.That(sut.Products.Any(x => x.Id == ProductMock1.Object.Id), Is.True);
            Assert.That(sut.ProductDict.ContainsKey(ProductMock1.Object.Id), Is.True);
            Assert.That(sut.Products.Any(x => x.Id == ProductMock2.Object.Id), Is.True);
            Assert.That(sut.ProductDict.ContainsKey(ProductMock2.Object.Id), Is.True);

            // with value 0
            Assert.That(sut.GetProductAmount(ProductMock1.Object), Is.EqualTo(value));
            Assert.That(sut.GetProductAmount(ProductMock2.Object), Is.EqualTo(0));
        }

        #endregion IncludeProducts

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

            // Check that it was multiplied
            Assert.That(result.GetProductAmount(ProductMock1.Object),
                Is.EqualTo(TestValue1 * scalar));
            Assert.That(result.GetProductAmount(ProductMock2.Object),
                Is.EqualTo(TestValue2 * scalar));

            // ensure the original is unharmed.
            Assert.That(sut.GetProductAmount(ProductMock1.Object), Is.EqualTo(TestValue1));
            Assert.That(sut.GetProductAmount(ProductMock2.Object), Is.EqualTo(TestValue2));
        }

        #endregion Multiply

        #region GetProducts

        [Test]
        public void ThrowArgumentNullFromGetProductsWhenProductsIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => sut.GetProducts(null));
        }

        [Test]
        public void ThrowArgumentNullFromGetProductsWhenProductInListIsNull()
        {
            var values = new List<IProduct> { null, ProductMock1.Object };

            Assert.Throws<ArgumentNullException>(() => sut.GetProducts(values));
        }

        [Test]
        public void ReturnListOfEmptyValuesWhenNoProductsFoundInCollection()
        {
            // Ensure base list is empty
            Assert.That(sut.Products.Count, Is.EqualTo(0));
            Assert.That(sut.ProductDict.Count, Is.EqualTo(0));

            // make desired product list
            var values = new List<IProduct> { ProductMock1.Object, ProductMock2.Object };

            // get product list
            var result = sut.GetProducts(values);

            // ensure that all desired products are there
            Assert.That(result.Products.Count, Is.EqualTo(2));
            Assert.That(result.ProductDict.Count, Is.EqualTo(2));
            Assert.That(result.Products.Any(x => x.Id == ProductMock1.Object.Id), Is.True);
            Assert.That(result.ProductDict.ContainsKey(ProductMock1.Object.Id), Is.True);
            Assert.That(result.Products.Any(x => x.Id == ProductMock2.Object.Id), Is.True);
            Assert.That(result.ProductDict.ContainsKey(ProductMock2.Object.Id), Is.True);

            // ensure all are equal to 0
            Assert.That(result.GetProductAmount(ProductMock1.Object), Is.EqualTo(0));
            Assert.That(result.GetProductAmount(ProductMock2.Object), Is.EqualTo(0));
        }

        [Test]
        public void ReturnListOfCorrectValuesWhenProductsFoundInCollection()
        {
            // Add base items to the list
            sut.SetProductAmount(ProductMock1.Object, TestValue1);
            sut.SetProductAmount(ProductMock2.Object, TestValue2);

            // make desired product list
            var values = new List<IProduct> { ProductMock1.Object, ProductMock2.Object };

            // get product list
            var result = sut.GetProducts(values);

            // ensure that all desired products are there
            Assert.That(result.Products.Count, Is.EqualTo(2));
            Assert.That(result.ProductDict.Count, Is.EqualTo(2));
            Assert.That(result.Products.Any(x => x.Id == ProductMock1.Object.Id), Is.True);
            Assert.That(result.ProductDict.ContainsKey(ProductMock1.Object.Id), Is.True);
            Assert.That(result.Products.Any(x => x.Id == ProductMock2.Object.Id), Is.True);
            Assert.That(result.ProductDict.ContainsKey(ProductMock2.Object.Id), Is.True);

            // ensure all are equal to 0
            Assert.That(result.GetProductAmount(ProductMock1.Object), Is.EqualTo(TestValue1));
            Assert.That(result.GetProductAmount(ProductMock2.Object), Is.EqualTo(TestValue2));
        }

        #endregion GetProducts

        #region Contains

        [Test]
        public void ThrowArgumentNullFromContainsWhenProductIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => sut.Contains(null));
        }

        [Test]
        public void ReturnTrueFromContainsWhenCollectionContainsProduct()
        {
            // Add product to collection
            sut.AddProducts(ProductMock1.Object, 0);

            // chech Contains detects it
            Assert.That(sut.Contains(ProductMock1.Object), Is.True);
        }

        [Test]
        public void ReturnFalseFromContainsWhenCollectionDoesNotContainProduct()
        {
            // Add product to collection
            sut.AddProducts(ProductMock1.Object, 0);

            // chech Contains detects it
            Assert.That(sut.Contains(ProductMock2.Object), Is.False);
        }

        #endregion Contains
    }
}
