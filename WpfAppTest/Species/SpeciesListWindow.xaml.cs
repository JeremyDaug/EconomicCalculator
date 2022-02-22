using EconomicCalculator;
using EconomicCalculator.DTOs.Pops.Species;
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

namespace EditorInterface.Species
{
    /// <summary>
    /// Interaction logic for SpeciesListWindow.xaml
    /// </summary>
    public partial class SpeciesListWindow : Window
    {
        DTOManager manager;

        public SpeciesListWindow()
        {
            InitializeComponent();

            manager = DTOManager.Instance;

            SpeciesGrid.ItemsSource = manager.Species.Values;
        }

        private void ClickEdit(object sender, MouseButtonEventArgs e)
        {
            EditSpecies(sender, e);
        }

        private void NewSpecies(object sender, RoutedEventArgs e)
        {
            var newSpec = new SpeciesDTO();

            newSpec.Id = manager.NewSpeciesId;

            Window win = new SpeciesEditor(newSpec);
            win.ShowDialog();

            SpeciesGrid.ItemsSource = manager.Species.Values;
            SpeciesGrid.Items.Refresh();
        }

        private void EditSpecies(object sender, RoutedEventArgs e)
        {
            var selected = (SpeciesDTO)SpeciesGrid.SelectedItem;
            if (selected == null)
                return;

            Window win = new SpeciesEditor(selected);
            win.ShowDialog();

            SpeciesGrid.ItemsSource = manager.Species.Values;
            SpeciesGrid.Items.Refresh();
        }

        private void CopySpecies(object sender, RoutedEventArgs e)
        {
            var selected = (SpeciesDTO)SpeciesGrid.SelectedItem;
            if (selected == null)
                return;

            var dup = new SpeciesDTO
            {
                Id = manager.NewSpeciesId,
                Name = selected.Name,
                VariantName = selected.VariantName,
                BirthRate = selected.BirthRate,
                LifeSpan = selected.LifeSpan,
                Needs = selected.Needs.ToList(),
                RelatedSpecies = selected.RelatedSpecies.ToList(),
                RelatedSpeciesIds = selected.RelatedSpeciesIds.ToList(),
                Tags = selected.Tags.ToList(),
                TagStrings = selected.TagStrings.ToList(),
                Wants = selected.Wants.ToList(),
            };

            Window win = new SpeciesEditor(dup);
            win.ShowDialog();

            SpeciesGrid.ItemsSource = manager.Species.Values;
            SpeciesGrid.Items.Refresh();
        }

        private void SaveSpecies(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Are you sure?", "Save Species", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                manager.SaveSpecies(@"D:\Projects\EconomicCalculator\EconomicCalculator\Data\CommonSpecies.json");
                MessageBox.Show("Saved!", "Species Saved.", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }
    }
}
