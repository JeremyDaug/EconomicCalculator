using EconomicCalculator.Helpers;
using EconomicCalculator.Objects.Products;
using EconomicCalculator.Objects.Products.ProductTags;
using EconomicCalculator.Objects.Technology;
using EconomicCalculator.Objects.Wants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EconomicCalculator.Objects
{
    public static class RequiredItems
    {
        private static Dictionary<string, IProduct> _products = new Dictionary<string, IProduct>();
        private static Dictionary<string, IWant> _wants = new Dictionary<string, IWant>();

        public readonly static IWant Land = new Want
        {
            Id = 0,
            Name = "Land",
            Description = "A space of land, roughly 1/8 of an acre in size."
        };

        /// <summary>
        /// Abstract land which all other lands are variations of .
        /// </summary>
        public readonly static IProduct AbstractLand = new Product
        {
            Id = 0,
            Name = "Land",
            VariantName = "",
            UnitName = "1/8 Acre",
            Quality = 0,
            Mass = 0,
            Bulk = 0,
            Fractional = false,
            Wants = new List<(IWant want, decimal amount)>
                    {
                        (Land, 1)
                    },
            _productTags = new Dictionary<ProductTag, List<object>>
                    {
                        {ProductTag.Fixed, null},
                        {ProductTag.Nondegrading, null},
                        {ProductTag.Storage, new List<object>{"All", 1000, -1} },
                        {ProductTag.Abstract, null},
                    },
            Icon = ""
        };

        public readonly static IProduct Wasteland = new Product
        {
            Id = 1,
            Name = "Land",
            VariantName = "Wasteland",
            UnitName = "1/8 Acre",
            Quality = 0,
            Mass = 0,
            Bulk = 0,
            Fractional = false,
            Wants = new List<(IWant want, decimal amount)>
                    {
                        (Land, 1)
                    },
            _productTags = new Dictionary<ProductTag, List<object>>
                    {
                        {ProductTag.Fixed, null},
                        {ProductTag.Nondegrading, null},
                        {ProductTag.Storage, new List<object>{"All", 1000, -1} }
                    },
            Icon = ""
        };
        public readonly static IProduct MarginalLand = new Product
        {
            Id = 2,
            Name = "Land",
            VariantName = "Marginal Land",
            UnitName = "1/8 Acre",
            Quality = 1,
            Mass = 0,
            Bulk = 0,
            Fractional = false,
            Wants = new List<(IWant want, decimal amount)>
                    {
                        (Land, 1)
                    },
            _productTags = new Dictionary<ProductTag, List<object>>
                    {
                        {ProductTag.Fixed, null},
                        {ProductTag.Nondegrading, null},
                        {ProductTag.Storage, new List<object>{"All", 1000, -1} }
                    },
            Icon = ""
        };
        public readonly static IProduct Scrubland = new Product
        {
            Id = 3,
            Name = "Land",
            VariantName = "Scrubland",
            UnitName = "1/8 Acre",
            Quality = 2,
            Mass = 0,
            Bulk = 0,
            Fractional = false,
            Wants = new List<(IWant want, decimal amount)>
                    {
                        (Land, 1)
                    },
            _productTags = new Dictionary<ProductTag, List<object>>
                    {
                        {ProductTag.Fixed, null},
                        {ProductTag.Nondegrading, null},
                        {ProductTag.Storage, new List<object>{"All", 1000, -1} }
                    },
            Icon = ""
        };
        public readonly static IProduct QualityLand = new Product
        {
            Id = 4,
            Name = "Land",
            VariantName = "Quality Land",
            UnitName = "1/8 Acre",
            Quality = 3,
            Mass = 0,
            Bulk = 0,
            Fractional = false,
            Wants = new List<(IWant want, decimal amount)>
                    {
                        (Land, 1)
                    },
            _productTags = new Dictionary<ProductTag, List<object>>
                    {
                        {ProductTag.Fixed, null},
                        {ProductTag.Nondegrading, null},
                        {ProductTag.Storage, new List<object>{"All", 1000, -1} }
                    },
            Icon = ""
        };
        public readonly static IProduct FertileLand = new Product
        {
            Id = 5,
            Name = "Land",
            VariantName = "Fertile Land",
            UnitName = "1/8 Acre",
            Quality = 4,
            Mass = 0,
            Bulk = 0,
            Fractional = false,
            Wants = new List<(IWant want, decimal amount)>
                    {
                        (Land, 1)
                    },
            _productTags = new Dictionary<ProductTag, List<object>>
                    {
                        {ProductTag.Fixed, null},
                        {ProductTag.Nondegrading, null},
                        {ProductTag.Storage, new List<object>{"All", 1000, -1} }
                    },
            Icon = ""
        };
        public readonly static IProduct VeryFertileLand = new Product
        {
            Id = 6,
            Name = "Land",
            VariantName = "Very Fertile Land",
            UnitName = "1/8 Acre",
            Quality = 5,
            Mass = 0,
            Bulk = 0,
            Fractional = false,
            Wants = new List<(IWant want, decimal amount)>
                    {
                        (Land, 1)
                    },
            _productTags = new Dictionary<ProductTag, List<object>>
                    {
                        {ProductTag.Fixed, null},
                        {ProductTag.Nondegrading, null},
                        {ProductTag.Storage, new List<object>{"All", 1000, -1} }
                    },
            Icon = ""
        };

        /// <summary>
        /// The first technology, the ability to solve problems and learn from the solution.
        /// </summary>
        public readonly static ITechnology Brainstorming = new Technology.Technology
        {
            Id = 0,
            Name = "Brainstorming",
            TechCostBase = 0,
            Tier = 0,
            Description = "The simplist thing an intelligent creature can do. Think of things, find problems, find solutions, and learn from doing so.",
            Category = TechCategory.Primary,
            Families = new List<TechFamily>(),
            Children = new List<Technology.Technology>(),
            Parents = new List<Technology.Technology>()
        };

        public static IReadOnlyDictionary<string, ITechnology> Technologies
        {
            get
            {
                return new Dictionary<string, ITechnology> { { Brainstorming.Name, Brainstorming } };
            }
        }

        /// <summary>
        /// The products available.
        /// </summary>
        public static IReadOnlyDictionary<string, IProduct> Products
        {
            get
            {
                // if nothing in products populate here.
                if (_products.Count == 0)
                {
                    // Timeblock maybe? Not used,
                    // as it's a calculation management/limiter, hard coded, not a product to buy/sell (can't be bought/sold)

                    // Standard Lands Set
                    // Lands (Wasteland, Marginal, Scrub, Quality, Fertile, Very Fertile)
                    _products[AbstractLand.Name] = AbstractLand;
                    _products[Wasteland.VariantName] = Wasteland;
                    _products[MarginalLand.VariantName] = MarginalLand;
                    _products[Scrubland.VariantName] = Scrubland;
                    _products[QualityLand.VariantName] = QualityLand;
                    _products[FertileLand.VariantName] = FertileLand;
                    _products[VeryFertileLand.VariantName] = VeryFertileLand;
                }

                return _products;
            }
        }

        public static IReadOnlyList<(IProduct land, decimal fertility)> LandOptions = new List<(IProduct land, decimal fertility)>
        {
            (Wasteland, 0.2M),
            (MarginalLand, 0.6M),
            (Scrubland, 0.8M),
            (QualityLand, 1M),
            (FertileLand, 1.2M),
            (VeryFertileLand, 1.4M)
        };

        /// <summary>
        /// The wants required by the system.
        /// </summary>
        public static IReadOnlyDictionary<string, IWant> Wants
        {
            get
            {
                if (_wants.Count == 0)
                {
                    _wants[Land.Name] = Land;
                }

                return _wants;
            }
        }
    }
}
