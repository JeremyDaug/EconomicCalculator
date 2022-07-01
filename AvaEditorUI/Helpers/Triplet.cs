namespace AvaEditorUI.Helpers;

public class Triplet<T1, T2, T3>
{
    public Triplet(T1 first, T2 second, T3 third)
    {
        First = first;
        Second = second;
        Third = third;
    }
    
    public T1 First { get; set; }
    public T2 Second { get; set; }
    public T3 Third { get; set; }
}