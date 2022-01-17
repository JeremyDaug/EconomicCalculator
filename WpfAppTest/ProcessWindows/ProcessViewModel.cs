using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using EconomicCalculator;
using EconomicCalculator.DTOs;
using EconomicCalculator.DTOs.Processes;
using EconomicCalculator.DTOs.Processes.ProcessTags;
using EconomicCalculator.DTOs.Processes.ProductionTags;
using EconomicCalculator.DTOs.Products;
using EditorInterface.Helpers;

namespace EditorInterface.ProcessWindows
{
    public class ProcessViewModel : INotifyPropertyChanged
    {
        private ProcessDTO process;
        private DTOManager manager;
        public ProcessModel model;

        private ICommand commit;
        private ICommand newInputProduct;
        private ICommand newInputWant;
        private ICommand newCapitalProduct;
        private ICommand newCapitalWant;
        private ICommand newOutputProduct;
        private ICommand newOutputWant;

        private ICommand deleteInputProduct;
        private ICommand deleteInputWant;
        private ICommand deleteCapitalProduct;
        private ICommand deleteCapitalWant;
        private ICommand deleteOutputProduct;
        private ICommand deleteOutputWant;
        private RelayCommand selectImage;
        private BitmapSource iconImage;
        private Bitmap iconBitmap;
        private string selectedProduct;

        public ProcessViewModel(ProcessDTO process)
        {
            model = new ProcessModel(process);

            manager = DTOManager.Instance;

            AvailableSkills = new ObservableCollection<string>
                (manager.Skills.Values.Select(x => x.Name));

            AvailableProducts = new ObservableCollection<string>
                (manager.Products.Values.Select(x => x.GetName()));

            SelectedProduct = manager.Products.Values.First().GetName();

            this.process = process;

            // there should only ever be 1 input group
            InputMass = new ObservableCollection<ProcessMassGroup>();
            InputMass.Add(new ProcessMassGroup
            {
                Part = "Inputs",
                Mass = InputProducts.Sum(x => x.Amount * manager.Products[x.ProductId].Mass)
            });

            // just clear it out, it'll be easier. If this becomes
            // a probelm we'll deal with it then.
            OutputMass = new ObservableCollection<ProcessMassGroup>();
            // get all output grouped up.
            var ChanceOutputs = OutputProducts
                .Where(x => x.Tags.Any(y => y.Tag == ProductionTag.Chance));
            // if no chances included, 
            if (!ChanceOutputs.Any())
            {
                var group = new ProcessMassGroup
                {
                    Part = "Output",
                    Mass = OutputProducts.Sum(x => x.Amount * manager.Products[x.ProductId].Mass)
                };
                OutputMass.Add(group);
                return;
            }
            var sharedMass = OutputProducts
                .Where(x => x.Tags.All(y => y.Tag != ProductionTag.Chance))
                .Sum(x => x.Amount * manager.Products[x.ProductId].Mass);

            var chanceGroups = ChanceOutputs
                .GroupBy(x => x.Tags.Single(y => y.Tag == ProductionTag.Chance)[0]);

            foreach (var group in chanceGroups)
            {
                var mass = group.Sum(x => x.Amount * manager.Products[x.ProductId].Mass);

                OutputMass.Add(new ProcessMassGroup
                {
                    Part = "Output Group - " + group.First().Tags
                    .Single(x => x.Tag == ProductionTag.Chance)[0],
                    Mass = mass + sharedMass,
                });
            }

            // set checkboxes
        }

        #region ListData

        public ObservableCollection<string> AvailableSkills { get; set; }

        #endregion ListData

        #region Properties

        public int ProcessId
        {
            get
            {
                return model.ProcessId;
            }
            set
            {
                if (model.ProcessId != value)
                {
                    model.ProcessId = value;
                    RaisePropertyChanged();
                }
            }
        }

        public string ProcessName
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

        public decimal MinTime
        {
            get
            {
                return model.MinTime;
            }
            set
            {
                if (model.MinTime != value)
                {
                    model.MinTime = value;
                    RaisePropertyChanged();
                }
            }
        }

        public string Skill
        {
            get
            {
                return model.Skill;
            }
            set
            {
                if (model.Skill != value)
                {
                    model.Skill = value;
                    RaisePropertyChanged();
                }
            }
        }

        public decimal MinSkill
        {
            get
            {
                return model.MinSkill;
            }
            set
            {
                if (model.MinSkill != value)
                {
                    RaisePropertyChanged();
                }
            }
        }

