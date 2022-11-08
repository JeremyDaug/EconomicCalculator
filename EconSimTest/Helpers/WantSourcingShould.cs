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

    private Product testProduct1;
    private Product testProduct2;
    private Product testProduct3;

    private Process UseProcess;
    private Process ConsumptionProcess;

    [SetUp]
    public void Setup()
    {
        testWant = new Want
        {
            Name = "Want"
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
                }
            },
            ProcessWants = new List<ProcessWant>
            {
                new ProcessWant
                {
                    Want = testWant, Amount = 1, Part = ProcessPartTag.Output
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
                }
            },
            ProcessWants = new List<ProcessWant>
            {
                new ProcessWant
                {
                    Want = testWant, Amount = 1, Part = ProcessPartTag.Output
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
        test.UseSource[UseProcess] = 1;
        
        
    }
}