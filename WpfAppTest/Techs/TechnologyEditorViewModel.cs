using EconomicCalculator;
using EconomicCalculator.DTOs.Technology;
using EconomicCalculator.Enums;
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

namespace EditorInterface.Techs
{
    internal class TechnologyEditorViewModel : INotifyPropertyChanged
    {
        private ITechnologyDTO tech;
        private DTOManager manager;


        private TechnologyEditorModel model;
        private ICommand commit;
        private ICommand addFam;
        private ICommand removeFam;
        private ICommand addParent;
        private ICommand removeParent;
        private ICommand addChild;
        private ICommand removeChild;

        private string _selectedFamily;
        private string _selectedFam;
        private string _selectedParentTech;
        private string _selectedParent;
        private string _selectedChildren;
        private string _selectedChild;

        public TechnologyEditorViewModel(ITechnologyDTO tech)
        {
            model = new TechnologyEditorModel(tech);
            manager = DTOManager.Instance;

            this.tech = tech;

            // Select all families not already in the tech.
            AvailableFamilies = new ObservableCollection<string>(
                manager.TechFamilies.Values
                .Select(x => x.Name)
                .Where(x => !tech.Families.Contains(x)));
            // All techs not in parents or children currently.
            AvailableTechs = new ObservableCollection<string>(
                manager.Technologies.Values
                .Select(x => x.Name)
                .Where(x => !tech.Parents.Contains(x))
                .Where(x => !tech.Children.Contains(x)));

            AllCategories = new ObservableCollection<string>(
                Enum.GetNames(typeof(TechCategory)));
        }

        #region Props

        public string Name
        {
            get
            {
                return model.Name;
            }
            set
            {
                if (model.Name != value)
                {
                    model.Name = value;
                    RaisePropertyChanged();
                }
            }
        }

        public int TechBaseCost
        {
            get
            {
                return model.TechBaseCost;
            }
            set
            {
                if (model.TechBaseCost != value)
                {
                    model.TechBaseCost = value;
                    RaisePropertyChanged();
                }
            }
        }

        public string TechCategory
        {
            get
            {
                return model.TechCategory;
            }
            set
            {
                if (model.TechCategory != value)
                {
                    model.TechCategory = value;
                    RaisePropertyChanged();
                }
            }
        }

        public int Tier
        {
            get
            {
                return model.Tier;
            }
            set
            {
                if (model.Tier != value)
                {
                    model.Tier = value;
                    RaisePropertyChanged();
                }
            }
        }

        public string Description
        {
            get
            {
                return model.Description;
            }
            set
            {
                if (model.Description != value)
                {
                    model.Description = value;
                    RaisePropertyChanged();
                }
            }
        }

        public string SelectedFamily
        {
            get
            {
                return _selectedFamily;
            }
            set
            {
                if (_selectedFamily != value)
                {
                    _selectedFamily = value;
                    RaisePropertyChanged();
                }
            }
        }

        public string SelectedFam
        {
            get
            {
                return _selectedFam;
            }
            set
            {
                if (_selectedFam != value)
                {
                    _selectedFam = value;
                    RaisePropertyChanged();
                }
            }
        }

        public string SelectedParentTech
        {
            get
            {
                return _selectedParentTech;
            }
            set
            {
                if (_selectedParentTech != value)
                {
                    _selectedParentTech = value;
                    RaisePropertyChanged();
                }
            }
        }

        public string SelectedParent
        {
            get
            {
                return _selectedParent;
            }
            set
            {
                if (_selectedParent != value)
                {
                    SelectedParent = value;
                    RaisePropertyChanged();
                }
            }
        }

        public string SelectedChildren
        {
            get
            {
                return _selectedChildren;
            }
            set
            {
                if (_selectedChildren != value)
                {
                    _selectedChildren = value;
                    RaisePropertyChanged();
                }
            }
        }

        public string SelectedChild
        {
            get
            {
                return _selectedChild;
            }
            set
            {
                if (_selectedChild != value)
                {
                    _selectedChild = value;
                    RaisePropertyChanged();
                }
            }
        }

        #endregion Props

        #region Commands

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

        public ICommand AddFam
        {
            get
            {
                if (addFam == null)
                {
                    addFam = new RelayCommand(
                        param => AddFamily());
                }
                return addFam;
            }
        }

