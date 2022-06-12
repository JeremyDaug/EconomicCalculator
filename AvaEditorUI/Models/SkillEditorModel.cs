using System.Collections.ObjectModel;
using EconomicSim.Objects.Skills;

namespace AvaEditorUI.Models;

public class SkillEditorModel
{
    public SkillEditorModel()
    {
        Groups = new ObservableCollection<string>();
        Relations = new ObservableCollection<(string skill, decimal rate)>();
    }

    public SkillEditorModel(Skill skill) : this()
    {
        Name = skill.Name;
        Description = skill.Description;
        Labor = skill.Labor.GetName();
        foreach (var group in skill.Groups)
            Groups.Add(group.Name);
        foreach (var rel in skill.Relations)
            Relations.Add((rel.relation.Name, rel.rate));
    }

    public string Name { get; set; } = "";
    public string Description { get; set; } = "";
    public ObservableCollection<string> Groups { get; set; }
    public string GroupString => string.Join('\n', Groups);
    public ObservableCollection<(string skill, decimal rate)> Relations { get; set; }
    public string RelationsString => string.Join('\n', Relations);
    public string Labor { get; set; } = "";
}