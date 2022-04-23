using EconomicSim;
using EconomicSim.DTOs.Firms;
using EconomicSim.DTOs.Pops;
using EconomicSim.Objects.Firms;
using EditorInterface.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace EditorInterface.Firms
{
    public class FirmEditorModel : INotifyPropertyChanged
    {
        private DTOManager manager = DTOManager.Instance;
        private IFirmDTO original;

        private string name;
        private string parentFirm;
        private FirmRank firmRank;
        private OwnershipStructure ownershipStructure;
        private ProfitStructure profitStructure;
        private OrganizationalStructure organization;
        private string market;

        public FirmEditorModel(IFirmDTO original)
        {
            this.original = original;

            Name = original.Name;
            ParentFirm = original.ParentFirm;
            ownershipStructure = original.OwnershipStructureEnum;
            profitStructure = original.ProfitStructureEnum;
            organization = original.OrganizationalStructureEnum;
            market = original.Market;

            ChildFirms = new ObservableCollection<DummyWrapper<string>>();
            foreach (var item in original.Children)
                ChildFirms.Add(new DummyWrapper<string> { Value = item });

            Processes = new ObservableCollection<DummyWrapper<string>>();
            foreach (var item in original.Processes)
                Processes.Add(new DummyWrapper<string> { Value=item });

            Regions = new ObservableCollection<DummyWrapper<string>>();
            foreach (var item in original.Regions)
                Regions.Add(new DummyWrapper<string> { Value = item });

            JobData = new ObservableCollection<JobWageDataStorage>();
            foreach (var job in original.JobData)
                JobData.Add(new JobWageDataStorage {
                    JobName = job.JobName,
                    Wage = job.Wage,
                    WageType = job.WageType,
                });

            Employees = new ObservableCollection<PopDTO>();
            foreach (var pop in original.Employees)
                Employees.Add((PopDTO)manager.Pops[pop]);

            ProductPrices = new ObservableCollection<ProductDecimalPair>();
            foreach (var product in original.ProductPrices)
                ProductPrices.Add(new ProductDecimalPair { Product = product.Key, Price = product.Value });

            Resources = new ObservableCollection<ProductDecimalPair>();
            foreach (var resource in original.Resources)
                Resources.Add(new ProductDecimalPair { Product = resource.Key, Price = resource.Value });
        }

        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                if (name != value)
                {
                    name = value;
                    RaisePropertyChanged();
                }
            }
        }

        public string ParentFirm
        {
            get { return parentFirm; }
            set
            {
                if (parentFirm != value)
                {
                    parentFirm = value;
                    RaisePropertyChanged();
                }
            }
        }

        public string FirmRank
        {
            get { return firmRank.ToString(); }
            set
            {
                if (firmRank.ToString() != value)
                {
                    firmRank = (FirmRank)Enum.Parse(typeof(FirmRank), value);
                    RaisePropertyChanged();
                }
            }
        }

        public string OwnershipStructure
        {
            get { return ownershipStructure.ToString(); }
            set
            {
                if (ownershipStructure.ToString() != value)
                {
                    ownershipStructure = (OwnershipStructure)Enum.Parse(typeof(OwnershipStructure), value);
                    RaisePropertyChanged();
                }
            }
        }

        public string ProfitStructure
        {
            get { return profitStructure.ToString(); }
            set
            {
                if (profitStructure.ToString() != value)
                {
                    profitStructure = (ProfitStructure)Enum.Parse(typeof(ProfitStructure), value);
                    RaisePropertyChanged();
                }
            }
        }

        public string OrganizationalStructure
        {
            get { return organization.ToString(); }
            set
            {
                if (organization.ToString() != value)
                {
                    organization = (OrganizationalStructure)Enum.Parse(typeof(OrganizationalStructure), value);
                    RaisePropertyChanged();
                }
            }
        }

        public string Market
        {
            get { return market; }
            set
            {
                if (market != value)
                {
                    market = value;
                    RaisePropertyChanged();
                }
            }
        }

        #region Collections

        public ObservableCollection<DummyWrapper<string>> ChildFirms { get; }

        public ObservableCollection<JobWageDataStorage> JobData { get; }

        public ObservableCollection<DummyWrapper<string>> Processes { get; }

        public ObservableCollection<PopDTO> Employees { get; }

        public ObservableCollection<DummyWrapper<string>> Regions { get; }

        public ObservableCollection<ProductDecimalPair> ProductPrices { get; }

        public ObservableCollection<ProductDecimalPair> Resources { get; }

        #endregion Collections

        private void RaisePropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
