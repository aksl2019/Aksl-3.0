using System;
using System.Windows;

using Unity;
using Prism.Modularity;
using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;

using Aksl.Toolkit.Services;

using Contoso.Infrastructure.Events;
using Contoso.Infrastructure;

namespace Contoso.Modules.Shell.ViewModels
{
    public class ShellViewModel : BindableBase
    {
        #region Members
        private readonly IUnityContainer _container;
        private readonly IEventAggregator _eventAggregator;
      //  private readonly IModuleManager _moduleManager;
        private readonly IRegionManager _regionManager;
        private readonly IDialogViewService _dialogViewService;
        // private object _currentView;
        #endregion

        #region Constructors
        public ShellViewModel(IUnityContainer container, IEventAggregator eventAggregator, IRegionManager regionManager, IDialogViewService dialogViewService)
        {
            _container = container;
            _eventAggregator = eventAggregator;
          //  _moduleManager = moduleManager;
            _regionManager = regionManager;
            _dialogViewService = dialogViewService;

            RegisterContentChangedEvents();
        }
        #endregion

        #region Register Method
        private void RegisterContentChangedEvents()
        {
            _eventAggregator.GetEvent<OnContentChangedViewEvent>().Subscribe((ccve) =>
            {
                try
                {
                    string viewTypeAssemblyQualifiedName = ccve.CurrentMenuItem.ViewName;
                    Type viewType = Type.GetType(viewTypeAssemblyQualifiedName);
                    // var view = _container.Resolve(viewType);
                    if (viewType != null)
                    {
                        IRegion region = _regionManager.Regions[RegionNames.ContentRegion];
                        region.RemoveAll();

                        // _currentView = region.GetView(viewType.FullName); 

                        var viewName = viewType.Name;

                        //if (_currentView != null)
                        //{
                        //    region.Remove(_currentView);
                        //}

                        //_currentView = view;

                        if (!string.IsNullOrEmpty(ccve.CurrentMenuItem.ModuleName) && ccve.CurrentMenuItem.IsHome)
                        {
                            _regionManager.RequestNavigate(RegionNames.ContentRegion, viewName);
                        }
                        else
                        {
                            var navigationParameters = new NavigationParameters();
                            navigationParameters.Add("CurrentMenuItem", ccve.CurrentMenuItem);

                            _regionManager.RequestNavigate(RegionNames.ContentRegion, viewName, navigationParameters);
                        }
                    }
                    else
                    {
                        _dialogViewService.AlertAsync($"Unable to display \"{ccve.CurrentMenuItem.ModuleName}\" module.");
                    }
                }
                catch(Exception ex)
                {
                    _dialogViewService.AlertAsync($"While loading \"{ccve.CurrentMenuItem.ModuleName}\" in {nameof(ShellViewModel)} error:  { Environment.NewLine}{ex.Message}.");
                }
            }, ThreadOption.UIThread, true);
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
        //            MessageBox.Show($"Unable to display \"{smni.ModuleName}\" module.");
        //        }
        //    }, ThreadOption.UIThread, true);
        //}
    }
}
