using System.ComponentModel;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;

namespace PlayApp.Helpers;

public class Quartet<T1, T2, T3, T4> : INotifyPropertyChanged
{
    private T1 _first;
    private T2 _second;
    private T3 _third;
    private T4 _fourth;

    public Quartet(T1 first, T2 second, T3 third, T4 fourth)
    {
        First = first;
        Second = second;
        Third = third;
        Fourth = fourth;
    }

    public T1 First
    {
        get => _first;
        set
        {
            _first = value;
            OnPropertyChanged();
        }
    }

    public T2 Second
    {
        get => _second;
        set
        {
            _second = value;
            OnPropertyChanged();
        }
    }

    public T3 Third
    {
        get => _third;
        set
        {
            _third = value;
            OnPropertyChanged();
        }
    }

    public T4 Fourth
    {
        get => _fourth;
        set
        {
            _fourth = value;
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