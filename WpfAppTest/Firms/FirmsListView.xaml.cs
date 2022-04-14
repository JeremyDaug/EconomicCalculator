using EconomicCalculator;
using EconomicCalculator.DTOs.Firms;
using EconomicCalculator.DTOs.Pops;
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

namespace EditorInterface.Firms
{
    /// <summary>
    /// Interaction logic for FirmsListView.xaml
    /// </summary>
    public partial class FirmsListView : Window
    {
        private DTOManager manager = DTOManager.Instance;

        public FirmsListView()
        {
            InitializeComponent();

            FirmGrid.ItemsSource = manager.Firms
                .Values.Where(x => x.ParentFirm == null);
        }

        private void NewFirm(object sender, RoutedEventArgs e)
        {
            var newFirm = new FirmDTO();

            newFirm.Id = manager.NewFirmId;

            var window = new FirmEditorView(newFirm);

            window.ShowDialog();

            FirmGrid.ItemsSource = manager.Firms.Values;
            FirmGrid.Items.Refresh();
        }

        private void EditFirm(object sender, RoutedEventArgs e)
        {
            var selected = (FirmDTO)FirmGrid.SelectedItem;

            if (selected == null)
                return;

            var window = new FirmEditorView(selected);

            window.ShowDialog();

            CleanupOrphanedPops();

            FirmGrid.ItemsSource = manager.Firms.Values;
            FirmGrid.Items.Refresh();
        }

        private void CopyFirm(object sender, RoutedEventArgs e)
        {
            var selected = (FirmDTO)FirmGrid.SelectedItem;

            if (selected == null)
                return;

            var dup = new FirmDTO
            {
                Id = manager.NewFirmId,
                Name = selected.Name,
                FirmRank = selected.FirmRank,
                Market = selected.Market,
                MarketId = selected.MarketId,
                OrganizationalStructure = selected.OrganizationalStructure,
                OwnershipStructure = selected.OwnershipStructure,
                ProfitStructure = selected.ProfitStructure,
                ParentFirm = selected.ParentFirm,
                Processes = selected.Processes,
                Children = selected.Children,
                JobData = selected.JobData,
                ProductPrices = selected.ProductPrices,
                Regions = selected.Regions,
                RegionIds = selected.RegionIds,
                Resources = selected.Resources
                // do separate employees
            };

            foreach (var pop in selected.Employees)
            {
                var oldPop = manager.Pops[pop];
                var popDup = new PopDTO
                {
                    Id = manager.NewPopId,
                    Count = oldPop.Count,
                    Firm = oldPop.Firm,
                    FirmId = dup.Id,
                    Job = oldPop.Job,
                    JobId = oldPop.JobId,
                    Market = oldPop.Market,
                    MarketId = oldPop.MarketId,
                    Skill = oldPop.Skill,
                    SkillId = oldPop.SkillId,
                    SkillLevel = oldPop.SkillLevel,
                    CulturePortions = oldPop.CulturePortions,
                    SpeciesPortions = oldPop.SpeciesPortions
                };
                manager.Pops.Add(popDup.Id, popDup);
                dup.Employees.Add(popDup.Id);
            }

            var window = new FirmEditorView(dup);

            window.ShowDialog();

            CleanupOrphanedPops();

            FirmGrid.ItemsSource = manager.Firms.Values;
            FirmGrid.Items.Refresh();
        }

        private void CleanupOrphanedPops()
        {
            // go through all pops
            var removeIds = new List<int>();
            foreach (var pop in manager.Pops.Values)
            {
                if (!manager.Firms.ContainsKey(pop.Id))
                    removeIds.Add(pop.Id);
            }

            foreach (var id in removeIds)
                manager.Pops.Remove(id);
        }

        private void SaveFirms(object sender, RoutedEventArgs e)
        {
            manager.SaveFirms(@"D:\Projects\EconomicCalculator\EconomicCalculator\Data\Firms.json");

            manager.SavePops(@"D:\Projects\EconomicCalculator\EconomicCalculator\Data\Pops.json");

            MessageBox.Show("Firms and new Pops Saved!", "Save Successful!");
        }
    }
}
