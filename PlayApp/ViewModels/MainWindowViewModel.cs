using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Threading.Tasks;
using Avalonia.Controls;
using EconomicSim.Objects;
using PlayApp.Helpers;
using PlayApp.Views;
using ReactiveUI;

namespace PlayApp.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private IDataContext dc = DataContextFactory.GetDataContext;
        private decimal _loadProgress;
        private string _loadingText = "";
        private bool _isLoading;
        private bool _isNotLoading = true;
        private bool _canLoadSave = false;
        private string? _selectedSave = "";
        private Window _window;

        public MainWindowViewModel()
        {
            // begin loading
            Sets = new ObservableCollection<Pair<string, bool>>();
            AvailableSaves = new ObservableCollection<string>(dc.AvailableSaves);
            
            foreach (var set in dc.AvailableSets.Select(x => Path.GetFileName(x)))
                Sets.Add(new Pair<string, bool>(set, set == "Common"));

            LoadData = ReactiveCommand.Create(_loadData);
            SelectSave = ReactiveCommand.Create(_selectSave);
        }

        public MainWindowViewModel(Window window) : this()
        {
            _window = window;
        }
        
        public ReactiveCommand<Unit, Task> LoadData { get; set; }
        public ReactiveCommand<Unit, Task> SelectSave { get; set; }

        private async Task _selectSave()
        {
            try
            {
                // try loading the save first
                if (SelectedSave == null) return;
                dc.LoadSave(SelectedSave);
                // succeeded on save, load into view selection
                var newWin = new GameModeSelectionWindow();
                newWin.Show();
                _window.Close();
            }
            catch (Exception e)
            {
                LoadingText = e.Message;
            }
        }

        private void CheckCanLoadSave()
        {
            CanLoadSave = dc.Sets.Any() && !string.IsNullOrWhiteSpace(SelectedSave);
        }
        
        private async Task _loadData()
        {
            try
            {
                if (!IsLoading)
                {
                    dc.ClearData();
                    IsLoading = true;
                    var progressIndicator = new Progress<(decimal, string)>();
                    progressIndicator.ProgressChanged += (sender, data) =>
                    {
                        LoadProgress = data.Item1;
                        LoadingText = data.Item2;
                    };
                    await dc.LoadData(Sets.Where(x => x.Secondary)
                        .Select(x => x.Primary), progressIndicator);
                }
            }
            catch (Exception e)
            {
                LoadingText = e.Message;
            }
            finally
            {
                IsLoading = false;
                CheckCanLoadSave();
            }
        }
        
        public ObservableCollection<Pair<string, bool>> Sets { get; set; }

        public ObservableCollection<string> AvailableSaves { get; set; }

        public bool CanLoadSave
        {
            get => _canLoadSave;
            set => this.RaiseAndSetIfChanged(ref _canLoadSave, value);
        }

        public string? SelectedSave
        {
            get => _selectedSave;
            set
            {
                this.RaiseAndSetIfChanged(ref _selectedSave, value);
                CheckCanLoadSave();
            }
        }

        public bool IsNotLoading
        {
            get => _isNotLoading;
            set => this.RaiseAndSetIfChanged(ref _isNotLoading, value);
        }
        
        public bool IsLoading
        {
            get => _isLoading;
            set
            {
                this.RaiseAndSetIfChanged(ref _isLoading, value);
                IsNotLoading = !_isLoading;
            }
        }

        public decimal LoadProgress
        {
            get => _loadProgress;
            set => this.RaiseAndSetIfChanged(ref _loadProgress, value);
        }

        public string LoadingText
        {
            get => _loadingText;
            set => this.RaiseAndSetIfChanged(ref _loadingText, value);
        }
    }
}