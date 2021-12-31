using EconDTOs;
using EconDTOs.DTOs.Products;
using EconDTOs.DTOs.Products.ProductTags;
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
using WpfAppTest.Products;
using MessageBox = System.Windows.MessageBox;

namespace EditorInterface.Products
{
    /// <summary>
    /// Interaction logic for ProductWindow.xaml
    /// </summary>
    public partial class ProductWindow : Window
    {
        private Manager manager;
        private Product product;

        private List<WantWeight> wants;
        private List<string> availableWants;

        private List<TagData> ProductTags;

        public ProductWindow()
        {
            InitializeComponent();

            // get the manager
            manager = Manager.Instance;

            product = new Product();

            product.Id = manager.NewProductId;

            availableWants = manager.Wants.Values.Select(x => x.Name).ToList();

            wants = new List<WantWeight>();

            WantsGrid.ItemsSource = wants;
            Name.ItemsSource = availableWants;

            ProductTags = new List<TagData>();

            TagGrid.ItemsSource = ProductTags;
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

            // Set wants
            wants = new List<WantWeight>();
            foreach (var want in product.Wants)
            {
                wants.Add(new WantWeight
                {
                    Name = manager.Wants[want.Key].Name,
                    Satisfaction = want.Value
                });
            }

            WantsGrid.ItemsSource = wants;
            availableWants = manager.Wants.Values.Select(x => x.Name).ToList();
            Name.ItemsSource = availableWants;

            // Set Tags
            ProductTags = new List<TagData>();
            foreach (var tag in product.TagStrings)
                ProductTags.Add(new TagData { Tag = tag });

            TagGrid.ItemsSource = ProductTags;

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
            // sanity checks
            // ensure no wants appear twice.
            var wants = WantsGrid.Items.OfType<WantWeight>().Select(x => x.Name);
            if (wants.GroupBy(x => x).Any(c => c.Count() > 1))
            {
                MessageBox.Show("Duplicate Want found. Please remove duplicates.");
                return;
            }

            try
            {
                // easy product stuff.
                product = new Product
                {
                    Id = product.Id,
                    Name = ProductName.Text.Trim(),
                    VariantName = VariantName.Text.Trim(),
                    UnitName = Unit.Text.Trim(),
                    Quality = int.Parse(Quality.Text),
                    Mass = decimal.Parse(Mass.Text),
                    Bulk = decimal.Parse(Bulk.Text),
                    Fractional = IsFractional.IsChecked.Value,
                    Icon = product.Icon
                };

                // Wants
                foreach (WantWeight want in WantsGrid.Items.OfType<WantWeight>())
                {
                    var origin = manager.GetWantByName(want.Name);

                    product.Wants[origin.Id] = want.Satisfaction;
                }

                // WantString
                foreach (var wantId in product.Wants.Keys.OrderBy(x => x))
                {
                    var want = manager.Wants[wantId];
                    product.WantStrings
                        .Add(want.ToSatisfactionString(product.Wants[wantId]));
                }

                // Tags
                foreach (string tag in ProductTags.Select(x => x.Tag))
                {
                    try
                    {
                        var newTag = ProductTagInfo.ProcessTagString(tag);
                        product.Tags.Add(newTag);
                        product.TagStrings.Add(tag);
                    }
                    catch (ArgumentException tagError)
                    {
                        MessageBox.Show(tagError.Message, "Invalid Tag", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                }
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

        private void AddTag(object sender, RoutedEventArgs e)
        {
            ProductTagSelector win = new ProductTagSelector();
            win.ShowDialog();

            var newTag = win.SelectedTag;

            string example;
            try
            {
                example = ProductTagInfo.GetProductExample(newTag);
            }
            catch (ArgumentNullException)
            { // if the user didn't select anything, just return out.
                return;
            }

            ProductTags.Add(new TagData { Tag = example });

            TagGrid.Items.Refresh();
        }
    }
}
