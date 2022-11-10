using EconomicSim.Objects.Processes;
using EconomicSim.Objects.Processes.ProcessTags;
using EconomicSim.Objects.Products;
using EconomicSim.Objects.Products.ProductTags;
using EconomicSim.Objects.Technology;
using EconomicSim.Objects.Wants;

namespace EconomicSim.Objects
{
    public static class RequiredItems
    {
        private static Dictionary<string, IProduct> _products = new Dictionary<string, IProduct>();
        private static Dictionary<string, IWant> _wants = new Dictionary<string, IWant>();
        private static Dictionary<string, IProcess> _processes = new Dictionary<string, IProcess>();

        public readonly static IWant Land = new Want
        {
            Id = 0,
            Name = "Land",
            Description = "A space of land, roughly 1/8 of an acre in size."
        };
        
        public readonly static IWant Rest = new Want
        {
            Id = 1,
            Name = "Rest",
            Description = "A time to rest and recuperate from the hardships of life."
        };

        /// <summary>
        /// Space, used or not, which a pop desires/owns.
        /// </summary>
        public readonly static IWant Space = new Want
        {
            Id = 2,
            Name = "Space",
            Description = "Space is needed to do lots of things, and gets used regularly."
        };
        
        /// <summary>
        /// The Extra space which a pop desires, possibly for use, possibly just to feel less cramped.
        /// </summary>
        public readonly static IWant FreeSpace = new Want
        {
            Id = 3,
            Name = "Free Space",
            Description = "Space which isn't being used to store things."
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
            Wants = new Dictionary<IWant, decimal>
            {
                {Land, 1}
                    },
            ProductTags = new List<(ProductTag tags, Dictionary<string, object>? parameters)>
                    {
                        (ProductTag.Fixed, null),
                        (ProductTag.Nondegrading, null),
                        (ProductTag.Storage, new Dictionary<string, object>
                        {
                            { "Type", StorageType.Perfect },
                            { "Volume", 1000 },
                            { "Mass", -1 }
                        } ),
                        (ProductTag.Abstract, null),
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
            Wants = new Dictionary<IWant, decimal>
            {
                {Land, 1}
                    },
            ProductTags = new List<(ProductTag tags, Dictionary<string, object>? parameters)>
            {
                (ProductTag.Fixed, null),
                (ProductTag.Nondegrading, null),
                (ProductTag.Storage, new Dictionary<string, object>
                {
                    { "Type", StorageType.Perfect },
                    { "Volume", 1000 },
                    { "Mass", -1 }
                } ),
                (ProductTag.Abstract, null),
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
            Wants = new Dictionary<IWant, decimal>
            {
                {Land, 1}
                    },
            ProductTags = new List<(ProductTag tags, Dictionary<string, object>? parameters)>
            {
                (ProductTag.Fixed, null),
                (ProductTag.Nondegrading, null),
                (ProductTag.Storage, new Dictionary<string, object>
                {
                    { "Type", StorageType.Perfect },
                    { "Volume", 1000 },
                    { "Mass", -1 }
                } ),
                (ProductTag.Abstract, null),
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
            Wants = new Dictionary<IWant, decimal>
            {
                {Land, 1}
                    },
            ProductTags = new List<(ProductTag tags, Dictionary<string, object>? parameters)>
            {
                (ProductTag.Fixed, null),
                (ProductTag.Nondegrading, null),
                (ProductTag.Storage, new Dictionary<string, object>
                {
                    { "Type", StorageType.Perfect },
                    { "Volume", 1000 },
                    { "Mass", -1 }
                } ),
                (ProductTag.Abstract, null),
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
            Wants = new Dictionary<IWant, decimal>
            {
                {Land, 1}
                    },
            ProductTags = new List<(ProductTag tags, Dictionary<string, object>? parameters)>
            {
                (ProductTag.Fixed, null),
                (ProductTag.Nondegrading, null),
                (ProductTag.Storage, new Dictionary<string, object>
                {
                    { "Type", StorageType.Perfect },
                    { "Volume", 1000 },
                    { "Mass", -1 }
                } ),
                (ProductTag.Abstract, null),
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
            Wants = new Dictionary<IWant, decimal>
            {
                        {Land, 1}
                    },
            ProductTags = new List<(ProductTag tags, Dictionary<string, object>? parameters)>
            {
                (ProductTag.Fixed, null),
                (ProductTag.Nondegrading, null),
                (ProductTag.Storage, new Dictionary<string, object>
                {
                    { "Type", StorageType.Perfect },
                    { "Volume", 1000 },
                    { "Mass", -1 }
                } ),
                (ProductTag.Abstract, null),
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
            Wants = new Dictionary<IWant, decimal>
            {
                {Land, 1}
            },
            ProductTags = new List<(ProductTag tags, Dictionary<string, object>? parameters)>
            {
                (ProductTag.Fixed, null),
                (ProductTag.Nondegrading, null),
                (ProductTag.Storage, new Dictionary<string, object>
                {
                    { "Type", StorageType.Perfect },
                    { "Volume", 1000 },
                    { "Mass", -1 }
                } ),
                (ProductTag.Abstract, null),
            },
            Icon = ""
        };

        public readonly static IProduct Time = new Product
        {
            Name = "Time",
            UnitName = "Hour",
            Quality = 0,
            Bulk = 0,
            Mass = 0,
            Fractional = true,
            ProductTags = new List<(ProductTag tag, Dictionary<string, object>? parameters)>
            {
                (ProductTag.Invariant, null),
                (ProductTag.Service, null)
            }
        };
        
        public readonly static IProduct Shopping = new Product
        {
            Name = "Shopping",
            UnitName = "Block",
            Quality = 0,
            Bulk = 0,
            Mass = 0,
            Fractional = false,
            ProductTags = new List<(ProductTag tag, Dictionary<string, object>? parameters)>
            {
                (ProductTag.Service, null)
            }
        };

        public static readonly IProduct Nothing = new Product
        {
            Name = "Nothing",
            UnitName = "Nothings",
            Quality = 0,
            Bulk = 0,
            Mass = 0,
            Fractional = true,
            ProductTags = new List<(ProductTag tag, Dictionary<string, object>? parameters)>
            {
                (ProductTag.Invariant, null),
                (ProductTag.Service, null)
            }
        };

        public readonly static IProcess ShoppingTime = new Process
        {
            Name = "Shopping",
            VariantName = "Manual",
            Description = "Shopping is a common habit for those in a market. It allows one to find things they cannot produce.",
            ProcessProducts = new List<ProcessProduct>
            {
                new ProcessProduct
                {
                    Product = (Product)Time,
                    Amount = 1, 
                    Part = ProcessPartTag.Input
                },
                new ProcessProduct
                {
                    Product = (Product)Shopping,
                    Amount = 10, 
                    Part = ProcessPartTag.Output
                }
            },
            MinimumTime = 0, 
            Skill = null,
            ProcessTags = new Dictionary<ProcessTag, Dictionary<string, object>?>
            {
                {ProcessTag.Consumption, new Dictionary<string, object>
                {
                    {"Product", Time}
                }}
            }
        };
        
        /*public readonly static IProcess RestTime = new Process
        {
            Name = "Rest",
            Description = "Rest is important, but it still takes time.",
            ProcessProducts = new List<ProcessProduct>
            {
                new ProcessProduct
                {
                    Product = (Product)Time,
                    Amount = 1, 
                    Part = ProcessPartTag.Input
                }
            },
            ProcessWants = new List<ProcessWant>
            {
                new ProcessWant
                {
                    Want = (Want)Rest,
                    Amount = 1, 
                    Part = ProcessPartTag.Output
                }
            },
            MinimumTime = 0, 
            Skill = null,
            ProcessTags = new Dictionary<ProcessTag, Dictionary<string, object>?>
            {
                {ProcessTag.Consumption, new Dictionary<string, object>
                {
                    {"Product", Time}
                }}
            }
        };*/

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
                    // Standard Lands Set
                    // Lands (Wasteland, Marginal, Scrub, Quality, Fertile, Very Fertile)
                    _products[AbstractLand.Name] = AbstractLand;
                    _products[Wasteland.GetName()] = Wasteland;
                    _products[MarginalLand.GetName()] = MarginalLand;
                    _products[Scrubland.GetName()] = Scrubland;
                    _products[QualityLand.GetName()] = QualityLand;
                    _products[FertileLand.GetName()] = FertileLand;
                    _products[VeryFertileLand.GetName()] = VeryFertileLand;
                    _products[Time.GetName()] = Time;
                    _products[Nothing.GetName()] = Nothing;
                    _products[Shopping.GetName()] = Shopping;
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
                    _wants[Rest.Name] = Rest;
                    _wants[Space.Name] = Space;
                    _wants[FreeSpace.Name] = FreeSpace;
                }

                return _wants;
            }
        }

        public static IReadOnlyDictionary<string, IProcess> Processes
        {
            get
            {
                if (!_processes.Any())
                {
                    _processes[ShoppingTime.GetName()] = ShoppingTime;
                    //_processes[RestTime.GetName()] = RestTime;
                }

                return _processes;
            }
        }
    }
}