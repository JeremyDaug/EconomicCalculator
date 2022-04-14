using EconomicCalculator.DTOs.Technology;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace EditorInterface.TechFamilies
{
    internal class TechFamilyModel : INotifyPropertyChanged
    {
        private string _name;
        private string description;

        public TechFamilyModel(TechFamilyDTO fam)
        {
            Name = fam.Name;
            Description = fam.Description;
            RelatedFamilies = new ObservableCollection<string>(fam.RelatedFamilyStrings);
            TechStrings = new ObservableCollection<string>(fam.TechStrings);
        }

        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                if (_name != value)
                {
                    _name = value;
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

        public ObservableCollection<string> RelatedFamilies { get; }

        public ObservableCollection<string> TechStrings { get; }
        public string SelectedAvailableTechs { get; internal set; }
        public string SelectedTechs { get; internal set; }
        public string SelectedAvailableFamilies { get; internal set; }
        public string SelectedRelations { get; internal set; }

        private void RaisePropertyChanged([CallerMemberName] string v = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(v));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
