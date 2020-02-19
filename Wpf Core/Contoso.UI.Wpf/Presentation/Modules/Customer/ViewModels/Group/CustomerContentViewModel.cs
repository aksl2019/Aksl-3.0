using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

using Prism.Mvvm;

using Contoso.Modules.Customer.Service;
using Contoso.Modules.Customer.Models;
using System.Collections.Generic;

namespace Contoso.Modules.Customer.ViewModels
{
    public class CustomerContentViewModel : BindableBase
    {
        #region Members
        private IEnumerable<CustomerDto> _customers;
        #endregion

        #region Constructors
        public CustomerContentViewModel(int groupIndex,  IEnumerable<CustomerDto> customers)
        {
            GroupIndex = groupIndex;
            _customers = customers;

            Customers = new ObservableCollection<CustomerItemViewModel>();
           // CreateCustomerItemViewModels();
        }
        #endregion

        #region Properties
        public int GroupIndex { get; }

        public ObservableCollection<CustomerItemViewModel> Customers { get; private set; }

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
                    }
                }
            }
        }

        private bool _isLoading;
        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty<bool>(ref _isLoading, value);
        }
        #endregion

        internal void ClearSelectedCustomer()
        {
            SelectedCustomerItem.IsSelected = false;
            SelectedCustomerItem = null;
        }

        #region Create ViewModel Method
        internal void CreateCustomerItemViewModels()
        {
            IsLoading = true;

            foreach (var customer in _customers)
            {
                var customerItemViewModel = new CustomerItemViewModel(GroupIndex, customer);
                Customers.Add(customerItemViewModel);
            }

            IsLoading = false;
        }
        #endregion
    }
}
