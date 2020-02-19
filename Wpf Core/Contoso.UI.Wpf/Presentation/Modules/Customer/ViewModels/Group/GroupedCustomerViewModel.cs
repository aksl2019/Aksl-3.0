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
    public class GroupedCustomerViewModel : BindableBase
    {
        #region Members
        private bool _isCompany;
        private IEnumerable<CustomerDto> _customers;
        #endregion

        #region Constructors
        public GroupedCustomerViewModel(int groupIndex, bool isCompany, IEnumerable<CustomerDto> customers)
        {
            GroupIndex = groupIndex;
            _isCompany = isCompany;
            _customers = customers;

          //  AllCustomers = new ObservableCollection<CustomerItemViewModel>();
            //CreateCustomerItemViewModels();
        }
        #endregion

        #region Properties
        public int GroupIndex { get; }

        public string HeaderTitle => _isCompany ? "People" : "Companies";

        public CustomerContentViewModel CustomerContent { get; private set; }

        public CustomerItemViewModel SelectedCustomerItem { get; private set; }

        private bool _isLoading;
        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }
        #endregion

        #region Create ViewModel Method
       internal void CreateCustomerContentViewModels()
        {
            IsLoading = true;
            var customerContentViewModel = new CustomerContentViewModel(GroupIndex, _customers);
            customerContentViewModel.PropertyChanged += (sender, e) =>
            {
                if (sender is CustomerContentViewModel ccvm)
                {
                    if (e.PropertyName == nameof(CustomerContentViewModel.IsLoading) && !ccvm.IsLoading)
                    {
                        IsLoading = false;
                    }

                    if (e.PropertyName == nameof(CustomerContentViewModel.SelectedCustomerItem))
                    {
                        SelectedCustomerItem = ccvm.SelectedCustomerItem;
                        OnPropertyChanged(()=> CustomerContent);
                    }
                }
            };
            customerContentViewModel.CreateCustomerItemViewModels();
            CustomerContent = customerContentViewModel;

            IsLoading = false;
        }

        //internal void CreateCustomerItemViewModels()
        //{
        //    IsLoading = true;

        //    foreach (var customer in _customers)
        //    {
        //        var customerItemViewModel = new CustomerItemViewModel(GroupIndex, customer);
        //        AllCustomers.Add(customerItemViewModel);
        //    }

        //    IsLoading = false;
        //}
        #endregion
    }
}
