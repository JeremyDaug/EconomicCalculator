using EconomicSim;
using EconomicSim.DTOs.Technology;
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

namespace EditorInterface.TechFamilies
{
    /// <summary>
    /// Interaction logic for TechFamiliesListWindow.xaml
    /// </summary>
    public partial class TechFamiliesListWindow : Window
    {
        DTOManager manager;

        public TechFamiliesListWindow()
        {
            InitializeComponent();

            manager = DTOManager.Instance;

            TechFamGrid.ItemsSource = manager.TechFamilies.Values.ToList();
        }

        private void CommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            this.Close();
        }

        private void NewTechFamily(object sender, RoutedEventArgs e)
        {
            var newWant = new TechFamilyDTO();

            newWant.Id = manager.NewTechFamilyId;
            Window win = new TechFamilyWindow(newWant);
            win.ShowDialog();

            TechFamGrid.ItemsSource = manager.TechFamilies.Values.ToList();
            TechFamGrid.Items.Refresh();
        }

        private void EditTechFamily(object sender, RoutedEventArgs e)
        {
            var selected = (TechFamilyDTO)TechFamGrid.SelectedItem;

            if (selected == null)
                return;

            Window win = new TechFamilyWindow(selected);
            win.ShowDialog();

            TechFamGrid.ItemsSource = manager.TechFamilies.Values.ToList();
            TechFamGrid.Items.Refresh();
        }

        private void CopyTechFamily(object sender, RoutedEventArgs e)
        {
            var selected = (TechFamilyDTO)TechFamGrid.SelectedItem;

            if (selected == null)
                return;
            
            var dup = new TechFamilyDTO
            {
                Id = manager.NewTechFamilyId,
                Name = selected.Name,
                Description = selected.Description,
                RelatedFamilies = new List<int>(selected.RelatedFamilies),
                RelatedFamilyStrings = new List<string>(selected.RelatedFamilyStrings),
                Techs = new List<int>(selected.Techs),
                TechStrings = new List<string>(selected.TechStrings)
            };

            Window win = new TechFamilyWindow(dup);
            win.ShowDialog();

            TechFamGrid.ItemsSource = manager.TechFamilies.Values.ToList();
            TechFamGrid.Items.Refresh();
        }

        private void SaveToFile(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Are you sure?", "Save TechFamilies", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                manager.SaveTechFamilies(@"D:\Projects\EconomicCalculator\EconomicCalculator\Data\CommonTechFamilies.json");
                MessageBox.Show("Saved!", "Tech Families Saved", MessageBoxButton.OK);
            }
        }
    }
}
