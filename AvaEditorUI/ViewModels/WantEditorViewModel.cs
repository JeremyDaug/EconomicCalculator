using System.Collections.Generic;
using System.Reactive;
using Avalonia.Controls;
using EconomicSim.Objects;
using EconomicSim.Objects.Wants;
using MessageBox.Avalonia.Enums;
using ReactiveUI;

namespace AvaEditorUI.ViewModels;

public class WantEditorViewModel : ViewModelBase, IReactiveObject
{
    private IWant original; 
    private string _name;
    private string _description;

    public WantEditorViewModel()
    {
        original = new Want();
        _name = "";
        _description = "";
        Commit = ReactiveCommand.Create(CommitWant);
    }
    
    public WantEditorViewModel(IWant want)
    {
        original = want;
        _name = want.Name;
        _description = want.Description;
        Commit = ReactiveCommand.Create(CommitWant);
    }
    
    public ReactiveCommand<Unit, Unit> Commit { get; }

    private void CommitWant()
    {
        var errors = new List<string>();
        var dc = DataContextFactory.GetDataContext;
        // assert that we are not updating and it's not taken. 
        if (dc.Wants.ContainsKey(Name) && original.Name != Name)
            errors.Add("Want Name Already Exists.");
        
        // if errors found, get out and try again
        if (errors.Count > 0)
        {
            MessageBox.Avalonia.MessageBoxManager
                .GetMessageBoxStandardWindow("Invalid Want.",
                    "Errors found: \n" + string.Join('\n', errors),
                    ButtonEnum.Ok, Icon.Error,WindowStartupLocation.CenterScreen)
                .ShowDialog(Parent);
            return;
        }
        
        // everything's fine, add or update
        var newWant = new Want
        {
            Name = Name,
            Description = Description
        };
        
        if (dc.Wants.ContainsKey(original.Name))
        { // if update
            if (dc.Wants.Remove(original.Name))
            {
                dc.Wants.Add(newWant.Name, newWant);
            }
        }
        else
        {
            dc.Wants.Add(newWant.Name, newWant);
        }
        
        MessageBox.Avalonia.MessageBoxManager
            .GetMessageBoxStandardWindow("Saved!", "Want has been saved.",
                ButtonEnum.Ok, Icon.Info,WindowStartupLocation.CenterScreen)
            .Show();
    }

    public Window Parent { get; set; }
    
    public string Name
    {
        get => _name;
        set => this.RaiseAndSetIfChanged(ref _name, value);
    }

    public string Description
    {
        get => _description;
        set => this.RaiseAndSetIfChanged(ref _description, value);
    }
}