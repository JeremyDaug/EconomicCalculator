using EconomicSim.Objects.Processes;
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
        Test.ProcessWants.Add(CapitalWantPart1);
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
    public void AddProcessesTogether()
    {
        var sum = Test.AddProcess(Test);
        
        Assert.That(sum.GetName(), Is.EqualTo($"{Test.GetName()} + {Test.GetName()}"));
        Assert.That(sum.MinimumTime, Is.EqualTo(Test.MinimumTime*2));
        Assert.That(sum.Skill, Is.SameAs(Test.Skill));
        Assert.That(sum.SkillMinimum, Is.EqualTo(Test.SkillMinimum));
        Assert.That(sum.SkillMaximum, Is.EqualTo(Test.SkillMaximum));
        Assert.That(sum.TechRequirement, Is.SameAs(Test.TechRequirement));
        
        Assert.That(sum.InputProducts.Count, Is.EqualTo(1));
    }
}