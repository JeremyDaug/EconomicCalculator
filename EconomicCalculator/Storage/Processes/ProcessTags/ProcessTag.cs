using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EconomicCalculator.Storage.Processes.ProcessTags
{
    /// <summary>
    /// Available Tags for processes
    /// </summary>
    public enum ProcessTag
    {
        /// <summary>
        /// This process is a failure process for a product.
        /// It has Exactly one input (a unit of the product), and no
        /// capital period. It may or may not have an output.
        /// </summary>
        Failure,
        /// <summary>
        /// This process is a consumption process for a product.
        /// It has exactly one input (a unit of the product), and
        /// No Captial Period. It may or may not have an output.
        /// </summary>
        Consumption,
        /// <summary>
        /// This process is a maintenance process for a product.
        /// It should contain a unit of the product as input and 
        /// the product must also appear as an output. Imperfect
        /// maintenance processes should be chance procceses also.
        /// </summary>
        Maintenance,
        /// <summary>
        /// This process is a process with outputs that are not guaranteed.
        /// At least one output should have a Chance ProductionTag Attached to it.
        /// </summary>
        Chance,
        /// <summary>
        /// This process is a farming process and so draws parts of it's effects
        /// from the fertility of the land it is on. The more fertile the more
        /// efficient the process is.
        /// </summary>
        Crop,
        /// <summary>
        /// Mine processes are attached to a node in a territory. The richness of the
        /// node defines how well it functions and how efficient it is.
        /// </summary>
        Mine,
        /// <summary>
        /// This process extracts resources from the available local material. This gives
        /// a skew towards raw resources available in the environment like soil or water.
        /// </summary>
        Extractor,
        /// <summary>
        /// This process extracts resources from the planet as a whole. This gives now scew
        /// based on local environment, but the output of the process is dependent on the
        /// planet as a whole.
        /// </summary>
        Tap,
        /// <summary>
        /// This process is an extraction or Tap process which pulls out a specific resource
        /// the rest is returned to the environment. How successful the process is depends on
        /// the composition of the planet and the resource being targeted.
        /// </summary>
        Refiner,
        /// <summary>
        /// This process is an Extraction or Tap process which takes everything given and
        /// sorts it into it's constituent parts. Nothing returns to the environment naturally.
        /// </summary>
        Sorter,
        /// <summary>
        /// Scrapping<Product(Variant)>, a scrapping procedure, often based on a consumption
        /// or failure process. This is for turning processed goods into simpler goods, usually
        /// more condusive to being made into other things.
        /// </summary>
        Scrapping
    }
}
