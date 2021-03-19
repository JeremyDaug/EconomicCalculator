using EconModels.JobModels;
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
            Jobs = new List<Job>();
        }

        [DisplayName("Process Id")]
        public int Id { get; set; }
        // Index with Variant Name
        [DisplayName("Process Name")]
        [Required, StringLength(30, MinimumLength = 3)]
        public string Name { get; set; }

        [DisplayName("Variant Name")]
        [StringLength(20)]
        public string VariantName { get; set; }

        [DisplayName("Inputs")]
        public virtual ICollection<ProcessInput> Inputs { get; set; }

        [DisplayName("Outputs")]
        public virtual ICollection<ProcessOutput> Outputs { get; set; }
        
        [DisplayName("Capital")]
        public virtual ICollection<ProcessCapital> Capital { get; set; }

        // Required Navigation Properties
        // connects to Job.Process
        public virtual ICollection<Job> Jobs { get; set; }
    }
}