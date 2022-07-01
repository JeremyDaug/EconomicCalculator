using System.Collections.ObjectModel;
using AvaEditorUI.Helpers;

namespace AvaEditorUI.ViewModels;

public class SpeciesEditorViewModel
{
    // TODO, return to this later. Testing only needs 1 species right now.
    public SpeciesEditorViewModel()
    {
        CultureModifiers = new ObservableCollection<Pair<string, decimal>>();
        Needs = new ObservableCollection<Triplet<string, int, decimal>>();
        Wants = new ObservableCollection<Triplet<string, int, decimal>>();
        RelatedSpecies = new ObservableCollection<string>();
        CultureOptions = new ObservableCollection<string>();
        ProductOptions = new ObservableCollection<string>();
        WantOptions = new ObservableCollection<string>();
        SpeciesOptions = new ObservableCollection<string>();
    }
    
    public string Name { get; set; }
    public string VariantName { get; set; }
    public decimal BirthRate { get; set; }
    
    public bool Unborn { get; set; }
    public bool Sterile { get; set; }
    public bool Deathless { get; set; }
    public bool Drone { get; set; }
    
    public ObservableCollection<Pair<string, decimal>> CultureModifiers { get; set; }
    public Pair<string, decimal> SelectedCulture { get; set; }
    public ObservableCollection<Triplet<string, int, decimal>> Needs { get; set; }
    public Triplet<string, int, decimal> SelectedNeed { get; set; }
    public ObservableCollection<Triplet<string, int, decimal>> Wants { get; set; }
    public Triplet<string, int, decimal> SelectedWant { get; set; }

    public ObservableCollection<string> RelatedSpecies { get; set; }
    
    public string SpeciesToAdd { get; set; }
    public string SpeciesToRemove { get; set; }
    
    public ObservableCollection<string> CultureOptions { get; set; }
    public string NewCulture { get; set; }
    public ObservableCollection<string> ProductOptions { get; set; }
    public string NewProduct { get; set; }
    public ObservableCollection<string> WantOptions { get; set; }
    public string NewWant { get; set; }
    public ObservableCollection<string> SpeciesOptions { get; set; }
    public string NewSpecies { get; set; }
}