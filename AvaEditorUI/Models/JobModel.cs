using System.Collections.Generic;
using System.Diagnostics;
using EconomicSim.Objects.Jobs;

namespace AvaEditorUI.Models;

public class JobModel
{
    public JobModel()
    {
        Processes = new List<string>();
    }

    public JobModel(Job original)
    {
        Name = original.Name;
        VariantName = original.VariantName;
        Labor = original.Labor.GetName();
        Skill = original.Skill.Name;

        Processes = new List<string>();
        foreach (var process in original.Processes)
            Processes.Add(process.GetName());
    }

    public string Name { get; set; } = "";
    public string VariantName { get; set; } = "";
    public string Labor { get; set; } = "";
    public string Skill { get; set; } = "";
    
    public List<string> Processes { get; set; }

    public string ProcessesString
    {
        get
        {
            var result = "";

            foreach (var process in Processes)
                result += process + "\n";
            
            return result.Trim();
        }
    }
    
    public string GetName()
    {
        if (!string.IsNullOrWhiteSpace(VariantName))
            return $"{Name}({VariantName})";
        return Name;
    }
}