using EconomicCalculator;
using EconomicCalculator.DTOs.Pops;
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

namespace EditorInterface.Populations
{
    /// <summary>
    /// Interaction logic for PopsListView.xaml
    /// </summary>
    public partial class PopsListView : Window
    {
        private DTOManager manager = DTOManager.Instance;

        public PopsListView()
        {
            InitializeComponent();

            PopGrid.ItemsSource = manager.Pops.Values;
        }

        private void EditPop(object sender, RoutedEventArgs e)
        {
            if (PopGrid.SelectedItem == null)
                return;

            PopEditorView win = new PopEditorView((PopDTO)PopGrid.SelectedItem);

            win.ShowDialog();

            PopGrid.ItemsSource = manager.Pops.Values;
            PopGrid.Items.Refresh();
        }

        private void SavePops(object sender, RoutedEventArgs e)
        {
            manager.SavePops(@"D:\Projects\EconomicCalculator\EconomicCalculator\Data\Pops.json");

            MessageBox.Show("Pops Saved.", "Saved!", MessageBoxButton.OK, MessageBoxImage.Exclamation);
        }

        private void PopGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            EditPop(sender, e);
        }
    }
}
