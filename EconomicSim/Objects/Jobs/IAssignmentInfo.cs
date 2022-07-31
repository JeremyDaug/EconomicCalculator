namespace EconomicSim.Objects.Jobs;

public interface IAssignmentInfo
{
    /// <summary>
    /// The number of iterations the process is trying to run.
    /// </summary>
    decimal Iterations { get; set; }
    
    /// <summary>
    /// The progress made on a process, if one was incomplete.
    /// </summary>
    decimal Progress { get; set; }
}