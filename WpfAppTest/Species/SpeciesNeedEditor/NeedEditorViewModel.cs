using EconomicSim;
using EconomicSim.DTOs.Pops.Species;
using EconomicSim.Objects.Pops;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace EditorInterface.Species.SpeciesNeedEditor
{
    internal class NeedEditorViewModel : INotifyPropertyChanged
    {
        public SpeciesNeedDTO original;
        private NeedEditorModel model;
        private DTOManager manager = DTOManager.Instance;

        public NeedEditorViewModel(SpeciesNeedDTO need)
        {
            original = need;

            model = new NeedEditorModel(need);

            AvailableProducts = new ObservableCollection<string>(
                manager.Products.Values.Select(x => x.GetName()));

            AvailableTiers = new ObservableCollection<string>(
                Enum.GetNames(typeof(DesireTier)));

            Complete = false;
        }

        public string Product
        {
            get
            {
                return model.Product;
            }
            set
            {
                if (model.Product != value)
                {
                    model.Product = value;
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

        public ObservableCollection<string> AvailableProducts { get; set; }

        public ObservableCollection<string> AvailableTiers { get; set; }
        
        public bool Complete { get; set; }

        private void RaisePropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
