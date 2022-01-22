using EconomicCalculator;
using EconomicCalculator.DTOs.Technology;
using Editor.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace EditorInterface.TechFamilies
{
    internal class TechFamilyViewModel : INotifyPropertyChanged
    {
        private TechFamilyDTO techFamily;
        private DTOManager manager;
        private TechFamilyModel model;
        public ICommand shiftTechToFamily;
        public ICommand shiftTechFromFamily;
        public ICommand addRelation;
        public ICommand removeRelation;
        public ICommand commitFamily;

        public TechFamilyViewModel(TechFamilyDTO techFam)
        {
            manager = DTOManager.Instance;

            this.techFamily = techFam;

            model = new TechFamilyModel(this.techFamily);

            //AvailableTechs = new ObservableCollection<string>(manager.Te)

            AvailableFamilies = new ObservableCollection<string>(manager.TechFamilies
                .Values.Select(x => x.Name)
                .Where(x => !model.RelatedFamilies.Contains(x) && techFamily.Name != x));
        }

        #region Commands

        public ICommand ShiftTechToFamily
        {
            get
            {
                if (shiftTechToFamily == null)
                {
                    shiftTechToFamily = new RelayCommand(
                        param => AddTech());
                }
                    return shiftTechToFamily;
            }
        }
        
        public ICommand ShiftTechFromFamily
        {
            get
            {
                if (shiftTechFromFamily == null)
                {
                    shiftTechFromFamily = new RelayCommand(
                        param => RemoveTech());
                }
                return shiftTechFromFamily;
            }
        }

        public ICommand AddRelation
        {
            get
            {
                if (addRelation == null)
                {
                    addRelation = new RelayCommand(
                        param => ShiftRelationToFamily());
                }
                return addRelation;
            }
        }

        public ICommand RemoveRelation
        {
            get
            {
                if (removeRelation == null)
                {
                    removeRelation = new RelayCommand(
                        param => ShiftRelationFromFamily());
                }
                return removeRelation;
            }
        }

        public ICommand CommitFamily
        {
            get
            {
                if (commitFamily == null)
                {
                    commitFamily = new RelayCommand(
                        param => Commit());
                }
                return commitFamily;
            }
        }

        #endregion Commands

        #region CommandFunctions

        public void Commit()
        {
            var newFam = new TechFamilyDTO
            {
                Name = techFamily.Name,
                Description = techFamily.Description
            };

            // Families
            foreach (var rel in Relations)
            {
                newFam.RelatedFamilyStrings.Add(rel);
                newFam.RelatedFamilies.Add(manager.TechFamilies.Values.Single(x => x.Name == rel).Id);
            }

            // techs
            foreach (var tech in Techs)
            {
                newFam.TechStrings.Add(tech);
                //newFam.Techs.Add(manager.tech)
            }

            newFam.Id = techFamily.Id;

            // set old to new.
            manager.TechFamilies[newFam.Id] = newFam;
        }

        public void AddTech()
        {
            if (SelectedAvailableTechs != null)
            {
                Techs.Add(SelectedAvailableTechs);
                AvailableTechs.Remove(SelectedAvailableTechs);
            }
        }

        public void RemoveTech()
        {
            if (SelectedTechs != null)
            {
                AvailableTechs.Add(SelectedTechs);
                Techs.Remove(SelectedTechs);
            }
        }

        public void ShiftRelationToFamily()
        {
            if (SelectedAvailableFamilies != null)
            {
                Relations.Add(SelectedAvailableFamilies);
                AvailableFamilies.Remove(SelectedAvailableFamilies);
            }
        }

        public void ShiftRelationFromFamily()
        {
            if (SelectedRelations != null)
            {
                AvailableFamilies.Add(SelectedRelations);
                Relations.Remove(SelectedRelations);
            }
        }

        #endregion CommandFunctions

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

        public string SelectedAvailableTechs
        {
            get
            {
                return model.SelectedAvailableTechs;
            }
            set
            {
                if (model.SelectedAvailableTechs != value)
                {
                    model.SelectedAvailableTechs = value;
                    RaisePropertyChanged();
                }
            }
        }

        public string SelectedTechs
        {
            get
            {
                return model.SelectedTechs;
            }
            set
            {
                if (model.SelectedTechs != value)
                {
                    model.SelectedTechs = value;
                    RaisePropertyChanged();
                }
            }
        }

        public string SelectedAvailableFamilies
        {
            get
            {
                return model.SelectedAvailableFamilies;
            }
            set
            {
                if (model.SelectedAvailableFamilies != value)
                {
                    model.SelectedAvailableFamilies = value;
                    RaisePropertyChanged();
                }
            }
        }

        public string SelectedRelations
        {
            get
            {
                return model.SelectedRelations;
            }
            set
            {
                if (model.SelectedRelations != value)
                {
                    model.SelectedRelations = value;
                    RaisePropertyChanged();
                }
            }
        }

        #endregion Props

        #region Lists

        public ObservableCollection<string> Techs
        {
            get
            {
                return model.TechStrings;
            }
        }

        public ObservableCollection<string> Relations
        {
            get
            {
                return model.RelatedFamilies;
            }
        }

        public ObservableCollection<string> AvailableTechs { get; set; }

        public ObservableCollection<string> AvailableFamilies { get; set; }

        #endregion Lists

        private void RaisePropertyChanged([CallerMemberName] string v = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(v));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
