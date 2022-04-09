using EconomicCalculator;
using EconomicCalculator.DTOs.Firms;
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
    /// Interaction logic for FirmsListView.xaml
    /// </summary>
    public partial class FirmsListView : Window
    {
        private DTOManager manager = DTOManager.Instance;

        public FirmsListView()
        {
            InitializeComponent();

            FirmGrid.ItemsSource = manager.Firms
                .Values.Where(x => x.ParentFirm == null);
        }

        private void NewFirm(object sender, RoutedEventArgs e)
        {
            var newFirm = new FirmDTO();

            newFirm.Id = manager.NewFirmId;

            var window = new FirmEditorView(newFirm);

            window.ShowDialog();

            FirmGrid.ItemsSource = manager.Firms.Values;
            FirmGrid.Items.Refresh();
        }

        private void EditFirm(object sender, RoutedEventArgs e)
        {

        }

        private void CopyFirm(object sender, RoutedEventArgs e)
        {

        }

        private void SaveFirms(object sender, RoutedEventArgs e)
        {

        }
    }
}
