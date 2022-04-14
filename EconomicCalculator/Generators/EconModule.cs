using EconomicCalculator.Randomizer;
using Ninject.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EconomicCalculator.Generators
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
