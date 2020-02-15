using Unity;

using Prism.Ioc;
using Prism.Modularity;
using Prism.Mvvm;
using Prism.Regions;

using Contoso.Infrastructure;
using Contoso.Modules.MenuUI.ViewModels;
using Contoso.Modules.MenuUI.Views;

namespace Contoso.Modules.Menu
{
    public class MenuUIModule : IModule
    {
        private readonly IUnityContainer _container;
        private readonly IRegionManager _regionManager;

        public MenuUIModule(IUnityContainer container, IRegionManager regionManager)
        {
            this._container = container;
            this._regionManager = regionManager;
        }

        #region IModule 成员
        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<MenuHubView>();
        }

        public void OnInitialized(IContainerProvider containerProvider)
        {
            ViewModelLocationProvider.Register(typeof(MenuHubView).ToString(),
                                                () => this._container.Resolve<MenuHubViewModel>());
        }
        #endregion
    }
}
