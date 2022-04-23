using EconomicSim;
using EconomicSim.DTOs.Territory;
using EconomicSim.Enums;
using Editor.Helpers;
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

namespace EditorInterface.SimpleTerritory
{
    internal class SimpleTerritoryViewModel : INotifyPropertyChanged
    {
        private SimpleTerritoryModel model;

        private DTOManager manager = DTOManager.Instance;
        private ICommand commitTerritory;

        public SimpleTerritoryViewModel(SimpleTerritoryDTO original)
        {
            model = new SimpleTerritoryModel(original);

            AvailableTerritories = new ObservableCollection<string>(manager
                .SimpleTerritories.Where(x => x.Name != original.Name)
                .Select(x => x.Name));

            // TODO refine to physical resources
            AvailableResources = new ObservableCollection<string>(manager
                .Products.Values
                .Select(x => x.GetName()));
        }

        public ICommand CommitTerritory
        {
            get
            {
                if (commitTerritory == null)
                {
                    commitTerritory = new RelayCommand(
                        param => CommitData());
                }
                return commitTerritory;
            }
        }

        private void CommitData()
        {
            // check name
            if (string.IsNullOrEmpty(Name))
            {
                MessageBox.Show("Invalid Name", "Name Given invalid",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // If hex location taken, check with user for override.
            if (manager.SimpleTerritories.Any(curr => curr.Coords.x == X && curr.Coords.y == Y))
            {
                var result = MessageBox.Show("Location Taken by another territory, override?", "Override Location?",
                    MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.No)
                    return;
            }

            // make sure land and water not larger than size
            if (Size < Land || Size < Water)
            {
                MessageBox.Show("Cannot have more land or water than overall territory.", "Size-Land-Water mismatch!",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // if any water, then it must have a lake or coast
            if (Water > 0 && !(HasLake || IsCoastal))
            {
                MessageBox.Show("Water exists in Territory, so it must have either a lake or a coast.", "Water Incongruity!",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // plots should match up
            if (RemainingPlots != 0)
            {
                MessageBox.Show("Remaining Plots should be equal to 0.", "Plots Mismatch!",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // no negative neighbor connection lengths
            if (Neighbors.Any(x => x.Distance < 0))
            {
                MessageBox.Show("No neighbor can have a distance of less than 0!", "Negative Distance!",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // No negative node sizes.
            if (ResourceNodes.Any(x => x.Stockpile <= 0))
            {
                MessageBox.Show("Resource Node stockpiles must have a positive number!", "Non-positive Node Size!",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // no non-positive Resources
            if (Resources.Any(x => x.Amount <= 0))
            {
                MessageBox.Show("Surface Stockpiles must be a positive value!", "Non Positive Surface Stockpile!",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // All checks out, (it seems) 
            var newTerr = new SimpleTerritoryDTO
            {
                Coords = new EconomicSim.DTOs.Hexmap.HexCoord(X, Y),
                HasLake = HasLake,
                IsCoastal = IsCoastal,
                Land = Land,
                Name = Name,
                Size = Size,
                Water = Water,
                Plots = new List<ulong>(Plots.Select(x => x.PlotCount)),
                Neighbors = new List<NeighborConnection>(Neighbors),
                Nodes = new List<ResourceNode>(ResourceNodes),
                Resources = new List<TerritoryResource>(Resources)
            };

            // if hex already exists in territory, override
            var extant = manager.SimpleTerritories.SingleOrDefault(hex => hex.Coords.x == X && hex.Coords.y == Y);

            if (extant != null)
                manager.SimpleTerritories.Remove(extant);

            manager.SimpleTerritories.Add(newTerr);

            MessageBox.Show("Successful commit! Remember to save territories from the list Window.", "Successful Commit!",
                    MessageBoxButton.OK, MessageBoxImage.Exclamation);
        }

        #region Properties

        public int X
        {
            get { return model.X; }
            set
            {
                if (model.X != value)
                {
                    RaisePropertyChanged();
                    model.X = value;
                }
            }
        }

        public int Y
        {
            get { return model.Y; }
            set
            {
                if (model.Y != value)
                {
                    RaisePropertyChanged();
                    model.Y = value;
                }
            }
        }

        public string Name
        {
            get { return model.Name; }
            set
            {
                if (model.Name != value)
                {
                    RaisePropertyChanged();
                    model.Name = value;
                }
            }
        }

        public bool IsCoastal
        {
            get { return model.IsCoastal; }
            set
            {
                if (model.IsCoastal != value)
                {
                    RaisePropertyChanged();
                    model.IsCoastal = value;
                }
            }
        }

        public bool HasLake
        {
            get { return model.HasLake; }
            set
            {
                if (model.HasLake != value)
                {
                    RaisePropertyChanged();
                    model.HasLake = value;
                }
            }
        }

        public ulong Size
        {
            get { return model.Size; }
            set
            {
                if (model.Size != value)
                {
                    model.Size = value;
                    RaisePropertyChanged();
                    RaisePropertyChanged("Water");
                    RaisePropertyChanged("Land");
                    RaisePropertyChanged(nameof(RemainingPlots));
                }
            }
        }

        public ulong Land
        {
            get { return model.Land; }
            set
            {
                if (model.Land != value)
                {
                    model.Land = value;
                    RaisePropertyChanged();
                    RaisePropertyChanged("Water");
                    RaisePropertyChanged("Size");
                    RaisePropertyChanged(nameof(RemainingPlots));
                }
            }
        }

        public ulong Water
        {
            get { return model.Water; }
            set
            {
                if (model.Water != value)
                {
                    model.Water = value;
                    RaisePropertyChanged();
                    RaisePropertyChanged("Land");
                    RaisePropertyChanged("Size");
                    RaisePropertyChanged(nameof(RemainingPlots));
                }
            }
        }

        public long RemainingPlots
        {
            get
            {
                long remaining = (long)(Land * 8);
                foreach (var item in Plots)
                    remaining -= (long)item.PlotCount;
                return remaining;
            }
            set { RaisePropertyChanged(); }
        }

        #endregion Properties

        #region Lists

        public ObservableCollection<PlotOptions> Plots => model.Plots;

        public ObservableCollection<NeighborConnection> Neighbors => model.Neighbors;

        public ObservableCollection<ResourceNode> ResourceNodes => model.ResourceNodes;

        public ObservableCollection<TerritoryResource> Resources => model.Resources;

        #region StaticLists

        public ObservableCollection<string> AvailableTerritories { get; private set; }

        public ObservableCollection<string> AvailableResources { get; private set; }

        public ObservableCollection<string> ConnectionTypeEnumOptions 
            => new ObservableCollection<string>(Enum.GetNames(typeof(TerritoryConnectionType)));

        #endregion StaticLists

        #endregion Lists

        private void RaisePropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
