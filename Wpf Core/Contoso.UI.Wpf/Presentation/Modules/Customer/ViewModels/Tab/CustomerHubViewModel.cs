using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;

using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;
using Unity;

using Aksl.Toolkit.Services;
using Contoso.Modules.Customer.Service;

namespace Contoso.Modules.Customer.ViewModels
{
    public class CustomerHubViewModel : BindableBase, INavigationAware
    {
        #region Members
        private readonly IUnityContainer _container;
        private readonly IEventAggregator _eventAggregator;
        private readonly IDialogViewService _dialogViewService;
        private ObservableCollection<WorkspaceViewModel> _workspaces;
        private ICustomerService _customerService;
        #endregion

        #region Constructors
        public CustomerHubViewModel(IUnityContainer container, IEventAggregator eventAggregator, IDialogViewService dialogViewService, ICustomerService customerService)
        {
            _container = container;
            _eventAggregator = eventAggregator;
            _dialogViewService = dialogViewService;
            _customerService = customerService;

            CreateNewCustomerCommand();
            CreateCustomersViewModelAsync().GetAwaiter().GetResult();
        }
        #endregion

        #region Properties
        public ObservableCollection<WorkspaceViewModel> Workspaces
        {
            get
            {
                if (_workspaces == null)
                {
                    _workspaces = new ObservableCollection<WorkspaceViewModel>();
                    _workspaces.CollectionChanged += (sender, e) =>
                    {
                        if (e.NewItems != null && e.NewItems.Count != 0)
                        {
                            foreach (WorkspaceViewModel workspace in e.NewItems)
                            {
                                workspace.RequestClose += this.OnWorkspaceRequestClose;
                            }
                        }

                        if (e.OldItems != null && e.OldItems.Count != 0)
                        {
                            foreach (WorkspaceViewModel workspace in e.OldItems)
                            {
                                workspace.RequestClose -= this.OnWorkspaceRequestClose;
                            }
                        }
                    };
                }
                return _workspaces;
            }
        }

        private void OnWorkspaceRequestClose(object sender, EventArgs e)
        {
            WorkspaceViewModel workspace = sender as WorkspaceViewModel;
            workspace.Dispose();
            Workspaces.Remove(workspace);

            OnPropertyChanged(() => Workspaces);
        }

        private bool _isLoading;
        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty<bool>(ref _isLoading, value);
        }
        #endregion

        #region New Customer Command
        public ICommand NewCustomerCommand { get; private set; }

        private void CreateNewCustomerCommand()
        {
            NewCustomerCommand = new DelegateCommand(() =>
            {
                ExecuteNewCustomerCommand();
            },
            () =>
            {
                return CanExecuteNewCustomerCommand();
            });
        }

        private void ExecuteNewCustomerCommand()
        {
            var customerViewModel = new CustomerViewModel(new Models.CustomerDto());
            Workspaces.Add(customerViewModel);
            SetActiveWorkspace(customerViewModel);

            OnPropertyChanged(() => Workspaces);
        }

        private bool CanExecuteNewCustomerCommand()
        {
            return true;
        }
        #endregion

        #region Active Workspac Method
        private void SetActiveWorkspace(WorkspaceViewModel workspace)
        {
            Debug.Assert(this.Workspaces.Contains(workspace));

            ICollectionView collectionView = CollectionViewSource.GetDefaultView(this.Workspaces);
            if (collectionView != null)
            {
                collectionView.MoveCurrentTo(workspace);
            }
        }
        #endregion

        #region Create ViewModel Method
        internal async Task CreateCustomersViewModelAsync()
        {
            IsLoading = true;
            string errorMessage = string.Empty;

            try
            {
                var customerListViewModel = _container.Resolve<CustomerListViewModel>();
                customerListViewModel.PropertyChanged += (sender, propertyName) =>
                {
                    if (sender is CustomerListViewModel customersViewModel)
                    {
                        if (!customersViewModel.IsLoading)
                        {
                            IsLoading = false;
                        }
                    }
                };
                customerListViewModel.CreateCustomerViewModelsAsync().GetAwaiter().GetResult();
                Workspaces.Add(customerListViewModel);
                SetActiveWorkspace(customerListViewModel);

                OnPropertyChanged(() => Workspaces);
            }
            catch (Exception ex)
            {
                errorMessage = $"The following error messages were {nameof(CustomerHubViewModel)} : { Environment.NewLine}{ex.Message}";
            }
            finally
            {
                if (IsLoading)
                {
                    IsLoading = false;
                }
            }

            await _dialogViewService.AlertWhenAsync(errorMessage, "Error Messages:");
        }
        #endregion

        #region INavigationAware
        public void OnNavigatedTo(NavigationContext navigationContext)
        {
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {

        }
        #endregion
    }
}
