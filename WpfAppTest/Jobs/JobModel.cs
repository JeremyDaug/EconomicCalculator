using EconomicCalculator.DTOs.Jobs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Editor.Jobs
{
    public class JobModel : INotifyPropertyChanged
    {
        private string name;
        private string variantName;
        private string labor;
        private string skill;

        public JobModel(JobDTO job)
        {
            Name = job.Name;
            VariantName = job.VariantName;
            Labor = job.Labor;
            Skill = job.Skill;

            Processes = new ObservableCollection<string>(job.ProcessNames);
        }

        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                if (name != value)
                {
                    name = value;
                    RaisePropertyChanged();
                }
            }
        }

        public string VariantName
        {
            get
            {
                return variantName;
            }
            set
            {
                if (variantName != value)
                {
                    variantName = value;
                    RaisePropertyChanged();
                }
            }
        }

        public string Labor
        {
            get
            {
                return labor;
            }
            set
            {
                if (labor != value)
                {
                    labor = value;
                    RaisePropertyChanged();
                }
            }
        }

        public string Skill
        {
            get
            {
                return skill;
            }
            set
            {
                if (skill != value)
                {
                    skill = value;
                    RaisePropertyChanged();
                }
            }
        }

        public ObservableCollection<string> Processes { get; }

        public void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?
                .Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
