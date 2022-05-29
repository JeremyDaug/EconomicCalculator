using System.Collections.ObjectModel;
using System.Linq;
using EconomicSim.Objects.Technology;

namespace AvaEditorUI.Models;

public class TechFamilyEditorModel
{
    public TechFamilyEditorModel()
    {
        Relations = new ObservableCollection<string>();
        Techs = new ObservableCollection<string>();
    }

    public TechFamilyEditorModel(TechFamily fam)
    {
        Name = fam.Name;
        Description = fam.Description;
        Relations = new ObservableCollection<string>(fam.Relations.Select(x => x.Name));
        Techs = new ObservableCollection<string>(fam.Techs.Select(x => x.Name));
    }

    public string Name { get; set; } = "";
    public string Description { get; set; } = "";
    public ObservableCollection<string> Relations { get; set; }
    public string RelationsString => string.Join('\n', Relations);
    public ObservableCollection<string> Techs { get; set; }
    public string TechString => string.Join('\n', Techs);
}