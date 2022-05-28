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
    private Window _window;

    
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
        
        AddTech = ReactiveCommand.Create(addTech);
        AddFamily = ReactiveCommand.Create(addFamily);
        EditTech = ReactiveCommand.Create(editTech);
        EditFamily = ReactiveCommand.Create(editFamily);
        Save = ReactiveCommand.Create(save);
    }
    
    public TechListEditorViewModel(TechListEditorWindow window)
    {
        _window = window;
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

        AddTech = ReactiveCommand.Create(addTech);
        AddFamily = ReactiveCommand.Create(addFamily);
        EditTech = ReactiveCommand.Create(editTech);
        EditFamily = ReactiveCommand.Create(editFamily);
        Save = ReactiveCommand.Create(save);
    }
    
    public ReactiveCommand<Unit, Task> AddTech { get; set; }
    public ReactiveCommand<Unit, Task> AddFamily { get; set; }
    public ReactiveCommand<Unit, Task> EditTech { get; set; }
    public ReactiveCommand<Unit, Task> EditFamily { get; set; }
    public ReactiveCommand<Unit, Task> Save { get; set; }

    private async Task addTech()
    {
        
    }

    private async Task addFamily()
    {
        
    }

    private async Task editTech()
    {
        
    }

    private async Task editFamily()
    {
        
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