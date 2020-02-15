using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Prism.Modularity;
using Prism.Regions;
using Prism.Mvvm;

using Tree.UI.Modules.LayoutUI.Views;
using Tree.UI.Modules.LayoutUI.ViewModels;
using Unity;

using Prism.Ioc;
using Contoso.Infrastructure;

namespace Tree.UI.Modules.LayoutUI
{
    public class LayoutUIModule : IModule
    {
        private readonly IUnityContainer _container;
        private readonly IRegionManager _regionManager;

        public LayoutUIModule(IUnityContainer container, IRegionManager regionManager)
        {
            this._container = container;
            this._regionManager = regionManager;
        }

        #region IModule 成员
        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<ToolBarHubView>();
            containerRegistry.RegisterForNavigation<TreeBarHubView>();
        }

        public void OnInitialized(IContainerProvider containerProvider)
        {
            ConfigureViewModelLocator();

            RegisterViewsWithRegions();
        }
        #endregion

        private void RegisterViewsWithRegions()
        {
            //_regionManager.RegisterViewWithRegion(RegionNames.TreeBarRegion,
            //                                           () => this._container.Resolve<TreeBarHubView>());

            //_regionManager.RegisterViewWithRegion(RegionNames.ToolBarRegion,
            //                                          () => this._container.Resolve<ToolBarHubView>());

            var navigationParameters = new NavigationParameters();
            navigationParameters.Add(nameof(ToolBarHubView), "1");

            //_regionManager.RequestNavigate(RegionNames.ToolBarRegion, nameof(ToolBarHubView), navigationParameters);
            //_regionManager.RequestNavigate(RegionNames.TreeBarRegion, nameof(TreeBarHubView));
        }

        private void ConfigureViewModelLocator()
        {
            ViewModelLocationProvider.Register(typeof(ToolBarHubView).ToString(),
                                                () => this._container.Resolve<ToolBarHubViewModel>());

            ViewModelLocationProvider.Register(typeof(TreeBarHubView).ToString(),
                                               () => this._container.Resolve<TreeBarHubViewModel>());

        }
    }
}
