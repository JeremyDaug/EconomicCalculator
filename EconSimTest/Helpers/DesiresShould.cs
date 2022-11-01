using EconomicSim.Helpers;
using EconomicSim.Objects;
using EconomicSim.Objects.Market;
using EconomicSim.Objects.Processes;
using EconomicSim.Objects.Products;
using EconomicSim.Objects.Wants;
using Moq;

namespace EconSimTest.Helpers;

public class DesiresShould
{
    private Desires test;
    private Mock<IMarket> MarketMock;

    private List<INeedDesire> TestNeeds;
    private List<IWantDesire> TestWants;

    private Mock<IProduct> ProductMock1;
    private Mock<IProduct> ProductMock2;
    private Mock<IProduct> ProductMock3;
    private Mock<IProduct> ProductMock4;
    private Mock<IProduct> ProductMock5;
    private Mock<IProduct> ProductMock6;
    
    private Mock<IWant> WantMock1;
    private Mock<IWant> WantMock2;
    private Mock<IWant> WantMock3;
    private Mock<IWant> WantMock4;
    private Mock<IWant> WantMock5;
    private Mock<IWant> WantMock6;

    // consumption process mocks
    private Mock<IProcess> P1toW1Con;
    private Mock<IProcess> P2toW2Con;
    private Mock<IProcess> P3toW3Con;
    private Mock<IProcess> P4toW4Con;
    private Mock<IProcess> P5toW5Con;
    private Mock<IProcess> P6toW6Con;
    
    // use process mocks
    private Mock<IProcess> P5toW1Use;
    private Mock<IProcess> P6toW2Use;
    private Mock<IProcess> P1toW3Use;
    private Mock<IProcess> P2toW4Use;
    private Mock<IProcess> P3toW5Use;
    private Mock<IProcess> P4toW6Use;

    private NeedDesire SingleNeedMock1;
    private NeedDesire SingleNeedMock2;
    private NeedDesire StretchedNeedMock1;
    private NeedDesire StretchedNeedMock2;
    private NeedDesire InfiniteNeedMock1;
    private NeedDesire InfiniteNeedMock2;

    private WantDesire SingleWantMock1;
    private WantDesire SingleWantMock2;
    private WantDesire StretchedWantMock1;
    private WantDesire StretchedWantMock2;
    private WantDesire InfiniteWantMock1;
    private WantDesire InfiniteWantMock2;

    private Dictionary<IProduct, decimal> testProperty;
    private List<IProduct> allProducts;

