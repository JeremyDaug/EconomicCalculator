using EconomicSim.Helpers;
using EconomicSim.Objects.Products;
using EconomicSim.Objects.Wants;
using Moq;

namespace EconSimTest.Helpers;

public class ADesireShould
{
    private ADesire test;

    private Mock<IWant> _want1;
    private Mock<IWant> _want2;
    
    private Mock<IProduct> _proudct1;
    private Mock<IProduct> _product2;

    [SetUp]
    public void Setup()
    {
        test = new WantDesire();
        test.Amount = 1;
        test.StartTier = 0;
        test.Step = 0;
        test.EndTier = null;
        test.Reserved = 0;
        test.Satisfaction = 0;

        _want1 = new Mock<IWant>();
        _want2 = new Mock<IWant>();
        _proudct1 = new Mock<IProduct>();
        _product2 = new Mock<IProduct>();
    }

    [TestCase(1, 1, 100, 100)]
    [TestCase(5, 5, 100, 20)]
    [TestCase(0, 0, null, -1)]
    [TestCase(0, 1, null, -1)]
    public void CalculateStepsCorrectly(int start, int step, int? end, int correctValue)
    {
        test.StartTier = start;
        test.Step = step;
        test.EndTier = end;
        Assert.That(test.Steps, Is.EqualTo(correctValue));
    }

    [Test]
    public void CorrectlyReturnStretched()
    {
        test.Step = 1;
        Assert.That(test.IsStretched, Is.True);
        test.Step = 0;
        Assert.That(test.IsStretched, Is.False);
    }

    [Test]
    public void CorrectlyReturnsInfinite()
    {
        test.EndTier = null;
        Assert.That(test.IsInfinite, Is.False);
        test.Step = 1;
        Assert.That(test.IsInfinite, Is.True);
        test.EndTier = 5;
        Assert.That(test.IsInfinite, Is.False);
    }

    [TestCase(0, 0, null, 1, 1)]
    [TestCase(0, 1, null, 100, 101)]
    [TestCase(0, 5, null, 100, 21)]
    [TestCase(0, 5, 10, 100, 3)]
    [TestCase(0, 5, 10, 5, 2)]
    [TestCase(0, 5, null, -10, 0)]
    public void ReturnCorrectDesireAtTier(int start, int step, int? end, int tier, decimal target)
    {
        test.StartTier = start;
        test.Step = step;
        test.EndTier = end;
        Assert.That(test.TotalDesireAtTier(tier), Is.EqualTo(target));
    }

    [TestCase(1, 1)]
    [TestCase(1, 2)]
    [TestCase(2, 1)]
    [TestCase(100, 1)]
    public void DivideSatisfactionByAmountForTotalSatisfaction(decimal satisfaction, decimal amount)
    {
        test.Satisfaction = satisfaction;
        test.Amount = amount;
        Assert.That(test.TotalSatisfaction(), Is.EqualTo(satisfaction / amount));
    }

    [TestCase(0, 0, null, 1, 1)]
    [TestCase(0, 1, null, 1, -1)]
    [TestCase(0, 1, 10, 1, 11)]
    [TestCase(0, 5, 10, 1, 3)]
    public void CalculateTotalDesireCorrectly(int start, int step, int? end, decimal amount, decimal target)
    {
        test.StartTier = start;
        test.Step = step;
        test.EndTier = end;
        test.Amount = amount;
        Assert.That(test.TotalDesire(), Is.EqualTo(target));
    }

    [TestCase(0, 0, null, -1, false)]
    [TestCase(0, 0, null, 0, true)]
    [TestCase(0, 1, null, 1, true)]
    [TestCase(0, 2, null, 1, false)]
    [TestCase(0, 1, 2, 3, false)]
    [TestCase(0, 1, 2, 2, true)]
    public void CorrectlyReturnWhenTierSteppedOn(int start, int step, int? end,
        int tier, bool expectation)
    {
        test.StartTier = start;
        test.Step = step;
        test.EndTier = end;
        Assert.That(test.StepsOnTier(tier), Is.EqualTo(expectation));
    }

    [Test]
    public void ThrowIfSatisfiedAtTierReceivesInvalidTier()
    {
        test.StartTier = 0;
        test.Step = 0;
        test.EndTier = null;
        Assert.Throws<ArgumentException>(() => test.SatisfiedAtTier(1));
    }

    [TestCase(0, 0, null, 1, 0.5, 0, 0.5)]
    [TestCase(0, 1, null, 1, 1.5, 0, 1)]
    [TestCase(0, 1, null, 1, 1.5, 1, 0.5)]
    [TestCase(0, 1, null, 1, 1.5, 2, 0)]
    public void CorrectlyReturnSatisfiedAtTier(int start, int step, int? end, decimal amount,
        decimal satisfaction, int tier, decimal target)
    {
        test.StartTier = start;
        test.Step = step;
        test.EndTier = end;
        test.Amount = amount;
        test.Satisfaction = satisfaction;
        Assert.That(test.SatisfiedAtTier(tier), Is.EqualTo(target));
    }

