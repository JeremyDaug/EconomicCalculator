namespace AvaEditorUI.Helpers;

public class Pair<T1, T2>
{
    public Pair(T1 primary, T2 secondary)
    {
        Primary = primary;
        Secondary = secondary;
    }

    public T1 Primary { get; set; }
    public T2 Secondary { get; set; }
}