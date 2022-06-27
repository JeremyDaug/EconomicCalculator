using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Threading.Tasks;
using AvaEditorUI.Models;
using Avalonia.Controls;
using EconomicSim.Objects;
using EconomicSim.Objects.Processes;
using EconomicSim.Objects.Processes.ProductionTags;
using MessageBox.Avalonia;
using ReactiveUI;

namespace AvaEditorUI.ViewModels;

public class ProcessWantEditorViewModel : ViewModelBase
{
    private Window? _window;
    private IDataContext dc = DataContextFactory.GetDataContext;
    private ProcessWantModel _original;
    private string _want = "";
    private decimal _amount;
    private bool _optional;
    private bool _optionalEnabled;
    private decimal _optionalBonus;
    private bool _consumed;
    private bool _consumedEnabled;
    private bool _fixed;
    private bool _fixedEnabled;
    private bool _pollutant;
    private bool _pollutantEnabled;
    private bool _chance;
    private bool _chanceEnabled;
    private char _chanceGroup = 'a';
    private uint _chanceWeight;
    private bool _offset;
    private bool _offsetEnabled;

    private bool _checkingEnableds;
    private bool _isCommitted;

    public ProcessWantEditorViewModel()
    {
        CommitPart = ReactiveCommand.Create(Commit);
        WantOptions = new ObservableCollection<string>(dc.Wants.Keys);
        Part = ProcessPartTag.Input.ToString();
        _original = new ProcessWantModel();
        _original.Part = ProcessPartTag.Input;
    }
    
    public ProcessWantEditorViewModel(Window window)
    {
        CommitPart = ReactiveCommand.Create(Commit);
        _window = window;
        WantOptions = new ObservableCollection<string>(dc.Wants.Keys);
        Part = ProcessPartTag.Input.ToString();
        _original = new ProcessWantModel();
        _original.Part = ProcessPartTag.Input;
    }
    
    public ProcessWantEditorViewModel(ProcessPartTag part, Window win)
    {
        CommitPart = ReactiveCommand.Create(Commit);
        _window = win;
        WantOptions = new ObservableCollection<string>(dc.Wants.Keys);
        Part = part.ToString();
        _original = new ProcessWantModel();
        _original.Part = part;
        
        RecheckEnableds();
    }

    public ProcessWantEditorViewModel(ProcessWantModel old, Window win)
    {
        CommitPart = ReactiveCommand.Create(Commit);
        _window = win;
        WantOptions = new ObservableCollection<string>(dc.Wants.Keys);
        Want = old.Want;
        Part = old.Part.ToString();
        _original = old;
        Amount = old.Amount;
        Optional = old.Tags.Any(x => x.tag == ProductionTag.Optional);
        Consumed = old.Tags.Any(x => x.tag == ProductionTag.Consumed);
        Fixed = old.Tags.Any(x => x.tag == ProductionTag.Fixed);
        Pollutant = old.Tags.Any(x => x.tag == ProductionTag.Pollutant);
        Chance = old.Tags.Any(x => x.tag == ProductionTag.Chance);
        Offset = old.Tags.Any(x => x.tag == ProductionTag.Offset);

        if (Optional)
        {
            OptionalBonus = (decimal)old.Tags.Single(x => x.tag == ProductionTag.Optional)
                .parameters["Bonus"];
        }

        if (Chance)
        {
            ChanceGroup = (char) old.Tags.Single(x => x.tag == ProductionTag.Chance)
                .parameters["Group"];
            ChanceWeight = (uint) old.Tags.Single(x => x.tag == ProductionTag.Chance)
                .parameters["Weight"];
        }
        
        RecheckEnableds();
    }
    
    public ReactiveCommand<Unit, Task> CommitPart { get; set; }

    public async Task Commit()
    {
        IsCommitted = false;
        CompleteModel = null;
        // check parameters are valid.
        var errors = new List<string>();
        if (string.IsNullOrWhiteSpace(Want))
            errors.Add("Must select a Want.");
        if (!Offset && Amount < 0)
            errors.Add("Amount must be positive.");
        else if (Offset && Amount > 0)
            errors.Add("Amount must be negative as an offset.");
        if (Optional && OptionalBonus <= 0)
            errors.Add("Optional bonus must be a positive value.");
        
        if (errors.Any())
        {
            var errorWin = MessageBoxManager.GetMessageBoxStandardWindow("Error!", "Error Found:\n" + String.Join('\n', errors));
            await errorWin.ShowDialog(_window);
            return;
        }
        
        // update original
        _original = new ProcessWantModel
        {
            Want = Want,
            Amount = Amount,
            Part = _original.Part
        };
        
        // update tags
        if (Optional)
        {
            _original.Tags.Add((ProductionTag.Optional, new Dictionary<string, object>
            {
                { "Bonus", OptionalBonus }
            }));
        }
        if (Chance)
        {
            _original.Tags.Add((ProductionTag.Chance, new Dictionary<string, object>
            {
                { "Group", ChanceGroup },
                { "Weight", ChanceWeight }
            }));
        }
        if (Consumed) _original.Tags.Add((ProductionTag.Consumed, new Dictionary<string, object>()));
        if (Fixed) _original.Tags.Add((ProductionTag.Fixed, new Dictionary<string, object>()));
        if (Pollutant) _original.Tags.Add((ProductionTag.Pollutant, new Dictionary<string, object>()));
        if (Offset) _original.Tags.Add((ProductionTag.Offset, new Dictionary<string, object>()));

        CompleteModel = _original;
        IsCommitted = true;
    }

