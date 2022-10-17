using EconomicSim.Helpers;
using EconomicSim.Objects.Processes;
using EconomicSim.Objects.Processes.ProcessTags;
using EconomicSim.Objects.Processes.ProductionTags;
using EconomicSim.Objects.Products;
using EconomicSim.Objects.Skills;
using EconomicSim.Objects.Technology;
using EconomicSim.Objects.Wants;
using Moq;

namespace EconSimTest.Objects.Processes;

[TestFixture]
public class ProcessesShould
{
    private IProduct InputProduct1;
    private IProduct CapitalProduct1;
    private IProduct OutputProduct1;
    private IWant InputWant1;
    private IWant CapitalWant1;
    private IWant OutputWant1;
    private ISkill TestSkill;
    private ITechnology TestTech;

    private Process Test;

    private ProcessProduct InputProductPart1;
    private ProcessProduct CapitalProductPart1;
    private ProcessProduct OutputProductPart1;
    private ProcessWant InputWantPart1;
    private ProcessWant CapitalWantPart1;
    private ProcessWant OutputWantPart1;

    [SetUp]
    public void Setup()
    {
        InputProduct1 = new Product
        {
            Name = nameof(InputProduct1)
        };
        CapitalProduct1 = new Product
        {
            Name = nameof(CapitalProduct1)
        };
        OutputProduct1 = new Product
        {
            Name = nameof(OutputProduct1)
        };

        InputWant1 = new Want
        {
            Name = nameof(InputWant1)
        };
        CapitalWant1 = new Want
        {
            Name = nameof(CapitalWant1)
        };
        OutputWant1 = new Want
        {
            Name = nameof(OutputWant1)
        };
        
        InputProductPart1 = new ProcessProduct
        {
            Product = InputProduct1,
            Amount = 1,
            Part = ProcessPartTag.Input
        };
        CapitalProductPart1 = new ProcessProduct
        {
            Product = CapitalProduct1,
            Amount = 1,
            Part = ProcessPartTag.Capital
        };
        OutputProductPart1 = new ProcessProduct
        {
            Product = OutputProduct1,
            Amount = 1,
            Part = ProcessPartTag.Output
        };
        
        InputWantPart1 = new ProcessWant
        {
            Want = InputWant1,
            Amount = 1,
            Part = ProcessPartTag.Input
        };
        CapitalWantPart1 = new ProcessWant
        {
            Want = CapitalWant1,
            Amount = 1,
            Part = ProcessPartTag.Capital
        };
        OutputWantPart1 = new ProcessWant
        {
            Want = OutputWant1,
            Amount = 1,
            Part = ProcessPartTag.Output
        };

        TestSkill = new Skill
        {
            Name = nameof(TestSkill),
            Description = "A Test Skill"
        };

        TestTech = new Technology
        {
            Name = nameof(TestTech)
        };
        
        Test = new Process
        {
            Name = "TestName",
            VariantName = "VariantName",
            Description = "A Test Process",
            MinimumTime = 0,
            Skill = TestSkill,
            SkillMinimum = 2,
            SkillMaximum = 3,
            TechRequirement = TestTech
        };

        Test.ProcessProducts.Add(InputProductPart1);
        Test.ProcessProducts.Add(CapitalProductPart1);
        Test.ProcessProducts.Add(OutputProductPart1);
        
        Test.ProcessWants.Add(InputWantPart1);
        //Test.ProcessWants.Add(CapitalWantPart1);
        Test.ProcessWants.Add(OutputWantPart1);
    }
    
    [TestCase("FirstName")]
    public void GetSingularNameCorrectly(string primary)
    {
        var proc = new Process
        {
            Name = primary
        };
        
        Assert.That(proc.GetName(), Is.EqualTo("FirstName"));
    }

    [TestCase("FirstName", "SecondName")]
    public void GetPairedNameCorrectly(string primary, string secondary)
    {
        var proc = new Process
        {
            Name = primary,
            VariantName = secondary
        };
        Assert.That(proc.GetName(), Is.EqualTo($"{primary}({secondary})"));
    }

    [Test]
    public void ReturnFalseWhenProcessesHaveTags()
    {
        var proc1 = new Process
        {
            ProcessTags = new Dictionary<ProcessTag, Dictionary<string, object>?>
            {
                {ProcessTag.Chance, new Dictionary<string, object>()}
            }
        };
        // Proc has 1, test has none.
        Assert.That(Test.ValidProcessTagAddition(proc1), Is.False);
    }

