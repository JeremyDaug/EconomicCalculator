using EconomicSim.Helpers;
using EconomicSim.Objects.Processes;
using EconomicSim.Objects.Processes.ProcessTags;
using EconomicSim.Objects.Products;
using EconomicSim.Objects.Wants;
using NUnit.Framework;

namespace EconSimTest.Helpers;

[TestFixture]
public class WantSourcingShould
{
    private WantSourcing test;

    private IWant testWant;

    private IWant inputWant2;
    private IWant inputWant3;

    private Product testProduct1;
    private Product testProduct2;
    private Product testProduct3;
    
    private Product trashProduct2;
    private Product trashProduct3;

    private Process UseProcess;
    private Process ConsumptionProcess;

    [SetUp]
    public void Setup()
    {
        testWant = new Want
        {
            Name = "Want"
        };
        inputWant2 = new Want
        {
            Name = "inputWant2"
        };
        inputWant3 = new Want
        {
            Name = "inputWant3"
        };
        testProduct1 = new Product
        {
            Name = "prod1",
            Wants = new Dictionary<IWant, decimal>
            {
                {testWant, 1}
            }
        };
        testProduct2 = new Product
        {
            Name = "prod2"
        };
        testProduct3 = new Product
        {
            Name = "prod3"
        };
        trashProduct2 = new Product
        {
            Name = "trash2"
        };
        trashProduct3 = new Product
        {
            Name = "trash3"
        };

        UseProcess = new Process
        {
            Name = "Use Process",
            ProcessProducts = new List<ProcessProduct>
            {
                new ProcessProduct
                {
                    Product = testProduct2,
                    Amount = 1,
                    Part = ProcessPartTag.Capital
                },
                new ProcessProduct
                {
                    Product = trashProduct2,
                    Amount = 1,
                    Part = ProcessPartTag.Output
                }
            },
            ProcessWants = new List<ProcessWant>
            {
                new ProcessWant
                {
                    Want = testWant, Amount = 1, Part = ProcessPartTag.Output
                },
                new ProcessWant
                {
                Want = inputWant2, Amount = 1, Part = ProcessPartTag.Input
                }
            },
            ProcessTags = new Dictionary<ProcessTag, Dictionary<string, object>?>
            {
                { ProcessTag.Use , new Dictionary<string, object>
                {
                    {"Product", testProduct2}
                }}
            }
        };
        
        ConsumptionProcess = new Process
        {
            Name = "Consumption Process",
            ProcessProducts = new List<ProcessProduct>
            {
                new ProcessProduct
                {
                    Product = testProduct3,
                    Amount = 1,
                    Part = ProcessPartTag.Input
                },
                new ProcessProduct
                {
                    Product = trashProduct3,
                    Amount = 1,
                    Part = ProcessPartTag.Output
                }
            },
            ProcessWants = new List<ProcessWant>
            {
                new ProcessWant
                {
                    Want = testWant, Amount = 1, Part = ProcessPartTag.Output
                },
                new ProcessWant
                {
                    Want = inputWant3, Amount = 1, Part = ProcessPartTag.Input
                }
            },
            ProcessTags = new Dictionary<ProcessTag, Dictionary<string, object>?>
            {
                { ProcessTag.Consumption , new Dictionary<string, object>
                {
                    {"Product", testProduct3}
                }}
            }
        };
        
        testProduct2.ProductProcesses.Add(UseProcess);
        testProduct3.ProductProcesses.Add(ConsumptionProcess);

        testWant.OwnershipSources.Add(testProduct1);
        testWant.UseSources.Add(testProduct2);
        testWant.ConsumptionSources.Add(testProduct3);

        test = new WantSourcing(testWant);
    }

    [Test]
    public void SeedSourcesOnConstruction()
    {
        Assert.That(test.Want, Is.EqualTo(testWant));
        
        Assert.That(test.OwnSource.Count(), Is.EqualTo(1));
        Assert.That(test.OwnSource.ContainsKey(testProduct1),
            Is.True);
        
        Assert.That(test.UseSource.Count(), Is.EqualTo(1));
        Assert.That(test.UseSource.ContainsKey(UseProcess),
            Is.True);

        Assert.That(test.ConsumptionSource.Count(), Is.EqualTo(1));
        Assert.That(test.ConsumptionSource.ContainsKey(ConsumptionProcess),
            Is.True);
    }

