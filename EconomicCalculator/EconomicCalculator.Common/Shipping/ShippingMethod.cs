using EconomicCalculator.Common.Resource;
using EconomicCalculator.Common.Sources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EconomicCalculator.Common.Shipping
{
    /// <summary>
    /// Shipping handler class
    /// </summary>
    public class ShippingMethod
    {
        /// <summary>
        /// Name of the shipping method.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The specific shipping method.
        /// </summary>
        public string Variant { get; set; }

        /// <summary>
        /// Whether the shipping uses sea distances or not.
        /// </summary>
        public bool SeaFairing { get; set; }

        /// <summary>
        /// What vehicle it is.
        /// name, lifespan, price
        /// </summary>
        public Tuple<string, double, double> Vehicle { get; set; }

        /// <summary>
        /// What (if any) animal is driving the vehicle.
        /// name of crewmate (can be animal), number of them, their price per day
        /// </summary>
        public IList<Tuple<string, double, double>> Crew { get; set; } 
    }
}
