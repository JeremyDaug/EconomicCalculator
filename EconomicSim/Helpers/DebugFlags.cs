namespace EconomicSim.Helpers;

[Flags]
public enum DebugFlags
{
    None = 0,
    PopGrowthDisabled = 1,
    PopMobilityDisabled = 2,
    RandomMovementDisabled = 4,
    PriceChangedDisabled = 8,
    SkillChangeDisabled = 16
}