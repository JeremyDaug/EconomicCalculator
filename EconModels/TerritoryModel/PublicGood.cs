using EconModels.ProductModel;
using System.ComponentModel.DataAnnotations;

namespace EconModels.TerritoryModel
{
    public class PublicGood
    {


        /// <summary>
        /// The Public Good ID.
        /// </summary>
        [Required]
        public int GoodId { get; set; }

        /// <summary>
        /// The Good
        /// </summary>
        public virtual Product Good { get; set; }

        /// <summary>
        /// The amount of the good in the territory currently.
        /// </summary>
        public decimal Amount { get; set; }
    }
}