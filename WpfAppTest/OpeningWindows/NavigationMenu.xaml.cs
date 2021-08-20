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

namespace WpfAppTest.OpeningWindows
{
    /// <summary>
    /// Interaction logic for NavigationMenu.xaml
    /// </summary>
    public partial class NavigationMenu : Window
    {
        public NavigationMenu()
        {
            InitializeComponent();
        }

        private void ToProducts(object sender, RoutedEventArgs e)
        {
            var win = new Products.ProductListWindow();

            win.Show();

            this.Close();
        }

        private void ToWants(object sender, RoutedEventArgs e)
        {
            var win = new Wants.WantsListWindow();

            win.Show();

            this.Close();
        }

        private void ToProductTags(object sender, RoutedEventArgs e)
        {
            var win = new ProductTags.ProductTagsListWindow();

            win.Show();

            this.Close();
        }

        private void ProductTagsShortcut(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.D3)
            {
                ToProductTags(sender, e);
            }
            return;
        }
    }
}
