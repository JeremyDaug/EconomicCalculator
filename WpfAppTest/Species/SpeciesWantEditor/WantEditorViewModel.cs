using EconomicCalculator;
using EconomicCalculator.DTOs.Pops.Species;
using EconomicCalculator.Objects.Pops;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace EditorInterface.Species.SpeciesWantEditor
{
    internal class WantEditorViewModel : INotifyPropertyChanged
    {
        public SpeciesWantDTO original;
        private WantEditorModel model;
        private DTOManager manager = DTOManager.Instance;

        public WantEditorViewModel(SpeciesWantDTO want)
        {
            original = want;

            model = new WantEditorModel(want);

            AvailableWants = new ObservableCollection<string>(
                manager.Wants.Values.Select(x => x.Name));

            AvailableTiers = new ObservableCollection<string>(
                Enum.GetNames(typeof(DesireTier)));

            Complete = false;
        }

        public string Want
        {
            get
            {
                return model.Want;
            }
            set
            {
                if (model.Want != value)
                {
                    model.Want = value;
                    RaisePropertyChanged();
                }
            }
        }

        public string Tier
        {
            get
            {
                return model.Tier;
            }
            set
            {
                if (model.Tier != value)
                {
                    model.Tier = value;
                    RaisePropertyChanged();
                }
            }
        }

        public decimal Amount
        {
            get
            {
                return model.Amount;
            }
            set
            {
                if (model.Amount != value)
                {
                    model.Amount = value;
                    RaisePropertyChanged();
                }
            }
        }

        public ObservableCollection<string> AvailableWants { get; set; }

        public ObservableCollection<string> AvailableTiers { get; set; }

        public bool Complete { get; set; }

        private void RaisePropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
