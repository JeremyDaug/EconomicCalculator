using EconomicCalculator.Refactor.Storage.Products;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EconomicCalculator.Refactor.Storage
{
    internal class ProductAmountCollection : IProductAmountCollection, IReadOnlyProductAmountCollection
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

        public int Count() => Products.Count();

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

        public void SubtractProducts(IProduct product, double value)
        {
            AddProducts(product, -value);
        }

        public void AddProducts(IProductAmountCollection products)
        {
            if (products is null)
                throw new ArgumentNullException(nameof(products));

            foreach (var pair in products)
            {
                AddProducts(pair.Item1, pair.Item2);
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

        public double GetProductValue(IProduct product)
        {
            if (product is null)
                throw new ArgumentNullException(nameof(product));

            return ProductDict[product.Id];
        }

        public bool TryGetProductValue(IProduct product, out double sat)
        {
            try
            {
                sat = GetProductValue(product);
            }
            catch (KeyNotFoundException)
            {
                sat = 0;
                return false;
            }
            return true;
        }

        public void IncludeProduct(IProduct product)
        {
            if (product is null)
                throw new ArgumentNullException(nameof(product));

            if (_products.Any(x => x.Id == product.Id))
                return;

            _products.Add(product);
            _productDict[product.Id] = 0;
        }

        public void IncludeProducts(IList<IProduct> products)
        {
            if (products is null)
                throw new ArgumentNullException(nameof(products));

            foreach (var product in products)
            {
                IncludeProduct(product);
            }
        }

        public IProductAmountCollection Multiply(double value)
        {
            var result = new ProductAmountCollection();

            // Copy products over.
            result._products = _products.ToList();

            // Copy values over and multiply.
            result._productDict = _productDict.ToDictionary(x => x.Key, x => x.Value * value);

            return result;
        }

        public IProductAmountCollection GetProducts(IList<IProduct> products)
        {
            if (products is null)
                throw new ArgumentNullException(nameof(products));

            if (products.Any(x => x is null))
                throw new ArgumentNullException("Product in list is null.");

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
            if (product is null)
                throw new ArgumentNullException(nameof(product));

            return ProductDict.ContainsKey(product.Id);
        }

        public IProductAmountCollection Copy()
        {
            return new ProductAmountCollection
            {
                _products = this._products.ToList(),
                _productDict = this._productDict.ToDictionary(x => x.Key, x => x.Value)
            };
        }

        public IProductAmountCollection MultiplyBy(IProductAmountCollection other)
        {
            if (other is null)
                throw new ArgumentNullException(nameof(other));

            // create the result preemptively.
            var result = new ProductAmountCollection();

            // combine through the current and multiply by other if it's there.
            foreach (var product in Products)
            {
                // get first value
                var value = ProductDict[product.Id];

                // check if second is there.
                if (other.Contains(product))
                {
                    // if it is, multiply
                    value *= other.ProductDict[product.Id];
                }
                else // if it isn't there then treat it as 0.
                    value = 0;
                // finalize and add value to collection.
                result.AddProducts(product, value);
            }

            // Go through the other list and ensure we got everything.
            foreach (var product in other.Products)
            {
                // if it's not in there already, then just include it with a value of 0.
                if (!result.Contains(product))
                    result.IncludeProduct(product);
            }

            // return the final result.
            return result;
        }

        public IProductAmountCollection OrderProductsBy(Func<IProduct, object> func)
        {
            var result = new ProductAmountCollection();

            // Copy the list in the order we want.
            result._products = _products.OrderBy(func).ToList();

            // copy the dict over properly.
            result._productDict = _productDict.ToDictionary(x => x.Key, x => x.Value);

            // return result.
            return result;
        }
    }
}
