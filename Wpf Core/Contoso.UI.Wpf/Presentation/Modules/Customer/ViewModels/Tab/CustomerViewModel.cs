using System;
using System.Windows.Input;
using Prism.Commands;
using Prism.Events;
using Prism.Modularity;
using Prism.Mvvm;
using Unity;

using Contoso.Modules.Customer.Models;

namespace Contoso.Modules.Customer.ViewModels
{
    public class CustomerViewModel : WorkspaceViewModel
    {
        #region Constructors
        public CustomerViewModel(CustomerDto customer) : base()
        {
            Customer = customer;

            CreateSaveCommand();
        }
        #endregion

        #region Properties
        public CustomerDto Customer { get; }

        public override string DisplayName
        {
            get
            {
                if (this.IsNewCustomer)
                {
                    return "New Customer";
                }
                else if (Customer.IsCompany)
                {
                    return Customer.FirstName;
                }
                else
                {
                    return $"{Customer.LastName}, {Customer.FirstName}";
                }
            }
        }

        private bool IsNewCustomer => Customer.Id == 0;

        public bool IsCompany => Customer.IsCompany;

        public double TotalSales => Customer.TotalSales;
        #endregion

        #region Editable Properties
        private string[] _customerTypeOptions;
        public string[] CustomerTypeOptions
        {
            get
            {
                if (_customerTypeOptions == null)
                {
                    _customerTypeOptions = new string[]
                    {
                        "(Not Specified)",
                        "Person",
                        "Company"
                    };
                }
                return _customerTypeOptions;
            }
        }

        private string _customerType;
        public string CustomerType
        {
            get => _customerType;
            set
            {
                if (value == _customerType || string.IsNullOrEmpty(value))
                {
                    return;
                }

                SetProperty<string>(ref _customerType, value);

                if (_customerType == "Company")
                {
                    Customer.IsCompany = true;
                }
                else if (_customerType == "Person")
                {
                    Customer.IsCompany = false;
                }

                base.OnPropertyChanged(() => LastName);
            }
        }

        private string _firstName;
        public string FirstName
        {
            get => _firstName;
            set => SetProperty<string>(ref _firstName, value);
        }

        private string _lastName;
        public string LastName
        {
            get => _lastName;
            set => SetProperty<string>(ref _lastName, value);
        }

        private string _email;
        public string Email
        {
            get => _email;
            set => SetProperty<string>(ref _email, value);
        }

        private bool _isSelected = false;
        public bool IsSelected
        {
            get => _isSelected;
            set => SetProperty(ref _isSelected, value);
        }
        #endregion

        #region Save Command
        public ICommand SaveCommand { get; private set; }

        private void CreateSaveCommand()
        {
            SaveCommand = new DelegateCommand(() =>
            {
                ExecuteSaveCommand();
            },
            () => CanExecuteSaveCommand());
        }

        private void ExecuteSaveCommand()
        {

        }

        private bool CanExecuteSaveCommand()
        {
            return true;
        }
        #endregion
    }
}
