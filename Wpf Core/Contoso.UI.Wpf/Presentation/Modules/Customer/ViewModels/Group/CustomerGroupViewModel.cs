using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

using Contoso.Modules.Customer.Service;

namespace Contoso.Modules.Customer.ViewModels
{
    public class CustomerGroupViewModel : WorkspaceViewModel
    {
        #region Members
        private ICustomerService _customerService;
        private int _currentGroupeIndex = -1;
        #endregion

        #region Constructors
        public CustomerGroupViewModel(ICustomerService customerService) : base()
        {
            _customerService = customerService;

            base.DisplayName = "All Customers";

            GroupedCustomers = new ObservableCollection<GroupedCustomerViewModel>();
        }
        #endregion

        #region Properties
        public ObservableCollection<GroupedCustomerViewModel> GroupedCustomers { get; private set; }

        private CustomerItemViewModel _selectedCustomerItem;
        public CustomerItemViewModel SelectedCustomerItem
        {
            get => _selectedCustomerItem;
            set
            {
                if (SetProperty(ref _selectedCustomerItem, value))
                {
                    if (_selectedCustomerItem != null)
                    {
                        OnPropertyChanged(() => TotalSelectedSales);
                    }
                }
            }
        }

        public double TotalSelectedSales => _selectedCustomerItem != null ? _selectedCustomerItem.TotalSales : 0.0d;
        #endregion

        #region Create ViewModel Method
        internal async Task CreateGroupedCustomersViewModelsAsync()
        {
            IsLoading = true;

            int groupeIndex = 0;
            var customers = await _customerService.GetCustomersAsync();
            var groupedCustomers = (from c in customers
                                    orderby c.FirstName, c.LastName
                                    group c by c.IsCompany into g
                                    select new { key = g.Key, gc = g.ToList() }).ToList();
            foreach (var gc in groupedCustomers)
            {
                var groupedCustomerViewModel = new GroupedCustomerViewModel(groupeIndex++, gc.key, gc.gc);
                groupedCustomerViewModel.PropertyChanged += (sender, e) =>
                {
                    if (sender is GroupedCustomerViewModel gcvm)
                    {
                        if (e.PropertyName == nameof(GroupedCustomerViewModel.IsLoading))
                        {
                            if (gcvm.GroupIndex == GroupedCustomers.Count() && !gcvm.IsLoading)
                            {
                                IsLoading = false;
                            }
                        }

                        if (e.PropertyName == nameof(GroupedCustomerViewModel.CustomerContent))
                        {
                            if (_currentGroupeIndex == gcvm.GroupIndex)
                            {
                                SelectedCustomerItem = gcvm.CustomerContent.SelectedCustomerItem;
                            }
                            else
                            {
                                foreach (var gc in GroupedCustomers)
                                {
                                    if (_currentGroupeIndex == gc.GroupIndex)
                                    {
                                        gc.CustomerContent.ClearSelectedCustomer();
                                    }
                                }

                                _currentGroupeIndex = gcvm.GroupIndex;
                                SelectedCustomerItem = gcvm.CustomerContent.SelectedCustomerItem;
                            }
                        }
                    }
                };
                groupedCustomerViewModel.CreateCustomerContentViewModels();
                GroupedCustomers.Add(groupedCustomerViewModel);
            }

            IsLoading = false;
        }
        #endregion
    }
}
