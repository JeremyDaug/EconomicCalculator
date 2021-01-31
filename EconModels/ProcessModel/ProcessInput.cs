using EconModels.ProductModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EconModels.ProcessModel
{
    public class ProcessInput
    {
        [Required, Index("UniqueCoupling", 1, IsUnique = true)]
        [DisplayName("Process Id")]
        public int ProcessId { get; set; }

        [DisplayName("Process")]
        public virtual Process Process { get; set; }

        [Required, Index("UniqueCoupling", 2, IsUnique = true)]
        [DisplayName("Input Id")]
        public int InputId { get; set; }

        [DisplayName("Input")]
        public virtual Product Input { get; set; }

        [Required]
        [DisplayName("Amount")]
        public double Amount { get; set; }
    }
}