    [Test]
    public void AddProcessesTogether()
    {
        var sum = Test.AddProcess(Test);
        
        Assert.That(sum.GetName(), Is.EqualTo($"{Test.GetName()} + {Test.GetName()}"));
        Assert.That(sum.MinimumTime, Is.EqualTo(Test.MinimumTime*2));
        Assert.That(sum.Skill, Is.SameAs(Test.Skill));
        Assert.That(sum.SkillMinimum, Is.EqualTo(Test.SkillMinimum));
        Assert.That(sum.SkillMaximum, Is.EqualTo(Test.SkillMaximum));
        Assert.That(sum.TechRequirement, Is.SameAs(Test.TechRequirement));
        
        Assert.That(sum.InputProducts.Count, Is.EqualTo(2));
        Assert.That(sum.InputWants.Count, Is.EqualTo(2));
        Assert.That(sum.CapitalProducts.Count, Is.EqualTo(2));
        //Assert.That(sum.CapitalWants.Count, Is.EqualTo(2));
        Assert.That(sum.OutputProducts.Count, Is.EqualTo(2));
        Assert.That(sum.OutputWants.Count, Is.EqualTo(2));

        var inProdCount = sum.InputProducts
            .Count(x => Equals(x.Product, InputProductPart1.Product) &&
                        x.Amount == InputProductPart1.Amount);
        var inWantCount = sum.InputWants
            .Count(x => Equals(x.Want, InputWantPart1.Want) &&
                        x.Amount == InputWantPart1.Amount);
        var capProdCount = sum.CapitalProducts
            .Count(x => Equals(x.Product, CapitalProductPart1.Product) &&
                        x.Amount == CapitalProductPart1.Amount);
        var capWantCount = sum.CapitalWants
            .Count(x => Equals(x.Want, CapitalWantPart1.Want) &&
                        x.Amount == CapitalWantPart1.Amount);
        var outProdCount = sum.OutputProducts
            .Count(x => Equals(x.Product, OutputProductPart1.Product) &&
                        x.Amount == OutputProductPart1.Amount);
        var outWantCount = sum.OutputWants
            .Count(x => Equals(x.Want, OutputWantPart1.Want) &&
                        x.Amount == OutputWantPart1.Amount);
        
        Assert.That(inProdCount, Is.EqualTo(2));
        Assert.That(inWantCount, Is.EqualTo(2));
        Assert.That(capProdCount, Is.EqualTo(2));
        //Assert.That(capWantCount, Is.EqualTo(2));
        Assert.That(outProdCount, Is.EqualTo(2));
        Assert.That(outWantCount, Is.EqualTo(2));
    }

    [Test]
    public void ThrowInvalidOperationWhenAdditionHasTags()
    {
        var proc1 = new Process
        {
            ProcessTags = new Dictionary<ProcessTag, Dictionary<string, object>?>
            {
                {ProcessTag.Chance, new Dictionary<string, object>()}
            }
        };
        // Proc has 1, test has none.
        Assert.Throws<ProcessTagMismatchException>(() => Test.AddProcess(proc1));
    }

    [Test]
    public void ThrowInvalidOpIfEitherHasTagsWhenInputtingProcess()
    {
        var proc1 = new Process
        {
            ProcessProducts = new List<ProcessProduct>
            {
                OutputProductPart1
            },
            ProcessTags = new Dictionary<ProcessTag, Dictionary<string, object>?>
            {
                {ProcessTag.Chance, new Dictionary<string, object>()}
            }
        };
        Assert.Throws<ProcessTagMismatchException>(() => Test.InputProcess(proc1));
        Test.ProcessTags.Add(ProcessTag.Chance, new Dictionary<string, object>());
        var proc2 = new Process();
        Assert.Throws<ProcessTagMismatchException>(() => Test.InputProcess(proc2));
    }
    
    [Test]
    public void ThrowFromInputProcessIfOtherDoesNotOutputAnInput()
    {
        var proc = new Process();
        Assert.Throws<ProcessInterfaceMismatchException>(() => Test.InputProcess(proc));
    }

    [Test]
    public void ThrowFromInputProcessIfOutputProductMatchedHasATag()
    {
        var proc = new Process
        {
            ProcessProducts = new List<ProcessProduct>
            {
                new ProcessProduct
                {
                    Amount = 1, Part = ProcessPartTag.Output,
                    Product = InputProduct1,
                    TagData = new List<(ProductionTag tag, Dictionary<string, object> parameters)>
                    {
                        (ProductionTag.Chance, new Dictionary<string, object>())
                    }
                }
            }
        };
        Assert.Throws<ProcessInterfaceTagMismatchException>(() => Test.InputProcess(proc));
    }
    
    [Test]
    public void ThrowFromInputProcessIfInputProductMatchedHasATag()
    {
        var proc = new Process
        {
            ProcessProducts = new List<ProcessProduct>
            {
                new ProcessProduct
                {
                    Amount = 1, Part = ProcessPartTag.Output,
                    Product = InputProduct1
                }
            }
        };
        Test.ProcessProducts.Add(new ProcessProduct
        {
            Amount = 1, Product = InputProduct1, Part = ProcessPartTag.Input,
            TagData = new List<(ProductionTag tag, Dictionary<string, object> parameters)>
            {
                (ProductionTag.Chance, new Dictionary<string, object>())
            }
        });
        Assert.Throws<ProcessInterfaceTagMismatchException>(() => Test.InputProcess(proc));
    }

