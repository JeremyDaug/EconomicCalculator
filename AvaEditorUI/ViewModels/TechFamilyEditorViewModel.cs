using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.NetworkInformation;
using System.Reactive;
using System.Threading.Tasks;
using AvaEditorUI.Models;
using Avalonia.Controls;
using EconomicSim.Objects;
using EconomicSim.Objects.Technology;
using MessageBox.Avalonia;
using ReactiveUI;

namespace AvaEditorUI.ViewModels;

public class TechFamilyEditorViewModel : ViewModelBase
{
    private IDataContext dc = DataContextFactory.GetDataContext;
    private TechFamilyEditorModel original;
    private Window? _window;
    private string _name = "";
    private string _description = "";
    private string? _techToRemove = "";
    private string? _techToAdd = "";
    private string? _familyToRemove = "";
    private string? _familyToAdd = "";

    public TechFamilyEditorViewModel()
    {
        original = new TechFamilyEditorModel();
        TechOptions = new ObservableCollection<string>(dc.Technologies.Keys);
        FamilyOptions = new ObservableCollection<string>(dc.TechFamilies.Keys);
        Relations = new ObservableCollection<string>();
        Techs = new ObservableCollection<string>();

        AddFamily = ReactiveCommand.Create(AddFamilyToFam);
        RemoveFamily = ReactiveCommand.Create(RemoveFamilyFromFam);
        AddTech = ReactiveCommand.Create(AddTechToFam);
        RemoveTech = ReactiveCommand.Create(RemoveTechFromFam);
        Commit = ReactiveCommand.Create(CommitFamily);
    }

    public TechFamilyEditorViewModel(Window win) : this()
    {
        _window = win;
    }

    public TechFamilyEditorViewModel(TechFamilyEditorModel model, Window win) : this()
    {
        _window = win;
        original = model;
        Name = model.Name;
        Description = model.Description;
        foreach (var rel in model.Relations)
            Relations.Add(rel);
        foreach (var tech in model.Techs)
            Techs.Add(tech);
    }
    
    public ReactiveCommand<Unit,Unit> AddFamily { get; set; }
    public ReactiveCommand<Unit, Unit> RemoveFamily { get; set; }
    public ReactiveCommand<Unit, Unit> AddTech { get; set; }
    public ReactiveCommand<Unit, Unit> RemoveTech { get; set; }
    public ReactiveCommand<Unit, Task> Commit { get; set; }

    private void AddFamilyToFam()
    {
        if (FamilyToAdd == null)
            return;
        Relations.Add(FamilyToAdd);
        FamilyOptions.Remove(FamilyToAdd);
    }

    public void RemoveFamilyFromFam()
    {
        if (FamilyToRemove == null)
            return;
        FamilyOptions.Add(FamilyToRemove);
        Relations.Remove(FamilyToRemove);
    }

    public void AddTechToFam()
    {
        if (TechToAdd == null)
            return;
        Techs.Add(TechToAdd);
        TechOptions.Remove(TechToAdd);
    }

    public void RemoveTechFromFam()
    {
        if (TechToRemove == null) return;
        TechOptions.Add(TechToRemove);
        Techs.Remove(TechToRemove);
    }

    public async Task CommitFamily()
    {
        var errors = new List<string>();
        if (string.IsNullOrWhiteSpace(Name)) 
            errors.Add("Must have a Name.");

        if (errors.Any())
        {
            var error = MessageBoxManager.GetMessageBoxStandardWindow("Error!",
                "Errors Found:\n" + string.Join('\n', errors));
            await error.ShowDialog(_window);
            return;
        }

        var newFamily = new TechFamily
        {
            Name = Name,
            Description = Description
        };

        // if it already exists, remove and disconnect.
        if (dc.TechFamilies.ContainsKey(original.Name))
        {
            var old = dc.TechFamilies[original.Name];
            dc.TechFamilies.Remove(original.Name);
            foreach (var tech in old.Techs)
                tech.Families.Remove(old);
            foreach (var family in old.Relations)
                family.Relations.Remove(old);
        }
        dc.TechFamilies.Add(newFamily.Name, newFamily);
        
        // Add relations
        foreach (var fam in Relations)
        {
            newFamily.Relations.Add(dc.TechFamilies[fam]);
            dc.TechFamilies[fam].Relations.Add(newFamily);
        }
        // Add Techs
        foreach (var tech in Techs)
        {
            newFamily.Techs.Add(dc.Technologies[tech]);
            dc.Technologies[tech].Families.Add(newFamily);
        }

        var newOriginal = new TechFamilyEditorModel(newFamily);
        original = newOriginal;

        var success = MessageBoxManager
            .GetMessageBoxStandardWindow("Tech Family Committed!",
                "Tech Family has been Committed, remember to save it.");
        await success.ShowDialog(_window);
    }

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

    public string? FamilyToAdd
    {
        get => _familyToAdd;
        set => this.RaiseAndSetIfChanged(ref _familyToAdd, value);
    }

    public string? FamilyToRemove
    {
        get => _familyToRemove;
        set => this.RaiseAndSetIfChanged(ref _familyToRemove, value);
    }

    public string? TechToAdd
    {
        get => _techToAdd;
        set => this.RaiseAndSetIfChanged(ref _techToAdd, value);
    }

    public string? TechToRemove
    {
        get => _techToRemove;
        set => this.RaiseAndSetIfChanged(ref _techToRemove, value);
    }

    public ObservableCollection<string> Relations { get; set; }
    public ObservableCollection<string> Techs { get; set; }
    
    public ObservableCollection<string> TechOptions { get; set; }
    public ObservableCollection<string> FamilyOptions { get; set; }
}