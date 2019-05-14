using EconomicCalculator.Common.Resource;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace EconomicCalculator.Common.Sources
{
    /// <summary>
    /// Animal class
    /// </summary>
    public class Animal
    {
        /// <summary>
        /// Name of the Animal IE Cow, Sheep
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The Variant name (empty means default)
        /// </summary>
        public string Variant { get; set; }

        /// <summary>
        /// Whether the animal is game or livestock
        /// </summary>
        public bool Game { get; set; }

        /// <summary>
        /// The name of the crop product that it eats.
        /// Tuple(name, amountPerDay)
        /// </summary>
        public IList<Tuple<string, double> > FoodCrop { get; set; }

        /// <summary>
        /// Live Weight of the animal at slaughter
        /// </summary>
        public double LiveWeight { get; set; }

        /// <summary>
        /// The Carcass weight of the animal after basic dressing (useful weight)
        /// </summary>
        public double CarcasWeight { get; set; }

        /// <summary>
        /// How long the Anmial lives in days (for calculating lifespan cost)
        /// </summary>
        public int Lifespan { get; set; }

        /// <summary>
        /// What the animal Produces while Alive, how much per day,
        /// and it's unit price in UC.
        /// Tuple(name, amountPerDay, Price)
        /// </summary>
        public IList<Tuple<string, double, double> > LiveProducts { get; set; }

        /// <summary>
        /// What offspring this animal can produce, how many, and how often in a lifetime.
        /// </summary>
        public IList<Tuple<string, double, double>> Offspring { get; set; }
    }
}
