using System.Collections.ObjectModel;
using System.Linq;
using EconomicSim.Objects.Skills;

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

    public SkillGroupEditorModel(SkillGroup group)
    {
        Name = group.Name;
        Description = group.Description;
        Default = 0;
        Skills = new ObservableCollection<string>(group.Skills.Select(x => x.Name));
    }

    public string Name { get; set; }
    public string Description { get; set; }
    public decimal Default { get; set; }
    public ObservableCollection<string> Skills { get; set; }
    public string SkillsString => string.Join('\n', Skills);
}