using EconomicSim.DTOs.Pops.Species;
using EconomicSim.Objects.Pops;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace EditorInterface.Species.SpeciesWantEditor
{
    internal class WantEditorModel : INotifyPropertyChanged
    {
        private string want;
        public DesireTier tierEnum;
        private decimal amount;

        public WantEditorModel(SpeciesWantDTO need)
        {
            want = need.Want;
            tierEnum = need.TierEnum;
            amount = need.Amount;
        }

        public string Want
        {
            get
            {
                return want;
            }
            set
            {
                if (want != value)
                {
                    want = value;
                    RaisePropertyChanged();
                }
            }
        }

        public string Tier
        {
            get
            {
                return tierEnum.ToString();
            }
            set
            {
                if (tierEnum.ToString() != value)
                {
                    tierEnum = (DesireTier)Enum.Parse(typeof(DesireTier), value);
                    RaisePropertyChanged();
                }
            }
        }

        public decimal Amount
        {
            get
            {
                return amount;
            }
            set
            {
                if (amount != value)
                {
                    amount = value;
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
