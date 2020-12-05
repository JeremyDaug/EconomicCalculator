using System;
using System.Collections.Generic;
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

        public int Id { get; set; }

        [Required, StringLength(30, MinimumLength = 3)]
        public string Name { get; set; }

        public virtual ICollection<ProcessInput> Inputs { get; set; }

        public virtual ICollection<ProcessOutput> Outputs { get; set; }
        
        public virtual ICollection<ProcessCapital> Capital { get; set; }
    }
}