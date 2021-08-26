﻿using EconomicCalculator;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using EditorInterface.Products;
using EditorInterface.Wants;

namespace EditorInterface
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        Manager manager;

        private void BackToMenu(object sender, RoutedEventArgs e)
        {
            // reopen main window
            Window win = new OpeningWindows.NavigationMenu();
            

            // close all other windows and go back to menu.
            foreach (var window in Current.Windows.OfType<Window>().Where(x => x.IsActive))
            {
                window.Close();
            }

            win.Show();
        }

        private void NewProduct(object sender, RoutedEventArgs e)
        {
            Window win = new ProductWindow();
            win.ShowDialog();

            var Windows = (ProductListWindow)Current.Windows
                .OfType<Window>()
                .Where(x => x.IsActive)
                .SingleOrDefault(x => x.GetType() == typeof(ProductListWindow));

            if (Windows != null)
            {
                Windows.ProductGrid.Items.Refresh();
            }
        }

        private void NewWant(object sender, RoutedEventArgs e)
        {
            Window win = new ProductWindow();
            win.ShowDialog();
            var Windows = (WantsListWindow)Current.Windows
                .OfType<Window>()
                .Where(x => x.IsActive)
                .SingleOrDefault(x => x.GetType() == typeof(WantsListWindow));

            if (Windows != null)
            {
                Windows.WantGrid.Items.Refresh();
            }
        }
    }
}