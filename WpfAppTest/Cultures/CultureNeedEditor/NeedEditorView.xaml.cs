using EconomicSim;
using EconomicSim.DTOs.Pops.Culture;
using EconomicSim.DTOs.Pops.Species;
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

namespace EditorInterface.Cultures.CultureNeedEditor
{
    /// <summary>
    /// Interaction logic for NeedEditorView.xaml
    /// </summary>
    public partial class NeedEditorView : Window
    {
        DTOManager manager;

        internal NeedEditorViewModel viewModel;

        public NeedEditorView(CultureNeedDTO origin)
        {
            InitializeComponent();

            viewModel = new NeedEditorViewModel(origin);

            DataContext = viewModel;
        }

        private void AddNeed(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(viewModel.Product))
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