    [SetUp]
    public void Setup()
    {
        MarketMock = new Mock<IMarket>();
        TestNeeds = new List<INeedDesire>();
        TestWants = new List<IWantDesire>();
        ProductMock1 = new Mock<IProduct>();
        ProductMock2 = new Mock<IProduct>();
        ProductMock3 = new Mock<IProduct>();
        ProductMock4 = new Mock<IProduct>();
        ProductMock5 = new Mock<IProduct>();
        ProductMock6 = new Mock<IProduct>();
        WantMock1 = new Mock<IWant>();
        WantMock2 = new Mock<IWant>();
        WantMock3 = new Mock<IWant>();
        WantMock4 = new Mock<IWant>();
        WantMock5 = new Mock<IWant>();
        WantMock6 = new Mock<IWant>();
        P1toW1Con = new Mock<IProcess>();
        P2toW2Con = new Mock<IProcess>();
        P3toW3Con = new Mock<IProcess>();
        P4toW4Con = new Mock<IProcess>();
        P5toW5Con = new Mock<IProcess>();
        P6toW6Con = new Mock<IProcess>();
        P5toW1Use = new Mock<IProcess>();
        P6toW2Use = new Mock<IProcess>();
        P1toW3Use = new Mock<IProcess>();
        P2toW4Use = new Mock<IProcess>();
        P3toW5Use = new Mock<IProcess>();
        P4toW6Use = new Mock<IProcess>();
        SingleNeedMock1 = new NeedDesire
        {
            Product = ProductMock1.Object,
            Amount = 1,
            StartTier = 0
        };
        SingleNeedMock2 = new NeedDesire
        {
            Product = ProductMock2.Object,
            Amount = 1,
            StartTier = 2,
        };
        StretchedNeedMock1 = new NeedDesire
        {
            Product = ProductMock3.Object,
            Amount = 1,
            StartTier = 1,
            Step = 1,
            EndTier = 10,
        };
        StretchedNeedMock2 = new NeedDesire
        {
            Product = ProductMock4.Object,
            Amount = 1,
            StartTier = 10,
            Step = 5,
            EndTier = 50,
        };
        InfiniteNeedMock1 = new NeedDesire
        {
            Product = ProductMock5.Object,
            Amount = 1,
            StartTier = 0,
            Step = 1,
        };
        InfiniteNeedMock2 = new NeedDesire
        {
            Product = ProductMock6.Object,
            Amount = 1,
            StartTier = 10,
            Step = 5,
        };
        SingleWantMock1 = new WantDesire
        {
            Want = WantMock1.Object,
            Amount = 1,
            StartTier = 0,
        };
        SingleWantMock2 = new WantDesire
        {
            Want = WantMock2.Object,
            Amount = 1,
            StartTier = 2,
        };
        StretchedWantMock1 = new WantDesire
        {
            Want = WantMock3.Object,
            Amount = 1,
            StartTier = 1,
            Step = 1,
            EndTier = 10,
        };
        StretchedWantMock2 = new WantDesire
        {
            Want = WantMock4.Object,
            Amount = 1,
            StartTier = 10,
            Step = 5,
            EndTier = 50,
        };
        InfiniteWantMock1 = new WantDesire
        {
            Want = WantMock5.Object,
            Amount = 1,
            StartTier = 0,
            Step = 1,
        };
        InfiniteWantMock2 = new WantDesire
        {
            Want = WantMock6.Object,
            Amount = 1,
            StartTier = 10,
            Step = 5,
        };
        testProperty = new Dictionary<IProduct, decimal>();
        allProducts = new List<IProduct>();

        #region ProductDataSetup
        
        ProductMock1.Setup(x => x.Name).Returns(nameof(ProductMock1));
        ProductMock2.Setup(x => x.Name).Returns(nameof(ProductMock2));
        ProductMock3.Setup(x => x.Name).Returns(nameof(ProductMock3));
        ProductMock4.Setup(x => x.Name).Returns(nameof(ProductMock4));
        ProductMock5.Setup(x => x.Name).Returns(nameof(ProductMock5));
        ProductMock6.Setup(x => x.Name).Returns(nameof(ProductMock6));

        MarketMock.Setup(x => x.GetMarketPrice(ProductMock1.Object)).Returns(10);
        MarketMock.Setup(x => x.GetMarketPrice(ProductMock2.Object)).Returns(20);
        MarketMock.Setup(x => x.GetMarketPrice(ProductMock3.Object)).Returns(15);
        MarketMock.Setup(x => x.GetMarketPrice(ProductMock4.Object)).Returns(1);
        MarketMock.Setup(x => x.GetMarketPrice(ProductMock5.Object)).Returns(50);
        MarketMock.Setup(x => x.GetMarketPrice(ProductMock6.Object)).Returns(100);

        #endregion
        
        #region WantSetups

        #region OwnershipSources
        
        WantMock1.Setup(x => x.OwnershipSources).Returns(new HashSet<IProduct>
        {
            ProductMock3.Object
        });
        ProductMock3.Setup(x => x.Wants)
            .Returns(new Dictionary<IWant, decimal> { { WantMock1.Object, 1 } });
        WantMock2.Setup(x => x.OwnershipSources).Returns(new HashSet<IProduct>
        {
            ProductMock4.Object
        });
        ProductMock4.Setup(x => x.Wants)
            .Returns(new Dictionary<IWant, decimal> { { WantMock2.Object, 2 } });
        WantMock3.Setup(x => x.OwnershipSources).Returns(new HashSet<IProduct>
        {
            ProductMock5.Object
        });
        ProductMock5.Setup(x => x.Wants)
            .Returns(new Dictionary<IWant, decimal> { { WantMock3.Object, 5 } });
        WantMock4.Setup(x => x.OwnershipSources).Returns(new HashSet<IProduct>
        {
            ProductMock6.Object
        });
        ProductMock6.Setup(x => x.Wants)
            .Returns(new Dictionary<IWant, decimal> { { WantMock4.Object, 10 } });
        WantMock5.Setup(x => x.OwnershipSources).Returns(new HashSet<IProduct>
        {
            ProductMock1.Object
        });
        ProductMock1.Setup(x => x.Wants)
            .Returns(new Dictionary<IWant, decimal> { { WantMock5.Object, 0.5m } });
        WantMock6.Setup(x => x.OwnershipSources).Returns(new HashSet<IProduct>
        {
            ProductMock2.Object
        });
        ProductMock2.Setup(x => x.Wants)
            .Returns(new Dictionary<IWant, decimal> { { WantMock6.Object, 0.25m } });
        #endregion
        
        #region ConsumptionSources
        WantMock1.Setup(x => x.ConsumptionSources).Returns(new HashSet<IProduct>
        {
            ProductMock1.Object
        });
        WantMock2.Setup(x => x.ConsumptionSources).Returns(new HashSet<IProduct>
        {
            ProductMock2.Object
        });
        WantMock3.Setup(x => x.ConsumptionSources).Returns(new HashSet<IProduct>
        {
            ProductMock3.Object
        });
        WantMock4.Setup(x => x.ConsumptionSources).Returns(new HashSet<IProduct>
        {
            ProductMock4.Object
        });
        WantMock5.Setup(x => x.ConsumptionSources).Returns(new HashSet<IProduct>
        {
            ProductMock5.Object
        });
        WantMock6.Setup(x => x.ConsumptionSources).Returns(new HashSet<IProduct>
        {
            ProductMock6.Object
        });
        #endregion

        #region UseSources
        WantMock1.Setup(x => x.UseSources).Returns(new HashSet<IProduct>
        {
            ProductMock5.Object
        });
        WantMock2.Setup(x => x.UseSources).Returns(new HashSet<IProduct>
        {
            ProductMock6.Object
        });
        WantMock3.Setup(x => x.UseSources).Returns(new HashSet<IProduct>
        {
            ProductMock1.Object
        });
        WantMock4.Setup(x => x.UseSources).Returns(new HashSet<IProduct>
        {
            ProductMock2.Object
        });
        WantMock5.Setup(x => x.UseSources).Returns(new HashSet<IProduct>
        {
            ProductMock3.Object
        });
        WantMock6.Setup(x => x.UseSources).Returns(new HashSet<IProduct>
        {
            ProductMock4.Object
        });
        #endregion
        
        #endregion

        #region Processes

        P1toW1Con = new Mock<IProcess>();
        P1toW1Con.Setup(x => x.Name).Returns(nameof(P1toW1Con));
        P1toW1Con.Setup(x => x.ProjectedWantAmount(WantMock1.Object, ProcessPartTag.Output))
            .Returns(1);
        P1toW1Con.Setup(x => x.DoProcess())
        P2toW2Con = new Mock<IProcess>();
        P3toW3Con = new Mock<IProcess>();
        P4toW4Con = new Mock<IProcess>();
        P5toW5Con = new Mock<IProcess>();
        P6toW6Con = new Mock<IProcess>();
        
        P1toW3Use = new Mock<IProcess>();
        P2toW4Use = new Mock<IProcess>();
        P3toW5Use = new Mock<IProcess>();
        P4toW6Use = new Mock<IProcess>();
        P5toW1Use = new Mock<IProcess>();
        P6toW2Use = new Mock<IProcess>();

        #endregion
        
        TestNeeds.Add(SingleNeedMock1);
        TestNeeds.Add(SingleNeedMock2);
        TestNeeds.Add(StretchedNeedMock1);
        TestNeeds.Add(StretchedNeedMock2);
        TestNeeds.Add(InfiniteNeedMock1);
        TestNeeds.Add(InfiniteNeedMock2);
        
        TestWants.Add(SingleWantMock1);
        TestWants.Add(SingleWantMock2);
        TestWants.Add(StretchedWantMock1);
        TestWants.Add(StretchedWantMock2);
        TestWants.Add(InfiniteWantMock1);
        TestWants.Add(InfiniteWantMock2);
        
        allProducts.Add(ProductMock1.Object);
        allProducts.Add(ProductMock2.Object);
        allProducts.Add(ProductMock3.Object);
        allProducts.Add(ProductMock4.Object);
        allProducts.Add(ProductMock5.Object);
        allProducts.Add(ProductMock6.Object);

        foreach (var product in allProducts)
            testProperty.Add(product, 10);

        test = new Desires(MarketMock.Object);
    }

