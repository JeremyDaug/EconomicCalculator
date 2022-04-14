using EconomicCalculator;
using EconomicCalculator.DTOs.Wants;
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

namespace Editor.Wants
{
    /// <summary>
    /// Interaction logic for WantWindow.xaml
    /// </summary>
    public partial class WantWindow : Window
    {
        private DTOManager manager;
        private WantDTO want;

        public WantWindow()
        {
            manager = DTOManager.Instance;

            want = new WantDTO
            {
                Id = manager.NewWantId
            };

            InitializeComponent();

            WantName.Text = want.Name;
            WantDescription.Text = want.Description;
        }

        public WantWindow(WantDTO want)
        {
            this.want = want;
            manager = DTOManager.Instance;
            InitializeComponent();

            WantName.Text = want.Name;
            WantDescription.Text = want.Description;
        }

        private void CloseBind_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to close?", "Close", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                this.Close();
        }

        private void CommitWant(object sender, RoutedEventArgs e)
        {
            want = new WantDTO
            {
                Id = want.Id,
                Name = WantName.Text,
                Description = WantDescription.Text
            };

            // check it's valid.
            if (WantDTO.NameIsValid(want.Name))
            {
                MessageBox.Show("Name Cannot have whitespace, and can only contain letters.");
                return;
            }

            // If it exists in the manager already, update.
            if (manager.ContainsWant(want))
            {
                manager.Wants[want.Id] = want;
            }

            // if not already in, check for dups,
            var dup = manager.FindDuplicate(want);
            if (dup != null)
            {
                var result = MessageBox.Show("This want already exists. Do you want to overwrite?", "Duplicate found", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes)
                {
                    want.Id = dup.Id;
                    want.Description = WantDescription.Text;
                    manager.Wants[want.Id] = want;
                }

                return;
            }

            // if not already in, just add it.
            manager.Wants[want.Id] = want;
        }

        private void CommitAndQuit(object sender, RoutedEventArgs e)
        {
            want = new WantDTO
            {
                Id = want.Id,
                Name = WantName.Text,
                Description = WantDescription.Text
            };

            // check it's valid.
            if (string.IsNullOrWhiteSpace(want.Name))
            {
                MessageBox.Show("Name Cannot be empty or whitespace.");
                return;
            }

            // If it exists in the manager already, update.
            if (manager.ContainsWant(want))
            {
                manager.Wants[want.Id] = want;
            }

            // if not already in, check for dups,
            var dup = manager.FindDuplicate(want);
            if (dup != null)
            {
                var result = MessageBox.Show("This want already exists. Do you want to overwrite?", "Duplicate found", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes)
                {
                    want.Id = dup.Id;
                    manager.Wants[want.Id] = want;
                }

                return;
            }

            // if not already in, just add it.
            manager.Wants[want.Id] = want;

            Close();
        }
    }
}
