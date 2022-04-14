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
    /// Interaction logic for JobSelectionView.xaml
    /// </summary>
    public partial class SelectionView : Window
    {
        public string Selection;

        public SelectionView(List<string> Options)
        {
            InitializeComponent();

            SelectionOptions.ItemsSource = Options;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Selection = (string)SelectionOptions.SelectedItem;

            this.Close();
        }
    }
}
