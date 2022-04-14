using EconomicCalculator;
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
    /// Interaction logic for MarketsListView.xaml
    /// </summary>
    public partial class MarketsListView : Window
    {
        private DTOManager manager = DTOManager.Instance;

        public MarketsListView()
        {
            InitializeComponent();

            MarketGrid.ItemsSource = manager.Markets.Values;
        }

        private void NewMarket(object sender, RoutedEventArgs e)
        {
            var newMarket = new MarketDTO();
            newMarket.Id = manager.NewMarketId;

            var win = new MarketEditorView(newMarket);

            win.ShowDialog();

            MarketGrid.ItemsSource = manager.Markets.Values;
            MarketGrid.Items.Refresh();
        }

        private void EditMarket(object sender, RoutedEventArgs e)
        {
            var selected = (MarketDTO)MarketGrid.SelectedItem;

            if (selected == null)
                return;

            var win = new MarketEditorView(selected);

            win.ShowDialog();
            MarketGrid.ItemsSource = manager.Markets.Values;
            MarketGrid.Items.Refresh();
        }

        private void CopyMarket(object sender, RoutedEventArgs e)
        {

        }

        private void SaveMarkets(object sender, RoutedEventArgs e)
        {
            manager.SaveMarkets(@"D:\Projects\EconomicCalculator\EconomicCalculator\Data\Markets.json");

            MessageBox.Show("Markets Saved!");
        }
    }
}
