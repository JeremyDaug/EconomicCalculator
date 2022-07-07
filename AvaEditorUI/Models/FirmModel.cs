using System.Collections.ObjectModel;
using AvaEditorUI.Helpers;
using EconomicSim.Objects.Firms;

namespace AvaEditorUI.Models;

public class FirmModel
{
    public FirmModel()
    {
        ChildFirms = new ObservableCollection<string>();
        Jobs = new ObservableCollection<FirmJobModel>();
        Products = new ObservableCollection<Pair<string, decimal>>();
        Resources = new ObservableCollection<Pair<string, decimal>>();
        Regions = new ObservableCollection<string>();
        Techs = new ObservableCollection<Pair<string, int>>();
    }

    public FirmModel(Firm original) : this()
    {
        Name = original.Name;
        FirmRank = original.FirmRank.ToString();
        OwnershipStructure = original.OwnershipStructure.ToString();
        ProfitStructure = original.ProfitStructure.ToString();
        if (original.Parent != null) ParentFirm = original.Parent.Name;
        foreach (var child in original.Children) ChildFirms.Add(child.Name);
        foreach (var product in original.Products)
            Products.Add(new Pair<string, decimal>(product.Key.GetName(), product.Value));
        foreach (var resource in original.Resources)
            Resources.Add(new Pair<string, decimal>(resource.Key.GetName(), resource.Value));

        HeadquarterMarket = original.HeadQuarters.Name;
        foreach (var region in original.Regions)
            Regions.Add(region.Name);
        foreach (var tech in original.Techs)
            Techs.Add(new Pair<string, int>(tech.tech.Name, tech.research));
    }

    public string Name { get; set; } = "";
    public string FirmRank { get; set; } = "";
    public string OwnershipStructure { get; set; } = "";
    public string ProfitStructure { get; set; } = "";
    public string ParentFirm { get; set; } = "";
    public ObservableCollection<string> ChildFirms { get; set; }
    public string ChildrenString
    {
        get
        {
            var result = "";
            foreach (var child in ChildFirms)
                result += $"{child}\n";
            return result;
        }
    }
    public ObservableCollection<FirmJobModel> Jobs { get; set; }
    public string JobsString
    {
        get
        {
            var result = "";
            foreach (var job in Jobs)
                result += $"{job.Job}: {job.WageType}, {job.Wage}\n";
            return result;
        }
    }
    public ObservableCollection<Pair<string, decimal>> Products { get; set; }
    public ObservableCollection<Pair<string, decimal>> Resources { get; set; }
    public string HeadquarterMarket { get; set; } = "";
    public ObservableCollection<string> Regions { get; set; }
    public string RegionsString
    {
        get
        {
            var result = "";
            foreach (var region in Regions)
                result += $"{region}\n";
            return result;
        }
    }
    public ObservableCollection<Pair<string, int>> Techs { get; set; }
}