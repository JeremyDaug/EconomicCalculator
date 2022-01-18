using EconomicCalculator.DTOs.Processes;
using EconomicCalculator.DTOs.Processes.ProductionTags;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Editor.ProcessWindows
{
    public class ProcessWantModel : INotifyPropertyChanged
    {
        public ProcessWantModel(ProcessWantDTO want)
        {
            WantName = want.WantName;
            Amount = want.Amount;
            Optional = want.Tags.Any(x => x.Tag == ProductionTag.Optional);
            if (Optional)
                OptionalBonus = (decimal)want.Tags.Single(x => x.Tag == ProductionTag.Optional)[0];
            Consumed = want.Tags.Any(x => x.Tag == ProductionTag.Consumed);
            Fixed = want.Tags.Any(x => x.Tag == ProductionTag.Fixed);
            Investment = want.Tags.Any(x => x.Tag == ProductionTag.Investment);
            Pollutant = want.Tags.Any(x => x.Tag == ProductionTag.Pollutant);
            Chance = want.Tags.Any(x => x.Tag == ProductionTag.Chance);
            if (Chance)
            {
                ChanceGroup = (char)want.Tags.Single(x => x.Tag == ProductionTag.Chance)[0];
                ChanceWeight = (int)want.Tags.Single(x => x.Tag == ProductionTag.Chance)[1];
            }
            else
            {
                ChanceGroup = 'a';
                ChanceWeight = 1;
            }
            Offset = want.Tags.Any(x => x.Tag == ProductionTag.Offset);
            DivisionCapital = want.Tags.Any(x => x.Tag == ProductionTag.DivisionCapital);
            DivisionInput = want.Tags.Any(x => x.Tag == ProductionTag.DivisionInput);
            AutomationCapital = want.Tags.Any(x => x.Tag == ProductionTag.AutomationCapital);
            AutomationInput = want.Tags.Any(x => x.Tag == ProductionTag.AutomationInput);
        }

        private string _productName;
        private decimal _amount;
        private bool _optional;
        private decimal _optionalBonus;
        private bool _consumed;
        private bool _fixed;
        private bool _investment;
        private bool _pollutant;
        private bool _chance;
        private char _chanceGroup;
        private int _chanceWeight;
        private bool _offset;
        private bool _divisionCapital;
        private bool _divisionInput;
        private bool _automationCapital;
        private bool _automationInput;
        private ProcessSection _section;

        public ProcessSection Section
        {
            get
            {
                return _section;
            }
            set
            {
                if (_section != value)
                {
                    _section = value;
                    RaisePropertyChanged();
                }
            }
        }

        public string WantName
        {
            get
            {
                return _productName;
            }
            set
            {
                if (_productName != value)
                {
                    _productName = value;
                    RaisePropertyChanged();
                }
            }
        }

        public decimal Amount
        {
            get
            {
                return _amount;
            }
            set
            {
                if (_amount != value)
                {
                    _amount = value;
                    RaisePropertyChanged();
                }
            }
        }

        public bool Optional
        {
            get
            {
                return _optional;
            }
            set
            {
                if (_optional != value)
                {
                    _optional = value;
                    RaisePropertyChanged();
                }
            }
        }

        public decimal OptionalBonus
        {
            get
            {
                return _optionalBonus;
            }
            set
            {
                if (_optionalBonus != value)
                {
                    _optionalBonus = value;
                    RaisePropertyChanged();
                }
            }
        }

        public bool Consumed
        {
            get
            {
                return _consumed;
            }
            set
            {
                if (_consumed != value)
                {
                    _consumed = value;
                    RaisePropertyChanged();
                }
            }
        }

        public bool Fixed
        {
            get
            {
                return _fixed;
            }
            set
            {
                if (_fixed != value)
                {
                    _fixed = value;
                    RaisePropertyChanged();
                }
            }
        }

        public bool Investment
        {
            get
            {
                return _investment;
            }
            set
            {
                if (_investment != value)
                {
                    _investment = value;
                    RaisePropertyChanged();
                }
            }
        }

        public bool Pollutant
        {
            get
            {
                return _pollutant;
            }
            set
            {
                if (_pollutant != value)
                {
                    _pollutant = value;
                    RaisePropertyChanged();
                }
            }
        }

        public bool Chance
        {
            get
            {
                return _chance;
            }
            set
            {
                if (_chance != value)
                {
                    _chance = value;
                    RaisePropertyChanged();
                }
            }
        }

        public char ChanceGroup
        {
            get
            {
                return _chanceGroup;
            }
            set
            {
                if (_chanceGroup != value)
                {
                    _chanceGroup = value;
                    RaisePropertyChanged();
                }
            }
        }

        public int ChanceWeight
        {
            get
            {
                return _chanceWeight;
            }
            set
            {
                if (_chanceWeight != value)
                {
                    _chanceWeight = value;
                    RaisePropertyChanged();
                }
            }
        }

        public bool Offset
        {
            get
            {
                return _offset;
            }
            set
            {
                if (_offset != value)
                {
                    _offset = value;
                    RaisePropertyChanged();
                }
            }
        }

        public bool DivisionCapital
        {
            get
            {
                return _divisionCapital;
            }
            set
            {
                if (_divisionCapital != value)
                {
                    _divisionCapital = value;
                    RaisePropertyChanged();
                }
            }
        }

        public bool DivisionInput
        {
            get
            {
                return _divisionInput;
            }
            set
            {
                if (_divisionInput != value)
                {
                    _divisionInput = value;
                    RaisePropertyChanged();
                }
            }
        }

        public bool AutomationCapital
        {
            get
            {
                return _automationCapital;
            }
            set
            {
                if (_automationCapital != value)
                {
                    _automationCapital = value;
                    RaisePropertyChanged();
                }
            }
        }

        public bool AutomationInput
        {
            get
            {
                return _automationInput;
            }
            set
            {
                if (_automationInput != value)
                {
                    _automationInput = value;
                    RaisePropertyChanged();
                }
            }
        }

        private void RaisePropertyChanged([CallerMemberName] string v = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(v));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