    [Test]
    public void CompleteConstructionWithInputDesiresCorrectly()
    {
        test = new Desires(MarketMock.Object, TestNeeds, TestWants);
        
        // check that needs and wants are added correctly.
        foreach (var need in TestNeeds)
        {
            var single = test.Needs.Single(x => Equals(x.Product, need.Product));
            
            Assert.That(single.Product, Is.EqualTo(need.Product));
            Assert.That(single.Amount, Is.EqualTo(need.Amount));
            Assert.That(single.StartTier, Is.EqualTo(need.StartTier));
            Assert.That(single.Step, Is.EqualTo(need.Step));
            Assert.That(single.EndTier, Is.EqualTo(need.EndTier));
        }
        
        foreach (var want in TestWants)
        {
            var single = test.Wants.Single(x => Equals(x.Want, want.Want));
            
            Assert.That(single.Want, Is.EqualTo(want.Want));
            Assert.That(single.Amount, Is.EqualTo(want.Amount));
            Assert.That(single.StartTier, Is.EqualTo(want.StartTier));
            Assert.That(single.Step, Is.EqualTo(want.Step));
            Assert.That(single.EndTier, Is.EqualTo(want.EndTier));
        }
    }

    [Test]
    public void AddDesiresToAppropriateSections()
    {
        test.AddDesire(SingleNeedMock1);
        Assert.That(test.Needs.Any(x => x.Product == SingleNeedMock1.Product), Is.True);
        Assert.That(test.StretchedNeeds.Any(x => x.Product == SingleNeedMock1.Product), Is.False);
        Assert.That(test.InfiniteNeeds.Any(x => x.Product == SingleNeedMock1.Product), Is.False);
        Assert.That(test.DesiredProducts.Contains(SingleNeedMock1.Product), Is.True);
        Assert.That(test.ProductsSatisfied[SingleNeedMock1.Product], Is.EqualTo(0));
        Assert.That(test.ProductTargets[SingleNeedMock1.Product], Is.EqualTo(SingleNeedMock1.Amount));
        
        test.AddDesire(StretchedNeedMock1);
        Assert.That(test.Needs.Any(x => x.Product == StretchedNeedMock1.Product), Is.True);
        Assert.That(test.StretchedNeeds.Any(x => x.Product == StretchedNeedMock1.Product), Is.True);
        Assert.That(test.InfiniteNeeds.Any(x => x.Product == StretchedNeedMock1.Product), Is.False);
        Assert.That(test.DesiredProducts.Contains(StretchedNeedMock1.Product), Is.True);
        Assert.That(test.ProductsSatisfied[StretchedNeedMock1.Product], Is.EqualTo(0));
        Assert.That(test.ProductTargets[StretchedNeedMock1.Product], Is.EqualTo(SingleNeedMock1.TotalDesire()));
        
        test.AddDesire(InfiniteNeedMock1);
        Assert.That(test.Needs.Any(x => x.Product == InfiniteNeedMock1.Product), Is.True);
        Assert.That(test.StretchedNeeds.Any(x => x.Product == InfiniteNeedMock1.Product), Is.True);
        Assert.That(test.InfiniteNeeds.Any(x => x.Product == InfiniteNeedMock1.Product), Is.True);
        Assert.That(test.DesiredProducts.Contains(InfiniteNeedMock1.Product), Is.True);
        Assert.That(test.ProductsSatisfied[InfiniteNeedMock1.Product], Is.EqualTo(0));
        Assert.That(test.ProductTargets[InfiniteNeedMock1.Product], Is.EqualTo(-1));
        
        test.AddDesire(SingleWantMock1);
        Assert.That(test.Wants.Any(x => x.Want == SingleWantMock1.Want), Is.True);
        Assert.That(test.StretchedWants.Any(x => x.Want == SingleWantMock1.Want), Is.False);
        Assert.That(test.InfiniteWants.Any(x => x.Want == SingleWantMock1.Want), Is.False);
        Assert.That(test.DesiredWants.Contains(SingleWantMock1.Want), Is.True);
        Assert.That(test.WantsSatisfied[SingleWantMock1.Want], Is.EqualTo(0));
        Assert.That(test.WantTargets[SingleWantMock1.Want], Is.EqualTo(SingleWantMock1.Amount));
        
        test.AddDesire(StretchedWantMock1);
        Assert.That(test.Wants.Any(x => x.Want == StretchedWantMock1.Want), Is.True);
        Assert.That(test.StretchedWants.Any(x => x.Want == StretchedWantMock1.Want), Is.True);
        Assert.That(test.InfiniteWants.Any(x => x.Want == StretchedWantMock1.Want), Is.False);
        Assert.That(test.DesiredWants.Contains(StretchedWantMock1.Want), Is.True);
        Assert.That(test.WantsSatisfied[StretchedWantMock1.Want], Is.EqualTo(0));
        Assert.That(test.WantTargets[StretchedWantMock1.Want], Is.EqualTo(StretchedWantMock1.TotalDesire()));
        
        test.AddDesire(InfiniteWantMock1);
        Assert.That(test.Wants.Any(x => x.Want == InfiniteWantMock1.Want), Is.True);
        Assert.That(test.StretchedWants.Any(x => x.Want == InfiniteWantMock1.Want), Is.True);
        Assert.That(test.InfiniteWants.Any(x => x.Want == InfiniteWantMock1.Want), Is.True);
        Assert.That(test.DesiredWants.Contains(InfiniteWantMock1.Want), Is.True);
        Assert.That(test.WantsSatisfied[InfiniteWantMock1.Want], Is.EqualTo(0));
        Assert.That(test.WantTargets[InfiniteWantMock1.Want], Is.EqualTo(-1));
    }

