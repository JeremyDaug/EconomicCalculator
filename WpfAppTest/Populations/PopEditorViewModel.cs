using EconomicSim;
using EconomicSim.DTOs.Pops;
using Editor.Helpers;
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

namespace EditorInterface.Populations
{
    internal class PopEditorViewModel : INotifyPropertyChanged
    {
        private PopDTO original;
        private DTOManager manager = DTOManager.Instance;

        private PopEditorModel model;

        private bool lockCount;

        private bool countSpecies;
        private bool countCultures;

        private ulong speciesSum;
        private ulong cultureSum;

        private ICommand commit;

        public PopEditorViewModel(PopDTO original)
        {
            this.original = original;

            model = new PopEditorModel(original);

            AvailableCultures = new ObservableCollection<string>(manager
                .Cultures.Values.Select(x => x.ToString()).ToList());
            AvailableSpecies = new ObservableCollection<string>(manager
                .Species.Values.Select(x => x.ToString()).ToList());

            UnlockCount = true;
        }

        public ulong Count
        {
            get { return model.Count; }
            set
            {
                if (value != model.Count)
                {
                    model.Count = value;
                    RaisePropertyChanged();
                }
            }
        }

        public string Job
        {
            get { return model.Job; }
            set
            {
                if (model.Job != value)
                {
                    model.Job = value;
                    RaisePropertyChanged();
                }
            }
        }

        public string Firm
        {
            get { return model.Firm; }
            set
            {
                if (model.Firm != value)
                {
                    model.Firm = value;
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

        public string Skill
        {
            get { return model.Skill; }
            set
            {
                if (model.Skill != value)
                {
                    model.Skill = value;
                    RaisePropertyChanged();
                }
            }
        }

        public decimal SkillLevel
        {
            get { return model.SkillLevel; }
            set
            {
                if (model.SkillLevel != value)
                {
                    model.SkillLevel = value;
                    RaisePropertyChanged();
                }
            }
        }

        public bool UnlockCount
        {
            get { return lockCount; }
            set
            {
                if (lockCount != value)
                {
                    lockCount = value;
                    RaisePropertyChanged();
                }
            }
        }

        private bool CountSpecies
        {
            get { return countSpecies; }
            set
            {
                if (countSpecies != value)
                {
                    countSpecies = value;
                    RaisePropertyChanged();
                }
            }
        }

        public bool CountCultures
        {
            get { return countCultures; }
            set
            {
                if (value != countCultures)
                {
                    CountCultures = value;
                    RaisePropertyChanged();
                }
            }
        }

        public ulong SpeciesSum
        {
            get
            {
                speciesSum = 0;
                foreach(var part in PopSpeciesPortions)
                {
                    speciesSum += part.Amount;
                }
                return speciesSum;
            }
            set { RaisePropertyChanged(); }
        }

        public ulong CultureSum
        {
            get
            {
                cultureSum = 0;
                foreach (var part in PopCulturePortions)
                {
                    cultureSum += part.Amount;
                }
                return cultureSum;
            }
            set
            {
                RaisePropertyChanged();
            }
        }

        public ObservableCollection<PopSpeciesPortion> PopSpeciesPortions => model.PopSpeciesPortions;

        public ObservableCollection<PopCulturePortion> PopCulturePortions => model.PopCulturePortions;

        public ObservableCollection<string> AvailableSpecies { get; set; }

        public ObservableCollection<string> AvailableCultures { get; set; }

        public ICommand Commit
        {
            get
            {
                if (commit == null)
                {
                    commit = new RelayCommand(
                        param => CommitData());
                }
                return commit;
            }
        }

        private void CommitData()
        {
            // check for empty boxes
            if (Count < 0)
            {
                MessageBox.Show("Population Count must be a whole number greater than 0!", "Pops too low!", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (SkillLevel < 0)
            {
                MessageBox.Show("Skill Level must be at least 0.", "Skill too low.", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // next Species and Culture and total should be equal.
            if (SpeciesSum != CultureSum ||
                Count != CultureSum)
            {
                MessageBox.Show("Total Population, Species, and Culture totals should match!", "Sum Mismatch!", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // go through for duplicates, if any duplicates combine and kickback for user to
            // doublecheck
            if (PopSpeciesPortions.Count != PopSpeciesPortions.Select(x => x.Species).Distinct().Count() ||
                PopCulturePortions.Count != PopCulturePortions.Select(x => x.Culture).Distinct().Count())
            {
                // consolidate species
                if (PopSpeciesPortions.Count != PopSpeciesPortions.Select(x => x.Species).Distinct().Count())
                {
                    var dupSpecies = PopSpeciesPortions.GroupBy(x => x.Species)
                        .Select(x => new PopSpeciesPortion
                        {
                            Species = x.Key,
                            Amount = x.Select(y => y.Amount)
                            .Aggregate((a, c) => a + c)
                        })
                        .ToList();
                    // remove duplicates
                    PopSpeciesPortions.Clear();
                    var index = 0;
                    foreach (var dupe in dupSpecies)
                    {
                        PopSpeciesPortions.Insert(index, dupe);
                        index++;
                    }
                    
                }

                // consolidate Cultures
                if (PopCulturePortions.Count != PopCulturePortions.Select(x => x.Culture).Distinct().Count())
                {
                    var dupSpecies = PopCulturePortions.GroupBy(x => x.Culture)
                        .Select(x => new PopCulturePortion
                        {
                            Culture = x.Key,
                            Amount = x.Select(y => y.Amount)
                            .Aggregate((a, c) => a + c)
                        })
                        .ToList();
                    // remove duplicates
                    PopCulturePortions.Clear();
                    var index = 0;
                    foreach (var dupe in dupSpecies)
                    {
                        PopCulturePortions.Insert(index, dupe);
                        index++;
                    }

                }

                MessageBox.Show("Duplicates found in Species or Culture Make-up, make sure consolidation is to your liking.", "Duplicate entries found.",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // get species and cultures linked
            foreach (var spec in PopSpeciesPortions)
            {
                spec.SpeciesId = manager.GetSpeciesByName(spec.Species).Id;
            }
            foreach (var cult in PopCulturePortions)
            {
                cult.CultureId = manager.GetCultureByName(cult.Culture).Id;
            }

            // create new pop and save to manager.
            var newPop = new PopDTO
            { // TODO Complete Id connections with
                Id = original.Id,
                Count = Count,
                Firm = Firm,
                // FirmId
                Job = Job,
                // JobId
                Market = Market,
                // MarketId
                Skill = Skill,
                SkillId = manager.GetSkillByName(Skill).Id,
                SkillLevel = SkillLevel,
                CulturePortions = new List<IPopCulturePortion>(PopCulturePortions),
                SpeciesPortions = new List<IPopSpeciesPortion>(PopSpeciesPortions)
            };

            manager.Pops[newPop.Id] = newPop;

            MessageBox.Show("Commited Pop changes!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void RaisePropertyChanged([CallerMemberName] string name = null)
        {
            switch (name)
            {
                case nameof(PopSpeciesPortions):
                    RaisePropertyChanged(nameof(SpeciesSum));
                    break;
                case nameof(PopCulturePortions):
                    RaisePropertyChanged(nameof(CultureSum));
                    break;
            }

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
