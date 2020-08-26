using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EconomicCalculator.Storage
{
    internal class ProductAmountCollection : IProductAmountCollection
    {
        private List<IProduct> _products;
        private Dictionary<Guid, double> _productDict;

        public IReadOnlyList<IProduct> Products
        {
            get => _products;
        }

        public IReadOnlyDictionary<Guid, double> ProductDict
        {
            get => _productDict;
        }

        public ProductAmountCollection()
        {
            _products = new List<IProduct>();
            _productDict = new Dictionary<Guid, double>();
        }

        public void SetProductAmount(IProduct product, double value)
        {
            if (product is null)
                throw new ArgumentNullException(nameof(product));

            if (Products.Any(x => x.Id == product.Id))
            {
                _productDict[product.Id] = value;
            }
            else
            {
                _products.Add(product);
                _productDict[product.Id] = value;
            }
        }

        public void AddProducts(IProduct product, double value)
        {
            if (product is null)
                throw new ArgumentNullException(nameof(product));
            if (value < 0)
                throw new ArgumentOutOfRangeException(string.Format("{0} must not be less than 0.", value));

            if (ProductDict.ContainsKey(product.Id))
            {
                _productDict[product.Id] += value;
            }
            else
            {
                _products.Add(product);
                _productDict.Add(product.Id, value);
            }
        }

        public void DeleteProduct(IProduct product)
        {
            if (product is null)
                throw new ArgumentNullException(nameof(product));

            if (!ProductDict.ContainsKey(product.Id))
                throw new KeyNotFoundException(string.Format("{0} does not exist in the collection.", nameof(product)));

            _products.RemoveAll(x => x.Id == product.Id);
            _productDict.Remove(product.Id);
        }

        public double GetProductAmount(IProduct toFind)
        {
            if (toFind is null)
                throw new ArgumentNullException(nameof(toFind));

            return ProductDict[toFind.Id];
        }

        public void IncludeProduct(IProduct product)
        {
            if (product is null)
                throw new ArgumentNullException(nameof(product));

            if (_products.Contains(product))
                return;

            _productDict[product.Id] = 0;
        }

        public void RemoveProducts(IProduct product, double value)
        {
            if (product is null)
                throw new ArgumentNullException(nameof(product));
            if (!_products.Contains(product))
                throw new KeyNotFoundException(string.Format("{0} does not exist in the connection.", nameof(product)));
            if (value < 0)
                throw new ArgumentOutOfRangeException(string.Format("{0} cannot be less than 0.", value));

            _productDict[product.Id] -= value;
        }

        public IProductAmountCollection Multiply(double value)
        {
            var result = new ProductAmountCollection();

            // Copy products over.
            result._products = _products;

            // Copy values over and multiply.
            result._productDict = _productDict.ToDictionary(x => x.Key, x => x.Value * value);

            return result;
        }

        public IProductAmountCollection GetProducts(IList<IProduct> products)
        {
            var result = new ProductAmountCollection();

            result._products = new List<IProduct>(products);

            foreach (var product in products)
            {
                if (ProductDict.ContainsKey(product.Id))
                {
                    result._productDict[product.Id] = ProductDict[product.Id];
                }
                else
                {
                    result._productDict[product.Id] = 0;
                }
            }

            return result;
        }

        public IEnumerator<Tuple<IProduct, double>> GetEnumerator()
        {
            foreach (var product in Products)
            {
                yield return new Tuple<IProduct, double>(product, ProductDict[product.Id]);
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public bool Contains(IProduct product)
        {
            return Products.Any(x => x.Id == product.Id);
        }

        public void AddProducts(IProductAmountCollection products)
        {
            foreach(var pair in products)
            {
                AddProducts(pair.Item1, pair.Item2);
            }
        }
    }
}
