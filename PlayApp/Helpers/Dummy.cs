namespace PlayApp.Helpers;

public class Dummy<T>
{
    public Dummy(T item)
    {
        Wrapped = item;
    }
    
    public T Wrapped { get; set; }
}