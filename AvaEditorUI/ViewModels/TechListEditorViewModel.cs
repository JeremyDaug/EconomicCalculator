using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Threading.Tasks;
using AvaEditorUI.Models;
using AvaEditorUI.Views;
using Avalonia.Controls;
using EconomicSim.Objects;
using ReactiveUI;

namespace AvaEditorUI.ViewModels;

public class TechListEditorViewModel : ViewModelBase
{
    private IDataContext dc = DataContextFactory.GetDataContext;
    private TechnologyEditorModel? _selectedTech;
    private TechFamilyEditorModel? _selectedFamily;
    private Window? _window;
    
    public TechListEditorViewModel()
    {
        _window = null;
        Techs = new ObservableCollection<TechnologyEditorModel>();
        TechFamilies = new ObservableCollection<TechFamilyEditorModel>();
        foreach (var tech in dc.Technologies.Values)
        {
            Techs.Add(new TechnologyEditorModel
            {
                Category = tech.Category.ToString(),
                Children = new ObservableCollection<string>(tech.Children.Select(x => x.Name)),
                Parents = new ObservableCollection<string>(tech.Parents.Select(x => x.Name)),
                Description = tech.Description,
                Families = new ObservableCollection<string>(tech.Families.Select(x => x.Name)),
                Name = tech.Name,
                TechBaseCost = tech.TechCostBase
            });
        }

        foreach (var fam in dc.TechFamilies.Values)
        {
            TechFamilies.Add(new TechFamilyEditorModel
            {
                Name = fam.Name,
                Description = fam.Description,
                Relations = new ObservableCollection<string>(fam.Relations.Select(x => x.Name)),
                Techs = new ObservableCollection<string>(fam.Techs.Select(x => x.Name))
            });
        }
        
        NewTech = ReactiveCommand.Create(AddTech);
        NewFamily = ReactiveCommand.Create(AddFamily);
        EditTechnology = ReactiveCommand.Create(EditTech);
        EditTechFamily = ReactiveCommand.Create(EditFamily);
        SaveTechs = ReactiveCommand.Create(save);
    }
    
    public TechListEditorViewModel(TechListEditorWindow window)
    {
        _window = window;
        Techs = new ObservableCollection<TechnologyEditorModel>();
        TechFamilies = new ObservableCollection<TechFamilyEditorModel>();
        foreach (var tech in dc.Technologies.Values)
        {
            Techs.Add(new TechnologyEditorModel(tech));
        }

        foreach (var fam in dc.TechFamilies.Values)
        {
            TechFamilies.Add(new TechFamilyEditorModel(fam));
        }

        NewTech = ReactiveCommand.Create(AddTech);
        NewFamily = ReactiveCommand.Create(AddFamily);
        EditTechnology = ReactiveCommand.Create(EditTech);
        EditTechFamily = ReactiveCommand.Create(EditFamily);
        SaveTechs = ReactiveCommand.Create(save);
    }
    
    public ReactiveCommand<Unit, Task> NewTech { get; set; }
    public ReactiveCommand<Unit, Task> NewFamily { get; set; }
    public ReactiveCommand<Unit, Task> EditTechnology { get; set; }
    public ReactiveCommand<Unit, Task> EditTechFamily { get; set; }
    public ReactiveCommand<Unit, Task> SaveTechs { get; set; }

    private async Task AddTech()
    {
        var win = new TechnologyEditorWindow();
        await win.ShowDialog(_window);
        Techs.Clear();
        TechFamilies.Clear();
        foreach (var tech in dc.Technologies.Values)
            Techs.Add(new TechnologyEditorModel(tech));
        
        foreach (var fam in dc.TechFamilies.Values)
            TechFamilies.Add(new TechFamilyEditorModel(fam));
    }

    private async Task AddFamily()
    {
        Techs.Clear();
        TechFamilies.Clear();
        foreach (var tech in dc.Technologies.Values)
            Techs.Add(new TechnologyEditorModel(tech));
        
        foreach (var fam in dc.TechFamilies.Values)
            TechFamilies.Add(new TechFamilyEditorModel(fam));
    }

    private async Task EditTech()
    {
        if (SelectedTech == null)
            return;
        var win = new TechnologyEditorWindow(SelectedTech);
        await win.ShowDialog(_window);
        Techs.Clear();
        TechFamilies.Clear();
        foreach (var tech in dc.Technologies.Values)
            Techs.Add(new TechnologyEditorModel(tech));
        
        foreach (var fam in dc.TechFamilies.Values)
            TechFamilies.Add(new TechFamilyEditorModel(fam));
    }

    private async Task EditFamily()
    {
        Techs.Clear();
        TechFamilies.Clear();
        foreach (var tech in dc.Technologies.Values)
            Techs.Add(new TechnologyEditorModel(tech));
        
        foreach (var fam in dc.TechFamilies.Values)
            TechFamilies.Add(new TechFamilyEditorModel(fam));
    }

    private async Task save()
    {
        
    }
    
    public TechnologyEditorModel? SelectedTech
    {
        get => _selectedTech;
        set => this.RaiseAndSetIfChanged(ref _selectedTech, value);
    }

    public TechFamilyEditorModel? SelectedFamily
    {
        get => _selectedFamily;
        set => this.RaiseAndSetIfChanged(ref _selectedFamily, value);
    }

    public ObservableCollection<TechnologyEditorModel> Techs { get; set; }
    public ObservableCollection<TechFamilyEditorModel> TechFamilies { get; set; }
}