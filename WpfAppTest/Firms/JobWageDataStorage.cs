using EconomicCalculator.Objects.Firms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace EditorInterface.Firms
{
    public class JobWageDataStorage : INotifyPropertyChanged
    {
        private string jobName;
        private WageType wageType;
        private decimal wage;

        public JobWageDataStorage() {}

        public string JobName
        {
            get { return jobName; }
            set
            {
                if (jobName != value)
                {
                    jobName = value;
                    RaisePropertyChanged();
                }
            }
        }

        public string WageType
        {
            get { return wageType.ToString(); }
            set
            {
                if (wageType.ToString() != value)
                {
                    wageType = (WageType)Enum.Parse(typeof(WageType), value);
                    RaisePropertyChanged();
                }
            }
        }

        public decimal Wage
        {
            get { return wage; }
            set
            {
                if (wage != value)
                {
                    wage = value;
                    RaisePropertyChanged();
                }
            }
        }

        private void RaisePropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
