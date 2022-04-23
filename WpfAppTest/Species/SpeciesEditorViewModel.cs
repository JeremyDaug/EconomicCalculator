using EconomicSim;
using EconomicSim.DTOs.Pops.Species;
using EconomicSim.Objects.Pops;
using Editor.Helpers;
using EditorInterface.Helpers;
using EditorInterface.Species.SpeciesNeedEditor;
using EditorInterface.Species.SpeciesWantEditor;
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

namespace EditorInterface.Species
{
    internal class SpeciesEditorViewModel : INotifyPropertyChanged
    {
        private SpeciesEditorModel model;

        private SpeciesDTO original;

        private DTOManager manager;
        private ISpeciesNeedDTO selectedNeed;
        private ISpeciesWantDTO selectedWant;

        private ICommand addNeed;
        private ICommand removeNeed;
        private ICommand addWant;
        private ICommand removeWant;

        public SpeciesEditorViewModel(SpeciesDTO original)
        {
            model = new SpeciesEditorModel(original);

            this.original = original;

            manager = DTOManager.Instance;

            AllSpecies = new ObservableCollection<string>(
                manager.Species.Values
                .Select(x => x.ToString())
                .Where(x => x != original.ToString()));
        }

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

        public string VariantName
        {
            get
            {
                return model.VariantName;
            }
            set
            {
                if (model.VariantName != value)
                {
                    model.VariantName = value;
                    RaisePropertyChanged();
                }
            }
        }

        public decimal GrowthRate
        {
            get
            {
                return model.GrowthRate;
            }
            set
            {
                if (model.GrowthRate != value)
                {
                    model.GrowthRate = value;
                    RaisePropertyChanged();
                }
            }
        }

