using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EconomicCalculator.Objects.Technology
{
    /// <summary>
    /// Technology Data Class
    /// </summary>
    internal class Technology : ITechnology
    {
        public Technology()
        {
            Families = new List<ITechFamily>();
            Parents = new List<ITechnology>();
            Children = new List<ITechnology>();
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
        public List<ITechFamily> Families { get; set; }
        IReadOnlyList<ITechFamily> ITechnology.Families => Families;
        
        /// <summary>
        /// The techs that can come from this one.
        /// </summary>
        public List<ITechnology> Children { get; set; }
        IReadOnlyList<ITechnology> ITechnology.Children => Children;

        /// <summary>
        /// The techs this tech can come from.
        /// </summary>
        public List<ITechnology> Parents { get; set; }
        IReadOnlyList<ITechnology> ITechnology.Parents => Parents;
        
        /// <summary>
        /// The base cost of discovering the tech. Modified by
        /// </summary>
        public int TechCostBase { get; set; }

        /// <summary>
        /// A Description of the Technology.
        /// </summary>
        public string Description { get; set; }
    }
}
