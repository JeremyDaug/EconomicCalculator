using System.Text.Json.Serialization;
using EconomicSim.Objects.Products;

namespace EconomicSim.Objects.Territory;

[JsonConverter(typeof(NodeJsonConverter))]
public class Node : INode
{
    public Product Resource { get; set; }
    IProduct INode.Resource => Resource;
    public decimal Stockpile { get; set; }
    public int Depth { get; set; }
}