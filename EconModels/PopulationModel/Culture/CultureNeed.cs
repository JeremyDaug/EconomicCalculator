using EconModels.Enums;
using EconModels.ProductModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EconModels.PopulationModel
{
    /// <summary>
    /// This is a V1 class, likely to be replaced with the
    /// V2 "CultureWants" class. 
    /// </summary>
    public class CultureNeed
    {
        /// <summary>
        /// The Id of the cultuer it's attached to.
        /// </summary>
        [Required]
        [DisplayName("Culture Id")]
        public int CultureId { get; set; }

        /// <summary>
        /// The Culture this need connects back to.
        /// </summary>
        [ForeignKey("CultureId")]
        [DisplayName("Culture")]
        public virtual Culture Culture { get; set; }

        /// <summary>
        /// The Id of the Need
        /// </summary>
        [Required]
        [DisplayName("Need Id")]
        public int NeedId { get; set; }

        /// <summary>
        /// The product they need.
        /// </summary>
        [ForeignKey("NeedId")]
        [DisplayName("Need")]
        public virtual Product Need { get; set; }

        /// <summary>
        /// The Amount of the product they need per day per person.
        /// </summary>
        [Required, Range(0, double.MaxValue)]
        [DisplayName("Need Amount")]
        public decimal Amount { get; set; }

        /// <summary>
        /// The type of need it is, valid between Life, Daily, and Luxury
        /// </summary>
        [Required]
        [DisplayName("Need Type")]
        public NeedType NeedType { get; set; }
    }
}
