using EconomicCalculator;
using EconomicCalculator.DTOs.Firms;
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

        public ICommand RemovePop
        {
            get
            {
                if (removePop == null)
                {
                    removePop = new RelayCommand(
                        param => RemoveNewPop());
                }
                return removePop;
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
            // Call Pop Editor INterface
        }

        private void RemoveNewPop()
        {
            // Check that pop should be removed.
        }

        private void Commit()
        {
            // Check data then commit
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
