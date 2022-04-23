using EconomicSim;
using EconomicSim.DTOs.Pops.Species;
using EditorInterface.Species.SpeciesNeedEditor;
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
    /// Interaction logic for SpeciesEditor.xaml
    /// </summary>
    public partial class SpeciesEditor : Window
    {
        private SpeciesEditorViewModel vm;

        DTOManager manager = DTOManager.Instance;

        public SpeciesEditor(SpeciesDTO species)
        {
            InitializeComponent();

            vm = new SpeciesEditorViewModel(species);

            DataContext = vm;

            RelBox.ItemsSource = vm.AllSpecies;
        }

        private void EditNeed(object sender, MouseButtonEventArgs e)
        {
            vm.EditExistingNeed();
        }

        private void EditWant(object sender, MouseButtonEventArgs e)
        {
            vm.EditExistingWant();
        }

        private void Commit(object sender, RoutedEventArgs e)
        {
            vm.Commit();
        }
    }
}
