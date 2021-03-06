﻿using EconModels.ModelEnums;
using EconModels.ProductModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EconModels.ProcessModel
{
    public class ProcessOutput
    {
        [Required, Index("UniqueCoupling", 1, IsUnique = true)]
        [DisplayName("Process Id")]
        public int ProcessId { get; set; }

        [DisplayName("Process")]
        public virtual Process Process { get; set; }

        [Required, Index("UniqueCoupling", 2, IsUnique = true)]
        [DisplayName("Output Id")]
        public int OutputId { get; set; }

        [DisplayName("Output")]
        public virtual Product Output { get; set; }

        [Required]
        [DisplayName("Amount")]
        public double Amount { get; set; }

        // Not actually used currently, but may be later
        [DefaultValue(0)]
        [DisplayName("Output Tag")]
        public ProcessGoodTag Tag { get; set; } = 0;
    }
}
