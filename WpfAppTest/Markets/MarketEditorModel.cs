using EconomicSim.DTOs.Market;
using EditorInterface.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace EditorInterface.Markets
{
    public class MarketEditorModel : INotifyPropertyChanged
    {
        private MarketDTO original;

        public MarketEditorModel(MarketDTO original)
        {
            this.original = original;
            name = original.Name;
            territoryToAdd = original.Territories.FirstOrDefault();
            Territories = new ObservableCollection<DummyWrapper<string>>();
            Neighbors = new ObservableCollection<DummyTuple<string, decimal>>();
            Resources = new ObservableCollection<DummyTuple<string, decimal>>();

            foreach (var resource in original.Resources)
            {
                Resources.Add(new DummyTuple<string, decimal>
                {
                    Value1 = resource.Key,
                    Value2 = resource.Value,
                });
            }
        }

        private string name;
        private string territoryToAdd;
        
        public string Name
        {
            get { return name; }
            set
            {
                if (name != value)
                {
                    name = value;
                    RaisePropertyChanged();
                }
            }
        }

        public string TerritoryToAdd 
        {
            get { return territoryToAdd; }
            set
            {
                if (value != territoryToAdd)
                {
                    territoryToAdd = value;
                    RaisePropertyChanged();
                }
            }
        }

        public ObservableCollection<DummyWrapper<string>> Territories { get; private set; }

        public ObservableCollection<DummyTuple<string, decimal>> Neighbors { get; private set; }

        public ObservableCollection<DummyTuple<string, decimal>> Resources { get; private set; }

        private void RaisePropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
