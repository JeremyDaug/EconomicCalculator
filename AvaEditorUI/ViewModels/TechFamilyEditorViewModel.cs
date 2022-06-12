using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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
    private TechFamilyEditorModel _original;
    private Window? _window;
    private string _name = "";
    private string _description = "";
    private string? _techToRemove = "";
    private string? _techToAdd = "";
    private string? _familyToRemove = "";
    private string? _familyToAdd = "";

    public TechFamilyEditorViewModel()
    {
        _original = new TechFamilyEditorModel();
        Relations = new ObservableCollection<string>();
        Techs = new ObservableCollection<string>();
        TechOptions = new ObservableCollection<string>();
        FamilyOptions = new ObservableCollection<string>();
        AddFamily = ReactiveCommand.Create(AddFamilyToFam);
        RemoveFamily = ReactiveCommand.Create(RemoveFamilyFromFam);
        AddTech = ReactiveCommand.Create(AddTechToFam);
        RemoveTech = ReactiveCommand.Create(RemoveTechFromFam);
        Commit = ReactiveCommand.Create(CommitFamily);
    }

    public TechFamilyEditorViewModel(Window win) : this()
    {
        _window = win;
        TechOptions = new ObservableCollection<string>(dc.Technologies.Keys);
        FamilyOptions = new ObservableCollection<string>(dc.TechFamilies.Keys);
    }

    public TechFamilyEditorViewModel(TechFamilyEditorModel model, Window win) : this()
    {
        _window = win;
        _original = model;
        Name = model.Name;
        Description = model.Description;
        foreach (var rel in model.Relations)
            Relations.Add(rel);
        foreach (var tech in model.Techs)
            Techs.Add(tech);
        TechOptions = new ObservableCollection<string>(dc.Technologies.Keys
            .Where(x => !Techs.Contains(x)));
        FamilyOptions = new ObservableCollection<string>(dc.TechFamilies.Keys
            .Where(x => x != Name)
            .Where(x => !Relations.Contains(x)));
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

        // if updating
        if (dc.TechFamilies.ContainsKey(_original.Name))
        {
            var oldFam = dc.TechFamilies[_original.Name];
            // update easy data
            oldFam.Name = Name;
            oldFam.Description = Description;
            // if name changed, update
            if (_original.Name != Name)
            {
                dc.TechFamilies.Remove(_original.Name);
                dc.TechFamilies.Add(Name, oldFam);
            }
            
            // remove disconnected families
            var oldRelations = oldFam.Relations.ToList();
            foreach (var fam in oldRelations)
            {
                if (!Relations.Contains(fam.Name))
                {
                    oldFam.Relations.Remove(fam);
                    fam.Relations.Remove(oldFam);
                }
            }
            // add new family relations
            foreach (var newFam in Relations)
            {
                if (oldFam.Relations.Any(x => x.Name == newFam))
                    continue;
                var otherFam = dc.TechFamilies[newFam];
                oldFam.Relations.Add(otherFam);
                otherFam.Relations.Add(oldFam);
            }
            //remove old techs
            var oldTechs = oldFam.Techs.ToList();
            foreach (var tech in oldTechs)
            {
                if (!Techs.Contains(tech.Name))
                {
                    oldFam.Techs.Remove(tech);
                    tech.Families.Remove(oldFam);
                }
            }
            // add new techs
            foreach (var newTech in Techs)
            {
                if (oldFam.Techs.Any(x => x.Name == newTech))
                    continue;
                var otherTech = dc.Technologies[newTech];
                oldFam.Techs.Add(otherTech);
                otherTech.Families.Add(oldFam);
            }
        }
        else // if new family
        {
            var newFamily = new TechFamily
            {
                Name = Name,
                Description = Description
            };

            foreach (var tech in Techs)
            {
                // add tech
                var otherTech = dc.Technologies[tech];
                newFamily.Techs.Add(otherTech);
                otherTech.Families.Add(newFamily);
            }

            foreach (var rel in Relations)
            {// add families
                var otherFam = dc.TechFamilies[rel];
                newFamily.Relations.Add(otherFam);
                otherFam.Relations.Add(newFamily);
            }

            dc.TechFamilies[Name] = newFamily;
        }

        _original = new TechFamilyEditorModel(dc.TechFamilies[Name]);

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