namespace EconomicSim.Objects.Processes;

public interface IProcessNode
{
    /// <summary>
    /// The process in this node.
    /// </summary>
    IProcess Process { get; }
    
    /// <summary>
    /// Processes that can feed into the Inputs of <see cref="Process"/>.
    /// </summary>
    IReadOnlyList<IProcessNode> InputProcesses { get; }
    
    /// <summary>
    /// Processes that can feed into the Capitals of <see cref="Process"/>.
    /// </summary>
    IReadOnlyList<IProcessNode> CapitalProcesses { get; }
    
    /// <summary>
    /// Processes that can feed into the Capitals of <see cref="Process"/>.
    /// </summary>
    IReadOnlyList<IProcessNode> OutputProcesses { get; }
    
    /// <summary>
    /// Does the process output one of it's own inputs?
    /// </summary>
    bool CanFeedSelf { get; }

    string Name();
}