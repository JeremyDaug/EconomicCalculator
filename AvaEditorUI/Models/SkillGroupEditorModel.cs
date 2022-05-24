using System.Collections.ObjectModel;

namespace AvaEditorUI.Models;

public class SkillGroupEditorModel
{
    public SkillGroupEditorModel()
    {
        Name = "";
        Description = "";
        Default = 0;
        Skills = new ObservableCollection<string>();
    }

    public string Name { get; set; }
    public string Description { get; set; }
    public decimal Default { get; set; }
    public ObservableCollection<string> Skills { get; set; }
    public string SkillsString => string.Join('\n', Skills);
}