        public decimal MaxSkill
        {
            get
            {
                return model.MaxSkill;
            }
            set
            {
                if (model.MaxSkill != value)
                {
                    model.MaxSkill = value;
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

        public bool Failure
        {
            get
            {
                return model.Failure;
            }
            set
            {
                if (model.Failure != value)
                {
                    model.Failure = value;
                    RaisePropertyChanged();
                }
            }
        }

        public bool Consumption
        {
            get
            {
                return model.Consumption;
            }
            set
            {
                if (model.Consumption != value)
                {
                    model.Consumption = value;
                    RaisePropertyChanged();
                }
            }
        }

        public bool Maintenance
        {
            get
            {
                return model.Maintenance;
            }
            set
            {
                if (model.Maintenance != value)
                {
                    model.Maintenance = value;
                    RaisePropertyChanged();
                }
            }
        }

        public bool Use
        {
            get
            {
                return model.Use;
            }
            set
            {
                if (model.Use != value)
                {
                    model.Use = value;
                    RaisePropertyChanged();
                }
            }
        }

        public bool Chance
        {
            get
            {
                return model.Chance;
            }
            set
            {
                if (model.Chance != value)
                {
                    model.Chance = value;
                    RaisePropertyChanged();
                }
            }
        }

        public bool Crop
        {
            get
            {
                return model.Crop;
            }
            set
            {
                if (model.Crop != value)
                {
                    model.Crop = value;
                    RaisePropertyChanged();
                }
            }
        }

        public bool Mine
        {
            get
            {
                return model.Mine;
            }
            set
            {
                if (model.Mine != value)
                {
                    model.Mine = value;
                    RaisePropertyChanged();
                }
            }
        }

        public bool Extractor
        {
            get
            {
                return model.Extractor;
            }
            set
            {
                if (model.Extractor != value)
                {
                    model.Extractor = value;
                    RaisePropertyChanged();
                }
            }
        }

        public bool Tap
        {
            get
            {
                return model.Tap;
            }
            set
            {
                if (model.Tap != value)
                {
                    model.Tap = value;
                    RaisePropertyChanged();
                }
            }
        }

        public bool Refiner
        {
            get
            {
                return model.Refiner;
            }
            set
            {
                if (model.Refiner != value)
                {
                    model.Refiner = value;
                    RaisePropertyChanged();
                }
            }
        }

        public bool Sorter
        {
            get
            {
                return model.Sorter;
            }
            set
            {
                if (model.Sorter != value)
                {
                    model.Sorter = value;
                    RaisePropertyChanged();
                }
            }
        }

        public ObservableCollection<IProcessProductDTO> InputProducts
        {
            get => model.InputProducts;
            private set => model.InputProducts = value;
        }

        public IProcessProductDTO SelectedInputProduct { get; set; }

        public ObservableCollection<IProcessWantDTO> InputWants
        {
            get => model.InputWants;
            private set => model.InputWants = value;
        }

        public IProcessProductDTO SelectedInputWant { get; set; }

        public ObservableCollection<IProcessProductDTO> CapitalProducts
        {
            get => model.CapitalProducts;
            private set => model.CapitalProducts = value;
        }

        public IProcessProductDTO SelectedCapitalProduct { get; set; }

        public ObservableCollection<IProcessWantDTO> CapitalWants
        {
            get => model.CapitalWants;
            private set => model.CapitalWants = value;
        }

        public IProcessProductDTO SelectedCapitalWant { get; set; }

        public ObservableCollection<IProcessProductDTO> OutputProducts
        {
            get => model.OutputProducts;
            private set => model.OutputProducts = value;
        }

        public IProcessProductDTO SelectedOutputProduct { get; set; }

        public ObservableCollection<IProcessWantDTO> OutputWants
        {
            get => model.OutputWants;
            private set => model.OutputWants = value;
        }

        public IProcessProductDTO SelectedOutputWant { get; set; }

        public ObservableCollection<string> AvailableProducts { get; set; }

        public string SelectedProduct
        {
            get
            {
                return selectedProduct;
            }
            set
            {
                if (selectedProduct != value)
                {
                    UpdateSelectedProduct(selectedProduct, value);
                    selectedProduct = value;
                    RaisePropertyChanged();
                }
            }
        }

        public string SelectedImage
        {
            get
            {
                return model.SelectedImage;
            }
            set
            {
                if (model.SelectedImage != value)
                {
                    model.SelectedImage = value;
                    RaisePropertyChanged();
                }
            }
        }

        public BitmapSource IconImage
        {
            get
            {
                return iconImage;
            }
            set
            {
                iconImage = value;
                RaisePropertyChanged();
            }
        }

        public ObservableCollection<ProcessMassGroup> InputMass { get; set; }

        public ObservableCollection<ProcessMassGroup> OutputMass { get; set; }

        #endregion Properties

        #region Enableds

        public bool InputProductsEnabled
        {
            get
            {
                if (Failure ||
                    Consumption)
                {
                    return false;
                }
                return true;
            }
        }

        public bool InputWantsEnabled
        {
            get
            {
                if (Failure ||
                    Consumption)
                {
                    return false;
                }
                return true;
            }
        }

        public bool CapitalProductsEnabled
        {
            get
            {
                if (Failure ||
                    Consumption)
                {
                    return false;
                }
                return true;
            }
        }

        public bool CapitalWantsEnabled
        {
            get
            {
                if (Failure ||
                    Consumption)
                {
                    return false;
                }
                return true;
            }
        }

        public bool OutputProductsEnabled
        {
            get
            {
                return true;
            }
        }

        public bool OutputWantsEnabled
        {
            get
            {
                return true;
            }
        }

        public bool FailureEnabled
        {
            get
            {
                if (model.Mine ||
                    model.Consumption ||
                    model.Maintenance ||
                    model.Use ||
                    model.Extractor ||
                    model.Tap ||
                    model.Crop)
                {
                    return false;
                }
                return true;
            }
        }

        public bool ConsumptionEnabled
        {
            get
            {
                if (model.Failure ||
                    model.Mine ||
                    model.Maintenance ||
                    model.Use ||
                    model.Extractor ||
                    model.Tap ||
                    model.Crop)
                {
                    return false;
                }
                return true;
            }
        }

        public bool MaintenanceEnabled
        {
            get
            {
                if (model.Failure ||
                    model.Consumption ||
                    model.Mine ||
                    model.Use ||
                    model.Extractor ||
                    model.Tap ||
                    model.Crop)
                {
                    return false;
                }
                return true;
            }
        }

        public bool UseEnabled
        {
            get
            {
                if (model.Failure ||
                    model.Consumption ||
                    model.Maintenance ||
                    model.Mine ||
                    model.Extractor ||
                    model.Tap ||
                    model.Crop)
                {
                    return false;
                }
                return true;
            }
        }

        public bool ChanceEnabled
        {
            get
            {
                return true;
            }
        }

        public bool CropEnabled
        {
            get
            {
                if (model.Failure ||
                    model.Consumption ||
                    model.Maintenance ||
                    model.Use ||
                    model.Extractor ||
                    model.Tap ||
                    model.Mine)
                {
                    return false;
                }
                return true;
            }
        }

        public bool MineEnabled
        {
            get
            {
                if (model.Failure ||
                    model.Consumption ||
                    model.Maintenance ||
                    model.Use ||
                    model.Extractor ||
                    model.Tap ||
                    model.Crop)
                {
                    return false;
                }
                return true;
            }
        }

        public bool ExtractorEnabled
        {
            get
            {
                if (model.Failure ||
                    model.Consumption ||
                    model.Maintenance ||
                    model.Use ||
                    model.Tap ||
                    model.Crop)
                {
                    return false;
                }
                return true;
            }
        }

        public bool TapEnabled
        {
            get
            {
                if (model.Failure ||
                    model.Consumption ||
                    model.Maintenance ||
                    model.Use ||
                    model.Extractor ||
                    model.Crop)
                {
                    return false;
                }
                return true;
            }
        }

        public bool SelectedProductEnabled
        {
            get
            {
                if (Failure ||
                    Consumption ||
                    Maintenance ||
                    Use)
                {
                    return true;
                }
                return false;
            }
        }

        #endregion Enableds

        #region Commands

        public ICommand Commit
        {
            get
            {
                if (commit == null)
                {
                    commit = new RelayCommand(
                        param => CommitProcess());
                }
                return commit;
            }
        }

        public ICommand NewInputProduct
        {
            get
            {
                if (newInputProduct == null)
                {
                    newInputProduct = new RelayCommand(
                        param => NewProduct(ProcessSection.Input),
                        canExec => CanAddNewProduct(ProcessSection.Input));
                }
                return newInputProduct;
            }
        }

        public ICommand DeleteInputProduct
        {
            get
            {
                if (deleteInputProduct == null)
                {
                    deleteInputProduct = new RelayCommand(
                        param => DeleteProduct(ProcessSection.Input),
                        canEx => CanDeleteProduct(ProcessSection.Input));
                }
                return deleteInputProduct;
            }
        }

        public ICommand NewCapitalProduct
        {
            get
            {
                if (newCapitalProduct == null)
                {
                    newCapitalProduct = new RelayCommand(
                        param => NewProduct(ProcessSection.Capital),
                        canExec => CanAddNewProduct(ProcessSection.Capital));
                }
                return newCapitalProduct;
            }
        }

        public ICommand DeleteCapitalProduct
        {
            get
            {
                if (deleteCapitalProduct == null)
                {
                    deleteCapitalProduct = new RelayCommand(
                        param => DeleteProduct(ProcessSection.Capital),
                        canEx => CanDeleteProduct(ProcessSection.Capital));
                }
                return deleteCapitalProduct;
            }
        }

        public ICommand NewOutputProduct
        {
            get
            {
                if (newOutputProduct == null)
                {
                    newOutputProduct = new RelayCommand(
                        param => NewProduct(ProcessSection.Output),
                        canExec => CanAddNewProduct(ProcessSection.Output));
                }
                return newOutputProduct;
            }
        }

        public ICommand DeleteOutputProduct
        {
            get
            {
                if (deleteOutputProduct == null)
                {
                    deleteOutputProduct = new RelayCommand(
                        param => DeleteProduct(ProcessSection.Output),
                        canEx => CanDeleteProduct(ProcessSection.Output));
                }
                return deleteOutputProduct;
            }
        }

        public ICommand NewInputWant
        {
            get
            {
                if (newInputWant == null)
                {
                    newInputWant = new RelayCommand(
                        param => NewWant(ProcessSection.Input),
                        canExec => CanAddNewWant(ProcessSection.Input));
                }
                return newInputWant;
            }
        }

        public ICommand DeleteInputWant
        {
            get
            {
                if (deleteInputWant == null)
                {
                    deleteInputWant = new RelayCommand(
                        param => DeleteWant(ProcessSection.Input),
                        canEx => CanDeleteWant(ProcessSection.Input));
                }
                return deleteInputWant;
            }
        }

        public ICommand NewCapitalWant
        {
            get
            {
                if (newCapitalWant == null)
                {
                    newCapitalWant = new RelayCommand(
                        param => NewWant(ProcessSection.Capital),
                        canExec => CanAddNewWant(ProcessSection.Capital));
                }
                return newCapitalWant;
            }
        }

        public ICommand DeleteCapitalWant
        {
            get
            {
                if (deleteCapitalWant == null)
                {
                    deleteCapitalWant = new RelayCommand(
                        param => DeleteWant(ProcessSection.Capital),
                        canEx => CanDeleteWant(ProcessSection.Capital));
                }
                return deleteCapitalWant;
            }
        }

        public ICommand NewOutputWant
        {
            get
            {
                if (newOutputWant == null)
                {
                    newOutputWant = new RelayCommand(
                        param => NewWant(ProcessSection.Output),
                        canExec => CanAddNewWant(ProcessSection.Output));
                }
                return newOutputWant;
            }
        }

        public ICommand DeleteOutputWant
        {
            get
            {
                if (deleteOutputWant == null)
                {
                    deleteOutputWant = new RelayCommand(
                        param => DeleteWant(ProcessSection.Output),
                        canEx => CanDeleteWant(ProcessSection.Output));
                }
                return deleteOutputWant;
            }
        }

        public ICommand SelectImage
        {
            get
            {
                if (selectImage == null)
                {
                    selectImage = new RelayCommand(
                        param => SelectImageFile());
                }
                return selectImage;
            }
        }

        #endregion Commands

        #region CommandFunctions

        private void CommitProcess()
        {
            // Don't check for duplicates. We don't have time, and the user should
            // look out for them.

            // id machine selected.

            // check string name.
            var regex = new Regex(RegexHelper.Phrase);
            if (ProcessName == null ||
                !regex.IsMatch(ProcessName))
            {
                System.Windows.MessageBox.Show("Process name must be at least 3 characters long and can only contain letters and spaces.",
                    "Error", MessageBoxButton.OK);
                return;
            }

            // check variant name
            if (!string.IsNullOrEmpty(VariantName) &&
                !regex.IsMatch(VariantName))
            {
                System.Windows.MessageBox.Show("Variant Name must be at least 3 characters long and can only contain letters and spaces.",
                    "Error", MessageBoxButton.OK);
                return;
            }

            // minimum time
            if (MinTime < 0)
            {
                System.Windows.MessageBox.Show("Minimum Time cannot be Negative.",
                    "Error", MessageBoxButton.OK);
                return;
            }

            // Skill
            if (string.IsNullOrWhiteSpace(Skill))
            {
                System.Windows.MessageBox.Show("Skill Must be selected.",
                                    "Error", MessageBoxButton.OK);
                return;
            }

            if (MinSkill < 0 || MaxSkill < 0)
            {
                System.Windows.MessageBox.Show("Minimum and Maximum Skill cannot be negative.",
                    "Error", MessageBoxButton.OK);
                return;
            }
            if (MinSkill > MaxSkill)
            {
                System.Windows.MessageBox.Show("Minimum Skill Level must be less than Maximum Skill Level.",
                    "Error", MessageBoxButton.OK);
                return;
            }

            // wants and products should be confirmed safely up to this point and no
            // more checking should be required
            var newProc = new ProcessDTO
            {
                Id = process.Id,
                Name = ProcessName.Trim(),
                VariantName = VariantName.Trim(),
                SkillName = Skill,
                SkillId = manager.GetSkillByName(Skill).Id,
                MinimumTime = MinTime,
                SkillMinimum = MinSkill,
                SkillMaximum = MaxSkill,
                Description = Description,
                Icon = SelectedImage
            };

            // check for duplicated name
            if (manager.IsDuplicate(newProc))
            {
                System.Windows.MessageBox.Show("Name and Variant Name are Duplicates of another process, please change the name or variant name.",
                    "Error", MessageBoxButton.OK);
                return;
            }

            // if failure check it can attach to that product.
            if (Failure)
            {
                var prod = manager.Products[InputProducts.First().ProductId];

                // if product has a failure and it's not me, we have a problem.
                if(prod.Failure != null && prod.Failure.Id != newProc.Id)
                {
                    System.Windows.MessageBox.Show("Selected Product already has a failure process. Products can only have one failure process.",
                        "Error", MessageBoxButton.OK);
                    return;
                }
            }

            // inputs
            newProc.InputProducts = InputProducts.ToList();
            newProc.InputWants = InputWants.ToList();
            // capital
            newProc.CapitalProducts = CapitalProducts.ToList();
            newProc.CapitalWants = CapitalWants.ToList();
            // outputs
            newProc.OutputProducts = OutputProducts.ToList();
            newProc.OutputWants = OutputWants.ToList();

            // tags and checking for their validity
            if (Failure)
            {
                newProc.Tags.Add(ProcessTagInfo.ConstructTag(ProcessTag.Failure));

                // failure can only have one input, the product selected, and no
                // capital
                if (InputWants.Any() ||
                    CapitalProducts.Any() ||
                    CapitalWants.Any() ||
                    (InputProducts.Count() > 1) ||
                    !InputProducts.Any() ||
                    !InputProducts.Any(x => x.ProductName == SelectedProduct) ||
                    (InputProducts.First().Amount != 1))
                {
                    System.Windows.MessageBox
                        .Show("Failure Products only accept 1 input, a unit of the selected product.");
                    return;
                }
            }
            else if (Consumption)
            {
                // Consumption can only have one input, the product selected, and no
                // capital
                if (InputWants.Any() ||
                    CapitalProducts.Any() ||
                    CapitalWants.Any() ||
                    (InputProducts.Count() > 1) ||
                    !InputProducts.Any() ||
                    !InputProducts.Any(x => x.ProductName == SelectedProduct) ||
                    (InputProducts.First().Amount != 1))
                {
                    System.Windows.MessageBox
                        .Show("Consumption Processes only accept 1 input, a unit of the selected product.");
                    return;
                }
                newProc.Tags.Add(ProcessTagInfo.ConstructTag(ProcessTag.Consumption));
            }
            else if (Maintenance)
            {
                // maintenance should hold a unit of the selected product as input.
                // and an output of it as well, though it need not be 1 unit.
                if (!InputProducts.Where(x => x.ProductName == SelectedProduct &&
                                              x.Amount == 1).Any() &&
                     !OutputProducts.Where(x => x.ProductName == SelectedProduct).Any())
                {
                    System.Windows.MessageBox
                        .Show("Maintenace processes must take a unit of the selected product as input.");
                    return;
                }
                newProc.Tags.Add(ProcessTagInfo.ConstructTag(ProcessTag.Maintenance));
            }
            else if (Use)
            {
                // use must have a unit of the product as a capital good.
                if (!CapitalProducts.Where(x => x.ProductName == selectedProduct &&
                                                x.Amount == 1).Any())
                {
                    System.Windows.MessageBox
                        .Show("Use products must have a unit of the selected product as a capital product.");
                    return;
                }
                newProc.Tags.Add(ProcessTagInfo.ConstructTag(ProcessTag.Use));
            }
            else if (Crop)
            {
                newProc.Tags.Add(ProcessTagInfo.ConstructTag(ProcessTag.Crop));
            }
            else if (Mine)
            {
                newProc.Tags.Add(ProcessTagInfo.ConstructTag(ProcessTag.Mine));
            }

            if (Chance)
            {
                // ensure that there is at least 2 chance products with at least 2 different
                // possible outcomes.

                var options = OutputProducts
                    .Where(x => {
                        return x.Tags.Any(y => y.Tag == ProductionTag.Chance);
                        })
                    .ToList();

                if (options.Count() < 2)
                {
                    System.Windows.MessageBox
                        .Show("Chance process must have at least 2 possible outputs that are chances.");
                    return;
                }
                // group by chance group.
                var outcomeGroups = options
                    .GroupBy(x => x.Tags.Single(y => y.Tag == ProductionTag.Chance)[0])
                    .ToList();

                if (outcomeGroups.Count() < 2)
                {
                    System.Windows.MessageBox
                        .Show("Chance process must have at least 2 separate groups that it can output as.");
                    return;
                }
                
                newProc.AddTag(ProcessTagInfo.ConstructTag(ProcessTag.Chance));
            }

            // having gone through everything, add product connection.
            if (Failure)
            {
                var prod = manager.GetProductByFullName(selectedProduct);
            }
            if (Maintenance)
            {

            }
            if (Consumption)
            {

            }
            if (Use)
            {

            }

            System.Windows.MessageBox.Show("Changes Commited, Remember to save your changes in the list view.");
            process = newProc;

            if (Failure)
            {
                var prod = manager.Products[process.InputProducts.First().ProductId];
                ((ProductDTO)prod).Failure = process;
            }

            manager.Processes[process.Id] = process;
        }

        private void SelectImageFile()
        {
            var img = new OpenFileDialog();
            img.Filter = "Image Files|*.png;*.jpg;*.bmp";
            if (img.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                //var location = img.FileName.Replace(manager.DataFolder, "");

                SelectedImage = img.FileName;
                iconBitmap = (Bitmap)Bitmap.FromFile(SelectedImage, true);
                IconImage = BitmapConversion.BitmapToBitmapSource(iconBitmap);
            }
        }

        private bool CanAddNewProduct(ProcessSection section)
        {
            switch(section)
            {
                case ProcessSection.Input:
                    if (Failure || Consumption)
                        return false;
                    return true;
                case ProcessSection.Capital:
                    if (Failure || Consumption)
                        return false;
                    return true;
                case ProcessSection.Output:
                    return true;
            }
            return true;
        }

        private bool CanAddNewWant(ProcessSection section)
        {
            switch (section)
            {
                case ProcessSection.Input:
                    if (Failure || Consumption)
                        return false;
                    return true;
                case ProcessSection.Capital:
                    if (Failure || Consumption)
                        return false;
                    return true;
                case ProcessSection.Output:
                    return true;
            }
            return true;
        }

        private bool CanDeleteProduct(ProcessSection section)
        {
            switch (section)
            {
                case ProcessSection.Input:
                    return SelectedInputProduct != null;
                case ProcessSection.Capital:
                    return SelectedCapitalProduct != null;
                case ProcessSection.Output:
                    return SelectedOutputProduct != null;
            }
            return true;
        }

        private bool CanDeleteWant(ProcessSection section)
        {
            switch (section)
            {
                case ProcessSection.Input:
                    return SelectedInputWant != null;
                case ProcessSection.Capital:
                    return SelectedCapitalWant != null;
                case ProcessSection.Output:
                    return SelectedOutputWant != null;
            }
            return true;
        }

        private void NewProduct(ProcessSection sec)
        {
            var prod = new ProcessProductDTO();

            var win = new ProcessProductWindow(prod, sec);

            win.ShowDialog();

            if (!win.ViewModel.ValidCommited)
            { // if commit never hit, don't add it.
                return;
            }

            switch (sec)
            {
                case ProcessSection.Input:
                    InputProducts.Add(win.ViewModel.ProcessProduct);
                    break;
                case ProcessSection.Capital:
                    CapitalProducts.Add(win.ViewModel.ProcessProduct);
                    break;
                case ProcessSection.Output:
                    OutputProducts.Add(win.ViewModel.ProcessProduct);
                    break;
            }

            if (sec == ProcessSection.Input)
            {
                // there should only ever be 1 input group
                InputMass.Clear();
                InputMass.Add(new ProcessMassGroup
                {
                    Part = "Inputs",
                    Mass = InputProducts.Sum(x => x.Amount * manager.Products[x.ProductId].Mass)
                });
            }
            else if (sec == ProcessSection.Output)
            {
                // just clear it out, it'll be easier. If this becomes
                // a probelm we'll deal with it then.
                OutputMass.Clear();
                // get all output grouped up.
                var ChanceOutputs = OutputProducts
                    .Where(x => x.Tags.Any(y => y.Tag == ProductionTag.Chance));
                // if no chances included, 
                if (!ChanceOutputs.Any())
                {
                    var group = new ProcessMassGroup
                    {
                        Part = "Output",
                        Mass = OutputProducts.Sum(x => x.Amount * manager.Products[x.ProductId].Mass)
                    };
                    OutputMass.Add(group);
                    return;
                }
                var sharedMass = OutputProducts
                    .Where(x => x.Tags.All(y => y.Tag != ProductionTag.Chance))
                    .Sum(x => x.Amount * manager.Products[x.ProductId].Mass);

                var chanceGroups = ChanceOutputs
                    .GroupBy(x => x.Tags.Single(y => y.Tag == ProductionTag.Chance)[0]);

                foreach (var group in chanceGroups)
                {
                    var mass = group.Sum(x => x.Amount * manager.Products[x.ProductId].Mass);

                    OutputMass.Add(new ProcessMassGroup
                    {
                        Part = "Output Group - " + group.First().Tags
                        .Single(x => x.Tag == ProductionTag.Chance)[0],
                        Mass = mass + sharedMass,
                    });
                }
            }
        }

        public void EditProduct(ProcessProductDTO prod, ProcessSection sec)
        {
            var win = new ProcessProductWindow(prod, sec);

            win.ShowDialog();

            if (!win.ViewModel.ValidCommited)
            {
                return;
            }
            switch (sec)
            {
                case ProcessSection.Input:
                    InputProducts.Remove(prod);
                    InputProducts.Add(win.ViewModel.ProcessProduct);
                    break;
                case ProcessSection.Capital:
                    CapitalProducts.Remove(prod);
                    CapitalProducts.Add(win.ViewModel.ProcessProduct);
                    break;
                case ProcessSection.Output:
                    OutputProducts.Remove(prod);
                    OutputProducts.Add(win.ViewModel.ProcessProduct);
                    break;
            }
            if (sec == ProcessSection.Input)
            {
                // there should only ever be 1 input group
                InputMass.Clear();
                InputMass.Add(new ProcessMassGroup
                {
                    Part = "Inputs",
                    Mass = InputProducts.Sum(x => x.Amount * manager.Products[x.ProductId].Mass)
                });
            }
            else if (sec == ProcessSection.Output)
            {
                // just clear it out, it'll be easier. If this becomes
                // a probelm we'll deal with it then.
                OutputMass.Clear();
                // get all output grouped up.
                var ChanceOutputs = OutputProducts
                    .Where(x => x.Tags.Any(y => y.Tag == ProductionTag.Chance));
                // if no chances included, 
                if (!ChanceOutputs.Any())
                {
                    var group = new ProcessMassGroup
                    {
                        Part = "Output",
                        Mass = OutputProducts.Sum(x => x.Amount * manager.Products[x.ProductId].Mass)
                    };
                    OutputMass.Add(group);
                    return;
                }
                var sharedMass = OutputProducts
                    .Where(x => x.Tags.All(y => y.Tag != ProductionTag.Chance))
                    .Sum(x => x.Amount * manager.Products[x.ProductId].Mass);

                var chanceGroups = ChanceOutputs
                    .GroupBy(x => x.Tags.Single(y => y.Tag == ProductionTag.Chance)[0]);

                foreach (var group in chanceGroups)
                {
                    var mass = group.Sum(x => x.Amount * manager.Products[x.ProductId].Mass);

                    OutputMass.Add(new ProcessMassGroup
                    {
                        Part = "Output Group - " + group.First().Tags
                        .Single(x => x.Tag == ProductionTag.Chance)[0],
                        Mass = mass + sharedMass,
                    });
                }
            }
        }

        private void DeleteProduct(ProcessSection sec)
        {
            IProcessProductDTO selected = null;

            switch (sec)
            {
                case ProcessSection.Input:
                    selected = SelectedInputProduct;
                    break;
                case ProcessSection.Capital:
                    selected = SelectedCapitalProduct;
                    break;
                case ProcessSection.Output:
                    selected = SelectedOutputProduct;
                    break;
            }

            if (selected == null)
                return;

            switch (sec)
            {
                case ProcessSection.Input:
                    InputProducts.Remove(selected);
                    break;
                case ProcessSection.Capital:
                    CapitalProducts.Remove(selected);
                    break;
                case ProcessSection.Output:
                    OutputProducts.Remove(selected);
                    break;
            }
            if (sec == ProcessSection.Input)
            {
                // there should only ever be 1 input group
                InputMass.Clear();
                InputMass.Add(new ProcessMassGroup
                {
                    Part = "Inputs",
                    Mass = InputProducts.Sum(x => x.Amount * manager.Products[x.ProductId].Mass)
                });
            }
            else if (sec == ProcessSection.Output)
            {
                // just clear it out, it'll be easier. If this becomes
                // a probelm we'll deal with it then.
                OutputMass.Clear();
                // get all output grouped up.
                var ChanceOutputs = OutputProducts
                    .Where(x => x.Tags.Any(y => y.Tag == ProductionTag.Chance));
                // if no chances included, 
                if (!ChanceOutputs.Any())
                {
                    var group = new ProcessMassGroup
                    {
                        Part = "Output",
                        Mass = OutputProducts.Sum(x => x.Amount * manager.Products[x.ProductId].Mass)
                    };
                    OutputMass.Add(group);
                    return;
                }
                var sharedMass = OutputProducts
                    .Where(x => x.Tags.All(y => y.Tag != ProductionTag.Chance))
                    .Sum(x => x.Amount * manager.Products[x.ProductId].Mass);

                var chanceGroups = ChanceOutputs
                    .GroupBy(x => x.Tags.Single(y => y.Tag == ProductionTag.Chance)[0]);

                foreach (var group in chanceGroups)
                {
                    var mass = group.Sum(x => x.Amount * manager.Products[x.ProductId].Mass);

                    OutputMass.Add(new ProcessMassGroup
                    {
                        Part = "Output Group - " + group.First().Tags
                        .Single(x => x.Tag == ProductionTag.Chance)[0],
                        Mass = mass + sharedMass,
                    });
                }
            }
        }

        private void NewWant(ProcessSection sec)
        {
            var want = new ProcessWantDTO();

            var win = new ProcessWantWindow(want, sec);

            win.ShowDialog();

            if (!win.ViewModel.ValidCommited)
            {// if comit was never hit, return.
                return;
            }

            switch (sec)
            {
                case ProcessSection.Input:
                    InputWants.Add(win.ViewModel.ProcessWant);
                    break;
                case ProcessSection.Capital:
                    CapitalWants.Add(win.ViewModel.ProcessWant);
                    break;
                case ProcessSection.Output:
                    OutputWants.Add(win.ViewModel.ProcessWant);
                    break;
            }
        }

        public void EditWant(ProcessWantDTO want, ProcessSection sec)
        {
            var win = new ProcessWantWindow(want, sec);

            win.ShowDialog();

            if (!win.ViewModel.ValidCommited)
            {
                return;
            }

            switch (sec)
            {
                case ProcessSection.Input:
                    InputWants.Remove(want);
                    InputWants.Add(win.ViewModel.ProcessWant);
                    break;
                case ProcessSection.Capital:
                    CapitalWants.Remove(want);
                    CapitalWants.Add(win.ViewModel.ProcessWant);
                    break;
                case ProcessSection.Output:
                    OutputWants.Remove(want);
                    OutputWants.Add(win.ViewModel.ProcessWant);
                    break;
            }
        }

        private void DeleteWant(ProcessSection sec)
        {
            ProcessWantDTO selected = null;

            switch (sec)
            {
                case ProcessSection.Input:
                    //selected = (ProcessWant)InputWantGrid.SelectedItem;
                    break;
                case ProcessSection.Capital:
                    //selected = (ProcessWant)CapitalWantGrid.SelectedItem;
                    break;
                case ProcessSection.Output:
                    //selected = (ProcessWant)OutputWantGrid.SelectedItem;
                    break;
            }

            if (selected == null)
                return;

            switch (sec)
            {
                case ProcessSection.Input:
                    InputWants.Remove(selected);
                    break;
                case ProcessSection.Capital:
                    CapitalWants.Remove(selected);
                    break;
                case ProcessSection.Output:
                    OutputWants.Remove(selected);
                    break;
            }
        }

        #endregion CommandFunctions

        private void RefreshEnableds()
        {
            RaisePropertyChanged(nameof(FailureEnabled));
            RaisePropertyChanged(nameof(ConsumptionEnabled));
            RaisePropertyChanged(nameof(MaintenanceEnabled));
            RaisePropertyChanged(nameof(UseEnabled));
            RaisePropertyChanged(nameof(ChanceEnabled));
            RaisePropertyChanged(nameof(CropEnabled));
            RaisePropertyChanged(nameof(MineEnabled));

            RaisePropertyChanged(nameof(InputProductsEnabled));
            RaisePropertyChanged(nameof(InputWantsEnabled));
            RaisePropertyChanged(nameof(CapitalProductsEnabled));
            RaisePropertyChanged(nameof(CapitalWantsEnabled));
            RaisePropertyChanged(nameof(OutputProductsEnabled));
            RaisePropertyChanged(nameof(OutputWantsEnabled));

            RaisePropertyChanged(nameof(SelectedProductEnabled));
        }

        private void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            if (propertyName != null && !propertyName.Contains("Enabled")) 
                RefreshEnableds();

            // if one of these properties changed, add or remove products.
            if (propertyName == nameof(Failure) ||
                propertyName == nameof(Consumption) ||
                propertyName == nameof(Maintenance) ||
                propertyName == nameof(Use))
            { 
                // if input related
                if (propertyName == nameof(Failure) ||
                propertyName == nameof(Consumption) ||
                propertyName == nameof(Maintenance))
                {
                    // get if any of them are true or fals.
                    bool result = Failure || Consumption || Maintenance;

                    // if true
                    if (result)
                    {
                        // delete all existing products if not allowed.
                        if (Failure || Consumption)
                            InputProducts.Clear();

                        var newInput = new ProcessProductDTO();
                        newInput.ProductName = SelectedProduct;
                        newInput.Amount = 1;

                        InputProducts.Insert(0, newInput);
                    }
                    else // if false
                    {
                        // try to remove, may not always be there, may also be many of them.
                        var oldInput = InputProducts.Where(x => x.ProductName == SelectedProduct);

                        if (oldInput.Any())
                        {// if it is there, remove the first

                            InputProducts.Remove(oldInput.First());
                        }
                    }
                }

                // if capital related
                if (propertyName == nameof(Use))
                {
                    // if true add our selected product.
                    if (Use)
                    {
                        // clear old products that are unacceptable

                        var newCap = new ProcessProductDTO();
                        newCap.ProductName = SelectedProduct;
                        newCap.Amount = 1;

                        CapitalProducts.Insert(0, newCap);
                    }
                    else // if false
                    {
                        // try to remove, may not always be there, may also be many of them.
                        var oldCap = CapitalProducts.Where(x => x.ProductName == SelectedProduct);

                        if (oldCap.Any())
                        {// if it is there, remove the first

                            CapitalProducts.Remove(oldCap.First());
                        }
                    }
                }

                // if output Related
                if (propertyName == nameof(Maintenance))
                {
                    // if true add our selected product.
                    if (Maintenance)
                    {
                        var newOutput = new ProcessProductDTO();
                        newOutput.ProductName = SelectedProduct;
                        newOutput.Amount = 1;

                        OutputProducts.Insert(0, newOutput);
                    }
                    else // if false
                    {
                        // try to remove, may not always be there, may also be many of them.
                        var oldOutput = OutputProducts.Where(x => x.ProductName == SelectedProduct);

                        if (oldOutput.Any())
                        {// if it is there, remove the first
                            OutputProducts.Remove(oldOutput.First());
                        }
                    }
                }
            }
        }

        private void UpdateSelectedProduct(string old, string newProduct)
        {
            // if required in input
            if (Failure || Consumption || Maintenance)
            {
                // remove old
                if (old != null)
                {
                    var oldInput = InputProducts.Single(x => x.ProductName == old);
                    InputProducts.Remove(oldInput);
                }

                // add new product
                var newProd = new ProcessProductDTO();
                newProd.ProductName = newProduct;
                newProd.Amount = 1;

                InputProducts.Insert(0, newProd);
            }
            else if (Use)
            { // required as capital
                // remove old
                if (old != null)
                {
                    var oldCap = OutputProducts.Single(x => x.ProductName == old);
                    InputProducts.Remove(oldCap);
                }

                // add new
                var newCap = new ProcessProductDTO();
                newCap.ProductName = newProduct;
                newCap.Amount = 1;

                CapitalProducts.Insert(0, newCap);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
