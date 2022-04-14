using EconomicCalculator.DTOs.Pops;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace EditorInterface.Populations
{
    internal class PopEditorModel : INotifyPropertyChanged
    {
        private PopDTO original;

        private ulong count;
        private string job;
        private string firm;
        private string market;
        private string skill;
        private decimal skillLevel;

        public PopEditorModel(PopDTO original)
        {
            this.original = original;

            Count = original.Count;
            Job = original.Job;
            Firm = original.Firm;
            Market = original.Market;
            Skill = original.Skill;
            skillLevel = original.SkillLevel;

            PopSpeciesPortions = new ObservableCollection<PopSpeciesPortion>();
            PopCulturePortions = new ObservableCollection<PopCulturePortion>();

            foreach (var item in original.SpeciesPortions)
            {
                PopSpeciesPortions.Add((PopSpeciesPortion)item);
            }
            foreach (var item in original.CulturePortions)
            {
                PopCulturePortions.Add((PopCulturePortion)item);
            }
        }

        public ulong Count
        {
            get { return count; }
            set
            {
                if (value != count)
                {
                    count = value;
                    RaisePropertyChanged();
                }
            }
        }

        public string Job
        {
            get { return job; }
            set
            {
                if (job != value)
                {
                    job = value;
                    RaisePropertyChanged();
                }
            }
        }

        public string Firm
        {
            get { return firm; }
            set
            {
                if (firm != value)
                {
                    firm = value;
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

        public string Skill
        {
            get { return skill; }
            set
            {
                if (skill != value)
                {
                    skill = value;
                    RaisePropertyChanged();
                }
            }
        }

        public decimal SkillLevel
        {
            get { return skillLevel; }
            set
            {
                if (skillLevel != value)
                {
                    skillLevel = value;
                    RaisePropertyChanged();
                }
            }
        }

        public ObservableCollection<PopSpeciesPortion> PopSpeciesPortions { get; set; }

        public ObservableCollection<PopCulturePortion> PopCulturePortions { get; set; }

        private void RaisePropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
