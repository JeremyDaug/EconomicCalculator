using EconModels.ProductModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EconModels.PopulationModel
{
    public class OwnedProperty
    {
        [Required]
        public int OwnerId { get; set; }

        [ForeignKey("OwnerId")]
        public virtual PopulationGroup Owner { get; set; }

        [Required]
        public int ProductId { get; set; }

        [ForeignKey("ProductId")]
        public virtual Product Product { get; set; }

        [Required, Range(int.MinValue, int.MaxValue)]
        public decimal Amount { get; set; }
    }
}