        public int LifeSpan
        {
            get
            {
                return model.LifeSpan;
            }
            set
            {
                if (model.LifeSpan != value)
                {
                    model.LifeSpan = value;
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

        public ISpeciesNeedDTO SelectedNeed
        {
            get
            {
                return selectedNeed;
            }
            set
            {
                if (selectedNeed != value)
                {
                    selectedNeed = value;
                    RaisePropertyChanged();
                }
            }
        }

        public ISpeciesWantDTO SelectedWant
        {
            get
            {
                return selectedWant;
            }
            set
            {
                if (selectedWant != value)
                {
                    selectedWant = value;
                    RaisePropertyChanged();
                }
            }
        }

        public ObservableCollection<ISpeciesNeedDTO> Needs => model.Needs;

        public ObservableCollection<ISpeciesWantDTO> Wants => model.Wants;

        public ObservableCollection<SelectorClass> Relations => model.Relations;

        public ObservableCollection<string> AllSpecies { get; set; }

        public ICommand AddNeed
        {
            get
            {
                if (addNeed == null)
                {
                    addNeed = new RelayCommand(
                        param => CreateAndAddNeed());
                }
                return addNeed;
            }
        }

        public ICommand RemoveNeed
        {
            get
            {
                if (removeNeed == null)
                {
                    removeNeed = new RelayCommand(
                        param => RemoveSelectedNeed());
                }
                return removeNeed;
            }
        }

        public ICommand AddWant
        {
            get
            {
                if (addWant == null)
                {
                    addWant = new RelayCommand(
                        param => AddNewWant());
                }
                return addWant;
            }
        }

        public ICommand RemoveWant
        {
            get
            {
                if (removeWant == null)
                {
                    removeWant = new RelayCommand(
                        param => RemoveSelectedWant());
                }
                return removeWant;
            }
        }

        internal void Commit()
        {
            // check each box.
            if (string.IsNullOrWhiteSpace(Name))
            {
                MessageBox.Show("Species Must have a name.", "Bad Name",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (GrowthRate < 0)
            {
                MessageBox.Show("Growth must be non-negative.", "Invalid Growth",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (LifeSpan < 0)
            {
                MessageBox.Show("Life Span must be non-negative.", "Invalid Lifespan",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Ignore Duplicates for Desires as the eventual addition of tags
            // may change what a duplicate is.

            // update original
            original.Name = Name;
            original.VariantName = VariantName == null ? "" : VariantName;
            original.Description = Description;
            original.LifeSpan = LifeSpan;
            original.BirthRate = GrowthRate;

            // Ignore Duplicate Relations
            original.Needs = Needs.ToList();
            original.Wants = Wants.ToList();

            // get old connections for later.
            List<(int id, string name)> oldRels = new List<(int id, string name)>();
            for (int i = 0; i < original.RelatedSpeciesIds.Count; ++i)
            {
                oldRels.Add((original.RelatedSpeciesIds[i], original.RelatedSpecies[i]));
            }

            original.RelatedSpecies = Relations
                .Select(x => x.Selection)
                .Distinct().ToList();

            // clear out related Ids before refilling it.
            original.RelatedSpeciesIds.Clear();

            // add relations to referenced species while we're at it.
            foreach (var rel in original.RelatedSpecies)
            {
                var procName = SpeciesDTO.ProcessName(rel);
                // get related Id
                var related = manager.Species.Values
                                .Single(x => x.Name == procName.Name
                                    && x.VariantName == procName.VariantName);
                original.RelatedSpeciesIds.Add(related.Id);
                // go to related species and add this relation if it's not already there.
                if (!manager.Species[related.Id].RelatedSpeciesIds.Contains(original.Id))
                {
                    related.RelatedSpeciesIds.Add(original.Id);
                    related.RelatedSpecies.Add(original.ToString());
                }
            }

            // remove destroyed connections.
            oldRels = oldRels.Where(x => !original.RelatedSpeciesIds.Contains(x.id)).ToList();

            foreach (var remRel in oldRels)
            {
                var old = manager.Species[remRel.id];
                old.RelatedSpecies.Remove(original.ToString());
                old.RelatedSpeciesIds.Remove(original.Id);
            }

            if (!manager.Species.ContainsKey(original.Id))
                manager.Species[original.Id] = original;

            MessageBox.Show("Species Commited, be sure to save.", "Species commited.", MessageBoxButton.OK);
        }

        private void CreateAndAddNeed()
        {
            var newNeed = new SpeciesNeedDTO();

            NeedEditorView win = new NeedEditorView(newNeed);

            win.ShowDialog();

            if (win.viewModel.Complete)
            {
                var data = win.viewModel;
                // add complete need to list.
                newNeed.Product = data.Product;
                newNeed.Tier = data.Tier;
                newNeed.Amount = data.Amount;

                newNeed.ProductId = manager
                    .GetProductByFullName(newNeed.Product).Id;

                Needs.Add(newNeed);
            }
        }

        public void EditExistingNeed()
        {
            if (SelectedNeed == null)
                return;

            NeedEditorView win = new NeedEditorView((SpeciesNeedDTO)SelectedNeed);

            win.ShowDialog();

            if (win.viewModel.Complete)
            {
                var data = win.viewModel;

                // create new product
                var newNeed = new SpeciesNeedDTO
                {
                    ProductId = manager.GetProductByFullName(data.Product).Id,
                    Product = data.Product,
                    Tier = data.Tier,
                    Amount = data.Amount
                };

                // add new
                Needs.Add(newNeed);

                // remove old
                Needs.Remove(selectedNeed);
            }
        }

        public void RemoveSelectedNeed()
        {
            if (SelectedNeed == null)
                return;

            Needs.Remove(SelectedNeed);
        }

        private void AddNewWant()
        {
            var newWant = new SpeciesWantDTO();

            WantEditorView win = new WantEditorView(newWant);

            win.ShowDialog();

            if (win.viewModel.Complete)
            {
                var data = win.viewModel;
                // add complete need to list.
                newWant.Want = data.Want;
                newWant.Tier = data.Tier;
                newWant.Amount = data.Amount;

                newWant.WantId = manager
                    .GetWantByName(newWant.Want).Id;

                Wants.Add(newWant);
            }
        }

        public void EditExistingWant()
        {
            if (SelectedWant == null)
                return;

            WantEditorView win = new WantEditorView((SpeciesWantDTO)SelectedWant);

            win.ShowDialog();

            if (win.viewModel.Complete)
            {
                var data = win.viewModel;

                // create new product
                var newWant = new SpeciesWantDTO
                {
                    WantId = manager.GetWantByName(data.Want).Id,
                    Want = data.Want,
                    Tier = data.Tier,
                    Amount = data.Amount
                };

                // add new
                Wants.Add(newWant);

                // remove old
                Wants.Remove(selectedWant);
            }
        }

        public void RemoveSelectedWant()
        {
            if (SelectedWant == null)
                return;
            Wants.Remove(SelectedWant);
        }

        private void RaisePropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
