using System.Collections.Generic;
using System.Linq;
using AvaEditorUI.Helpers;
using EconomicSim.Objects.Products;

namespace AvaEditorUI.Models;

public class ProductEditorModel
{
    public ProductEditorModel()
    {
        ProductTags = new List<(string tag, Dictionary<string, object>? parameters)>();
        Wants = new List<Pair<string, decimal>>();
        UseProcesses = new List<string>();
        ConsumptionProcesses = new List<string>();
        MaintenanceProcesses = new List<string>();
    }

    public ProductEditorModel(Product product)
    {
        Name = product.Name;
        VariantName = product.VariantName;
        UnitName = product.UnitName;
        Quality = product.Quality;
        Mass = product.Mass;
        Bulk = product.Bulk;
        Fractional = product.Fractional;
        Icon = product.Icon;
        if (product.TechRequirement != null)
            Technology = product.TechRequirement.Name;
        ProductTags = new List<(string tag, Dictionary<string, object>? parameters)>();
        foreach (var tag in product.ProductTags)
        {
            ProductTags.Add((tag.tag.ToString(),
                tag.parameters == null ? null : new Dictionary<string, object>(tag.parameters)));
        }
        Wants = new List<Pair<string, decimal>>();
        foreach (var want in product.Wants)
            Wants.Add(new Pair<string, decimal>(want.want.Name, want.amount));
        UseProcesses = new List<string>(product.UseProcesses.Select(x => x.GetName()));
        ConsumptionProcesses = new List<string>(product.ConsumptionProcesses.Select(x => x.GetName()));
        MaintenanceProcesses = new List<string>(product.MaintenanceProcesses.Select(x => x.GetName()));
    }
    
    public string Name { get; set; } = "";
    public string VariantName { get; set; } = "";

    public string FullName
    {
        get
        {
            if (string.IsNullOrWhiteSpace(VariantName))
                return Name;
            return $"{Name}({VariantName})";
        }
    }
    
    private string TagToString(string tag, Dictionary<string, object>? parameters)
    {
        var result = tag;
        if (parameters != null)
        {
            result += " (";
            foreach (var parameter in parameters)
            {
                result += $"\n    {parameter.Key}:{parameter.Value.ToString()},";
            }

            result = result.TrimEnd(',') + ")";
        }

        return result;
    }
    public string UnitName { get; set; } = "";
    public int Quality { get; set; }
    public decimal Mass { get; set; }
    public decimal Bulk { get; set; }
    public bool Fractional { get; set; }
    public string Icon { get; set; } = "";
    public string Technology { get; set; } = "";
    
    public List<(string tag, Dictionary<string, object>? parameters)> ProductTags { get; set; }

    public string ProductTagString
    {
        get
        {
            var result = "";
            foreach (var tag in ProductTags)
            {
                result += TagToString(tag.tag, tag.parameters);
                result += "\n";
            }
            return result;
        }
    }

    public List<Pair<string, decimal>> Wants { get; set; }

    public string WantString
    {
        get
        {
            var result = "";
            foreach (var pair in Wants)
                result += $"{pair.Primary} : {pair.Secondary}\n";
            return result;
        }
    }

    public string FailureProcess { get; set; } = "";
    public List<string> UseProcesses { get; set; }

    public string UseProcessesString
    {
        get
        {
            var result = "";
            foreach (var proc in UseProcesses)
                result += $"{proc}\n";
            return result;
        }
    }
    
    public List<string> ConsumptionProcesses { get; set; }

    public string ConsumptionProcessesString
    {
        get
        {
            var result = "";
            foreach (var proc in ConsumptionProcesses)
                result += $"{proc}\n";
            return result;
        }
    }
    
    public List<string> MaintenanceProcesses { get; set; }

    public string MaintenanceProcessesString
    {
        get
        {
            var result = "";
            foreach (var proc in MaintenanceProcesses)
                result += $"{proc}\n";
            return result;
        }
    }
}