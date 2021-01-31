using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Text;

namespace EconModels.ProcessModel
{
    public class Process
    {
        public Process()
        {
            Inputs = new List<ProcessInput>();
            Outputs = new List<ProcessOutput>();
            Capital = new List<ProcessCapital>();
        }

        [DisplayName("Process Id")]
        public int Id { get; set; }

        [DisplayName("Process Name")]
        [Required, StringLength(30, MinimumLength = 3)]
        public string Name { get; set; }

        [DisplayName("Variant Name")]
        [StringLength(20, MinimumLength = 3)]
        public string VariantName { get; set; }

        [DisplayName("Inputs")]
        public virtual ICollection<ProcessInput> Inputs { get; set; }

        [DisplayName("Outputs")]
        public virtual ICollection<ProcessOutput> Outputs { get; set; }
        
        [DisplayName("Capital")]
        public virtual ICollection<ProcessCapital> Capital { get; set; }
    }
}