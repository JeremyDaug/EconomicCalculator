using EconomicCalculator.Common.Resource;
using System.Collections.Generic;

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
        public Vehicle Vehicle { get; set; }

        /// <summary>
        /// What (if any) animal is driving the vehicle.
        /// name of crewmate (can be animal), number of them, their price per day
        /// </summary>
        public IList<Crewmates> Crew { get; set; } 

        public IList<DrivingAnimal> DrivingAnimal { get; set; }
    }
}