        public ICommand RemoveFam
        {
            get
            {
                if (removeFam == null)
                {
                    removeFam = new RelayCommand(
                        param => RemoveFamily());
                }
                return removeFam;
            }
        }

        public ICommand AddParent
        {
            get
            {
                if (addParent == null)
                {
                    addParent = new RelayCommand(
                        param => AddParentToTech());
                }
                return addParent;
            }
        }

        public ICommand RemoveParent
        {
            get 
            { 
                if (removeParent == null)
                {
                    removeParent = new RelayCommand(
                        param => RemoveParentFromTech());
                }
                return removeParent;
            }
        }

        public ICommand AddChild
        {
            get
            {
                if (addChild == null)
                {
                    addChild = new RelayCommand(
                        param => AddChildToTech());
                }
                return addChild;
            }
        }

        public ICommand RemoveChild
        {
            get
            {
                if (removeChild == null)
                {
                    removeChild = new RelayCommand(
                        param => RemoveChildFromTech());
                }
                return removeChild;
            }
        }

        #endregion Commands

        #region Lists

        public ObservableCollection<string> AvailableFamilies { get; set; }

        public ObservableCollection<string> Families => model.Families;

        public ObservableCollection<string> AvailableTechs { get; set; }

        public ObservableCollection<string> Parents => model.Parents;

        public ObservableCollection<string> Children => model.Children;

        public ObservableCollection<string> AllCategories { get; set; }

        #endregion Lists

        #region Functions

        public void AddFamily()
        {
            if (SelectedFamily != null)
            {
                Families.Add(SelectedFamily);
                AvailableFamilies.Remove(SelectedFamily);
            }
        }

        public void RemoveFamily()
        {
            if (SelectedFam != null)
            {
                AvailableFamilies.Add(SelectedFam);
                Families.Remove(SelectedFam);
            }
        }

        public void AddParentToTech()
        {
            if (SelectedParentTech != null)
            {
                Parents.Add(SelectedParentTech);
                AvailableTechs.Remove(SelectedParentTech);
            }
        }
        
        public void RemoveParentFromTech()
        {
            if (SelectedParent != null)
            {
                AvailableTechs.Add(SelectedParent);
                Parents.Remove(SelectedParent);
            }
        }

        public void AddChildToTech() 
        {
            if (SelectedChildren != null)
            {
                Children.Add(SelectedChildren);
                AvailableTechs.Remove(SelectedChildren);
            }
        }

        public void RemoveChildFromTech()
        {
            if (SelectedChild != null)
            {
                AvailableTechs.Add(SelectedChild);
                Children.Remove(SelectedChild);
            }
        }

        private void CommitData()
        {
            // check name 
            if (string.IsNullOrWhiteSpace(Name))
            {
                MessageBox.Show("Tech must have a name.", "Bad Name", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (Name.Length < 3)
            {
                MessageBox.Show("Tech Name must be 3 letters or longer.", "Bad Name",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            // Tech cost
            if (TechBaseCost <= 0)
            {
                MessageBox.Show("Tech must have a cost greater than 0.", "Cost too low.",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (TechCategory == null)
            {
                MessageBox.Show("Tech must have a category.", "Bad Category",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (Parents.Count == 0)
            {
                MessageBox.Show("Tech must have at least 1 parent.", "Missing Name",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var newTech = new TechnologyDTO
            {
                Id = tech.Id,
                Name = Name,
                TechBaseCost = TechBaseCost,
                Tier = Tier,
                Description = Description,
                Category = TechCategory
            };

            // families
            foreach (var fam in Families)
            {
                newTech.Families.Add(fam);
                newTech.FamilyIds.Add(manager.TechFamilies
                    .Values.Single(x => x.Name == fam).Id);
            }

            // Parents
            foreach (var parent in Parents)
            {
                newTech.Parents.Add(parent);
                newTech.ParentIds.Add(manager.Technologies
                    .Values.Single(x => x.Name == parent).Id);
            }

            // children
            foreach (var child in Children)
            {
                newTech.Children.Add(child);
                newTech.ChildrenIds.Add(manager.Technologies
                    .Values.Single(x => x.Name == child).Id);
            }

            manager.Technologies[newTech.Id] = newTech;

            MessageBox.Show("Tech Commited!", "Tech Commited!", 
                MessageBoxButton.OK);
        }

        #endregion Functions

        private void RaisePropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
