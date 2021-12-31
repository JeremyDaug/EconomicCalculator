using EconDTOs;
using EconDTOs.DTOs.Processes;
using EconDTOs.DTOs.Processes.ProductionTags;
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

namespace EditorInterface.ProcessWindows
{
    public class ProcessProductViewModel : INotifyPropertyChanged
    {
        ProcessProductModel model;
        private ICommand commit;
        public ProcessProduct ProcessProduct;
        private bool _validCommit;

        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<string> AvailableProducts { get; set; }

        public ProcessProductViewModel(ProcessProduct proc, ProcessSection section)
        {
            model = new ProcessProductModel(proc);
            model.Section = section;
            // copy the processproduct so we don't override without commiting.

            ProcessProduct = new ProcessProduct();

            AvailableProducts = new ObservableCollection<string>(
                Manager.Instance.Products.Values.Select(x => x.GetName()));
            ValidCommited = false;
            RefreshEnableds();
        }

        #region Properties

        public bool ValidCommited
        {
            get
            {
                return _validCommit;
            }
            set
            {
                if (_validCommit != value)
                {
                    _validCommit = value;
                    RaisePropertyChanged();
                }
            }
        }

        public ProcessSection Section => model.Section;

        public string Product
        {
            get
            {
                return model.ProductName;
            }
            set
            {
                if (model.ProductName != value)
                {
                    model.ProductName = value;
                    RaisePropertyChanged();
                }
            }
        }

        public decimal Amount
        {
            get
            {
                return model.Amount;
            }
            set
            {
                if (Amount != value)
                {
                    model.Amount = value;
                    RaisePropertyChanged();
                    RaisePropertyChanged(nameof(MassCalc));
                }
            }
        }

        public string MassCalc
        {
            get
            {
                var result = "Total Mass: {0} kg";
                decimal weight = 0;
                if (!string.IsNullOrEmpty(Product))
                    weight = Manager.Instance.GetProductByFullName(Product).Mass * Amount;

                return string.Format(result, weight);
            }
        }

        public bool Fixed
        {
            get
            {
                return model.Fixed;
            }
            set
            {
                if (model.Fixed != value)
                {
                    model.Fixed = value;
                    RaisePropertyChanged();
                    RefreshEnableds();
                }
            }
        }

        public bool FixedEnabled
        {
            get
            {
                if (model.Optional ||
                    model.Consumed ||
                    model.DivisionInput ||
                    model.DivisionCapital ||
                    model.AutomationInput ||
                    model.AutomationCapital)
                {
                    return false;
                }
                return true;
            }
            set
            {
                RaisePropertyChanged();
            }
        }

        public bool Optional
        {
            get
            {
                return model.Optional;
            }
            set
            {
                if (model.Optional != value)
                {
                    model.Optional = value;
                    RaisePropertyChanged();
                    RefreshEnableds();
                }
            }
        }

        public bool OptionalEnabled
        {
            get
            {
                if (model.Section == ProcessSection.Output ||
                    model.Fixed ||
                    model.Pollutant ||
                    model.Chance ||
                    model.Offset ||
                    model.DivisionInput ||
                    model.DivisionCapital ||
                    model.AutomationInput ||
                    model.AutomationCapital)
                {
                    return false;
                }
                return true;
            }
            set
            {
                RaisePropertyChanged();
            }
        }

        public decimal OptionalBonus
        {
            get
            {
                return model.OptionalBonus;
            }
            set
            {
                model.OptionalBonus = value;
            }
        }

        public bool Investment
        {
            get
            {
                return model.Investment;
            }
            set
            {
                if (model.Investment != value)
                {
                    model.Investment = value;
                    RaisePropertyChanged();
                    RefreshEnableds();
                }
            }
        }

        public bool InvestmentEnabled
        {
            get
            {
                if (model.Section == ProcessSection.Output)
                {
                    return false;
                }
                return true;
            }
        }

        public bool Consumed
        {
            get
            {
                return model.Consumed;
            }
            set
            {
                if (model.Consumed != value)
                {
                    model.Consumed = value;
                    RaisePropertyChanged();
                    RefreshEnableds();
                }
            }
        }

