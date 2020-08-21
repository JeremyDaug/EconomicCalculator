using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EconomicCalculator.Generators
{
    public static class LaborRequirementsFactory
    {
        /// <summary>
        /// The greatest length LaborRequirement List can be. TODO turn this into a configuration, loaded.
        /// </summary>
        public static int Length = Properties.Settings.Default.MaxSkillLevel;

        public static List<double> CreateLaborList()
        {
            return new List<double>(Length);
        }
    }
}
