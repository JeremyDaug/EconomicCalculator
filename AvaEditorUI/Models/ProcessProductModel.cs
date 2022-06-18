using System.Collections.Generic;
using System.Linq;
using EconomicSim.Objects.Processes;
using EconomicSim.Objects.Processes.ProductionTags;

namespace AvaEditorUI.Models;

public class ProcessProductModel
{
    public ProcessProductModel()
    {
        Product = "";
        Tags = new List<(ProductionTag tag, Dictionary<string, object> parameters)>();
    }
    
    public ProcessProductModel(IProcessProduct part)
    {
        Product = part.Product.GetName();
        Amount = part.Amount;
        Part = part.Part;
        Tags = new List<(ProductionTag tag, Dictionary<string, object> parameters)>();
        foreach (var tag in part.TagData)
            Tags.Add((tag.tag, tag.parameters
                .ToDictionary(x => x.Key, y => y.Value)));
    }
    
    public string Product { get; set; }
    public decimal Amount { get; set; }
    public ProcessPartTag Part { get; set; }
    public string PartString => Part.ToString();
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
        return $"{Product} : {Amount}";
    }
}