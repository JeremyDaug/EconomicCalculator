using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EconomicCalculator.Storage.Wants
{
    /// <summary>
    /// The Wants available to the system.
    /// </summary>
    public class Want : IWant
    {
        /// <summary>
        /// Default Constructor
        /// </summary>
        public Want() { }

        /// <summary>
        /// Copy Constructor.
        /// </summary>
        /// <param name="other">What we are copying.</param>
        public Want(Want other)
        {
            Id = other.Id;
            Name = other.Name;
        }

        /// <summary>
        /// The Want Id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The Want Name
        /// </summary>
        public string Name { get; set; }
    }
}
