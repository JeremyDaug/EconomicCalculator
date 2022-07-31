namespace EconomicSim.Objects.Jobs;

public class AssignmentInfo : IAssignmentInfo
{
    public AssignmentInfo(decimal iterations, decimal progress = 0)
    {
        Iterations = iterations;
        Progress = progress;
    }
    
    /// <summary>
    /// The number of iterations the process is trying to run.
    /// </summary>
    public decimal Iterations { get; set; }
    
    /// <summary>
    /// The progress made on a process, if one was incomplete.
    /// </summary>
    public decimal Progress { get; set; }
}