using System.Net.Sockets;
using System.Text.Json.Serialization;
using EconomicSim.Objects.Jobs;
using EconomicSim.Objects.Market;
using EconomicSim.Objects.Pops;
using EconomicSim.Objects.Processes;
using EconomicSim.Objects.Products;

namespace EconomicSim.Objects.Firms;

[JsonConverter(typeof(FirmJobJsonConverter))]
public class FirmJob : IFirmJob
{
    private List<List<IProcess>>? _assignmentsInOrder;

    public FirmJob()
    {
        Assignments = new Dictionary<IProcess, IAssignmentInfo>();
    }
    
    public Job Job { get; set; }
    IJob IFirmJob.Job => Job;
    public WageType WageType { get; set; }
    
    /// <summary>
    /// The size of the wage. If a wage product exists, it is in units of that product or
    /// market value equivalent.
    /// </summary>
    public decimal Wage { get; set; }
    /// <summary>
    /// The product of payment used as the unit of pay.
    /// if null, wage is in the 'abstract' value of the local market.
    /// </summary>
    public IProduct? WageUnit { get; set; }
    
    /// <summary>
    /// Stores progress for the processes in assignments.
    /// Investments and optional goods are consumed at the start.
    /// If a process is cancelled, it should refund the non-service inputs.
    /// Currently not saved in Json.
    /// </summary>
    public Dictionary<IProcess, IAssignmentInfo> Assignments { get; set; }

    public IReadOnlyDictionary<IProcess, decimal> ProcessesByOutputValue(IMarket market)
    {
        var result = new Dictionary<IProcess, decimal>();

        foreach (var process in Assignments.Keys)
        { // for each process
            result[process] = 0;
            foreach (var output in process.OutputProducts)
            { // get the sum of all output's values on the market.
                result[process] += market.GetMarketPrice(output.Product)
                                   * process.ProjectedProductAmount(output.Product, ProcessPartTag.Output);
            }
        }

        return result;
    }

    // Don't convert this, let pops connect to here.
    public PopGroup Pop { get; set; }
    IPopGroup IFirmJob.Pop
    {
        get => this.Pop;
        set => this.Pop = (PopGroup) value;
    }

    public decimal ConsumedTime()
    {
        decimal result = 0;
        foreach (var pair in Assignments)
        {
            // if a product takes in time
            if (pair.Key.InputProducts
                .Any(x => x.Product == DataContext.Instance.Time))
            {
                // take it and multiply by the target iterations
                result +=
                    pair.Key.InputProducts
                        .Single(x => x.Product == DataContext.Instance.Time).Amount 
                    * pair.Value.Iterations;
            }
        }

        return result;
    }

    public override string ToString()
    {
        return $"{Job.GetName()}";
    }

    /// <summary>
    /// Gets the processes in an order that is most practical.
    /// Base processes go first, those which take inputs from base go
    /// later. 
    /// </summary>
    /// <returns>
    /// A 2d array of processes, broken into steps of which can be done
    /// and in what order. Those in later steps take inputs from earlier steps.
    /// </returns>
    public List<List<IProcess>> AssignmentsInOrder()
    {
        // only do this once, save it, for later then return it if(when) called again.
        if (_assignmentsInOrder != null)
            return _assignmentsInOrder;
        
        var result = new List<List<IProcess>>();

        // get all the processes into a list to deplete
        var holder = Assignments.Keys.ToList();

        int iter = 0;
        var max = holder.Count + 1;
        while (holder.Any() && iter < max)
        {
            // go until the list is empty, or we have iterated or each item +1
            iter += 1;
            // if result is currently empty or
            // anything was added in the previous step, add a new step
            if (!result.Any() || result.Last().Any())
                result.Add(new List<IProcess>());
            foreach (var item in holder)
            {
                // for each item in the holding list
                if (!item.Node.InputProcesses
                        .Any(x => holder.Contains(x.Process)))
                {
                    // if current item takes no inputs from other processes in our holder list,
                    // add to current step
                    result.Last().Add(item);
                }
            }

            // all of those added to the current step, remove from the holder.
            foreach (var proc in result.Last())
            {
                holder.Remove(proc);
            }
        }

        if (iter == max)
        { // if we iterated out, of the loop, we most likely have a cycle,
          // add them to the last part anyway.
            result.Last().AddRange(holder);
        }

        _assignmentsInOrder = result;
        
        return result;
    }
}