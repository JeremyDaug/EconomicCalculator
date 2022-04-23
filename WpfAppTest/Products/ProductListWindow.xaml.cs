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
using EconomicSim;
using EconomicSim.DTOs.Products;
using EconomicSim.DTOs.Wants;
using Editor.Wants;

namespace Editor.Products
{
    /// <summary>
    /// Interaction logic for ProductListWindow.xaml
    /// </summary>
    public partial class ProductListWindow : Window
    {
        private DTOManager manager;

        public ProductListWindow()
        {
            InitializeComponent();

            manager = DTOManager.Instance;

            //manager.LoadAll();

            ProductGrid.ItemsSource = manager.Products.Values;
        }

        private void NewProduct(object sender, RoutedEventArgs e)
        {
            var newProduct = new ProductDTO();

            newProduct.Id = manager.NewProductId;

            Window win = new ProductWindow(newProduct);
            win.ShowDialog();

            ProductGrid.ItemsSource = manager.Products.Values;
            ProductGrid.Items.Refresh();
        }

        private void NewWant(object sender, RoutedEventArgs e)
        {
            var newWant = new WantDTO();

            newWant.Id = manager.NewWantId;
            Window win = new WantWindow(newWant);
            win.ShowDialog();

            ProductGrid.ItemsSource = manager.Products.Values;
            ProductGrid.Items.Refresh();
        }

        private void EditProduct(object sender, RoutedEventArgs e)
        {
            var selected = (ProductDTO)ProductGrid.SelectedItem;

            if (selected == null)
                return;

            Window win = new ProductWindow(selected);

            win.ShowDialog();

            ProductGrid.ItemsSource = manager.Products.Values;
            ProductGrid.Items.Refresh();
        }

        private void SaveToFile(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Are you sure?", "Save Products", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                manager.SaveProducts(@"D:\Projects\EconomicCalculator\EconomicCalculator\Data\CommonProducts.json");
                MessageBox.Show("Saved!", "Products Saved", MessageBoxButton.OK);
            }
        }

        private void CloseBind_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to close?", "Close", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                this.Close();
        }

        private void CopyProduct(object sender, RoutedEventArgs e)
        {
            var selected = (ProductDTO)ProductGrid.SelectedItem;

            var dup = new ProductDTO(selected);

            dup.Id = manager.NewProductId;

            Window win = new ProductWindow(dup);

            win.ShowDialog();

            ProductGrid.ItemsSource = manager.Products.Values;
            ProductGrid.Items.Refresh();
        }

        private void BackToWelcomeScreen(object sender, RoutedEventArgs e)
        {
            Window win = new MainWindow();

            win.Show();
            this.Close();
        }
    }
}
