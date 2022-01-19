using EconomicCalculator;
using EconomicCalculator.DTOs.Wants;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace Editor.Wants
{
    /// <summary>
    /// Interaction logic for WantsListWindow.xaml
    /// </summary>
    public partial class WantsListWindow : Window
    {
        private DTOManager manager;
        private List<WantDTO> wants;

        public WantsListWindow()
        {
            InitializeComponent();

            manager = DTOManager.Instance;

            WantGrid.ItemsSource = manager.Wants.Values.ToList();
        }

        private void BackToWelcomeScreen(object sender, RoutedEventArgs e)
        {
            Window win = new MainWindow();

            win.Show();
            this.Close();
        }

        private void CloseBind_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to close?", "Close", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                this.Close();
        }

        private void NewWant(object sender, RoutedEventArgs e)
        {
            var newWant = new WantDTO();

            newWant.Id = manager.NewWantId;
            Window win = new WantWindow(newWant);
            win.ShowDialog();

            WantGrid.ItemsSource = manager.Wants.Values;
            WantGrid.Items.Refresh();
        }

        private void EditWant(object sender, RoutedEventArgs e)
        {
            var selected = (WantDTO)WantGrid.SelectedItem;

            Window win = new WantWindow(selected);
            win.ShowDialog();

            WantGrid.ItemsSource = manager.Wants.Values;
            WantGrid.Items.Refresh();
        }

        private void CopyWant(object sender, RoutedEventArgs e)
        {
            var selected = (WantDTO)WantGrid.SelectedItem;
            var dup = new WantDTO(selected);
            dup.Id = manager.NewWantId;

            Window win = new WantWindow(dup);
            win.ShowDialog();

            WantGrid.ItemsSource = manager.Wants.Values;
            WantGrid.Items.Refresh();
        }

        private void SaveToFile(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Are you sure?", "Save Wants", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                manager.SaveWants(@"D:\Projects\EconomicCalculator\EconomicCalculator\Data\CommonWants.json");
                MessageBox.Show("Saved!", "Wants Saved", MessageBoxButton.OK);
            }
        }
    }
}
