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

namespace EditorInterface.Techs
{
    /// <summary>
    /// Interaction logic for TechListWindow.xaml
    /// </summary>
    public partial class TechListWindow : Window
    {
        DTOManager manager;
        public TechListWindow()
        {
            InitializeComponent();

            manager = DTOManager.Instance;

            TechGrid.ItemsSource = manager.Technologies.Values;
        }

        private void NewTech(object sender, RoutedEventArgs e)
        {
            var newTech = new TechnologyDTO();

            newTech.Id = manager.NewTechId;

            Window win = new TechnologyEditorWindow(newTech);
            win.ShowDialog();

            TechGrid.ItemsSource = manager.Technologies.Values;
            TechGrid.Items.Refresh();
        }

        private void EditTech(object sender, RoutedEventArgs e)
        {
            var selected = (TechnologyDTO)TechGrid.SelectedItem;
            if (selected == null)
                return;

            Window win = new TechnologyEditorWindow(selected);
            win.ShowDialog();

            TechGrid.ItemsSource = manager.Technologies.Values;
            TechGrid.Items.Refresh();
        }

        private void CopyTech(object sender, RoutedEventArgs e)
        {
            var selected = (TechnologyDTO)TechGrid.SelectedItem;
            if (selected == null)
                return;

            var dup = new TechnologyDTO
            {
                Id = manager.NewTechId,
                Category = selected.Category,
                Children = selected.Children.ToList(),
                ChildrenIds = selected.ChildrenIds.ToList(),
                ParentIds = selected.ParentIds.ToList(),
                Description = selected.Description,
                Families = selected.Families.ToList(),
                FamilyIds = selected.FamilyIds.ToList(),
                Name = selected.Name,
                Parents = selected.Parents.ToList(),
                TechBaseCost = selected.TechBaseCost,
                Tier = selected.Tier
            };

            Window win = new TechnologyEditorWindow(selected);
            win.ShowDialog();

            TechGrid.ItemsSource = manager.Technologies.Values;
            TechGrid.Items.Refresh();
        }

        private void SaveToFile(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Are you sure?", "Save Technologies", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                manager.SaveTechs(@"D:\Projects\EconomicCalculator\EconomicCalculator\Data\CommonTechs.json");
                MessageBox.Show("Saved!", "Technologies Saved.", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }
    }
}
