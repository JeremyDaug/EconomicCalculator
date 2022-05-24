using System.Collections.ObjectModel;

namespace AvaEditorUI.Models;

public class SkillEditorModel
{
    public SkillEditorModel()
    {
        Groups = new ObservableCollection<string>();
        Relations = new ObservableCollection<(string skill, decimal rate)>();
    }

    public string Name { get; set; } = "";
    public string Description { get; set; } = "";
    public ObservableCollection<string> Groups { get; set; }
    public string GroupString => string.Join('\n', Groups);
    public ObservableCollection<(string skill, decimal rate)> Relations { get; set; }
    public string RelationsString => string.Join('\n', Relations);
    public string Labor { get; set; } = "";
}