using System.Collections.Generic;
using System.Linq;
using EconomicSim.Objects.Processes;
using EconomicSim.Objects.Processes.ProductionTags;
using ReactiveUI;

namespace AvaEditorUI.Models;

public class ProcessWantModel
{
    public ProcessWantModel()
    {
        Want = "";
        Tags = new List<(ProductionTag tag, Dictionary<string, object> parameters)>();
    }
    
    public ProcessWantModel(IProcessWant part)
    {
        Want = part.Want.Name;
        Amount = part.Amount;
        Part = part.Part;
        Tags = new List<(ProductionTag tag, Dictionary<string, object> parameters)>();
        foreach (var tag in part.TagData)
            Tags.Add((tag.tag, tag.parameters
                .ToDictionary(x => x.Key, y => y.Value)));
    }
    
    public string Want { get; set; }
    public decimal Amount { get; set; }
    public ProcessPartTag Part { get; set; }
    public List<(ProductionTag tag, Dictionary<string, object> parameters)> Tags { get; set; }

    public string TagString
    {
        get
        {
            var result = "";
            foreach (var tag in Tags)
            {
                result += $"{tag.tag}\n";
                foreach (var param in tag.parameters)
                    result += $"\t{param.Key}:{param.Value}\n";
            }

            return result;
        }
    }
    
    public override string ToString()
    {
        return $"{Want} : {Amount}";
    }
}