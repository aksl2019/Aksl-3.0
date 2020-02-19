using System;
using System.Windows.Input;

using Prism.Commands;
using Prism.Events;
using Prism.Modularity;
using Prism.Mvvm;
using Unity;

using Contoso.Infrastructure;
using Contoso.Infrastructure.Events;
using Contoso.Modules.Customer.Models;

namespace Contoso.Modules.Customer.ViewModels
{
    public class CustomerItemViewModel : BindableBase
    {
        #region Members
       
        #endregion

        #region Constructors
        public CustomerItemViewModel( CustomerDto customer)
        {
            Customer = customer;
        }

        public CustomerItemViewModel(int groupIndex, CustomerDto customer)
        {
            GroupIndex = groupIndex;
           Customer = customer;
        }
        #endregion

        #region Properties
        public int GroupIndex { get; }

        public CustomerDto Customer { get; }

        public string DisplayName
        {
            get
            {
                if (Customer.IsCompany)
                {
                    return Customer.FirstName;
                }
                else
                {
                    return $"{Customer.LastName}, {Customer.FirstName}";
                }
            }
        }

        public bool IsCompany => Customer.IsCompany;

        public string FirstName => Customer.FirstName;

        public string LastName => Customer.LastName;

        public string Email => Customer.Email;

        public double TotalSales => Customer.TotalSales;

        private bool _isSelected = false;
        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                if (SetProperty<bool>(ref _isSelected, value))
                {
                    if (_isSelected)
                    {
                    }
                }
            }
        }
        #endregion
    }
}
