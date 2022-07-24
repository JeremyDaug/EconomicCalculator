using System.ComponentModel;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;

namespace PlayApp.Helpers;

public class Pair<T1, T2> : INotifyPropertyChanged
{
    private T1 _primary;
    private T2 _secondary;

    public Pair(T1 primary, T2 secondary)
    {
        Primary = primary;
        Secondary = secondary;
    }

    public T1 Primary
    {
        get => _primary;
        set
        {
            _primary = value;
            OnPropertyChanged();
        }
    }

    public T2 Secondary
    {
        get => _secondary;
        set
        {
            _secondary = value;
            OnPropertyChanged();
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}