using EconomicCalculator.Storage.Products.ProductTags;
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

namespace WpfAppTest.Products
{
    /// <summary>
    /// Interaction logic for ProductTagSelector.xaml
    /// </summary>
    public partial class ProductTagSelector : Window
    {
        public string SelectedTag { get; set; }



        public ProductTagSelector()
        {
            InitializeComponent();

            Options.ItemsSource = ProductTagInfo.GetProductTags();
        }

        private void Options_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectedTag = (string)Options.SelectedValue;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
