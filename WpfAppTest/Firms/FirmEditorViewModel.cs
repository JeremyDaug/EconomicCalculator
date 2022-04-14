using EconomicCalculator;
using EconomicCalculator.DTOs.Firms;
using EconomicCalculator.DTOs.Market;
using EconomicCalculator.DTOs.Pops;
using EconomicCalculator.Objects.Firms;
using Editor.Helpers;
using EditorInterface.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace EditorInterface.Firms
{
    public class FirmEditorViewModel : INotifyPropertyChanged
    {
        private DTOManager manager = DTOManager.Instance;
        private IFirmDTO original;

        private FirmEditorModel model;

        private ICommand addPop;
        private ICommand removePop;
        private ICommand commitFirm;

        public FirmEditorViewModel(IFirmDTO original)
        {
            if (original == null)
                throw new ArgumentNullException(nameof(original));

            this.original = original;

            model = new FirmEditorModel(original);

            FirmRanks = new ObservableCollection<string>(
                Enum.GetNames(typeof(FirmRank)));

            ParentOptions = new ObservableCollection<string>(
                manager.Firms.Values
                .Where(x => x.FirmRankEnum >= original.FirmRankEnum)
                .Select(x => x.Name)
                .Append(""));

            PossibleChildren = new ObservableCollection<string>(
                manager.Firms.Values
                .Where(x => x.FirmRankEnum <= original.FirmRankEnum)
                .Select(x => x.Name));

            MarketOptions = new ObservableCollection<string>(
                manager.Markets.Values
                .Select(x => x.Name));

            OwnershipOptions = new ObservableCollection<string>(
                Enum.GetNames(typeof(OwnershipStructure)));

            ProfitOptions = new ObservableCollection<string>(
                Enum.GetNames(typeof(ProfitStructure)));

            OrganizationOptions = new ObservableCollection<string>(
                Enum.GetNames(typeof(OrganizationalStructure)));

            ProductOptions = new ObservableCollection<string>(
                manager.Products.Values
                .Select(x => x.Name));

            JobOptions = new ObservableCollection<string>(
                manager.Jobs.Values
                .Select(x => x.FullName()));

            WageOptions = new ObservableCollection<string>(
                Enum.GetNames(typeof(WageType)));

            ProcessOptions = new ObservableCollection<string>(
                manager.Processes.Values
                .Select(x => x.GetName()));
        }

        public string Name
        {
            get
            {
                return model.Name;
            }
            set
            {
                if (model.Name != value)
                {
                    model.Name = value;
                    RaisePropertyChanged();
                }
            }
        }

        public string ParentFirm
        {
            get { return model.ParentFirm; }
            set
            {
                if (model.ParentFirm != value)
                {
                    model.ParentFirm = value;
                    RaisePropertyChanged();
                }
            }
        }

        public string FirmRank
        {
            get { return model.FirmRank; }
            set
            {
                if (model.FirmRank != value)
                {
                    model.FirmRank = value;
                    RaisePropertyChanged();
                    ReloadParents();
                }
            }
        }

        public string OwnershipStructure
        {
            get { return model.OwnershipStructure; }
            set
            {
                if (model.OwnershipStructure != value)
                {
                    model.OwnershipStructure = value;
                    RaisePropertyChanged();
                }
            }
        }

        public string ProfitStructure
        {
            get { return model.ProfitStructure; }
            set
            {
                if (model.ProfitStructure != value)
                {
                    model.ProfitStructure = value;
                    RaisePropertyChanged();
                }
            }
        }

        public string OrganizationalStructure
        {
            get { return model.OrganizationalStructure; }
            set
            {
                if (model.OrganizationalStructure != value)
                {
                    model.OrganizationalStructure = value;
                    RaisePropertyChanged();
                }
            }
        }

        public string Market
        {
            get { return model.Market; }
            set
            {
                if (model.Market != value)
                {
                    model.Market = value;
                    RaisePropertyChanged();
                }
            }
        }

        #region Commands

        public ICommand AddPop
        {
            get
            {
                if (addPop == null)
                {
                    addPop = new RelayCommand(
                        param => AddNewPop());
                }
                return addPop;
            }
        }

        public ICommand CommitFirm
        {
            get
            {
                if (commitFirm == null)
                {
                    commitFirm = new RelayCommand(
                        param => Commit());
                }
                return commitFirm;
            }
        }

        private void AddNewPop()
        {
            if (JobData.Count == 0)
                return;

            var selWin = new SelectionView(JobData
                .Select(x => x.JobName).ToList());
            selWin.ShowDialog();

            if (selWin.Selection == null)
                return;

            // using selected job
            // check that such a pop doesn't already exist with us
            PopDTO newPop;
            if (Employees.Any(x => x.Job == selWin.Selection))
            { 
                newPop = Employees.Single(x => x.Job == selWin.Selection);
            }
            else 
                newPop = new PopDTO
                {
                    Id = manager.NewPopId,
                    Count = 0,
                    Firm = Name,
                    Market = Market,
                    Job = selWin.Selection,
                    Skill = manager.GetJobByName(selWin.Selection).Skill,
                    SkillLevel = 0
                };

            // Call Pop Editor Interface
            var win = new Populations.PopEditorView(newPop);

            win.ShowDialog();

            // Add new pop in
            var pops = manager.Pops.Values
                .Where(x => x.Firm == Name);

            foreach (var pop in pops)
            { // go through pops attached to this firm and if any new
                // ons found, add it. Should be just one.
                if (!Employees.Any(x => x.Id == pop.Id))
                    Employees.Add((PopDTO)pop);
            }
        }

        public void RemovePop(PopDTO toRemove)
        {
            Employees.Remove(toRemove);
        }

        private void Commit()
        {
            var errors = new List<string>();
            var rankEnum = (FirmRank)Enum.Parse(typeof(FirmRank), FirmRank);

            // check name
            if (string.IsNullOrWhiteSpace(Name))
            {
                errors.Add("Invalid Firm name.");
            }
            // check that parent firm is of higher rank.
            if (!string.IsNullOrEmpty(ParentFirm))
            {
                var parent = manager.Firms.Values
                    .Single(x => x.Name == ParentFirm);
                if (parent.FirmRankEnum <= rankEnum)
                    errors.Add("Parent Firm must be of a higher Rank.");
            }
            // check subdivisions are of a lower rank than this firm.
            if (ChildFirms.Count > 0)
            {
                foreach (var childName in ChildFirms.Select(x => x.Value))
                {
                    var child = manager.Firms.Values.Single(x => x.Name == childName);

                    if (child.FirmRankEnum >= rankEnum)
                        errors.Add(string.Format("Child {0} must be of a lower rank.", childName));
                }
            }
            // must have main market
            if (string.IsNullOrEmpty(Market))
                errors.Add("Firm must have a main market.");
            // if duplicate market(s) skip them, don't mark an error.
            // TODO check that a region it says it operates in has a valid
            // command structure to work from in that market.
            // Job wage should be Non-negative
            foreach (var job in JobData)
            {
                if (job.Wage < 0)
                    errors.Add(string
                        .Format("{0} must have a non-negative wage.", job.JobName));
            }
            // No checks on Processes or Pops for now.
            // Product Prices should be positive
            foreach (var price in ProductPrices)
            {
                if (price.Price < 0)
                    errors.Add(string
                        .Format("{0} must have a non-negative price.", price.Product));
            }
            // Resource amounts should be positive.
            foreach (var resource in Resources)
            {
                if (resource.Price < 0)
                    errors.Add(string
                        .Format("{0} must have a non-negative amount.", resource.Price));
            }    

            // if any errors, return out, and show errors
            if (errors.Count > 0)
            {
                var AllErrors = "Errors found: \n";
                foreach (var error in errors)
                    AllErrors += "\t" + error + "\n";
                MessageBox.Show(AllErrors, "Errors Found!",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // no errors found, let's-a-go.
            // clear out old market connections
            foreach (var oldmark in manager.Markets.Values)
            {
                if (oldmark.FirmIds.Contains(original.Id))
                {
                    oldmark.FirmIds.Remove(original.Id);
                    oldmark.Firms.Remove(original.Name);
                }
            }
            // clear out orphaned populations
            foreach (var oldPop in original.Employees)
            {
                if (!Employees.Any(x => x.Id == oldPop))
                {
                    manager.Pops.Remove(oldPop);
                }
            }

            var newFirm = new FirmDTO
            {
                Id = original.Id,
                Name = Name,
                FirmRank = FirmRank,
                ParentFirm = ParentFirm,
                Children = ChildFirms.Select(x => x.Value).ToList(),
                MarketId = manager.Markets.Values.Single(x => x.Name == Market).Id,
                Market = Market,
                Regions = Regions.Select(x => x.Value).ToList(),
                OwnershipStructure = OwnershipStructure,
                ProfitStructure = ProfitStructure,
                OrganizationalStructure = OrganizationalStructure,
                // Job data elsewhere
                Processes = Processes.Select(x => x.Value).ToList(),
                Employees = Employees.Select(x => x.Id).ToList(),
                ProductPrices = ProductPrices.ToDictionary(key => key.Product, val => val.Price),
                Resources = Resources.ToDictionary(key => key.Product, val => val.Price),
            };
            foreach (var job in JobData)
                newFirm.JobData.Add(new JobWageData
                {
                    JobId = manager.GetJobByName(job.JobName).Id,
                    JobName = job.JobName,
                    Wage = job.Wage,
                    WageType = job.WageType
                });
            // region Ids
            foreach (var region in Regions.Select(x => x.Value))
                newFirm.RegionIds.Add(manager.Markets.Values.Single(x => x.Name == region).Id);

            // correct/ add firm and market to pops.
            foreach (var pop in newFirm.Employees)
            {
                var update = (PopDTO)manager.Pops[pop];

                update.Firm = newFirm.Name;
                update.FirmId = newFirm.Id;

                // TODO update to allow for placement in alternative markets.
                update.Market = newFirm.Market;
                update.MarketId = newFirm.MarketId;
            }

            // add firm to markets
            var market = (MarketDTO)manager.Markets.Values.Single(x => x.Name == Market);
            if (!market.FirmIds.Contains(newFirm.Id))
            {
                market.Firms.Add(Name);
                market.FirmIds.Add(newFirm.Id);
            }
            // add to regions to market which doesn't have it.
            foreach (var region in newFirm.RegionIds)
            {
                var reg = (MarketDTO)manager.Markets[region];
                if (!reg.FirmIds.Contains(newFirm.Id))
                { // if not the main market, add
                    reg.Firms.Add(Name);
                    reg.FirmIds.Add(newFirm.Id);
                }
            }

            // finally, add firm to manager
            manager.Firms[newFirm.Id] = newFirm;

            MessageBox.Show("Firm Commited!", "Successful Commit");
        }

        #endregion Commands

        #region Collections

        public ObservableCollection<string> FirmRanks { get; }

        public ObservableCollection<string> ParentOptions { get; }

        public ObservableCollection<string> MarketOptions { get; }

        public ObservableCollection<string> OwnershipOptions { get; }

        public ObservableCollection<string> ProfitOptions { get; }

        public ObservableCollection<string> ProcessOptions { get; }

        public ObservableCollection<string> OrganizationOptions { get; }

        public ObservableCollection<DummyWrapper<string>> ChildFirms => model.ChildFirms;

        public ObservableCollection<string> PossibleChildren { get; }

        public ObservableCollection<JobWageDataStorage> JobData => model.JobData;

        public ObservableCollection<DummyWrapper<string>> Processes => model.Processes;

        public ObservableCollection<PopDTO> Employees => model.Employees;

        public ObservableCollection<DummyWrapper<string>> Regions => model.Regions;

        public ObservableCollection<ProductDecimalPair> ProductPrices => model.ProductPrices;

        public ObservableCollection<string> ProductOptions { get; }

        public ObservableCollection<string> WageOptions { get; }

        public ObservableCollection<string> JobOptions { get; }

        public ObservableCollection<ProductDecimalPair> Resources => model.Resources;

        #endregion Collections

        private void ReloadParents()
        {
            ParentOptions.Clear();
            var currRank = (FirmRank)Enum.Parse(typeof(FirmRank), FirmRank);
            foreach (var item in manager.Firms.Values
                .Where(x => x.FirmRankEnum >= currRank)
                .Select(x => x.Name))
                ParentOptions.Add(item);

            RaisePropertyChanged(nameof(ParentOptions));
        }

        private void RaisePropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
