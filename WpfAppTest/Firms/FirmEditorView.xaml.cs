using EconomicSim.DTOs.Firms;
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

namespace EditorInterface.Firms
{
    /// <summary>
    /// Interaction logic for FirmEditorView.xaml
    /// </summary>
    public partial class FirmEditorView : Window
    {
        private IFirmDTO original;

        private FirmEditorViewModel viewModel;

        public FirmEditorView(IFirmDTO original)
        {
            InitializeComponent();

            this.original = original;

            viewModel = new FirmEditorViewModel(original);

            DataContext = viewModel;

            SubfirmColumn.ItemsSource = viewModel.PossibleChildren;

            RegionOptions.ItemsSource = viewModel.MarketOptions;

            JobOptions.ItemsSource = viewModel.JobOptions;
            WageOptions.ItemsSource = viewModel.WageOptions;
            ProcessOptions.ItemsSource = viewModel.ProcessOptions;
            ProductOptions.ItemsSource = viewModel.ProductOptions;
            ResourceOptions.ItemsSource = viewModel.ProductOptions;
        }

        private void RemovePop(object sender, RoutedEventArgs e)
        {
            var selection = (PopDTO)EmployeeGrid.SelectedItem;

            if (selection == null)
                return;

            viewModel.RemovePop(selection);
        }
    }
}
