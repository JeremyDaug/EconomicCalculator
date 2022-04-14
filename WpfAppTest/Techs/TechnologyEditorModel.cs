using EconomicCalculator.DTOs.Technology;
using EconomicCalculator.Enums;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace EditorInterface.Techs
{
    internal class TechnologyEditorModel : INotifyPropertyChanged
    {
        private ITechnologyDTO tech;
        private string _name;
        private string _techCategory;
        private int _techBaseCost;
        private int _tier;
        private string _description;

        public TechnologyEditorModel(ITechnologyDTO tech)
        {
            this.tech = tech;

            Name = tech.Name;
            TechCategory = tech.Category.ToString();
            TechBaseCost = tech.TechBaseCost;
            Description = tech.Description;
            Tier = tech.Tier;

            Families = new ObservableCollection<string>(tech.Families);
            Parents = new ObservableCollection<string>(tech.Parents);
            Children = new ObservableCollection<string>(tech.Children);
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

        public string TechCategory
        {
            get
            {
                return _techCategory;
            }
            set
            {
                if (value != _techCategory)
                {
                    _techCategory = value;
                    RaisePropertyChanged();
                }
            }
        }

        public int TechBaseCost
        {
            get
            {
                return _techBaseCost;
            }
            set
            {
                if (_techBaseCost != value)
                {
                    _techBaseCost = value;
                    RaisePropertyChanged();
                }
            }
        }

        public int Tier
        {
            get
            {
                return _tier;
            }
            set
            {
                if (_tier != value)
                {
                    _tier = value;
                    RaisePropertyChanged();
                }
            }
        }

        public string Description
        {
            get
            {
                return _description;
            }
            set
            {
                if (_description != value)
                {
                    _description = value;
                    RaisePropertyChanged();
                }
            }
        }
        
        public ObservableCollection<string> Families { get; set; }

        public ObservableCollection<string> Parents { get; set; }

        public ObservableCollection<string> Children { get; set; }

        private void RaisePropertyChanged([CallerMemberName] string caller = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(caller));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
