using EconomicCalculator.Common.Resource;
using System;
using System.Collections.Generic;
using System.Text;

namespace EconomicCalculator.Common.Sources
{
    public class Population
    {
        /// <summary>
        /// The Name of the population
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The location of the Resource
        /// </summary>
        public string Location { get; set; }

        /// <summary>
        /// The Job of the Pop.
        /// </summary>
        public Job Job { get; set; }


    }
}
