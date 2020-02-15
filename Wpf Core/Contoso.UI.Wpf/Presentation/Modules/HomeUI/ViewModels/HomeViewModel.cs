using Prism.Events;
using Prism.Modularity;
using Prism.Mvvm;
using Prism.Regions;
using Unity;

namespace Contoso.Modules.Home.ViewModels
{
    public class HomeViewModel : BindableBase, INavigationAware
    {
        private readonly IUnityContainer _container;
        private readonly IEventAggregator _eventAggregator;
        private readonly IModuleManager _moduleManager;

        public HomeViewModel(IUnityContainer container, IEventAggregator eventAggregator, IModuleManager moduleManager)
        {
            _container = container;
            _eventAggregator = eventAggregator;
            _moduleManager = moduleManager;
        }

    
        private bool _isLoading;
        public bool IsLoading
        {
            get
            {
                return _isLoading;
            }
            set
            {
                SetProperty<bool>(ref _isLoading, value);
            }
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
            
        }
    }
}
