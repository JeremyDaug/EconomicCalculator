using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EconomicSim.Objects.Technology
{
    /// <summary>
    /// Technology Data Class
    /// </summary>
    internal class Technology : ITechnology
    {
        public Technology()
        {
            Families = new List<TechFamily>();
            Parents = new List<Technology>();
            Children = new List<Technology>();
        }

        /// <summary>
        /// Technology Id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Technology Name, should be unique.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The Category of tech this is, primary, secondary, and tertiary.
        /// </summary>
        public TechCategory Category { get; set; }

        /// <summary>
        /// The Tier of the Technology.
        /// </summary>
        public int Tier { get; set; }

        /// <summary>
        /// The families this tech belongs to.
        /// </summary>
        public List<TechFamily> Families { get; set; }
        IReadOnlyList<ITechFamily> ITechnology.Families => Families;
        
        /// <summary>
        /// The techs that can come from this one.
        /// </summary>
        public List<Technology> Children { get; set; }
        IReadOnlyList<ITechnology> ITechnology.Children => Children;

        /// <summary>
        /// The techs this tech can come from.
        /// </summary>
        public List<Technology> Parents { get; set; }
        IReadOnlyList<ITechnology> ITechnology.Parents => Parents;
        
        /// <summary>
        /// The base cost of discovering the tech. Modified by
        /// </summary>
        public int TechCostBase { get; set; }

        /// <summary>
        /// A Description of the Technology.
        /// </summary>
        public string Description { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}
