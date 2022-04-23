using EconomicSim.DTOs.Pops;
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
    /// Interaction logic for PopEditorView.xaml
    /// </summary>
    public partial class PopEditorView : Window
    {
        private PopDTO pop;

        private PopEditorViewModel viewModel;

        public PopEditorView(PopDTO pop)
        {
            InitializeComponent();
            this.pop = pop;

            viewModel = new PopEditorViewModel(pop);

            DataContext = viewModel;

            SpeciesBox.ItemsSource = viewModel.AvailableSpecies;
            CultureBox.ItemsSource = viewModel.AvailableCultures;
        }

        private void CultureGrid_LayoutUpdated(object sender, EventArgs e)
        {
            viewModel.CultureSum++;
        }

        private void Species_LayoutUpdated(object sender, EventArgs e)
        {
            viewModel.SpeciesSum++;
        }
    }
}
