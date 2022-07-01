using System.Collections.ObjectModel;
using System.Linq;
using AvaEditorUI.Helpers;
using EconomicSim.Objects.Pops.Species;

namespace AvaEditorUI.Models;

public class SpeciesModel
{
    public SpeciesModel()
    {
        CultureModifiers = new ObservableCollection<Pair<string, decimal>>();
        Needs = new ObservableCollection<Triplet<string, int, decimal>>();
        Wants = new ObservableCollection<Triplet<string, int, decimal>>();
        RelatedSpecies = new ObservableCollection<string>();
    }

    public SpeciesModel(Species original) : this()
    {
        Name = original.Name;
        VariantName = original.VariantName;
        BirthRate = original.BirthRate;

        Unborn = original.Tags.Any(x => x.Tag == SpeciesTag.Unborn);
        Sterile = original.Tags.Any(x => x.Tag == SpeciesTag.Sterile);
        Deathless = original.Tags.Any(x => x.Tag == SpeciesTag.Deathless);
        Drone = original.Tags.Any(x => x.Tag == SpeciesTag.Drone);

        foreach (var cultureModifier in original.Tags
                     .Where(x => x.Tag == SpeciesTag.CultureModifier))
        {
            CultureModifiers.Add(new Pair<string, decimal>(cultureModifier["Culture"].ToString(), (decimal)cultureModifier["Attraction"]));
        }
    }

    public string Name { get; set; } = "";
    public string VariantName { get; set; } = "";
    public decimal BirthRate { get; set; }
    
    public bool Unborn { get; set; }
    public bool Sterile { get; set; }
    public bool Deathless { get; set; }
    public bool Drone { get; set; }
    
    public ObservableCollection<Pair<string, decimal>> CultureModifiers { get; set; }
    public ObservableCollection<Triplet<string, int, decimal>> Needs { get; set; }
    public ObservableCollection<Triplet<string, int, decimal>> Wants { get; set; }
    
    public ObservableCollection<string> RelatedSpecies { get; set; }
    
    public string GetName()
    {
        if (!string.IsNullOrWhiteSpace(VariantName))
            return $"{Name}({VariantName})";
        return Name;
    }
}