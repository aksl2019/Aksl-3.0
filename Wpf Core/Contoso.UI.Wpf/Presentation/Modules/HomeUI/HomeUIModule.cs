using Prism.Ioc;
using Prism.Modularity;
using Prism.Mvvm;
using Prism.Regions;

using Unity;

using Contoso.Modules.Home.ViewModels;
using Contoso.Modules.Home.Views;

namespace Contoso.Modules.Home
{
    public class HomeUIModule : IModule
    {
        private readonly IUnityContainer _container;
        private readonly IRegionManager _regionManager;

        public HomeUIModule(IUnityContainer container, IRegionManager regionManager)
        {
            this._container = container;
            this._regionManager = regionManager;
        }

        #region IModule 成员
        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<HomeView>();
        }

        public void OnInitialized(IContainerProvider containerProvider)
        {
            ViewModelLocationProvider.Register(typeof(HomeView).ToString(),
                                               () => this._container.Resolve<HomeViewModel>());
        }
        #endregion
    }
}
