using EconomicCalculator.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EconModels.ProductModel
{
    public class Product
    {
        public int Id { get; set; }

        [Required, StringLength(30, MinimumLength = 3)]
        public string Name { get; set; }

        [StringLength(30)]
        public string VariantName { get; set; }

        [Required, StringLength(15)]
        public string UnitName { get; set; }

        [Required]
        public int Quality { get; set; }

        [Required]
        public decimal DefaultPrice { get; set; }

        [Required, Range(0, double.MaxValue)]
        public double Mass { get; set; }

        [Required, Range(0, double.MaxValue)]
        public double Bulk { get; set; }

        [Required]
        public ProductTypes ProductTypes { get; set; }

        [Required]
        public bool Maintainable { get; set; }

        [Required]
        public bool Fractional { get; set; }

        [Required]
        public int MeanTimeToFailure { get; set; }

        // Navigation Properties
        public virtual List<FailsIntoPair> FailsInto { get; set; }

        public virtual List<MaintenancePair> Maintenance { get; set; }
    }
}
