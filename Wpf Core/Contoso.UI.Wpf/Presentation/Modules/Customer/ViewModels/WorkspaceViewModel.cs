using System;
using System.Windows.Input;

using Prism.Commands;
using Prism.Events;
using Prism.Modularity;
using Prism.Mvvm;

using Unity;

namespace Contoso.Modules.Customer.ViewModels
{
    public abstract class WorkspaceViewModel : BindableBase, IDisposable
    {
        public WorkspaceViewModel()
        {
            CreateCloseCommand();
        }

        public event EventHandler RequestClose;

        public virtual string DisplayName { get;  set; }

        private bool _isLoading;
        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty<bool>(ref _isLoading, value);
        }

        public ICommand CloseCommand { get; private set; }

        private void CreateCloseCommand()
        {
            CloseCommand = new DelegateCommand(() =>
            {
                ExecuteCloseCommand();
            },
            () => CanExecuteCloseCommand());
        }

        private void ExecuteCloseCommand()
        {
            RequestClose ?.Invoke(this, EventArgs.Empty);
        }

        private bool CanExecuteCloseCommand()
        {
            return true;
        }

        public void Dispose()
        {
            OnDispose();
        }

        protected virtual void OnDispose()
        {
        }
    }
}
