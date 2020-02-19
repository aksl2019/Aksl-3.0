using System;

using Prism.Events;
using Prism.Modularity;
using Prism.Mvvm;
using Prism.Regions;
using Unity;

using Aksl.Toolkit.Services;

using Contoso.Infrastructure;
using Contoso.Infrastructure.Events;

namespace Contoso.Modules.TreeBar.ViewModels
{
    public class TreeBarHubViewModel : BindableBase, INavigationAware
    {
        #region Members
        private readonly IUnityContainer _container;
        private readonly IEventAggregator _eventAggregator;
        private readonly IRegionManager _regionManager;
        private readonly IDialogViewService _dialogViewService;

        private object _currentView;
        #endregion

        #region Constructors
        public TreeBarHubViewModel(IUnityContainer container, IEventAggregator eventAggregator, IRegionManager regionManager, IDialogViewService dialogViewService)
        {
            _container = container;
            _eventAggregator = eventAggregator;
            _regionManager = regionManager;
            _dialogViewService = dialogViewService;
        }
        #endregion

        #region Properties
        private bool _isLoading;
        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty<bool>(ref _isLoading, value);
        }

        public TreeBarsViewModel TreeBars { get; private set; }
        #endregion

        #region INavigationAware
        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            RegisterBuildWorkspaceViewEvents();

            var parameters = navigationContext.Parameters;
            if (parameters.TryGetValue("CurrentMenuItem", out MenuItem parentMenuItem))
            {
                CreateToolBarsViewModel(parentMenuItem);
            }
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {

        }
        #endregion

        #region RegisterEvent Method
        private void RegisterBuildWorkspaceViewEvents()
        {
            _eventAggregator.GetEvent<OnBuildWorkspaceViewEvent>().Subscribe((bwve) =>
            {
                try
                {
                    //_moduleManager.LoadModule(bwve.CurrentMenuItem.ModuleName);

                    string viewTypeAssemblyQualifiedName = bwve.CurrentMenuItem.ViewName;

                    Type viewType = Type.GetType(viewTypeAssemblyQualifiedName);
                    var view = _container.Resolve(viewType);
                    if (view != null)
                    {
                        IRegion region = _regionManager.Regions[RegionNames.WorkspaceRegion];
                        region.RemoveAll();

                        //_currentView = region.GetView(viewTypeAssemblyQualifiedName);

                        //if (_currentView != null)
                        //{
                        //    region.Remove(_currentView);
                        //}

                        _currentView = view;
                        region.Add(_currentView, viewTypeAssemblyQualifiedName);
                    }
                }
                catch
                {
                    //  MessageBox.Show($"Unable to display \"{smni.ModuleName}\" module.");
                }
            }, ThreadOption.UIThread, true);
        }
        #endregion

        #region Create ViewModel Method
        private void CreateToolBarsViewModel(MenuItem parentMenuItem)
        {
            IsLoading = true;

            TreeBars = new TreeBarsViewModel(_eventAggregator, parentMenuItem);
            TreeBars.PropertyChanged += (sender, propertyName) =>
            {
                if (sender is TreeBarsViewModel treeBarsVM)
                {
                    if (!treeBarsVM.IsLoading)
                    {
                        IsLoading = false;
                    }
                }
            };
            TreeBars.CreateTreeBarItemViewModels();
            OnPropertyChanged(() => TreeBars);
        }
        #endregion
    }
}
