using EconomicCalculator;
using EconomicCalculator.DTOs.Pops.Species;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace EditorInterface.Species
{
    internal class SpeciesEditorModel : INotifyPropertyChanged
    {
        private SpeciesDTO original;

        private string name;
        private string variantName;
        private decimal growthRate;
        private int lifeSpan;
        private string description;

        DTOManager manager;

        public SpeciesEditorModel(SpeciesDTO original)
        {
            this.original = original;

            manager = DTOManager.Instance;

            Name = original.Name;
            VariantName = original.VariantName;
            GrowthRate = original.GrowthRate;
            LifeSpan = original.LifeSpan;
            Description = original.Description;

            Needs = new ObservableCollection<ISpeciesNeedDTO>(
                original.Needs);
            Wants = new ObservableCollection<ISpeciesWantDTO>(
                original.Wants);
            Relations = new ObservableCollection<SelectorClass>();

            foreach (var rel in original.RelatedSpecies)
            {
                Relations.Add(new SelectorClass { Selection = rel });
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

        public decimal GrowthRate
        {
            get
            {
                return growthRate;
            }
            set
            {
                if (growthRate != value)
                {
                    growthRate = value;
                    RaisePropertyChanged();
                }
            }
        }

        public int LifeSpan
        {
            get
            {
                return lifeSpan;
            }
            set
            {
                if (lifeSpan != value)
                {
                    lifeSpan = value;
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

        public ObservableCollection<ISpeciesNeedDTO> Needs { get; set; }

        public ObservableCollection<ISpeciesWantDTO> Wants { get; set; }

        public ObservableCollection<SelectorClass> Relations { get; set; }

        private void RaisePropertyChanged([CallerMemberName]string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
