using EconomicCalculator;
using EconomicCalculator.DTOs.Pops.Culture;
using Editor.Helpers;
using EditorInterface.Cultures.CultureNeedEditor;
using EditorInterface.Cultures.CultureWantEditor;
using EditorInterface.Helpers;
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

namespace EditorInterface.Cultures
{
    internal class CultureEditorViewModel : INotifyPropertyChanged
    {
        private CultureEditorModel model;

        private CultureDTO original;

        private DTOManager manager = DTOManager.Instance;
        private ICultureNeedDTO selectedNeed;
        private ICultureWantDTO selectedWant;

        private ICommand addNeed;
        private ICommand removeNeed;
        private ICommand addWant;
        private ICommand removeWant;

        public CultureEditorViewModel(CultureDTO original)
        {
            this.original = original;
            model = new CultureEditorModel(original);
            AllCultures = new ObservableCollection<string>(manager.Cultures
                .Values.Where(x => x.Name != original.Name && x.VariantName != original.VariantName).Select(x => x.ToString()));
        }

        public string Name
        {
            get { return model.Name; }
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
            get { return model.VariantName; }
            set
            {
                if (value != model.VariantName)
                { 
                    model.VariantName = value;
                    RaisePropertyChanged();
                }
            }
        }

        public decimal BirthModifier
        {
            get { return model.GrowthMod; }
            set
            {
                if (value != model.GrowthMod)
                {
                    model.GrowthMod = value;
                    RaisePropertyChanged();
                }
            }
        }

        public decimal DeathModifier
        {
            get { return model.DeathMod; }
            set
            {
                if (model.DeathMod != value)
                {
                    model.DeathMod = value;
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

        public ICultureNeedDTO SelectedNeed
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

        public ICultureWantDTO SelectedWant
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

        public ObservableCollection<ICultureNeedDTO> Needs => model.Needs;

        public ObservableCollection<ICultureWantDTO> Wants => model.Wants;

        public ObservableCollection<SelectorClass> Relations => model.Relations;

        public ObservableCollection<string> AllCultures { get; set; }

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
                MessageBox.Show("Culture Must have a name.", "Bad Name",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (BirthModifier < 0)
            {
                MessageBox.Show("Growth must be non-negative.", "Invalid Growth",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (DeathModifier < 0)
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
            original.BirthModifier = BirthModifier;
            original.DeathModifier = DeathModifier;

            // Ignore Duplicate Relations
            original.Needs = Needs.ToList();
            original.Wants = Wants.ToList();

            // get old connections for later.
            List<(int id, string name)> oldRels = new List<(int id, string name)>();
            for (int i = 0; i < original.RelatedCulturesIds.Count; ++i)
            {
                oldRels.Add((original.RelatedCulturesIds[i], original.RelatedCultures[i]));
            }

            original.RelatedCultures = Relations
                .Select(x => x.Selection)
                .Distinct().ToList();

            // clear out related Ids before refilling it.
            original.RelatedCulturesIds.Clear();

            // add relations to referenced Culture while we're at it.
            foreach (var rel in original.RelatedCultures)
            {
                var procName = CultureDTO.ProcessName(rel);
                // get related Id
                var related = manager.Cultures.Values
                                .Single(x => x.Name == procName.Name
                                    && x.VariantName == procName.VariantName);
                original.RelatedCulturesIds.Add(related.Id);
                // go to related Culture and add this relation if it's not already there.
                if (!manager.Cultures[related.Id].RelatedCulturesIds.Contains(original.Id))
                {
                    related.RelatedCulturesIds.Add(original.Id);
                    related.RelatedCultures.Add(original.ToString());
                }
            }

            // remove destroyed connections.
            oldRels = oldRels.Where(x => !original.RelatedCulturesIds.Contains(x.id)).ToList();

            foreach (var remRel in oldRels)
            {
                var old = manager.Cultures[remRel.id];
                old.RelatedCultures.Remove(original.ToString());
                old.RelatedCulturesIds.Remove(original.Id);
            }

            manager.Cultures[original.Id] = original;

            MessageBox.Show("Culture Commited, be sure to save.", "Culture commited.", MessageBoxButton.OK);
        }

        private void CreateAndAddNeed()
        {
            var newNeed = new CultureNeedDTO();

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

            NeedEditorView win = new NeedEditorView((CultureNeedDTO)SelectedNeed);

            win.ShowDialog();

            if (win.viewModel.Complete)
            {
                var data = win.viewModel;

                // create new product
                var newNeed = new CultureNeedDTO
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
            var newWant = new CultureWantDTO();

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

            WantEditorView win = new WantEditorView((CultureWantDTO)SelectedWant);

            win.ShowDialog();

            if (win.viewModel.Complete)
            {
                var data = win.viewModel;

                // create new product
                var newWant = new CultureWantDTO
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
            PropertyChanged?.Invoke(name, new PropertyChangedEventArgs(name));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
