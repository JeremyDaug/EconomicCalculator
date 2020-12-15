using EconModels.ProductModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EconModels.PopulationModel
{
    public class OwnedProperty
    {
        public int Id { get; set; }

        [Required]
        public virtual PopulationGroup Owner { get; set; }

        [Required]
        public virtual Product Product { get; set; }

        [Required, Range(0, int.MaxValue)]
        public decimal Amount { get; set; }
    }
}
