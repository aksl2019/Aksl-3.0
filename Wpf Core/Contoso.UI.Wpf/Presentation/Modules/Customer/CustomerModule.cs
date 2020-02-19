using Prism.Ioc;
using Prism.Modularity;
using Prism.Mvvm;
using Prism.Regions;

using Unity;

using Contoso.Modules.Customer.ViewModels;
using Contoso.Modules.Customer.Views;
using Contoso.Modules.Customer.Service;

namespace Contoso.Modules.Customer
{
    public class CustomerModule : IModule
    {
        private readonly IUnityContainer _container;
        private readonly IRegionManager _regionManager;

        public CustomerModule(IUnityContainer container, IRegionManager regionManager)
        {
            this._container = container;
            this._regionManager = regionManager;
        }

        #region IModule 成员
        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterSingleton<ICustomerService, CustomerService>();

            containerRegistry.RegisterForNavigation<CustomerHubViewModel>();
            containerRegistry.RegisterForNavigation<CustomerGroupHubViewModel>();
        }

        public void OnInitialized(IContainerProvider containerProvider)
        {
            ViewModelLocationProvider.Register(typeof(CustomerHubView).ToString(),
                                               () => _container.Resolve<CustomerHubViewModel>());

            ViewModelLocationProvider.Register(typeof(CustomerGroupHubView).ToString(),
                                            () => _container.Resolve<CustomerGroupHubViewModel>());
        }
        #endregion
    }
}
