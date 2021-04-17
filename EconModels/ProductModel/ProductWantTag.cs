using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EconModels.ProductModel
{
    /// <summary>
    /// A class to contain tag strings, as these can be added to arbitrarily
    /// it is not an enum. A separate helper class or list of expected options may
    /// be wise, if a bit of extra/pointless work.
    /// </summary>
    public class ProductWantTag
    {
        /// <summary>
        /// The product this want is attached to.
        /// </summary>
        [Required]
        [Index("UniqueCoupling", 1, IsUnique = true)]
        [DisplayName("Product")]
        public int ProductId { get; set; }

        /// <summary>
        /// The Product this want is attached to.
        /// </summary>
        public virtual Product Product { get; set; }

        /// <summary>
        /// The Tag itself.
        /// </summary>
        [Required, StringLength(20, MinimumLength = 1)]
        [Index("UniqueCoupling", 2, IsUnique = true)]
        [DisplayName("Tag")]
        public string Tag { get; set; }
    }
}