        public bool ConsumedEnabled
        {
            get
            {
                if (model.Section != ProcessSection.Input ||
                    model.Fixed ||
                    model.Pollutant ||
                    model.Chance ||
                    model.Offset ||
                    model.DivisionInput ||
                    model.DivisionCapital ||
                    model.AutomationInput ||
                    model.AutomationCapital)
                {
                    return false;
                }
                return true;
            }
        }

        public bool Pollutant
        {
            get
            {
                return model.Pollutant;
            }
            set
            {
                if (model.Pollutant != value)
                {
                    model.Pollutant = value;
                    RaisePropertyChanged();
                    RefreshEnableds();
                }
            }
        }

        public bool PollutantEnabled
        {
            get
            {
                if (model.Section != ProcessSection.Output ||
                    model.Offset)
                {
                    return false;
                }
                return true;
            }
        }

        public bool Offset
        {
            get
            {
                return model.Offset;
            }
            set
            {
                if (model.Offset != value)
                {
                    model.Offset = value;
                    RaisePropertyChanged();
                    RefreshEnableds();
                }
            }
        }

        public bool OffsetEnabled
        {
            get
            {
                if (model.Section != ProcessSection.Output ||
                    model.Pollutant)
                {
                    return false;
                }
                return true;
            }
        }

        public bool DivisionInput
        {
            get
            {
                return model.DivisionInput;
            }
            set
            {
                if (model.DivisionInput != value)
                {
                    model.DivisionInput = value;
                    RaisePropertyChanged();
                    RefreshEnableds();
                }
            }
        }

        public bool DivisionInputEnabled
        {
            get
            {
                if (model.Section != ProcessSection.Input)
                {
                    return false;
                }
                return true;
            }
        }

        public bool DivisionCapital
        {
            get
            {
                return model.DivisionCapital;
            }
            set
            {
                if (model.DivisionCapital != value)
                {
                    model.DivisionCapital = value;
                    RaisePropertyChanged();
                    RefreshEnableds();
                }
            }
        }

        public bool DivisionCapitalEnabled
        {
            get
            {
                if (model.Section != ProcessSection.Capital)
                {
                    return false;
                }
                return true;
            }
        }

        public bool AutomationInput
        {
            get
            {
                return model.AutomationInput;
            }
            set
            {
                if (model.AutomationInput != value)
                {
                    model.AutomationInput = value;
                    RaisePropertyChanged();
                    RefreshEnableds();
                }
            }
        }

        public bool AutomationInputEnabled
        {
            get
            {
                if (model.Section != ProcessSection.Input)
                {
                    return false;
                }
                return true;
            }
        }

        public bool AutomationCapital
        {
            get
            {
                return model.AutomationCapital;
            }
            set
            {
                if (model.AutomationCapital != value)
                {
                    model.AutomationCapital = value;
                    RaisePropertyChanged();
                    RefreshEnableds();
                }
            }
        }

        public bool AutomationCapitalEnabled
        {
            get
            {
                if (model.Section != ProcessSection.Capital)
                {
                    return false;
                }
                return true;
            }
        }

        public bool Chance
        {
            get
            {
                return model.Chance;
            }
            set
            {
                if (model.Chance != value)
                {
                    model.Chance = value;
                    RaisePropertyChanged();
                    RefreshEnableds();
                }
            }
        }

        public bool ChanceEnabled
        {
            get
            {
                if (model.Section != ProcessSection.Output)
                {
                    return false;
                }
                return true;
            }
        }

        public string ChanceGroup
        {
            get
            {
                return model.ChanceGroup.ToString();
            }
            set
            {
                if (value.Length == 1)
                {
                    model.ChanceGroup = value[0];
                    RaisePropertyChanged();
                }
            }
        }

        public int ChanceWeight
        {
            get
            {
                return model.ChanceWeight;
            }
            set
            {
                if (model.ChanceWeight != value)
                {
                    model.ChanceWeight = value;
                    RaisePropertyChanged();
                }
            }
        }

        public bool ChanceOptionsEnabled
        {
            get
            {
                if (Chance)
                {
                    return true;
                }
                return false;
            }
        }

