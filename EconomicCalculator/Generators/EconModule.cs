﻿using EconomicCalculator.Randomizer;
using EconomicCalculator.Storage;
using EconomicCalculator.Storage.Jobs;
using EconomicCalculator.Storage.Organizations;
using EconomicCalculator.Storage.Population;
using EconomicCalculator.Storage.Processes;
using EconomicCalculator.Storage.Products;
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

            // Products
            Bind<IProduct>().To<Product>();
            Bind<IPlot>().To<Plot>();
            Bind<ICurrency>().To<Currency>(); // TODO, may add currency connection to parent organization.
            Bind<IBuilding>().To<Building>(); // TODO, will most likely need to add connection to home territory.

            // Process
            Bind<IProcess>().To<Process>();

            // Population
            Bind<IPopulations>().To<Populations>();

            // PopulationGroup
            Bind<IPopulationGroup>().To<PopulationGroup>();

            // Jobs
            Bind<IJob>().To<Job>();
            // Do other jobs here also.

            // Organizations
            Bind<IMarket>().To<Market>();
            Bind<ITerritory>().To<Territory>();
        }
    }
}
