using EconomicCalculator;
using EconomicCalculator.Storage.Processes;
using EconomicCalculator.Storage.Processes.ProductionTags;
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

namespace EditorInterface.ProcessWindows
{
    /// <summary>
    /// Interaction logic for ProcessProductWindow.xaml
    /// </summary>
    public partial class ProcessProductWindow : Window
    {
        public ProcessProduct product;
        public ProcessProductViewModel ViewModel;

        public ProcessProductWindow(ProcessProduct procProd, ProcessSection section)
        {
            ViewModel = new ProcessProductViewModel(procProd, section);

            DataContext = ViewModel;

            InitializeComponent();
        }

        private void Commit(object sender, RoutedEventArgs e)
        {
            product.ProductName = (string)ProductOptions.SelectedItem;

            product.Amount = decimal.Parse(Amount.Text);
        }
    }
}