    [Test]
    public void ReturnSourcingRequirementsCorrectly()
    {
        test.OwnSource[testProduct1] = 5;
        test.ConsumptionSource[ConsumptionProcess] = 10;
        test.UseSource[UseProcess] = 2;

        var (productsEffected, wantsChanged) =
            test.WantSourcingRequirements();
        
        Assert.That(productsEffected.Count, Is.EqualTo(3));
        Assert.That(productsEffected[testProduct1].Change, Is.EqualTo(0));
        Assert.That(productsEffected[testProduct1].Use, Is.EqualTo(5));
        Assert.That(productsEffected[testProduct2].Change, Is.EqualTo(0));
        Assert.That(productsEffected[testProduct2].Use, Is.EqualTo(2));
        Assert.That(productsEffected[testProduct3].Change, Is.EqualTo(-10));
        Assert.That(productsEffected[testProduct3].Use, Is.EqualTo(0));
        
        Assert.That(wantsChanged.Count, Is.EqualTo(2));
        Assert.That(wantsChanged[inputWant2], Is.EqualTo(-2));
        Assert.That(wantsChanged[inputWant3], Is.EqualTo(-10));
    }
    
    [Test]
    public void ReturnSourcingOutputsCorrectly()
    {
        test.OwnSource[testProduct1] = 5;
        test.ConsumptionSource[ConsumptionProcess] = 10;
        test.UseSource[UseProcess] = 2;

        var (productsEffected, wantsChanged) =
            test.WantSourcingOutputs();
        
        Assert.That(productsEffected.Count, Is.EqualTo(2));
        Assert.That(productsEffected[trashProduct2].Change, Is.EqualTo(2));
        Assert.That(productsEffected[trashProduct2].Use, Is.EqualTo(0));
        Assert.That(productsEffected[trashProduct3].Change, Is.EqualTo(10));
        Assert.That(productsEffected[trashProduct3].Use, Is.EqualTo(0));
        
        Assert.That(wantsChanged.Count, Is.EqualTo(1));
        Assert.That(wantsChanged[testWant], Is.EqualTo(17));
    }
    
    [Test]
    public void ReturnSourcingSummaryCorrectly()
    {
        test.OwnSource[testProduct1] = 5;
        test.ConsumptionSource[ConsumptionProcess] = 10;
        test.UseSource[UseProcess] = 2;

        var (productsEffected, wantsChanged) =
            test.WantSourcingSummary();
        
        Assert.That(productsEffected.Count, Is.EqualTo(5));
        Assert.That(productsEffected[testProduct1].Change, Is.EqualTo(0));
        Assert.That(productsEffected[testProduct1].Use, Is.EqualTo(5));
        Assert.That(productsEffected[testProduct2].Change, Is.EqualTo(0));
        Assert.That(productsEffected[testProduct2].Use, Is.EqualTo(2));
        Assert.That(productsEffected[testProduct3].Change, Is.EqualTo(-10));
        Assert.That(productsEffected[testProduct3].Use, Is.EqualTo(0));
        Assert.That(productsEffected[trashProduct2].Change, Is.EqualTo(2));
        Assert.That(productsEffected[trashProduct2].Use, Is.EqualTo(0));
        Assert.That(productsEffected[trashProduct3].Change, Is.EqualTo(10));
        Assert.That(productsEffected[trashProduct3].Use, Is.EqualTo(0));
        
        Assert.That(wantsChanged.Count, Is.EqualTo(3));
        Assert.That(wantsChanged[testWant], Is.EqualTo(17));
        Assert.That(wantsChanged[inputWant2], Is.EqualTo(-2));
        Assert.That(wantsChanged[inputWant3], Is.EqualTo(-10));
        
        Assert.That(test.Projected, Is.EqualTo(17));
    }
}