    private void RecheckEnableds()
    {
        if (!_checkingEnableds)
        {
            _checkingEnableds = true;

            OptionalEnabled = ((_original.Part == ProcessPartTag.Input) ||
                              (_original.Part == ProcessPartTag.Capital) ) &&
                              (!Consumed);
            ConsumedEnabled = (_original.Part == ProcessPartTag.Input) &&
                              (!Optional) && false; // Wants cannot be consumed by the process
            FixedEnabled = (_original.Part == ProcessPartTag.Input) ||
                           (_original.Part == ProcessPartTag.Capital);
            
            PollutantEnabled = _original.Part == ProcessPartTag.Output;
            ChanceEnabled = _original.Part == ProcessPartTag.Output;
            OffsetEnabled = _original.Part == ProcessPartTag.Output && !Pollutant;
            
            _checkingEnableds = false;
        }
    }

    public string Want
    {
        get => _want;
        set => this.RaiseAndSetIfChanged(ref _want, value);
    }

    public decimal Amount
    {
        get => _amount;
        set => this.RaiseAndSetIfChanged(ref _amount, value);
    }

    public string Part { get; set; }

    public bool IsCommitted
    {
        get => _isCommitted;
        set => this.RaiseAndSetIfChanged(ref _isCommitted, value);
    }

    #region Optional

    public bool Optional
    {
        get => _optional;
        set
        {
            this.RaiseAndSetIfChanged(ref _optional, value);
            RecheckEnableds();
        }
    }

    public bool OptionalEnabled
    {
        get => _optionalEnabled;
        set => this.RaiseAndSetIfChanged(ref _optionalEnabled, value);
    }

    public decimal OptionalBonus
    {
        get => _optionalBonus;
        set => this.RaiseAndSetIfChanged(ref _optionalBonus, value);
    }

    #endregion

    #region Consumed

    public bool Consumed
    {
        get => _consumed;
        set
        {
            this.RaiseAndSetIfChanged(ref _consumed, value);
            RecheckEnableds();
        }
    }

    public bool ConsumedEnabled
    {
        get => _consumedEnabled;
        set => this.RaiseAndSetIfChanged(ref _consumedEnabled, value);
    }

    #endregion

    #region Fixed

    public bool Fixed
    {
        get => _fixed;
        set
        {
            this.RaiseAndSetIfChanged(ref _fixed, value);
            RecheckEnableds();
        }
    }

    public bool FixedEnabled
    {
        get => _fixedEnabled;
        set => this.RaiseAndSetIfChanged(ref _fixedEnabled, value);
    }

    #endregion

    #region Pollutant

    public bool Pollutant
    {
        get => _pollutant;
        set
        {
            this.RaiseAndSetIfChanged(ref _pollutant, value);
            RecheckEnableds();
        }
    }

    public bool PollutantEnabled
    {
        get => _pollutantEnabled;
        set => this.RaiseAndSetIfChanged(ref _pollutantEnabled, value);
    }

    #endregion

    #region Chance

    public bool Chance
    {
        get => _chance;
        set
        {
            this.RaiseAndSetIfChanged(ref _chance, value);
            RecheckEnableds();
        }
    }

    public bool ChanceEnabled
    {
        get => _chanceEnabled;
        set => this.RaiseAndSetIfChanged(ref _chanceEnabled, value);
    }

    public char ChanceGroup
    {
        get => _chanceGroup;
        set => this.RaiseAndSetIfChanged(ref _chanceGroup, value);
    }

    public uint ChanceWeight
    {
        get => _chanceWeight;
        set => this.RaiseAndSetIfChanged(ref _chanceWeight, value);
    }

    #endregion

    #region Offset

    public bool Offset
    {
        get => _offset;
        set
        {
            this.RaiseAndSetIfChanged(ref _offset, value);
            RecheckEnableds();
        }
    }

    public bool OffsetEnabled
    {
        get => _offsetEnabled;
        set => this.RaiseAndSetIfChanged(ref _offsetEnabled, value);
    }

    #endregion
    
    // skip investment, division, and Automation for now.
    
    public ObservableCollection<string> WantOptions { get; set; }
    
    public ProcessWantModel? CompleteModel { get; set; }
}