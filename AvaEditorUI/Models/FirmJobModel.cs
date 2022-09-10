using System.ComponentModel;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;

namespace AvaEditorUI.Models;

public class FirmJobModel : INotifyPropertyChanged
{
    private string _job;
    private decimal _wage;
    private string _wageType;
    private string _assignments;

    public string Job
    {
        get => _job;
        set
        {
            if (value != _job)
            {
                _job = value;
                OnPropertyChanged();
            }
        }
    }

    public string WageType
    {
        get => _wageType;
        set
        {
            if (value != _wageType)
            {
                _wageType = value;
                OnPropertyChanged();
            }
        }
    }

    public decimal Wage
    {
        get => _wage;
        set
        {
            if (value != _wage)
            {
                _wage = value;
                OnPropertyChanged();
            }
        }
    }

    public string Assignments
    {
        get => _assignments;
        set
        {
            if (value != _assignments)
            {
                _assignments = value;
                OnPropertyChanged();
            }
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}