    [Test]
    public void ConsolidateEquivalentNeedsCorrectly()
    {
        test.AddDesire(SingleNeedMock1);
        Assert.That(test.Needs.Any(x => x.Product == SingleNeedMock1.Product), Is.True);
        Assert.That(test.StretchedNeeds.Any(x => x.Product == SingleNeedMock1.Product), Is.False);
        Assert.That(test.InfiniteNeeds.Any(x => x.Product == SingleNeedMock1.Product), Is.False);
        Assert.That(test.DesiredProducts.Contains(SingleNeedMock1.Product), Is.True);
        Assert.That(test.ProductsSatisfied[SingleNeedMock1.Product], Is.EqualTo(0));
        Assert.That(test.ProductTargets[SingleNeedMock1.Product], Is.EqualTo(SingleNeedMock1.Amount));
        
        test.AddDesire(SingleNeedMock1);
        var check = test.Needs.Single(x => x.Product == SingleNeedMock1.Product);
        Assert.That(check.Amount, Is.EqualTo(SingleNeedMock1.Amount * 2));
        
        Assert.That(test.Needs.Any(x => x.Product == SingleNeedMock1.Product), Is.True);
        Assert.That(test.StretchedNeeds.Any(x => x.Product == SingleNeedMock1.Product), Is.False);
        Assert.That(test.InfiniteNeeds.Any(x => x.Product == SingleNeedMock1.Product), Is.False);
        Assert.That(test.DesiredProducts.Contains(SingleNeedMock1.Product), Is.True);
        Assert.That(test.ProductsSatisfied[SingleNeedMock1.Product], Is.EqualTo(0));
        Assert.That(test.ProductTargets[SingleNeedMock1.Product], Is.EqualTo(SingleNeedMock1.Amount * 2));
    }

