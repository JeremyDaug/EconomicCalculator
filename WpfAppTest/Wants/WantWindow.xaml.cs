﻿using EconomicCalculator;
using EconomicCalculator.Storage.Wants;
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

namespace WpfAppTest.Wants
{
    /// <summary>
    /// Interaction logic for WantWindow.xaml
    /// </summary>
    public partial class WantWindow : Window
    {
        private Manager manager;
        private Want want;

        public WantWindow()
        {
            manager = Manager.Instance;

            want = new Want
            {
                Id = manager.NewWantId
            };

            InitializeComponent();

            WantName.Text = want.Name;
            WantId.Text = want.Id.ToString();
        }

        public WantWindow(Want want)
        {
            this.want = want;
            manager = Manager.Instance;
            InitializeComponent();

            WantName.Text = want.Name;
            WantId.Text = want.Id.ToString();
        }

        private void CloseBind_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to close?", "Close", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                this.Close();
        }

        private void CommitWant(object sender, RoutedEventArgs e)
        {
            want = new Want
            {
                Id = int.Parse(WantId.Text),
                Name = WantName.Text
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

                    WantId.Text = want.Id.ToString();
                }

                return;
            }

            // if not already in, just add it.
            manager.Wants[want.Id] = want;
        }

        private void CommitAndQuit(object sender, RoutedEventArgs e)
        {
            want = new Want
            {
                Id = int.Parse(WantId.Text),
                Name = WantName.Text
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

                    WantId.Text = want.Id.ToString();
                }

                return;
            }

            // if not already in, just add it.
            manager.Wants[want.Id] = want;

            Close();
        }
    }
}
