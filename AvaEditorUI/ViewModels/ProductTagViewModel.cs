using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Threading.Tasks;
using AvaEditorUI.Helpers;
using Avalonia.Controls;
using EconomicSim.Objects;
using EconomicSim.Objects.Products.ProductTags;
using MessageBox.Avalonia;
using MessageBox.Avalonia.Enums;
using ReactiveUI;

namespace AvaEditorUI.ViewModels;

public class ProductTagViewModel : ViewModelBase
{
    private IDataContext dc = DataContextFactory.GetDataContext;
    private string _selectedTag = "";
    private ProductTag _tag;
    private ProductTag _original;
    private Dictionary<string, object>? _originalParams;
    private Window? _window;
    private bool _productsVisible;
    private bool _wantsVisible;
    private bool _storageVisible;
    private bool _firmsVisible;
    private bool _isSaved = false;

    public bool IsSaved
    {
        get
        {
            return _isSaved;
        }
        set
        {
            _isSaved = value;
            if (_isSaved)
            {
                CommittedLabel = "Committed!";
            }
            else
                CommittedLabel = "";
        }
    }
    private string _committedLabel;

    public ProductTagViewModel()
    {
        _window = null;
        _originalParams = new Dictionary<string, object>();
        Parameters = new ObservableCollection<Pair<string, string>>();
        Commit = ReactiveCommand.Create(CommitTag);
        AvailableTags = new ObservableCollection<string>(Enum.GetNames(typeof(ProductTag)));
        AvailableFirms = new ObservableCollection<string>(dc.Firms.Keys);
        AvailableProducts = new ObservableCollection<string>(dc.Products.Keys);
        AvailableWants = new ObservableCollection<string>(dc.Wants.Keys);
        AvailableStorageTypes = new ObservableCollection<string>(Enum.GetNames(typeof(StorageType)));
        ExportParameters = new Dictionary<string, object>();
    }

    public ProductTagViewModel(Window win) : this()
    {
        _window = win;
    }

    public ProductTagViewModel(ProductTag original, Dictionary<string, object>? originalData, Window win) : this()
    {
        _window = win;
        _original = original;
        SelectedTag = original.ToString();
        _originalParams = originalData;
        Parameters.Clear();
        if (originalData != null)
            foreach (var data in originalData)
                Parameters.Add(new Pair<string, string>(data.Key, data.Value.ToString()));
        SetHelperVisibility();
    }

    private void SetHelperVisibility()
    {
        ProductsVisible = false;
        WantsVisible = false;
        FirmsVisible = false;
        StorageVisible = false;
        switch (_tag)
        {
            case ProductTag.Luxury: 
            case ProductTag.Bargain:
                WantsVisible = true;
                return;
            case ProductTag.Claim:
                ProductsVisible = true;
                return;
            case ProductTag.Share:
                FirmsVisible = true;
                return;
            case ProductTag.Storage:
                StorageVisible = true;
                return;
            default:
                return;
        }
    }

    public ReactiveCommand<Unit, Task> Commit { get; set; }

    private async Task CommitTag()
    {
        IsSaved = false;
        // check parameters are valid
        var stringParams = new Dictionary<string, object>();
        foreach (var param in Parameters)
        {
            stringParams.Add(param.Primary, param.Secondary);
        }
        // process and get parameters
        Dictionary<string, object>? finalParams;
        try
        {
            finalParams = _tag.ProcessParameters(stringParams);
        }
        catch (Exception e)
        {
            await MessageBoxManager.GetMessageBoxStandardWindow("Invalid Parameter.",
                e.Message, ButtonEnum.Ok, Icon.Error).ShowDialog(_window);
            return;
        }
        
        // parameters check out, commit to local, then approve for saving.
        ExportParameters.Clear();
        if (finalParams != null)
            foreach (var param in finalParams)
                ExportParameters.Add(param.Key, param.Value);

        IsSaved = true;
    }
    
    public Dictionary<string, object>? ExportParameters { get; set; }

    public string SelectedTag
    {
        get => _selectedTag;
        set
        {
            IsSaved = false;
            this.RaiseAndSetIfChanged(ref _selectedTag, value);
            _tag = (ProductTag) Enum.Parse(typeof(ProductTag), _selectedTag);
            ResetParameters();
            SetHelperVisibility();
        }
    }

    private void ResetParameters()
    {
        Parameters.Clear();
        var parameters = _tag.GetParameterNames();
        foreach (var param in parameters)
            Parameters.Add(new Pair<string, string>(param, ""));
    }

    private bool ProductsVisible
    {
        get => _productsVisible;
        set
        {
            IsSaved = false;
            this.RaiseAndSetIfChanged(ref _productsVisible, value);
        }
    }

    private bool WantsVisible
    {
        get => _wantsVisible;
        set
        {
            IsSaved = false;
            this.RaiseAndSetIfChanged(ref _wantsVisible, value);
        }
    }

    private bool FirmsVisible
    {
        get => _firmsVisible;
        set
        {
            IsSaved = false;
            this.RaiseAndSetIfChanged(ref _firmsVisible, value);
        }
    }

    private bool StorageVisible
    {
        get => _storageVisible;
        set
        {
            IsSaved = false;
            this.RaiseAndSetIfChanged(ref _storageVisible, value);
        }
    }

    public ObservableCollection<string> AvailableTags { get; set; }
    public ObservableCollection<string> AvailableProducts { get; set; }
    public ObservableCollection<string> AvailableWants { get; set; }
    public ObservableCollection<string> AvailableFirms { get; set; }
    public ObservableCollection<string> AvailableStorageTypes { get; set; }

    public ObservableCollection<Pair<string, string>> Parameters { get; set; }

    public string CommittedLabel
    {
        get => _committedLabel;
        set => this.RaiseAndSetIfChanged(ref _committedLabel, value);
    }
}