    [Test]
    public void ConsolidateEquivalentWantsCorrectly()
    {
        test.AddDesire(SingleWantMock1);
        Assert.That(test.Wants.Any(x => x.Want == SingleWantMock1.Want), Is.True);
        Assert.That(test.StretchedWants.Any(x => x.Want == SingleWantMock1.Want), Is.False);
        Assert.That(test.InfiniteWants.Any(x => x.Want == SingleWantMock1.Want), Is.False);
        Assert.That(test.DesiredWants.Contains(SingleWantMock1.Want), Is.True);
        Assert.That(test.WantsSatisfied[SingleWantMock1.Want], Is.EqualTo(0));
        Assert.That(test.WantTargets[SingleWantMock1.Want], Is.EqualTo(SingleWantMock1.Amount));
        
        test.AddDesire(SingleWantMock1);
        Assert.That(test.Wants.Any(x => x.Want == SingleWantMock1.Want), Is.True);
        var check = test.Wants.Single(x => x.Want == SingleWantMock1.Want);
        Assert.That(check.Amount, Is.EqualTo(SingleWantMock1.Amount * 2));
        Assert.That(test.StretchedWants.Any(x => x.Want == SingleWantMock1.Want), Is.False);
        Assert.That(test.InfiniteWants.Any(x => x.Want == SingleWantMock1.Want), Is.False);
        Assert.That(test.DesiredWants.Contains(SingleWantMock1.Want), Is.True);
        Assert.That(test.WantsSatisfied[SingleWantMock1.Want], Is.EqualTo(0));
        Assert.That(test.WantTargets[SingleWantMock1.Want], Is.EqualTo(SingleWantMock1.Amount * 2));
    }

    [Test]
    public void WalkUpTiersForNeedsCorrectly()
    {
        var list = new List<INeedDesire>
        {
            SingleNeedMock1,
            StretchedNeedMock1,
            InfiniteNeedMock1
        };

        int count = 0;
        var lastTier = -1001;
        List<string> order = new List<string>();
        foreach (var item in test.WalkUpTiersForNeeds(list))
        {
            order.Add($"{item.desire.Amount} {item.desire.Product}(s) at Tier: {item.tier}");
            if (count > 100)
                break;
            if (item.desire == SingleNeedMock1)
            {
                Assert.That(SingleNeedMock1.StepsOnTier(item.tier), Is.True);
                Assert.That(item.tier, Is.EqualTo(0));
                Assert.That(item.tier, Is.GreaterThanOrEqualTo(lastTier));
            }
            else if (item.desire == StretchedNeedMock1)
            {
                Assert.That(StretchedNeedMock1.StepsOnTier(item.tier), Is.True);
                Assert.That(item.tier, Is.GreaterThanOrEqualTo(item.desire.StartTier));
                Assert.That(item.tier, Is.LessThanOrEqualTo(item.desire.EndTier));
                Assert.That(item.tier, Is.GreaterThanOrEqualTo(lastTier));
            }
            else if (item.desire == InfiniteNeedMock1)
            {
                Assert.That(InfiniteNeedMock1.StepsOnTier(item.tier), Is.True);
                Assert.That(item.tier, Is.GreaterThanOrEqualTo(item.desire.StartTier));
                Assert.That(item.tier, Is.GreaterThanOrEqualTo(lastTier));
            }

            count += 1;
        }

        // var lines = string.Join('\n', order);
        // Console.WriteLine(lines);
    }

    [Test]
    public void WalkUpTiersForWantsCorrectly()
    {
        var list = new List<IWantDesire>
        {
            SingleWantMock1,
            StretchedWantMock1,
            InfiniteWantMock1
        };

        int count = 0;
        var lastTier = -1001;
        List<string> order = new List<string>();
        foreach (var item in test.WalkUpTiersForWants(list))
        {
            order.Add($"{item.desire.Amount} {item.desire.Want}(s) at Tier: {item.tier}");
            if (count > 100)
                break;
            if (item.desire == SingleNeedMock1)
            {
                Assert.That(SingleNeedMock1.StepsOnTier(item.tier), Is.True);
                Assert.That(item.tier, Is.EqualTo(0));
                Assert.That(item.tier, Is.GreaterThanOrEqualTo(lastTier));
            }
            else if (item.desire == StretchedNeedMock1)
            {
                Assert.That(StretchedNeedMock1.StepsOnTier(item.tier), Is.True);
                Assert.That(item.tier, Is.GreaterThanOrEqualTo(item.desire.StartTier));
                Assert.That(item.tier, Is.LessThanOrEqualTo(item.desire.EndTier));
                Assert.That(item.tier, Is.GreaterThanOrEqualTo(lastTier));
            }
            else if (item.desire == InfiniteNeedMock1)
            {
                Assert.That(InfiniteNeedMock1.StepsOnTier(item.tier), Is.True);
                Assert.That(item.tier, Is.GreaterThanOrEqualTo(item.desire.StartTier));
                Assert.That(item.tier, Is.GreaterThanOrEqualTo(lastTier));
            }

            count += 1;
        }
    }

