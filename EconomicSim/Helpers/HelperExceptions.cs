namespace EconomicSim.Helpers;

/// <summary>
/// An exception thrown when two processes being added have incorrectly matching
/// tags.
/// </summary>
[Serializable]
public class ProcessTagMismatchException : Exception
{
    public ProcessTagMismatchException() { }

    public ProcessTagMismatchException(string message) : base(message) {}
    
    public ProcessTagMismatchException(string message, Exception inner) : base(message, inner) {}
}

/// <summary>
/// Exception thrown when a process should have an output as an
/// input or capital of the other.
/// </summary>
[Serializable]
public class ProcessInterfaceMismatchException : Exception
{
    public ProcessInterfaceMismatchException() { }

    public ProcessInterfaceMismatchException(string message) : base(message) {}
    
    public ProcessInterfaceMismatchException(string message, Exception inner) : base(message, inner) {}
}

/// <summary>
/// Exception thrown when processes being added together have incompatible
/// tags on their interfacing products.
/// </summary>
[Serializable]
public class ProcessInterfaceTagMismatchException : Exception
{
    public ProcessInterfaceTagMismatchException() { }

    public ProcessInterfaceTagMismatchException(string message) : base(message) {}
    
    public ProcessInterfaceTagMismatchException(string message, Exception inner) : base(message, inner) {}

}