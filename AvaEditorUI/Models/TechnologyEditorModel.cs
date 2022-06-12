using System.Collections.ObjectModel;
using System.Linq;
using EconomicSim.Objects.Technology;

namespace AvaEditorUI.Models;

public class TechnologyEditorModel
{
    public TechnologyEditorModel()
    {
        Families = new ObservableCollection<string>();
        Parents = new ObservableCollection<string>();
        Children = new ObservableCollection<string>();
    }

    public TechnologyEditorModel(Technology tech)
    {
        Name = tech.Name;
        Description = tech.Description;
        Category = tech.Category.ToString();
        Tier = tech.Tier;
        TechBaseCost = tech.TechCostBase;
        Families = new ObservableCollection<string>(tech.Families.Select(x => x.Name));
        Parents = new ObservableCollection<string>(tech.Parents.Select(x => x.Name));
        Children = new ObservableCollection<string>(tech.Children.Select(x => x.Name));
    }

    public string Name { get; set; } = "";
    public string Description { get; set; } = "";
    public string Category { get; set; } = "";
    public int Tier { get; set; }
    public ObservableCollection<string> Families { get; set; }

    public string FamilyString => string.Join('\n', Families);
    public ObservableCollection<string> Parents { get; set; }

    public string ParentString => string.Join('\n', Parents);

    public ObservableCollection<string> Children { get; set; }
    public string ChildrenString => string.Join('\n', Children);
    public int TechBaseCost { get; set; }
}