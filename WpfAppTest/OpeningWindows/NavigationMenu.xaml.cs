﻿using EconomicCalculator;
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

namespace EditorInterface.OpeningWindows
{
    /// <summary>
    /// Interaction logic for NavigationMenu.xaml
    /// </summary>
    public partial class NavigationMenu : Window
    {
        public NavigationMenu()
        {
            InitializeComponent();

            Manager.Instance.LoadAll();
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

        private void ToSkills(object sender, RoutedEventArgs e)
        {
            var win = new Skills.SkillsListWindow();

            win.Show();

            this.Close();
        }

        private void ToProcesses(object sender, RoutedEventArgs e)
        {
            var win = new ProcessWindows.ProcessListWindow();

            win.Show();

            this.Close();
        }
    }
}
