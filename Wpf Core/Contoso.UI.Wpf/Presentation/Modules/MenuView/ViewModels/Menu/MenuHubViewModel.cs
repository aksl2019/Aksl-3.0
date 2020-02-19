using System;
using System.Linq;

using Unity;
using Prism.Events;
using Prism.Modularity;
using Prism.Mvvm;
using Prism.Regions;

using Contoso.Infrastructure;
using Contoso.Infrastructure.Events;

namespace Contoso.Modules.MenuUI.ViewModels
{
    public class MenuHubViewModel : BindableBase, INavigationAware
    {
        #region Members
        private readonly IUnityContainer _container;
        //private readonly IModuleManager _moduleManager;
        private readonly IMenuService _menuService;
        private readonly IEventAggregator _eventAggregator;
        //private readonly IRegionManager _regionManager;
        //private object _currentView;
        #endregion

        #region Constructors
        public MenuHubViewModel(IUnityContainer container, IEventAggregator eventAggregator, IMenuService menuService)
        {
            _container = container;
            _eventAggregator = eventAggregator;
            _menuService = menuService;

            // RegisterEvents();
            CreateMenusViewModel();
        }
        #endregion

        //private void RegisterEvents()
        //{
        //    _eventAggregator.GetEvent<OnBuildWorkspaceViewEvent>().Subscribe((smni) =>
        //    {
        //        if (string.IsNullOrEmpty(smni.ModuleName))
        //        {
        //            throw new ArgumentNullException(smni.ModuleName, "Illegal module name to load.");
        //        }

        //        try
        //        {
        //            _moduleManager.LoadModule(smni.ModuleName);

        //            string viewTypeAssemblyQualifiedName = smni.ViewName;

        //            Type viewType = Type.GetType(viewTypeAssemblyQualifiedName);
        //            var view = _container.Resolve(viewType);
        //            if (view != null)
        //            {
        //                IRegion region = _regionManager.Regions[RegionNames.WorkspaceRegion];
        //                _currentView = region.GetView(viewTypeAssemblyQualifiedName);

        //                if (_currentView != null)
        //                    region.Remove(_currentView);

        //                _currentView = view;
        //                region.Add(_currentView, viewTypeAssemblyQualifiedName);
        //            }
        //        }
        //        catch
        //        {
        //          //  MessageBox.Show($"Unable to display \"{smni.ModuleName}\" module.");
        //        }
        //    }, ThreadOption.UIThread, true);
        //}

        #region Properties
        public MenusViewModel Menus { get; private set; }

        private bool _isLoading;
        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }
        #endregion

        #region INavigationAware
        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            CreateMenusViewModel();
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {

        }
        #endregion

        #region Create ViewModel Method
        private void CreateMenusViewModel()
        {
            IsLoading = true;

            Menus = new MenusViewModel(_eventAggregator, _menuService);
            Menus.PropertyChanged += (sender, e) =>
            {
                if (sender is MenusViewModel mvm)
                {
                    if (e.PropertyName == nameof(MenusViewModel.IsLoading) && !mvm.IsLoading)
                    {
                        IsLoading = false;
                    }
                }
            };
            Menus.CreateMenuItemViewModelsAsync().GetAwaiter().GetResult();
            //OnPropertyChanged(() => Menus);
            //OnPropertyChanged(nameof(Menus));
        }
        #endregion
    }
}
