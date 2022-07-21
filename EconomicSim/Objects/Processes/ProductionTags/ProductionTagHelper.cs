namespace EconomicSim.Objects.Processes.ProductionTags;

public static class ProductionTagHelper
{
    public static Dictionary<string, object> ProcessTagData(ProductionTag tag, Dictionary<string, string> data)
    {
        Dictionary<string, object> result = new Dictionary<string, object>();

        switch (tag)
        {
            case ProductionTag.Optional:
                result["Bonus"] = decimal.Parse(data["Bonus"]);
                break;
            case ProductionTag.Chance:
                result["Group"] = char.Parse(data["Group"]);
                result["Weight"] = uint.Parse(data["Weight"]);
                break;
            case ProductionTag.Investment:
                result["Days"] = int.Parse(data["Days"]);
                break;
        }
            
        return result;
    }
}