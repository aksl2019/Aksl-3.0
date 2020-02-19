using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

using Contoso.Modules.Customer.Service;

namespace Contoso.Modules.Customer.ViewModels
{
    public class CustomerListViewModel : WorkspaceViewModel
    {
        #region Members
        private ICustomerService _customerService;
        #endregion

        #region Constructors
        public CustomerListViewModel(ICustomerService customerService) : base()
        {
            _customerService = customerService;

            base.DisplayName = "All Customers";

            AllCustomers = new ObservableCollection<CustomerItemViewModel>();
        }
        #endregion

        #region Properties
        public ObservableCollection<CustomerItemViewModel> AllCustomers { get; private set; }

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
                        _selectedCustomerItem.IsSelected = true;
                        OnPropertyChanged(() => TotalSelectedSales);
                    }
                }
            }
        }

       // public double TotalSelectedSales => this.AllCustomers.Sum(custVM => custVM.IsSelected ? custVM.TotalSales : 0.0);
        public double TotalSelectedSales
        {
            get
            {
                if (_selectedCustomerItem != null)
                {
                    return _selectedCustomerItem.TotalSales;
                }
                return 0.0d;
            }
        }
        #endregion

        #region Create ViewModel Method
        internal async Task CreateCustomerViewModelsAsync()
        {
            IsLoading = true;

            var customers = await _customerService.GetCustomersAsync();
            foreach (var customer in customers)
            {
                var customerItemViewModel = new CustomerItemViewModel(customer);
                //customerItemViewModel.PropertyChanged += (sender, propertyName) =>
                //{
                //    if (sender is CustomerItemViewModel customerVM)
                //    {
                //        if (customerVM.IsSelected)
                //        {
                //            OnPropertyChanged(() => TotalSelectedSales);
                //        }
                //    }
                //};

                AllCustomers.Add(customerItemViewModel);
            }

            IsLoading = false;
        }
        #endregion
    }
}
