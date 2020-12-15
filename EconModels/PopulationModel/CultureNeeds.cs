using EconModels.ProductModel;
using EconomicCalculator.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EconModels.PopulationModel
{
    public class CultureNeeds
    {
        public int Id { get; set; }

        [Required]
        public virtual Culture Culture { get; set; }

        [Required]
        public virtual Product Need { get; set; }

        [Required, Range(0, double.MaxValue)]
        public decimal Amount { get; set; }

        [Required]
        public NeedType NeedType { get; set; }
    }
}
