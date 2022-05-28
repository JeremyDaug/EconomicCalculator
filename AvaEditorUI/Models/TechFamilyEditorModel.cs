using System.Collections.ObjectModel;

namespace AvaEditorUI.Models;

public class TechFamilyEditorModel
{
    public TechFamilyEditorModel()
    {
        Relations = new ObservableCollection<string>();
        Techs = new ObservableCollection<string>();
    }

    public string Name { get; set; } = "";
    public string Description { get; set; } = "";
    public ObservableCollection<string> Relations { get; set; }
    public string RelationsString => string.Join('\n', Relations);
    public ObservableCollection<string> Techs { get; set; }
    public string TechString => string.Join('\n', Techs);
}