using System.Text.Json.Serialization;
using EconomicSim.Objects.Products;

namespace EconomicSim.Objects.Territory;

[JsonConverter(typeof(NodeJsonConverter))]
public interface INode
{
    IProduct Resource { get; }
    decimal Stockpile { get; }
    int Depth { get; }
}