    [TestCase(1, 2)]
    [TestCase(1, 6)]
    [TestCase(1, -100)]
    [TestCase(6, 2)]
    [TestCase(3, 2)]
    public void GiveCorrectEvalForTierDesireEquivalence(int start, int end)
    {
        var diff = test.TierDesireEquivalence(start, end);
        
        Assert.That(diff, Is.EqualTo(Math.Pow(Desires.TierRatio, start - end)));
    }
    
    [Test]
    public void SiftProductIntoDesiresCorrectly()
    {
        var SingleNeedMock1 = new NeedDesire(); // 1 at tier 0
        SingleNeedMock1.Product = ProductMock1.Object;
        SingleNeedMock1.Amount = 1;
        SingleNeedMock1.StartTier = 0;
        
        var SingleNeedMock2 = new NeedDesire(); // 5 at tier 2
        SingleNeedMock2.Product = ProductMock1.Object;
        SingleNeedMock2.Amount = 5;
        SingleNeedMock2.StartTier = 2;
        
        var StretchedNeedMock1 = new NeedDesire(); // 1 per tier (1-10) 10 total
        StretchedNeedMock1.Product = ProductMock1.Object;
        StretchedNeedMock1.Amount = 1;
        StretchedNeedMock1.StartTier = 1;
        StretchedNeedMock1.Step = 1;
        StretchedNeedMock1.EndTier = 10;
        
        var StretchedNeedMock2 = new NeedDesire(); // 1 per 5 tiers (10-50) 9 total
        StretchedNeedMock2.Product = ProductMock1.Object;
        StretchedNeedMock2.Amount = 1;
        StretchedNeedMock2.StartTier = 10;
        StretchedNeedMock2.Step = 5;
        StretchedNeedMock2.EndTier = 50;
        
        var InfiniteNeedMock1 = new NeedDesire(); // 1 per 6 tiers (0+6N)
        InfiniteNeedMock1.Product = ProductMock1.Object;
        InfiniteNeedMock1.Amount = 1;
        InfiniteNeedMock1.StartTier = 0;
        InfiniteNeedMock1.Step = 6;

        var InfiniteNeedMock2 = new NeedDesire(); // 1 per 5 tier (10+5n)
        InfiniteNeedMock2.Product = ProductMock1.Object;
        InfiniteNeedMock2.Amount = 1;
        InfiniteNeedMock2.StartTier = 10;
        InfiniteNeedMock2.Step = 5;

        var TestNeeds = new List<INeedDesire> { SingleNeedMock1, SingleNeedMock2,
            StretchedNeedMock1, StretchedNeedMock2,
            InfiniteNeedMock1, InfiniteNeedMock2
        };

        test = new Desires(MarketMock.Object, TestNeeds, new List<IWantDesire>());

        test.AddProducts(ProductMock1.Object, 100);

        test.SiftProduct(ProductMock1.Object);

        Assert.That(test.ProductsSatisfied[ProductMock1.Object], Is.EqualTo(100));
        
        foreach (var need in test.Needs.Where(x => !x.IsInfinite))
            Assert.That(need.Satisfaction, Is.EqualTo(need.TotalDesire()));

        var S1 = test.Needs.Single(x => x.StartTier == 0 && !x.IsStretched);
        Assert.That(S1.Satisfaction, Is.EqualTo(1));
        
        var S2 = test.Needs.Single(x => x.StartTier == 2 && !x.IsStretched);
        Assert.That(S2.Satisfaction, Is.EqualTo(5));
        
        var S3 = test.Needs.Single(x => x.StartTier == 1 && x.IsStretched && x.EndTier == 10);
        Assert.That(S3.Satisfaction, Is.EqualTo(10));
        
        var S4 = test.Needs.Single(x => x.StartTier == 10 && x.IsStretched && x.EndTier == 50);
        Assert.That(S4.Satisfaction, Is.EqualTo(9));
        
        var S5 = test.Needs.Single(x => x.StartTier == 0 && x.IsInfinite && x.Step == 6);
        Assert.That(S5.Satisfaction, Is.EqualTo(35));
        
        var S6 = test.Needs.Single(x => x.StartTier == 10 && x.IsInfinite && x.Step == 5);
        Assert.That(S6.Satisfaction, Is.EqualTo(40));
    }

