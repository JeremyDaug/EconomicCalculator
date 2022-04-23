﻿using EconomicSim;
using EconomicSim.Objects;
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

namespace Editor.OpeningWindows
{
    /// <summary>
    /// Interaction logic for NavigationMenu.xaml
    /// </summary>
    public partial class NavigationMenu : Window
    {
        public NavigationMenu()
        {
            InitializeComponent();

            var context = DataContextFactory.GetDataContext;

            context.LoadData(new List<string> { "Common" });

            // if anything is loaded, don't reload
            if (!DTOManager.Instance.Products.Any())
                DTOManager.Instance.LoadAll();
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

        private void ToJobs(object sender, RoutedEventArgs e)
        {
            var win = new Jobs.JobsListWindow();

            win.Show();

            this.Close();
        }

        private void ToTechFamilies(object sender, RoutedEventArgs e)
        {
            var win = new EditorInterface.TechFamilies.TechFamiliesListWindow();

            win.Show();

            this.Close();
        }

        private void ToTechs(object sender, RoutedEventArgs e)
        {
            var win = new EditorInterface.Techs.TechListWindow();

            win.Show();

            this.Close();
        }

        private void ToSpecies(object sender, RoutedEventArgs e)
        {
            var win = new EditorInterface.Species.SpeciesListWindow();

            win.Show();

            this.Close();
        }

        private void ToCultures(object sender, RoutedEventArgs e)
        {
            var win = new EditorInterface.Cultures.CultureListView();

            win.Show();

            this.Close();
        }

        private void ToPops(object sender, RoutedEventArgs e)
        {
            var win = new EditorInterface.Populations.PopsListView();

            win.Show();

            this.Close();
        }

        private void ToTerritories(object sender, RoutedEventArgs e)
        {
            var win = new EditorInterface.SimpleTerritory.SimpleTerritoryList();

            win.Show();

            this.Close();
        }

        private void ToMarkets(object sender, RoutedEventArgs e)
        {
            var win = new EditorInterface.Markets.MarketsListView();

            win.Show();

            this.Close();
        }

        private void ToFirms(object sender, RoutedEventArgs e)
        {
            var win = new EditorInterface.Firms.FirmsListView();

            win.Show();

            this.Close();
        }
    }
}
