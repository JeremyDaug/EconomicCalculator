using EconomicSim;
using EconomicSim.DTOs.Territory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace EditorInterface.SimpleTerritory
{
    /// <summary>
    /// Interaction logic for SimpleTerritoryView.xaml
    /// </summary>
    public partial class SimpleTerritoryView : Window
    {
        private DTOManager manager = DTOManager.Instance;
        private SimpleTerritoryViewModel viewModel;

        public SimpleTerritoryView(SimpleTerritoryDTO original)
        {
            InitializeComponent();

            viewModel = new SimpleTerritoryViewModel(original);

            DataContext = viewModel;

            TerritoryConns.ItemsSource = viewModel.AvailableTerritories;
            ConnTypes.ItemsSource = viewModel.ConnectionTypeEnumOptions;
            ResourceTypes.ItemsSource = viewModel.AvailableResources;
            SurfaceResource.ItemsSource = viewModel.AvailableResources;
            DepthOptions.ItemsSource = new List<int>(Enumerable.Range(1,11));
        }

        private void DataGrid_LayoutUpdated(object sender, EventArgs e)
        {
            viewModel.RemainingPlots++;
        }
    }
}
