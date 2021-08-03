using EconomicCalculator;
using EconomicCalculator.Storage.Products;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using MessageBox = System.Windows.MessageBox;

namespace WpfAppTest.Products
{
    /// <summary>
    /// Interaction logic for ProductWindow.xaml
    /// </summary>
    public partial class ProductWindow : Window
    {
        private Manager manager;
        private Product product;

        public ProductWindow()
        {
            InitializeComponent();

            // get the manager
            manager = Manager.Instance;
        }

        public ProductWindow(Product product)
        {
            // get manager
            manager = Manager.Instance;

            InitializeComponent();

            this.product = product;

            ProductName.Text = product.Name;
            VariantName.Text = product.VariantName;
            Unit.Text = product.UnitName;
            Quality.Text = product.Quality.ToString();
            Mass.Text = product.Mass.ToString();
            Bulk.Text = product.Bulk.ToString();
            IsFractional.IsChecked = product.Fractional;
            ProductId.Text = product.Id.ToString();
            ImageSelected.Text = product.Icon;

            // get image if any
            var imgLoc = "";
            if (!string.IsNullOrWhiteSpace(product.Icon))
            {
                imgLoc = System.IO.Path.Combine(manager.DataFolder, product.Icon);
            }
            else // get default
            {
                imgLoc = System.IO.Path.Combine(manager.DataFolder, manager.DefaultIcon);
            }
            ImageView.Source = new BitmapImage(new Uri(imgLoc));
        }

        /// <summary>
        /// Saves the product to the Manager.
        /// TODO, currently this just saves it to a file directly.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Save_Product(object sender, RoutedEventArgs e)
        {
            try
            {
                product = new Product
                {
                    Id = product.Id,
                    Name = ProductName.Text,
                    VariantName = VariantName.Text,
                    UnitName = Unit.Text,
                    Quality = int.Parse(Quality.Text),
                    Mass = decimal.Parse(Mass.Text),
                    Bulk = decimal.Parse(Bulk.Text),
                    Fractional = IsFractional.IsChecked.Value,
                    Icon = product.Icon
                };
            }
            catch (Exception error)
            {
                MessageBox.Show(error.Message, "Error Occured.", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // if Id already exists in manager, just override and stop
            // we won't be accidentally entering duplicate data.
            if (manager.ContainsProduct(product))
            {
                manager.Products[product.Id] = product;
                return;
            }

            // if not contained already, check for duplication, then
            // save or message user appropriately.
            var dup = manager.FindDuplicate(product);
            if (dup != null)
            {
                MessageBox.Show(string.Format("String is duplicate of {0} -> {1}", dup.Id, dup.ToString()), "Duplicate Found", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }

            manager.Products[product.Id] = product;
        }

        /// <summary>
        /// Loads product from the manager.
        /// TODO delete this entirely, this should not be used here.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Load_Products(object sender, RoutedEventArgs e)
        {
            using (StreamReader file = File.OpenText(@"D:\Projects\EconomicCalculator\EconomicCalculator\Data\CommonProducts.json"))
            {
                JsonSerializer jser = new JsonSerializer();
                product = (Product)jser.Deserialize(file, typeof(Product));
            }
        }

        private void BackToWelcomeScreen(object sender, RoutedEventArgs e)
        {
            Window win = new MainWindow();

            win.Show();
            this.Close();
        }

        private void CloseBind_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to close?", "Close", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                this.Close();
        }

        private void SelectImage(object sender, RoutedEventArgs e)
        {
            var img = new OpenFileDialog();
            img.Filter = "Image Files|*.png;*.jpg;*.bmp";
            if (img.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                var location = img.FileName.Replace(manager.DataFolder, "");

                product.Icon = location;
                ImageSelected.Text = location;
                ImageView.Source = new BitmapImage(new Uri(img.FileName));
            }
        }
    }
}
