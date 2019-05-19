using EconomicCalculator.Common.Resource;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EconomicCalculator.Common.Sources
{
    public class Crop
    {
        /// <summary>
        /// Name of the crop
        /// </summary>
        /// 
        public string Name { get; set; }

        /// <summary>
        /// The Specific Variant of the Crop
        /// </summary>
        public string Variant { get; set; }

        /// <summary>
        /// What type of crop it is.
        /// </summary>
        public CropType CropType { get; set; }

        /// <summary>
        /// The name of the Reseed Product and how many units are used.
        /// </summary>
        public ReseedProduct ReseedProduct { get; set; }

        /// <summary>
        /// What the crop produces, how much per acre, and it's unit price in UC.
        /// Tuple (name, amount, pricePerUnit)
        /// </summary>
        public IList<InputOutputs> Produces { get; set; }

        /// <summary>
        /// When it is sowed.
        /// </summary>
        public string SowingTime { get; set; }

        /// <summary>
        /// Total time between sowing and harvesting in days. For crazy people.
        /// </summary>
        public int GrowthTime { get; set; } = -1;

        /// <summary>
        /// When it is harvested.
        /// </summary>
        public string HarvestTime { get; set; }
    }
}
