using EconomicCalculator.DTOs.Market;
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

namespace EditorInterface.Markets
{
    /// <summary>
    /// Interaction logic for MarketEditorView.xaml
    /// </summary>
    public partial class MarketEditorView : Window
    {
        private MarketEditorViewModel viewModel;

        public MarketEditorView(MarketDTO original)
        {
            InitializeComponent();

            viewModel = new MarketEditorViewModel(original);

            DataContext = viewModel;

            //Territories.ItemsSource = viewModel.AvailableTerritories;

            AvailableNeighbors.ItemsSource = viewModel.AvailableMarkets;

            AvailableProducts.ItemsSource = viewModel.AvailableProducts;
        }
    }
}