    [Test]
    public void PredictWantOutputsCorrectlyAndCorrectlyConsumeProducts()
    {
        var want1 = new Mock<IWant>();
        
        // setup product which supplies via ownership.
        var ownProduct = new Mock<IProduct>();
        ownProduct.Setup(x => x.Name)
            .Returns("Own");
        ownProduct.Setup(x => x.Wants)
            .Returns(new Dictionary<IWant, decimal>
            {
                {want1.Object, 1}
            });
        
        // product which needs to be used.
        var useProduct = new Mock<IProduct>();
        useProduct.Setup(x => x.Name)
            .Returns("Used");
        useProduct.Setup(x => x.Wants)
            .Returns(new Dictionary<IWant, decimal>());
        // setup the process for the use product.
        var useProcess = new Mock<IProcess>();
        useProcess.Setup(x => x.Name)
            .Returns("Using!");
        useProcess.Setup(x => x.ProjectedWantAmount(want1.Object, ProcessPartTag.Output))
            .Returns(1);
        useProcess
            .Setup(x => x.DoProcess(1,
                It.IsAny<Dictionary<IProduct, decimal>>(),
                It.IsAny<Dictionary<IWant, decimal>>()))
            .Returns((1, new Dictionary<IProduct, decimal>(),
                new Dictionary<IProduct, decimal>
                {
                    {useProduct.Object, 1}
                }, 
                new Dictionary<IWant, decimal>
                {
                    {want1.Object, 1}
                }));
        useProcess
            .Setup(x => x.DoProcess(100,
                It.IsAny<Dictionary<IProduct, decimal>>(),
                It.IsAny<Dictionary<IWant, decimal>>()))
            .Returns((100, new Dictionary<IProduct, decimal>(),
                new Dictionary<IProduct, decimal>
                {
                    {useProduct.Object, 100}
                }, 
                new Dictionary<IWant, decimal>
                {
                    {want1.Object, 100}
                }));
        // add the use process into the use product.
        useProduct.Setup(x => x.UseProcess)
            .Returns(useProcess.Object);
        
        // product which needs to be consumed.
        var consumeProduct = new Mock<IProduct>();
        consumeProduct.Setup(x => x.Name)
            .Returns("Consumed");
        consumeProduct.Setup(x => x.Wants)
            .Returns(new Dictionary<IWant, decimal>());
        // create consumption process
        var consumptionProcess = new Mock<IProcess>();
        consumptionProcess.Setup(x => x.Name)
            .Returns("Consuming!");
        consumptionProcess.Setup(x => x.ProjectedWantAmount(want1.Object, ProcessPartTag.Output))
            .Returns(1);
        consumptionProcess
            .Setup(x => x.DoProcess(1,
                It.IsAny<Dictionary<IProduct, decimal>>(),
                It.IsAny<Dictionary<IWant, decimal>>()))
            .Returns((1, new Dictionary<IProduct, decimal>
                {
                    {consumeProduct.Object, -1}
                }, 
                new Dictionary<IProduct, decimal>(),
                new Dictionary<IWant, decimal>
                {
                    {want1.Object, 1}
                }));
        consumptionProcess
            .Setup(x => x.DoProcess(100,
                It.IsAny<Dictionary<IProduct, decimal>>(),
                It.IsAny<Dictionary<IWant, decimal>>()))
            .Returns((100, new Dictionary<IProduct, decimal>
                {
                    {consumeProduct.Object, -100}
                }, 
                new Dictionary<IProduct, decimal>(),
                new Dictionary<IWant, decimal>
                {
                    {want1.Object, 100}
                }));
        consumptionProcess
            .Setup(x => x.ProjectedWantAmount(want1.Object, ProcessPartTag.Output))
            .Returns(1);
        // add the use process into the use product.
        consumeProduct.Setup(x => x.ConsumptionProcess)
            .Returns(consumptionProcess.Object);
        want1.Setup(x => x.ConsumptionSources)
            .Returns(new HashSet<IProduct>
            {
                consumeProduct.Object
            });

        want1.Setup(x => x.OwnershipSources)
            .Returns(new HashSet<IProduct>
            {
                ownProduct.Object
            });
        want1.Setup(x => x.UseSources)
            .Returns(new HashSet<IProduct>
            {
                useProduct.Object
            });
        want1.Setup(x => x.ConsumptionSources)
            .Returns(new HashSet<IProduct>
            {
                consumeProduct.Object
            });

        var needDesire1 = new NeedDesire
        {
            Product = consumeProduct.Object,
            Amount = 1,
            Step = 1,
            StartTier = 0,
        };
        var needDesire2 = new NeedDesire
        {
            Product = useProduct.Object,
            Amount = 1,
            Step = 1,
            StartTier = 0
        };
        var needDesire3 = new NeedDesire
        {
            Product = ownProduct.Object,
            Amount = 1,
            Step = 1,
            StartTier = 0
        };
        var testNeeds = new List<INeedDesire>{needDesire1, needDesire2, needDesire3};

        var wantDesire1 = new WantDesire
        {
            Want = want1.Object,
            Amount = 1,
            StartTier = 0,
            Step = 1,
            EndTier = null
        };
        var testWants = new List<IWantDesire>{ wantDesire1 };

        test = new Desires(MarketMock.Object, testNeeds, testWants);

        test.AddProducts(consumeProduct.Object, 100);
        test.AddProducts(useProduct.Object, 100);
        test.AddProducts(ownProduct.Object, 100);

        test.UnclaimedWants[want1.Object] = 100;
        
        test.SiftProducts();
        // sanity check Products
        Assert.That(test.ProductsSatisfied[consumeProduct.Object], Is.EqualTo(100));
        Assert.That(test.ProductsSatisfied[useProduct.Object], Is.EqualTo(100));
        Assert.That(test.ProductsSatisfied[ownProduct.Object], Is.EqualTo(100));

        test.SiftWants();
        
        // check that products have been reserved correctly
        Assert.That(test.AllProperty[consumeProduct.Object].Reserved, Is.EqualTo(100));
        Assert.That(test.AllProperty[useProduct.Object].Reserved, Is.EqualTo(100));
        Assert.That(test.AllProperty[ownProduct.Object].Reserved, Is.EqualTo(100));
        
        // and check that the wants satisfied have also been recorded
        Assert.That(test.WantsSatisfied[want1.Object], Is.EqualTo(400));
        
        // and check that the satisfaction has been properly marked
        Assert.That(test.Wants.Single(x => x.Want == want1.Object)
            .Satisfaction, Is.EqualTo(400));
        
        var result = test.SatisfyWants();
        
        Assert.That(result, Is.EqualTo(0));
        
        // assert products have bene consumed to order
        Assert.That(test.AllProperty[ownProduct.Object].Total, Is.EqualTo(100));
        Assert.That(test.AllProperty[consumeProduct.Object].Total, Is.EqualTo(0));
        Assert.That(test.AllProperty[useProduct.Object].Total, Is.EqualTo(100));
    }
        
