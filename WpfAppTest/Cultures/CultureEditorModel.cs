using EconomicCalculator.DTOs.Pops.Culture;
using EditorInterface.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace EditorInterface.Cultures
{
    internal class CultureEditorModel : INotifyPropertyChanged
    {
        private string name;
        private string variantName;
        private decimal growthMod;
        private decimal deathMod;
        private string description;

        private CultureDTO original;

        public CultureEditorModel(CultureDTO original)
        {
            this.original = original;

            Name = original.Name;
            variantName = original.VariantName;
            GrowthMod = original.BirthModifier;
            deathMod = original.DeathModifier;
            Description = original.Description;

            Wants = new ObservableCollection<ICultureWantDTO>(original.Wants);
            Needs = new ObservableCollection<ICultureNeedDTO>(original.Needs);
            Relations = new ObservableCollection<SelectorClass>();

            foreach (var rel in original.RelatedCultures)
            {
                Relations.Add(new SelectorClass {  Selection = rel });
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
                if (value != name)
                {
                    name = value;
                    RaisePropertyChanged();
                }
            }
        }

        public string VariantName
        {
            get { return variantName; }
            set
            {
                if (variantName != value)
                {
                    variantName = value;
                    RaisePropertyChanged();
                }
            }
        }

        public decimal GrowthMod
        {
            get { return growthMod; }
            set
            {
                if (growthMod != value)
                {
                    growthMod = value;
                    RaisePropertyChanged();
                }
            }
        }

        public decimal DeathMod
        {
            get => deathMod;
            set
            {
                if (deathMod != value)
                {
                    deathMod = value;
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

        public ObservableCollection<ICultureWantDTO> Wants { get; set; }

        public ObservableCollection<ICultureNeedDTO> Needs { get; set; }

        // culture tags

        public ObservableCollection<SelectorClass> Relations { get; set; }

        private void RaisePropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
