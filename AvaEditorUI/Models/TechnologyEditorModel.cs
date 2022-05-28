using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reactive.Linq;

namespace AvaEditorUI.Models;

public class TechnologyEditorModel
{
    public TechnologyEditorModel()
    {
        Families = new ObservableCollection<string>();
        Parents = new ObservableCollection<string>();
        Children = new ObservableCollection<string>();
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