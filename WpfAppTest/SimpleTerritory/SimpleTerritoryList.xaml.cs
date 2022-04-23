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
    /// Interaction logic for SimpleTerritoryList.xaml
    /// </summary>
    public partial class SimpleTerritoryList : Window
    {

        private DTOManager manager = DTOManager.Instance;

        public SimpleTerritoryList()
        {
            InitializeComponent();

            TerritoryGrid.ItemsSource = manager.SimpleTerritories;
        }

        private void NewTerritory(object sender, RoutedEventArgs e)
        {
            var newTerr = new SimpleTerritoryDTO();

            Window win = new SimpleTerritoryView(newTerr);
            win.ShowDialog();

            TerritoryGrid.ItemsSource = manager.SimpleTerritories.ToList();
            TerritoryGrid.Items.Refresh();
        }

        private void EditTerritory(object sender, RoutedEventArgs e)
        {
            var selected = (SimpleTerritoryDTO)TerritoryGrid.SelectedItem;

            if (selected == null)
                return;

            Window win = new SimpleTerritoryView(selected);
            win.ShowDialog();

            TerritoryGrid.ItemsSource = manager.SimpleTerritories.ToList();
            TerritoryGrid.Items.Refresh();
        }

        private void CopyTerritory(object sender, RoutedEventArgs e)
        {
            var selected = (SimpleTerritoryDTO)TerritoryGrid.SelectedItem;

            if (selected == null)
                return;

            var dupe = new SimpleTerritoryDTO 
            { 
                Name = selected.Name,
                Coords = selected.Coords,
                HasLake = selected.HasLake,
                IsCoastal = selected.IsCoastal,
                Land = selected.Land,
                Size = selected.Size,
                Water = selected.Water,
                Neighbors = selected.Neighbors.ToList(),
                Nodes = selected.Nodes.ToList(),
                Plots = selected.Plots.ToList(),
                Resources = selected.Resources.ToList(),
            };

            Window win = new SimpleTerritoryView(dupe);
            win.ShowDialog();

            TerritoryGrid.ItemsSource = manager.SimpleTerritories.ToList();
            TerritoryGrid.Items.Refresh();
        }

        private void SaveTerritories(object sender, RoutedEventArgs e)
        {
            manager.SaveSimpleTerritories(@"D:\Projects\EconomicCalculator\EconomicCalculator\Data\Territories.json");

            MessageBox.Show("Territories Saved!");
        }
    }
}
