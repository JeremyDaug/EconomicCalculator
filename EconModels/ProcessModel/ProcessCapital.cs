using EconModels.ModelEnums;
using EconModels.ProductModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EconModels.ProcessModel
{
    public class ProcessCapital
    {
        [Required, Index("UniqueCoupling", 1, IsUnique = true)]
        [DisplayName("Process Id")]
        public int ProcessId { get; set; }

        [DisplayName("Process")]
        public virtual Process Process { get; set; }

        [Required, Index("UniqueCoupling", 2, IsUnique = true)]
        [DisplayName("Capital Id")]
        public int CapitalId { get; set; }

        [DisplayName("Capital")]
        public virtual Product Capital { get; set; }

        [Required]
        [DisplayName("Amount")]
        public double Amount { get; set; }

        [DefaultValue(0)]
        [DisplayName("Capital Tag")]
        public ProcessGoodTag Tag { get; set; } = 0;
    }
}