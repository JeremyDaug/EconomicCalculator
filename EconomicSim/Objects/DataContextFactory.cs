namespace EconomicSim.Objects
{
    public class DataContextFactory
    {
        public static IDataContext GetDataContext => DataContext.Instance;
    }
}
