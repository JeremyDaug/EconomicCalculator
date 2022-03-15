using EconomicCalculator;
using EconomicCalculator.DTOs.Territory;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace EditorInterface.SimpleTerritory
{
    internal class SimpleTerritoryModel : INotifyPropertyChanged
    {
        private SimpleTerritoryDTO original;

        private DTOManager manager = DTOManager.Instance;

        public SimpleTerritoryModel(SimpleTerritoryDTO original)
        {
            this.original = original;

            x = original.Coords.x;
            y = original.Coords.y;
            name = original.Name;
            isCoastal = original.IsCoastal;
            hasLake = original.HasLake;
            size = original.Size;
            land = original.Land;

            Plots = new ObservableCollection<PlotOptions>();

            // TODO don't hard code this shit. You're better than that.
            Plots.Add(new PlotOptions { PlotType = "Wasteland",         PlotCount = original.Plots[0] });
            Plots.Add(new PlotOptions { PlotType = "Marginal Land",     PlotCount = original.Plots[1] });
            Plots.Add(new PlotOptions { PlotType = "Scrub Land",        PlotCount = original.Plots[2] });
            Plots.Add(new PlotOptions { PlotType = "Quality Land",      PlotCount = original.Plots[3] });
            Plots.Add(new PlotOptions { PlotType = "Fertile Land",      PlotCount = original.Plots[4] });
            Plots.Add(new PlotOptions { PlotType = "Very Fertile Land", PlotCount = original.Plots[5] });

            Neighbors = new ObservableCollection<NeighborConnection>(original.Neighbors);
            ResourceNodes = new ObservableCollection<ResourceNode>(original.Nodes);
            Resources = new ObservableCollection<TerritoryResource>(original.Resources);
        }

        

        #region Properties

        private int x;
        private int y;
        private string name;
        private bool isCoastal;
        private bool hasLake;
        private ulong size;
        private ulong land;

        public int X
        {
            get { return x; }
            set
            {
                if (x != value)
                {
                    RaisePropertyChanged();
                    x = value;
                }
            }
        }

        public int Y
        {
            get { return y; }
            set
            {
                if (y != value)
                {
                    RaisePropertyChanged();
                    y = value;
                }
            }
        }

        public string Name
        {
            get { return name; }
            set
            {
                if (name != value)
                {
                    RaisePropertyChanged();
                    name = value;
                }
            }
        }

        public bool IsCoastal
        {
            get { return isCoastal; }
            set
            {
                if (isCoastal != value)
                {
                    RaisePropertyChanged();
                    isCoastal = value;
                }
            }
        }

        public bool HasLake
        {
            get { return hasLake; }
            set
            {
                if (hasLake != value)
                {
                    RaisePropertyChanged();
                    hasLake = value;
                }
            }
        }

        public ulong Size
        {
            get { return size; }
            set
            {
                if (size != value)
                {
                    size = value;
                    RaisePropertyChanged();
                    RaisePropertyChanged("Water");
                    RaisePropertyChanged("Land");
                }
            }
        }

        public ulong Land
        {
            get { return land; }
            set
            {
                if (land != value)
                {
                    land = value;
                    RaisePropertyChanged();
                    RaisePropertyChanged("Water");
                    RaisePropertyChanged("Size");
                }
            }
        }

        public ulong Water
        {
            get { return Size - Land; }
            set
            {
                if (Size - Land != value)
                {
                    Land = Size - value;
                    RaisePropertyChanged();
                    RaisePropertyChanged("Land");
                    RaisePropertyChanged("Size");
                }
            }
        }

        #endregion Properties

        #region Lists

        public ObservableCollection<PlotOptions> Plots { get; set; }

        public ObservableCollection<NeighborConnection> Neighbors { get; private set; }

        public ObservableCollection<ResourceNode> ResourceNodes { get; private set; }

        public ObservableCollection<TerritoryResource> Resources { get; private set; }

        #endregion Lists

        private void RaisePropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