    [Test]
    public void CalculateSatisfactionsCorrectly()
    {
        // create wants and products, 3 of each, each product satisfying each want, but by different methods
        var want1 = new Mock<IWant>();
        want1.Setup(x => x.Name).Returns(nameof(want1));
        var want2 = new Mock<IWant>();
        want2.Setup(x => x.Name).Returns(nameof(want2));
        var want3 = new Mock<IWant>();
        want3.Setup(x => x.Name).Returns(nameof(want3));

        var product1 = new Mock<IProduct>();
        product1.Setup(x => x.Name).Returns(nameof(product1));
        var product2 = new Mock<IProduct>();
        product2.Setup(x => x.Name).Returns(nameof(product2));
        var product3 = new Mock<IProduct>();
        product3.Setup(x => x.Name).Returns(nameof(product3));

        // prod1 want1 ownership
        var prod1_want2_cons_proc = new Mock<IProcess>();
        prod1_want2_cons_proc.Setup(x => x.Name).Returns(nameof(prod1_want2_cons_proc));
        var prod1_want3_used_proc = new Mock<IProcess>();
        prod1_want3_used_proc.Setup(x => x.Name).Returns(nameof(prod1_want3_used_proc));
        // prod2 want2 ownership
        var prod2_want3_cons_proc = new Mock<IProcess>();
        prod2_want3_cons_proc.Setup(x => x.Name).Returns(nameof(prod2_want3_cons_proc));
        var prod2_want1_used_proc = new Mock<IProcess>();
        prod2_want1_used_proc.Setup(x => x.Name).Returns(nameof(prod2_want1_used_proc));
        // prod3 want3 ownership
        var prod3_want1_cons_proc = new Mock<IProcess>();
        prod3_want1_cons_proc.Setup(x => x.Name).Returns(nameof(prod3_want1_cons_proc));
        var prod3_want2_used_proc = new Mock<IProcess>();
        prod3_want2_used_proc.Setup(x => x.Name).Returns(nameof(prod3_want2_used_proc));

        // try to use more complex values.
        product1.Setup(x => x.Wants)
            .Returns(new Dictionary<IWant, decimal>
            {
                { want1.Object, 1 }
            });
        product2.Setup(x => x.Wants)
            .Returns(new Dictionary<IWant, decimal>
            {
                { want2.Object, 5 }
            });
        product3.Setup(x => x.Wants)
            .Returns(new Dictionary<IWant, decimal>
            {
                { want3.Object, 2 }
            });

        var prod1_need = new NeedDesire
        {
            Amount = 1,
            Product = product1.Object,
            StartTier = 0
        };
        var prod1_need = new 
        
        test.AddProducts(product1.Object, 100);
        test.AddProducts(product2.Object, 200);
        test.AddProducts(product3.Object, 300);
        
        test.AddDesire(prod1_need);
    }
}