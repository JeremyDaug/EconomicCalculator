using EconomicCalculator.DTOs.Processes;
using EconomicCalculator.DTOs.Processes.ProcessTags;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace EditorInterface.ProcessWindows
{
    public class ProcessModel : INotifyPropertyChanged
    {
        private Process process;

        private int id;
        private string name;
        private string variantName;
        private decimal minTime;
        private string skill;
        private decimal minSkill;
        private decimal maxSkill;
        private string description;
        private bool failure;
        private bool consumption;
        private bool maintenance;
        private bool use;
        private bool chance;
        private bool crop;
        private bool mine;
        private bool extractor;
        private bool tap;
        private bool refiner;
        private bool sorter;
        private string selectedProduct;
        private string selectedImage;

        public ProcessModel(Process process)
        {
            ProcessId = process.Id;
            Name = process.Name;
            VariantName = process.VariantName;
            MinTime = process.MinimumTime;
            Skill = process.SkillName;
            MinSkill = process.SkillMinimum;
            MaxSkill = process.SkillMaximum;
            Description = process.Description;
            selectedImage = process.Icon;

            InputProducts = new ObservableCollection<IProcessProduct>(process.InputProducts);
            InputWants = new ObservableCollection<IProcessWant>(process.InputWants);
            CapitalProducts = new ObservableCollection<IProcessProduct>(process.CapitalProducts);
            CapitalWants = new ObservableCollection<IProcessWant>(process.CapitalWants);
            OutputProducts = new ObservableCollection<IProcessProduct>(process.OutputProducts);
            OutputWants = new ObservableCollection<IProcessWant>(process.OutputWants);

            Failure = process.Tags
                .Any(x => x.Tag == ProcessTag.Failure);
            Consumption = process.Tags
                .Any(x => x.Tag == ProcessTag.Consumption);
            Maintenance = process.Tags
                .Any(x => x.Tag == ProcessTag.Maintenance);
            Use = process.Tags
                .Any(x => x.Tag == ProcessTag.Use);
            Chance = process.Tags
                .Any(x => x.Tag == ProcessTag.Chance);
            Mine = process.Tags
                .Any(x => x.Tag == ProcessTag.Mine);
            Crop = process.Tags
                .Any(x => x.Tag == ProcessTag.Crop);

            if (Failure ||
                Consumption ||
                Maintenance)
            {
                SelectedProduct = InputProducts.First().ProductName;
            }

            if (Use)
            {
                SelectedProduct = CapitalProducts.First().ProductName;
            }
        }

        public int ProcessId
        {
            get
            {
                return id;
            }
            set
            {
                if (id != value)
                {
                    id = value;
                    RaisePropertyChanged();
                }
            }
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

        public string VariantName
        {
            get
            {
                return variantName;
            }
            set
            {
                if (variantName != value)
                {
                    variantName = value;
                    RaisePropertyChanged();
                }
            }
        }

        public decimal MinTime
        {
            get
            {
                return minTime;
            }
            set
            {
                if (minTime != value)
                {
                    minTime = value;
                    RaisePropertyChanged();
                }
            }
        }

        public string Skill
        {
            get
            {
                return skill;
            }
            set
            {
                if (skill != value)
                {
                    skill = value;
                    RaisePropertyChanged();
                }
            }
        }

        public decimal MinSkill
        {
            get
            {
                return minSkill;
            }
            set
            {
                if (minSkill != value)
                {
                    minSkill = value;
                    RaisePropertyChanged();
                }
            }
        }

        public decimal MaxSkill
        {
            get
            {
                return maxSkill;
            }
            set
            {
                if (maxSkill != value)
                {
                    maxSkill = value;
                    RaisePropertyChanged();
                }
            }
        }

        public string Description
        {
            get
            {
                return description;
            }
            set
            {
                if (description != value)
                {
                    description = value;
                    RaisePropertyChanged();
                }
            }
        }

        public string SelectedProduct
        {
            get
            {
                return selectedProduct;
            }
            set
            {
                if (selectedProduct != value)
                {
                    selectedProduct = value;
                    RaisePropertyChanged();
                }
            }
        }

        public ObservableCollection<IProcessProduct> InputProducts { get; set; }

        public ObservableCollection<IProcessWant> InputWants { get; set; }

        public ObservableCollection<IProcessProduct> CapitalProducts { get; set; }

        public ObservableCollection<IProcessWant> CapitalWants { get; set; }

        public ObservableCollection<IProcessProduct> OutputProducts { get; set; }

        public ObservableCollection<IProcessWant> OutputWants { get; set; }

        public bool Failure
        {
            get
            {
                return failure;
            }
            set
            {
                if (failure != value)
                {
                    failure = value;
                    RaisePropertyChanged();
                }
            }
        }

        public bool Consumption
        {
            get
            {
                return consumption;
            }
            set
            {
                if (consumption != value)
                {
                    consumption = value;
                    RaisePropertyChanged();
                }
            }
        }

        public bool Maintenance
        {
            get
            {
                return maintenance;
            }
            set
            {
                if (maintenance != value)
                {
                    maintenance = value;
                    RaisePropertyChanged();
                }
            }
        }

        public bool Use
        {
            get
            {
                return use;
            }
            set
            {
                if (use != value)
                {
                    use = value;
                    RaisePropertyChanged();
                }
            }
        }

        public bool Chance
        {
            get
            {
                return chance;
            }
            set
            {
                if (chance != value)
                {
                    chance = value;
                    RaisePropertyChanged();
                }
            }
        }

        public bool Crop
        {
            get
            {
                return crop;
            }
            set
            {
                if (crop != value)
                {
                    crop = value;
                    RaisePropertyChanged();
                }
            }
        }

        public bool Mine
        {
            get
            {
                return mine;
            }
            set
            {
                if (mine != value)
                {
                    mine = value;
                    RaisePropertyChanged();
                }
            }
        }

        public bool Extractor
        {
            get
            {
                return extractor;
            }
            set
            {
                if (extractor != value)
                {
                    extractor = value;
                    RaisePropertyChanged();
                }
            }
        }

        public bool Tap
        {
            get
            {
                return tap;
            }
            set
            {
                if (tap != value)
                {
                    tap = value;
                    RaisePropertyChanged();
                }
            }
        }

        public bool Refiner
        {
            get
            {
                return refiner;
            }
            set
            {
                if (refiner != value)
                {
                    refiner = value;
                    RaisePropertyChanged();
                }
            }
        }

        public bool Sorter
        {
            get
            {
                return sorter;
            }
            set
            {
                if (sorter != value)
                {
                    sorter = value;
                    RaisePropertyChanged();
                }
            }
        }

        public string SelectedImage
        {
            get
            {
                return selectedImage;
            }
            set
            {
                if (selectedImage != value)
                {
                    selectedImage = value;
                    RaisePropertyChanged();
                }
            }
        }

        public void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
