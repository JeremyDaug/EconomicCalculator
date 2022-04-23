using EconomicSim;
using EconomicSim.DTOs.Pops.Culture;
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

namespace EditorInterface.Cultures
{
    /// <summary>
    /// Interaction logic for CultureListView.xaml
    /// </summary>
    public partial class CultureListView : Window
    {
        private DTOManager manager;

        public CultureListView()
        {
            InitializeComponent();

            manager = DTOManager.Instance;

            CultureGrid.ItemsSource = manager.Cultures.Values;
        }

        private void NewCulture(object sender, RoutedEventArgs e)
        {
            var newCulture = new CultureDTO();

            newCulture.Id = manager.NewCultureId;

            Window win = new CultureEditorView(newCulture);
            win.ShowDialog();

            CultureGrid.ItemsSource = manager.Cultures.Values;
            CultureGrid.Items.Refresh();
        }

        private void CopyCulture(object sender, RoutedEventArgs e)
        {
            var selected = (CultureDTO)CultureGrid.SelectedItem;
            if (selected == null)
                return;

            var dup = new CultureDTO
            {
                Id = manager.NewCultureId,
                Name = selected.Name,
                VariantName = selected.VariantName,
                BirthModifier = selected.BirthModifier,
                DeathModifier = selected.DeathModifier,
                Needs = selected.Needs.ToList(),
                Wants = selected.Wants.ToList(),
                RelatedCultures = selected.RelatedCultures.ToList(),
                RelatedCulturesIds = selected.RelatedCulturesIds.ToList(),
                Tags = selected.Tags.ToList(),
                TagsStrings = selected.TagsStrings.ToList()
            };

            Window win = new CultureEditorView(dup);
            win.ShowDialog();

            CultureGrid.ItemsSource = manager.Cultures.Values;
            CultureGrid.Items.Refresh();
        }

        private void EditCulture(object sender, RoutedEventArgs e)
        {
            var selected = (CultureDTO)CultureGrid.SelectedItem;
            if (selected == null)
                return;

            Window win = new CultureEditorView(selected);
            win.ShowDialog();

            CultureGrid.ItemsSource = manager.Cultures.Values;
            CultureGrid.Items.Refresh();
        }

        private void SaveCultures(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Are you sure?", "Save Cultures", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                manager.SaveCultures(@"D:\Projects\EconomicCalculator\EconomicCalculator\Data\CommonCultures.json");
                MessageBox.Show("Saved!", "Cultures Saved.", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }
    }
}
