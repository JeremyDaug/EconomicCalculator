using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EconomicCalculator.Objects.Technology
{
    /// <summary>
    /// Tech Family Data Class
    /// </summary>
    internal class TechFamily : ITechFamily
    {
        /// <summary>
        /// Tech Family's Id.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The Tech Family Name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The Other families related to this one.
        /// </summary>
        public IReadOnlyList<ITechFamily> Relations => throw new NotImplementedException();

        /// <summary>
        /// The Techs in this family.
        /// </summary>
        public IReadOnlyList<ITechnology> Techs => throw new NotImplementedException();

        /// <summary>
        /// A Description of the Tech Family.
        /// </summary>
        public string Description { get; set; }
    }
}
