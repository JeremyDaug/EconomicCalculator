namespace EconomicSim.Objects.Processes;

public class ProcessNode : IProcessNode
{
    private readonly List<IProcessNode> _inputProcesses;
    private readonly List<IProcessNode> _capitalProcesses;
    private readonly List<IProcessNode> _outputProcesses;

    public ProcessNode(IProcess process)
    {
        Process = process;
        if (process.Node != null)
            throw new ArgumentException($"Process '{process.GetName()}' already has a node.");
        Process.Node = this;
        
        
        _inputProcesses = new List<IProcessNode>();
        _capitalProcesses = new List<IProcessNode>();
        _outputProcesses = new List<IProcessNode>();
        
        // if any input product is also in the output (checks both ways inherently)
        // or if any want it outputs is also accepted as an input.
        CanFeedSelf = process
                          .InputProducts.Any(x =>
                              process.OutputProducts
                                  .Any(y => y.Product == x.Product)) ||
                      process
                          .InputWants.Any(x =>
                              process.OutputWants
                                  .Any(y => y.Want == x.Want));
        
        // automatically connect it to other nodes while we're at it.
        // Existing nodes should be in the DataContext already.
        foreach (var node in DataContext.Instance.ProcessNodes.Values)
        {
            // if our outputs are inputs for another process, connect them.
            if (node.Process.InputProducts
                    .Select(x => x.Product)
                    .Intersect(Process.OutputProducts.Select(x => x.Product))
                    .Any() ||
                node.Process.InputWants
                    .Select(x => x.Want)
                    .Intersect(Process.OutputWants.Select(x => x.Want))
                    .Any())
            {
                _outputProcesses.Add(node);
                ((ProcessNode)node)._inputProcesses.Add(node);
            }
            // if our inputs are outputs of another process, connect them.
            if (node.Process.OutputProducts
                    .Select(x => x.Product)
                    .Intersect(Process.InputProducts.Select(x => x.Product))
                    .Any() ||
                node.Process.OutputWants
                    .Select(x => x.Want)
                    .Intersect(Process.InputWants.Select(x => x.Want))
                    .Any())
            {
                _inputProcesses.Add(node);
                ((ProcessNode)node)._outputProcesses.Add(node);
            }
            // if our capitals are outputs of another process, connect them.
            if (node.Process.OutputProducts
                    .Select(x => x.Product)
                    .Intersect(Process.CapitalProducts.Select(x => x.Product))
                    .Any() ||
                node.Process.OutputWants
                    .Select(x => x.Want)
                    .Intersect(Process.CapitalWants.Select(x => x.Want))
                    .Any())
            {
                _capitalProcesses.Add(node);
                ((ProcessNode)node)._inputProcesses.Add(node);
            }
        }
    }
    
    /// <summary>
    /// The process in this node.
    /// </summary>
    public IProcess Process { get; }

    /// <summary>
    /// Processes that can feed into the Inputs of <see cref="Process"/>.
    /// </summary>
    public IReadOnlyList<IProcessNode> InputProcesses => _inputProcesses;

    /// <summary>
    /// Processes that can feed into the Capitals of <see cref="Process"/>.
    /// </summary>
    public IReadOnlyList<IProcessNode> CapitalProcesses => _capitalProcesses;

    /// <summary>
    /// Processes that can feed into the Capitals of <see cref="Process"/>.
    /// </summary>
    public IReadOnlyList<IProcessNode> OutputProcesses => _outputProcesses;

    /// <summary>
    /// Does the process output one of it's own inputs?
    /// </summary>
    public bool CanFeedSelf { get; }

    public string Name()
    {
        return Process.GetName();
    }
    
    public override string ToString()
    {
        return Process.ToString();
    }
}