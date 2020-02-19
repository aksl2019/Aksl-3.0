using System.Collections.ObjectModel;
using System.Threading.Tasks;

using Prism.Events;
using Prism.Mvvm;

using Contoso.Infrastructure;

namespace Contoso.Modules.MenuUI.ViewModels
{
    public class MenusViewModel : BindableBase
    {
        #region Members
        private readonly IEventAggregator _eventAggregator;
        private readonly IMenuService _menuService;
        #endregion

        #region Constructors
        public MenusViewModel(IEventAggregator eventAggregator, IMenuService menuService)
        {
            _eventAggregator = eventAggregator;
            _menuService = menuService;

            MenuItems = new ObservableCollection<MenuItemViewModel>();
        }
        #endregion

        #region Properties
        public ObservableCollection<MenuItemViewModel> MenuItems { get; }

        private MenuItemViewModel _selectedMenuItem;
        public MenuItemViewModel SelectedMenuItem
        {
            get => _selectedMenuItem;
            set
            {
                var previewSelectedMenuItem = _selectedMenuItem;
                if (SetProperty(ref _selectedMenuItem, value))
                {
                    _selectedMenuItem.IsSelected = true;

                    if (previewSelectedMenuItem != null)
                    {
                        previewSelectedMenuItem.IsSelected = false;
                    }
                }
            }
        }

        private bool _isLoading;
        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty<bool>(ref _isLoading, value);
        }
        #endregion

        #region Create ViewModel Method
        internal async Task CreateMenuItemViewModelsAsync()
        {
            IsLoading = true;
            MenuItemViewModel selectedMenuItem = default;

            var rootMenuItems = await _menuService.BuildMenuAsync("");
            foreach (var mi in rootMenuItems)
            {
                var menuItemViewModel = new MenuItemViewModel(_eventAggregator, mi);

                if (mi.IsHome)
                {
                    selectedMenuItem = menuItemViewModel;
                }

                MenuItems.Add(menuItemViewModel);
            }

            SelectedMenuItem = selectedMenuItem;

            IsLoading = false;
        }
        #endregion
    }
}
