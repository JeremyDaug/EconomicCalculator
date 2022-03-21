using EconomicCalculator;
using EconomicCalculator.DTOs.Market;
using Editor.Helpers;
using EditorInterface.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace EditorInterface.Markets
{
    public class MarketEditorViewModel : INotifyPropertyChanged
    {
        private MarketEditorModel model;
        private MarketDTO original;
        private DTOManager manager = DTOManager.Instance;
        
        private ICommand commitMarket;
        private ICommand connectMarkets;

        public MarketEditorViewModel(MarketDTO original)
        {
            model = new MarketEditorModel(original);

            this.original = original;

            AvailableTerritories = new ObservableCollection<string>(
                manager.SimpleTerritories
                .Where(x => 
                    !manager.Markets.Values
                    .Where(y => y.Id != original.Id)
                    .Any(y => y.Territories.Any(z => z == x.Name)))
                .Select(x => x.Name));

            AvailableMarkets = new ObservableCollection<string>(
                manager.Markets.Values
                .Where(x => x.Id != original.Id)
                .Select(x => x.Name));

            AvailableProducts = new ObservableCollection<string>(
                manager.Products.Values
                .Select(x => x.GetName()));
        }

        public ICommand CommitMarket
        {
            get
            {
                if (commitMarket == null)
                {
                    commitMarket = new RelayCommand(
                        param => Commit());
                }
                return commitMarket;
            }
        }

        public ICommand ConnectMarkets
        {
            get
            {
                if (connectMarkets == null)
                {
                    connectMarkets = new RelayCommand(
                        param => ConnectMarketsTogether());
                }
                return connectMarkets;
            }
        }

        private void ConnectMarketsTogether()
        {
            MessageBox.Show("TODO, complete this option.", "Incomplete Function.");
        }

        private void Commit()
        {
            // Check Name
            if (string.IsNullOrEmpty(model.Name))
            {
                MessageBox.Show("Name cannot be empty.", "Invalid Name", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // must have at least one territory
            if (string.IsNullOrEmpty(TerritoryToAdd))
            {
                MessageBox.Show("Market requires a territory.", "No Territory Selected.",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // neighbor connections are automated

            // Check stockpiles
            var uniqueProds = Resources.Select(x => x.Value1).Distinct();
            if (Resources.Count() != uniqueProds.Count())
            {
                MessageBox.Show("Duplicate Products found, be sure to remove them.", "Duplicate Stockpile items found.",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // with checks done, let's get this stuff put together.
            var newMarket = new MarketDTO
            {
                Id = original.Id,
                Name = Name,
                Resources = Resources.ToDictionary(
                    key => key.Value1, value => value.Value2),
                Territories = new List<string> { TerritoryToAdd }
            };

            manager.Markets[newMarket.Id] = newMarket;

            MessageBox.Show("Market Commited, don't forget to save.");
        }

        public string Name
        {
            get { return model.Name; }
            set
            {
                if (model.Name != value)
                {
                    model.Name = value;
                    RaisePropertyChanged();
                }
            }
        }

        public string TerritoryToAdd
        {
            get { return model.TerritoryToAdd; }
            set
            {
                if (model.TerritoryToAdd != value)
                {
                    model.TerritoryToAdd = value;
                    RaisePropertyChanged();
                }
            }
        }

        public ObservableCollection<string> AvailableTerritories { get; set; }

        public ObservableCollection<string> AvailableMarkets { get; set; }

        public ObservableCollection<string> AvailableProducts { get; set; }

        public ObservableCollection<DummyWrapper<string>> Territories => model.Territories;

        public ObservableCollection<DummyTuple<string, decimal>> Neighbors => model.Neighbors;

        public ObservableCollection<DummyTuple<string, decimal>> Resources => model.Resources;

        private void RaisePropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
