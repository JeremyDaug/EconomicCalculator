using System;
using ReactiveUI;

namespace AvaEditorUI.ViewModels
{
    public abstract class ViewModelBase : ReactiveObject
    {
        public event EventHandler? ClosingRequest;

        protected internal void OnClosingRequest()
        {
            ClosingRequest?.Invoke(this, EventArgs.Empty);
        }
    }
}