        public ICommand Commit
        {
            get
            {
                if (commit == null)
                {
                    commit = new RelayCommand(
                        param => CommitProduct());
                }
                return commit;
            }
        }

        #endregion Properties

        private void CommitProduct()
        {
            ValidCommited = false;
            
            if (string.IsNullOrEmpty(model.ProductName))
            {
                MessageBox.Show("Product Not selected.");
                return;
            }
            ProcessProduct.ProductName = model.ProductName;

            if (model.Offset && model.Amount >= 0)
            {
                MessageBox.Show("This is an Offset, Amount value must be Negative.");
                return;
            }
            else if (!model.Offset && model.Amount <= 0)
            {
                MessageBox.Show("Amount must be Positive");
                return;
            }
            else if (Section == ProcessSection.Output &&
                !Manager.Instance.GetProductByFullName(Product).Fractional &&
                Amount % 1 > 0)
            {
                MessageBox.Show("Output products of non-fractional goods must be whole numbers.");
                return;
            }
            ProcessProduct.Amount = model.Amount;

            if (model.Consumed)
            {
                var tag = new AttachedProductionTag
                {
                    Tag = ProductionTag.Consumed
                };

                ProcessProduct.AddTag(tag);
            }
            if (model.Optional)
            {
                var tag = new AttachedProductionTag
                {
                    Tag = ProductionTag.Optional
                };

                if (OptionalBonus <= 0)
                {
                    MessageBox.Show("Throughput bonus from optional must be greater than 0.");
                    return;
                }

                tag.Add(model.OptionalBonus);
                ProcessProduct.AddTag(tag);
            }
            if (model.Fixed)
            {
                var tag = new AttachedProductionTag
                {
                    Tag = ProductionTag.Fixed
                };

                ProcessProduct.AddTag(tag);
            }
            if (model.Investment)
            {
                var tag = new AttachedProductionTag
                {
                    Tag = ProductionTag.Investment
                };

                ProcessProduct.AddTag(tag);
            }
            if (model.Pollutant)
            {
                var tag = new AttachedProductionTag
                {
                    Tag = ProductionTag.Pollutant
                };

                ProcessProduct.AddTag(tag);
            }
            if (model.Offset)
            {
                var tag = new AttachedProductionTag
                {
                    Tag = ProductionTag.Offset
                };

                if (ProcessProduct.Amount >= 0)
                {
                    MessageBox.Show("This product is an offset and it's Amount must be less than 0.");
                    return;
                }

                ProcessProduct.AddTag(tag);
            }
            if (model.Chance)
            {
                var tag = new AttachedProductionTag
                {
                    Tag = ProductionTag.Chance
                };

                if (!char.IsLetter(model.ChanceGroup))
                {
                    MessageBox.Show("Chance Group must be a letter.");
                    return;
                }
                if (model.ChanceWeight <= 0)
                {
                    MessageBox.Show("Chance weight must be an integer greater than 0.");
                    return;
                }

                tag.Add(model.ChanceGroup);
                tag.Add(model.ChanceWeight);

                ProcessProduct.AddTag(tag);
            }
            
            // TODO Division Capital and Automation Capital

            ValidCommited = true;
        }

        private void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            if (nameof(ValidCommited) != propertyName)
                ValidCommited = false;

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void RefreshEnableds()
        {
            RaisePropertyChanged(nameof(AutomationCapitalEnabled));
            RaisePropertyChanged(nameof(AutomationInputEnabled));
            RaisePropertyChanged(nameof(DivisionCapitalEnabled));
            RaisePropertyChanged(nameof(DivisionInputEnabled));
            RaisePropertyChanged(nameof(FixedEnabled));
            RaisePropertyChanged(nameof(InvestmentEnabled));
            RaisePropertyChanged(nameof(OptionalEnabled));
            RaisePropertyChanged(nameof(ConsumedEnabled));
            RaisePropertyChanged(nameof(PollutantEnabled));
            RaisePropertyChanged(nameof(OffsetEnabled));
            RaisePropertyChanged(nameof(ChanceEnabled));
            RaisePropertyChanged(nameof(ChanceOptionsEnabled));
        }
    }
}
