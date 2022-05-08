using Ninject.Modules;
using EconomicSim.Randomizer;

namespace EconomicSim.Generators
{
    public class EconModule : NinjectModule
    {
        public override void Load()
        {
            // Randomizer, only get one, maybe allow for multiple for more thorough testing.
            Bind<IRandomizer>().To<Randomizer.Randomizer>().InSingletonScope();
        }
    }
}