    [TestCase(0, 0, null, -1, 0)]
    [TestCase(0, 0, null, 0, -1001)]
    [TestCase(0, 1, 2, 2, -1001)]
    [TestCase(0, 5, 10, 10, -1001)]
    [TestCase(0, 5, null, 6, 10)]
    public void GetTheNextTierUpAvailable(int start, int step, int? end, int tier, int target)
    {
        test.StartTier = start;
        test.Step = step;
        test.EndTier = end;
        Assert.That(test.GetNextTierUp(tier), Is.EqualTo(target));
    }
    
    [TestCase(0, 0, null, -1, -1001)]
    [TestCase(0, 0, null, 0, -1001)]
    [TestCase(0, 0, null, 1, 0)]
    [TestCase(0, 1, 4, 2, 1)]
    [TestCase(0, 5, 10, 11, 10)]
    [TestCase(0, 5, 10, 10, 5)]
    [TestCase(0, 5, null, 6, 5)]
    public void GetTheNextTierDownAvailable(int start, int step, int? end, int tier, int target)
    {
        test.StartTier = start;
        test.Step = step;
        test.EndTier = end;
        Assert.That(test.GetNextTierDown(tier), Is.EqualTo(target));
    }

    [TestCase(1, 0, null, 1, 1, 1)]
    [TestCase(1, 1, 10, 1, 1, 1)]
    [TestCase(1, 1, 10, 1, 5.5, 6)]
    [TestCase(1, 1, null, 1, 1, 1)]
    [TestCase(1, 1, null, 1, 5.5, 6)]
    [TestCase(1, 1, null, 2, 5.5, 3)]
    [TestCase(5, 5, null, 1, 1, 5)]
    [TestCase(5, 5, null, 1, 1.5, 10)]
    public void ReturnLastSatisfacitonTierCorrectly(int start, int step, int? end, 
        decimal amount, decimal satisfaction, int target)
    {
        test.StartTier = start;
        test.Step = step;
        test.EndTier = end;
        test.Amount = amount;
        test.Satisfaction = satisfaction;
        Assert.That(test.SatisfactionUpToTier(), Is.EqualTo(target));
    }

    [TestCase(1, 1, 0,0, null, null, true)]
    [TestCase(1, 2, 0,0, null, null, false)]
    [TestCase(1, 1, 1,2, null, null, false)]
    [TestCase(1, 1, 1,1, null, 1, false)]
    public void MatchWantDesires(int start1, int start2,
        int step1, int step2,
        int? end1, int? end2,
        bool expectation)
    {
        var want = new Mock<IWant>();
        var desire1 = new WantDesire();
        desire1.Want = want.Object;
        desire1.StartTier = start1;
        desire1.EndTier = end1;
        desire1.Step = step1;
        var desire2 = new WantDesire();
        desire2.Want = want.Object;
        desire2.StartTier = start2;
        desire2.EndTier = end2;
        desire2.Step = step2;
        
        Assert.That(desire1.IsEquivalentTo(desire2), Is.EqualTo(expectation));
    }
    
    [Test]
    public void ReturnFalseWhenWantDesiresDontHaveSameWant()
    {
        var want1 = new Mock<IWant>();
        var want2 = new Mock<IWant>();
        var desire1 = new WantDesire();
        desire1.Want = want1.Object;
        desire1.StartTier = 0;
        var desire2 = new WantDesire();
        desire2.Want = want2.Object;
        desire2.StartTier = 0;
        
        Assert.That(desire1.IsEquivalentTo(desire2), 
            Is.EqualTo(false));
    }
    
    [TestCase(1, 1, 0,0, null, null, true)]
    [TestCase(1, 2, 0,0, null, null, false)]
    [TestCase(1, 1, 1,2, null, null, false)]
    [TestCase(1, 1, 1,1, null, 1, false)]
    public void MatchNeedDesires(int start1, int start2,
        int step1, int step2,
        int? end1, int? end2,
        bool expectation)
    {
        var product = new Mock<IProduct>();
        var desire1 = new NeedDesire();
        desire1.Product = product.Object;
        desire1.StartTier = start1;
        desire1.EndTier = end1;
        desire1.Step = step1;
        var desire2 = new NeedDesire();
        desire2.Product = product.Object;
        desire2.StartTier = start2;
        desire2.EndTier = end2;
        desire2.Step = step2;
        
        Assert.That(desire1.IsEquivalentTo(desire2), Is.EqualTo(expectation));
    }
    
    [Test]
    public void ReturnFalseWhenNeedDesiresDontHaveSameProduct()
    {
        var product1 = new Mock<IProduct>();
        var product2 = new Mock<IProduct>();
        var desire1 = new NeedDesire();
        desire1.Product = product1.Object;
        desire1.StartTier = 0;
        var desire2 = new NeedDesire();
        desire2.Product = product2.Object;
        desire2.StartTier = 0;
        
        Assert.That(desire1.IsEquivalentTo(desire2), 
            Is.EqualTo(false));
    }
}