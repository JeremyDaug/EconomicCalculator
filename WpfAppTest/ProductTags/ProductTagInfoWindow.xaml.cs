using EconomicCalculator;
using EconomicCalculator.Enums;
using EconomicCalculator.Storage.ProductTags;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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

namespace EditorInterface.ProductTags
{
    /// <summary>
    /// Interaction logic for ProductTagInfoWindow.xaml
    /// </summary>
    public partial class ProductTagInfoWindow : Window
    {
        private ProductTagInfo tag;
        private ObservableCollection<SelectedParam> paramList;
        private Manager manager;

        public ProductTagInfoWindow()
        {
            InitializeComponent();

            paramList = new ObservableCollection<SelectedParam>();

            DataContext = paramList;

            manager = Manager.Instance;
        }

        public ProductTagInfoWindow(ProductTagInfo tag)
        {
            InitializeComponent();

            manager = Manager.Instance;

            this.tag = tag;

            TagName.Text = tag.Tag;

            Description.Text = tag.Description;

            TagId.Text = tag.Id.ToString();

            paramList = new ObservableCollection<SelectedParam>();

            foreach (var param in tag.Params)
            {
                paramList.Add(new SelectedParam { ParameterType = param });
            }

            DataContext = paramList;
            
        }

        private void CommitTag(object sender, RoutedEventArgs e)
        {
            // get all into a tag.
            var tag = new ProductTagInfo
            {
                Id = this.tag.Id,
                Tag = TagName.Text.Trim(),
                Description = Description.Text
            };

            // ensure name has no whitespace.
            if (tag.Tag.Any(x => char.IsWhiteSpace(x)))
            {
                MessageBox.Show("Tag Cannot Have any Whitespace.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            foreach (var param in paramList)
                tag.Params.Add(param.ParameterType);

            // check for duplicates.
            if (manager.ContainsProductTag(tag))
            {
                // if it already exists, add it.
                manager.ProductTagInfo[tag.Id] = tag;
            }

            // if no id match, check for duplicates,
            var dup = manager.FindDuplicate(tag);
            if (dup != null)
            {
                MessageBox.Show(string.Format("Tag is duplicate of {0} -> {1}", tag.Id, tag.Tag),
                    "Duplicate Found", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // if no duplicates, add it.
            manager.ProductTagInfo[tag.Id] = tag;
        }
    }

    public class SelectedParam
    {
        public ParameterType ParameterType { get; set; }
    }
}
