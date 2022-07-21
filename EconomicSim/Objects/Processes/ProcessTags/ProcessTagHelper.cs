namespace EconomicSim.Objects.Processes.ProcessTags;

public static class ProcessTagHelper
{
    /// <summary>
    /// Processes a given tag and dictionary of strings to strings
    /// to the appropriate data kinds and data types.
    /// </summary>
    /// <param name="tag"></param>
    /// <param name="data"></param>
    /// <returns></returns>
    public static Dictionary<string, object> ProcessTags(ProcessTag tag, 
        Dictionary<string, string> data)
    {
        var result = new Dictionary<string, object>();

        switch (tag)
        {
            case ProcessTag.Consumption:
            case ProcessTag.Use: 
            case ProcessTag.Failure:
            case ProcessTag.Maintenance:
                result["Product"] = DataContext.Instance.Products[data["Product"]];
                break;
        }
        
        return result;
    }
}