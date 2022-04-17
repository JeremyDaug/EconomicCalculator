using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace EconomicCalculator.Objects.Technology
{
    /// <summary>
    /// Tech Family Data Class
    /// </summary>
    internal class TechFamily : ITechFamily
    {

        public TechFamily()
        {
            Relations = new List<TechFamily>();
            Techs = new List<Technology>();
        }

        /// <summary>
        /// Tech Family's Id.
        /// </summary>
        [JsonIgnore]
        public int Id { get; set; }

        /// <summary>
        /// The Tech Family Name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The Other families related to this one.
        /// </summary>
        public List<TechFamily> Relations { get; set; }
        IReadOnlyList<ITechFamily> ITechFamily.Relations => Relations;

        /// <summary>
        /// The Techs in this family.
        /// </summary>
        public List<Technology> Techs { get; set; }
        IReadOnlyList<ITechnology> ITechFamily.Techs => Techs;

        /// <summary>
        /// A Description of the Tech Family.
        /// </summary>
        public string Description { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}
