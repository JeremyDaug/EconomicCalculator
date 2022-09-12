using EconomicSim.Helpers;
using EconomicSim.Objects.Wants;

namespace EconSimTest.Helpers;

public class ADesireShould
{
    private ADesire test;

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
        test.IsConsumed = false;
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
    public void GetTheNextTierAvailable(int start, int step, int? end, int tier, int target)
    {
        test.StartTier = start;
        test.Step = step;
        test.EndTier = end;
        Assert.That(test.GetNextTier(tier), Is.EqualTo(target));
    }
}