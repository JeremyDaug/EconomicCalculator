using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EconModels.TerritoryModel
{
    /// <summary>
    /// A class to connect territories to other territories which don't
    /// follow standard adjacency rules.
    /// </summary>
    public class TerritoryConnection
    {
        /// <summary>
        /// The Starting Territory
        /// </summary>
        [Required, Index("UniqueCoupling", 1, IsUnique = true)]
        public int StartId { get; set; }

        /// <summary>
        /// The Starting Territory.
        /// </summary>
        [ForeignKey("StartId")]
        public virtual Territory Start { get; set; }

        /// <summary>
        /// The End Territory.
        /// </summary>
        [Required, Index("UniqueCoupling", 2, IsUnique = true)]
        public int EndId { get; set; }

        /// <summary>
        /// The Ending Territory.
        /// </summary>
        [ForeignKey("EndId")]
        public virtual Territory End { get; set; }

        /// <summary>
        /// Any effective distance between the two, for measuring time
        /// passed to cross.
        /// </summary>
        [Range(0, int.MaxValue)]
        public int Distance { get; set; }

        /// <summary>
        /// Whether the Connection allows Fluids to flow (including
        /// Water).
        /// </summary>
        [Required, DefaultValue(false)]
        public bool AllowsFluidTransfer { get; set; }
    }
}