    [Test]
    public void CorrectlyDoProcess()
    {
        var inputs = new Dictionary<IProduct, decimal>
        {
            {InputProduct1, 10},
            {CapitalProduct1, 10},
            {OutputProduct1, 10}
        };
        var wants = new Dictionary<IWant, decimal>
        {
            {InputWant1, 10},
            {CapitalWant1, 10},
            {OutputWant1, 10},
        };

        var results
            = Test.DoProcess(1, 0, inputs, wants);
        
        Assert.That(results.successes, Is.EqualTo(1));
        Assert.That(results.progress, Is.EqualTo(0));
        // should consume an input, and produce an output
        Assert.That(results.productChange[InputProduct1], Is.EqualTo(-1));
        Assert.That(results.productChange[OutputProduct1], Is.EqualTo(1));
        
        // should consume input, output a want
        Assert.That(results.wantsChange[InputWant1], Is.EqualTo(-1));
        //Assert.That(results.wantsChange[CapitalWant1], Is.EqualTo(-1));
        Assert.That(results.wantsChange[OutputWant1], Is.EqualTo(1));
        
        // should use a capital product
        Assert.That(results.productUsed[CapitalProduct1], Is.EqualTo(1));
    }
    
    [Test]
    public void ReduceSuccessesWhenInputsLimited()
    {
        var inputs = new Dictionary<IProduct, decimal>
        {
            {InputProduct1, 0.5m},
            {CapitalProduct1, 10},
            {OutputProduct1, 10}
        };
        var wants = new Dictionary<IWant, decimal>
        {
            {InputWant1, 10},
            {CapitalWant1, 10},
            {OutputWant1, 10},
        };

        var results
            = Test.DoProcess(1, 0, inputs, wants);
        
        Assert.That(results.successes, Is.EqualTo(0));
        Assert.That(results.progress, Is.EqualTo(0.5));
        // should consume an input, and produce an output
        Assert.That(results.productChange[InputProduct1], Is.EqualTo(-0.5));
        Assert.That(results.productChange[OutputProduct1], Is.EqualTo(0.5));
        
        // should consume input, output a want
        Assert.That(results.wantsChange[InputWant1], Is.EqualTo(-0.5));
        //Assert.That(results.wantsChange[CapitalWant1], Is.EqualTo(-1));
        Assert.That(results.wantsChange[OutputWant1], Is.EqualTo(0.5));
        
        // should use a capital product
        Assert.That(results.productUsed[CapitalProduct1], Is.EqualTo(0.5));
    }
    
    [Test]
    public void CompleteDoProcessWhenGivenOnlyPartialSuccess()
    {
        var inputs = new Dictionary<IProduct, decimal>
        {
            {InputProduct1, 0.5m},
            {CapitalProduct1, 10},
            {OutputProduct1, 10}
        };
        var wants = new Dictionary<IWant, decimal>
        {
            {InputWant1, 10},
            {CapitalWant1, 10},
            {OutputWant1, 10},
        };

        var results
            = Test.DoProcess(0, 0.5m, inputs, wants);
        
        Assert.That(results.successes, Is.EqualTo(0));
        Assert.That(results.progress, Is.EqualTo(0.5));
        // should consume an input, and produce an output
        Assert.That(results.productChange[InputProduct1], Is.EqualTo(-0.5));
        Assert.That(results.productChange[OutputProduct1], Is.EqualTo(0.5));
        
        // should consume input, output a want
        Assert.That(results.wantsChange[InputWant1], Is.EqualTo(-0.5));
        //Assert.That(results.wantsChange[CapitalWant1], Is.EqualTo(-1));
        Assert.That(results.wantsChange[OutputWant1], Is.EqualTo(0.5));
        
        // should use a capital product
        Assert.That(results.productUsed[CapitalProduct1], Is.EqualTo(0.5));
    }

    [Test]
    public void ThrowNotImplementedIfCapitalWantsExistWhenDoingDoProcess()
    {
        var proc = new Process
        {
            ProcessWants = new List<ProcessWant>
            {
                CapitalWantPart1
            }
        };
        var inputs = new Dictionary<IProduct, decimal>
        {
            {InputProduct1, 0.5m},
            {CapitalProduct1, 10},
            {OutputProduct1, 10}
        };
        var wants = new Dictionary<IWant, decimal>
        {
            {InputWant1, 10},
            {CapitalWant1, 10},
            {OutputWant1, 10},
        };
        
        Assert.Throws<NotImplementedException>(() => proc.DoProcess(1, 0, inputs, wants));
    }
}