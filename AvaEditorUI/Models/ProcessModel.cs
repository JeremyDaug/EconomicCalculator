using System.Collections.Generic;
using Avalonia.Input.TextInput;
using EconomicSim.Objects.Processes;
using EconomicSim.Objects.Processes.ProcessTags;

namespace AvaEditorUI.Models;

public class ProcessModel
{
    public ProcessModel()
    {
        InputProducts = new List<ProcessProductModel>();
        CapitalProducts = new List<ProcessProductModel>();
        OutputProducts = new List<ProcessProductModel>();
        InputWants = new List<ProcessWantModel>();
        CapitalWants = new List<ProcessWantModel>();
        OutputWants = new List<ProcessWantModel>();
        ProcessTags = new List<ProcessTag>();
    }

    public ProcessModel(IProcess process)
    {
        FullName = process.GetName();
        Name = process.Name;
        VariantName = process.VariantName;
        Description = process.Description;
        MinimumTime = process.MinimumTime;
        if (process.Skill != null)
            Skill = process.Skill.Name;
        SkillMin = process.SkillMinimum;
        SkillMax = process.SkillMaximum;
        if (process.TechRequirement != null)
            TechRequirement = process.TechRequirement.Name;

        InputProducts = new List<ProcessProductModel>();
        CapitalProducts = new List<ProcessProductModel>();
        OutputProducts = new List<ProcessProductModel>();
        InputWants = new List<ProcessWantModel>();
        CapitalWants = new List<ProcessWantModel>();
        OutputWants = new List<ProcessWantModel>();
        ProcessTags = new List<ProcessTag>();
        
        foreach (var input in process.InputProducts)
            InputProducts.Add(new ProcessProductModel(input));
        foreach (var input in process.InputWants)
            InputWants.Add(new ProcessWantModel(input));
        foreach (var capital in process.CapitalProducts)
            CapitalProducts.Add(new ProcessProductModel(capital));
        foreach (var capital in process.CapitalWants)
            CapitalWants.Add(new ProcessWantModel(capital));
        foreach (var output in process.OutputProducts)
            OutputProducts.Add(new ProcessProductModel(output));
        foreach (var output in process.OutputWants)
            OutputWants.Add(new ProcessWantModel(output));
        foreach (var tag in process.ProcessTags.Keys)
            ProcessTags.Add(tag);

        // Get Selected Product if its a FUMC
        if (process.ProcessTags.ContainsKey(ProcessTag.Failure))
            SelectedProduct = process.ProcessTags[ProcessTag.Failure]["Product"].ToString();
        if (process.ProcessTags.ContainsKey(ProcessTag.Use) )
            SelectedProduct = process.ProcessTags[ProcessTag.Use]["Product"].ToString();
        if (process.ProcessTags.ContainsKey(ProcessTag.Maintenance) )
            SelectedProduct = process.ProcessTags[ProcessTag.Maintenance]["Product"].ToString();
        if (process.ProcessTags.ContainsKey(ProcessTag.Consumption))
            SelectedProduct = process.ProcessTags[ProcessTag.Consumption]["Product"].ToString();
    }

    public string FullName { get; set; } = "";
    public string Name { get; set; } = "";
    public string VariantName { get; set; } = "";
    public string Description { get; set; } = "";
    public decimal MinimumTime { get; set; }
    
    // products
    public List<ProcessProductModel> InputProducts { get; set; }
    public List<ProcessProductModel> CapitalProducts { get; set; }
    public List<ProcessProductModel> OutputProducts { get; set; }
    
    // wants
    public List<ProcessWantModel> InputWants { get; set; }
    public List<ProcessWantModel> CapitalWants { get; set; }
    public List<ProcessWantModel> OutputWants { get; set; }
    
    // TODO Come back and replace with a more robust system if needed. 
    public List<ProcessTag> ProcessTags { get; set; }
    public string SelectedProduct { get; set; }
    
    public string Skill { get; set; } = "";
    public decimal SkillMin { get; set; }
    public decimal SkillMax { get; set; }

    public string TechRequirement { get; set; } = "";
}