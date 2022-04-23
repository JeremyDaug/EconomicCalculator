using Ninject.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
