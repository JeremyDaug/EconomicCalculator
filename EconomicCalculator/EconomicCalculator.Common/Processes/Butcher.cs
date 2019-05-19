using System.Xml.Serialization;
using System.Collections.Generic;
using EconomicCalculator.Common.Resource;
using System;

namespace EconomicCalculator.Common.Processes
{
    /// <summary>
    /// Butcher class manager
    /// </summary>
    [Serializable]
    public class Butcher
    {
        /// <summary>
        /// Name of what is being butchered.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The variant of the butchering.
        /// </summary>
        public string Variant { get; set; }

        /// <summary>
        /// What Animal is being butchered.
        /// </summary>
        /// <remarks>
        /// Animal must be made before it can be called.
        /// Animals are too complicated to keep around.
        /// </remarks>
        public string Animal { get; set; }

        /// <summary>
        /// What Products are made, how much, and their price.
        /// </summary>
        [XmlArrayItem]
        public List<InputOutputs> Products { get; set; } = new List<InputOutputs>();

        /// <summary>
        /// The price multiplier of the products from the animal.
        /// </summary>
        public double PriceMultiplier { get; set; }
    }
}
