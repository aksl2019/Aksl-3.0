using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Windows.Input;

namespace Contoso.Infrastructure.Mvvm
{
    public abstract class WorkspaceViewModel : BindableBase
    {
        protected WorkspaceViewModel()
        {
            ViewCloseCommand = new DelegateCommand(() =>
            {
                ExecuteViewClose();
            },
            () => CanExecuteViewClose());
        }

        public ICommand ViewCloseCommand { get; private set; }

        protected virtual bool CanExecuteViewClose()
        {
            return true;
        }

        protected virtual void ExecuteViewClose()
        {
        }

        public event EventHandler WorkspaceViewClose;

        private void RaiseWorkspaceViewClose()
        {
            if (WorkspaceViewClose != null)
            {
                WorkspaceViewClose(this, EventArgs.Empty);
            }
        }
    }
}