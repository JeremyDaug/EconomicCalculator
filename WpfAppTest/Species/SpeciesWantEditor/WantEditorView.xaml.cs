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

namespace EditorInterface.Species.SpeciesWantEditor
{
    /// <summary>
    /// Interaction logic for WantEditorView.xaml
    /// </summary>
    public partial class WantEditorView : Window
    {
        DTOManager manager;

        internal WantEditorViewModel viewModel;

        public WantEditorView(SpeciesWantDTO origin)
        {
            InitializeComponent();

            viewModel = new WantEditorViewModel(origin);

            DataContext = viewModel;
        }

        private void AddWant(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(viewModel.Want))
            {
                MessageBox.Show("Must select product.", "No Product", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (string.IsNullOrEmpty(viewModel.Tier))
            {
                MessageBox.Show("Must select Tier.", "No Tier", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (viewModel.Amount == 0)
            {
                MessageBox.Show("Amount must be Nonzero.", "No Amount", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            viewModel.Complete = true;

            Close();
        }

        private void CancelAddition(object sender, RoutedEventArgs e)
        {
            viewModel.Complete = false;

            Close();
        }
    }
}
