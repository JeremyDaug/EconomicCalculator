using EconomicCalculator;
using EconomicCalculator.DTOs.Pops.Culture;
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
    /// Interaction logic for CultureEditorView.xaml
    /// </summary>
    public partial class CultureEditorView : Window
    {
        private CultureEditorViewModel vm;

        DTOManager manager = DTOManager.Instance;

        public CultureEditorView(CultureDTO original)
        {
            InitializeComponent();

            vm = new CultureEditorViewModel(original);

            DataContext = vm;

            RelBox.ItemsSource = vm.AllCultures;
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
