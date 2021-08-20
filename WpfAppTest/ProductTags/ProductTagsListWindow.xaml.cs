using EconomicCalculator;
using EconomicCalculator.Storage.ProductTags;
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

namespace WpfAppTest.ProductTags
{
    /// <summary>
    /// Interaction logic for ProductTagsList.xaml
    /// </summary>
    public partial class ProductTagsListWindow : Window
    {
        private Manager manager;
        private List<IProductTagInfo> tags;

        public ProductTagsListWindow()
        {
            InitializeComponent();

            manager = Manager.Instance;

            manager.LoadProductTagInfo(@"D:\Projects\EconomicCalculator\EconomicCalculator\Data\CommonProductTagInfo.json");

            tags = manager.ProductTagInfo.Values.ToList();

            DataContext = tags;

            ProductTagGrid.ItemsSource = tags;
        }

        private void NewProductTag(object sender, RoutedEventArgs e)
        {
            var newTag = new ProductTagInfo();
            newTag.Id = manager.NewProductInfoTagId;
            Window win = new ProductTagInfoWindow(newTag);
            win.ShowDialog();

            ProductTagGrid.ItemsSource = manager.ProductTagInfo.Values;
            ProductTagGrid.Items.Refresh();
        }

        private void EditTag(object sender, RoutedEventArgs e)
        {
            var selected = (ProductTagInfo)ProductTagGrid.SelectedItem;

            if (selected == null)
                return;

            Window win = new ProductTagInfoWindow(selected);
            win.ShowDialog();

            ProductTagGrid.ItemsSource = manager.ProductTagInfo.Values;
            ProductTagGrid.Items.Refresh();
        }

        private void CopyTag(object sender, RoutedEventArgs e)
        {
            var selected = (ProductTagInfo)ProductTagGrid.SelectedItem;
            var dup = new ProductTagInfo(selected);
            dup.Id = manager.NewProductInfoTagId;

            Window win = new ProductTagInfoWindow(dup);
            win.ShowDialog();

            ProductTagGrid.ItemsSource = manager.ProductTagInfo.Values;
            ProductTagGrid.Items.Refresh();
        }

        private void SaveToFile(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Are you sure?", "Save Tags", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                manager.SaveProductTagInfo(@"D:\Projects\EconomicCalculator\EconomicCalculator\Data\CommonProductTagInfo.json");
                MessageBox.Show("Saved!", "Wants Saved", MessageBoxButton.OK);
            }
        }
    }
}
