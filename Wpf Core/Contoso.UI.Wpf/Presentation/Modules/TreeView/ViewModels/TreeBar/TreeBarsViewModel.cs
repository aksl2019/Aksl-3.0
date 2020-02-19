using System.Collections.ObjectModel;

using Prism.Events;
using Prism.Modularity;
using Prism.Mvvm;

using Contoso.Infrastructure;

namespace Contoso.Modules.TreeBar.ViewModels
{
    public class TreeBarsViewModel : BindableBase
    {
        #region Members
        private readonly IEventAggregator _eventAggregator;
        private MenuItem _parentMenuItem;
        #endregion

        #region Constructors
        public TreeBarsViewModel(IEventAggregator eventAggregator,  MenuItem parentMenuItem)
        {
            _eventAggregator = eventAggregator;
            _parentMenuItem = parentMenuItem;

            TreeBarItems = new ObservableCollection<TreeBarItemViewModel>();
        }
        #endregion

        #region Properties
        public ObservableCollection<TreeBarItemViewModel> TreeBarItems { get; }

        private bool _isLoading;
        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty<bool>(ref _isLoading, value);
        }
        #endregion

        #region Create ViewModel Method
        internal void CreateTreeBarItemViewModels()
        {
            IsLoading = true;

            foreach (var siteMapNodeInfo in _parentMenuItem.SubMenus)
            {
                var treeBarItemViewModel = new TreeBarItemViewModel(_eventAggregator, siteMapNodeInfo);
                //treeBarItemViewModel.PropertyChanged += (sender, propertyName) =>
                //{
                //    if (sender is TreeBarItemViewModel treeBarItemVM)
                //    {
                //        if (treeBarItemVM.IsSelected)
                //        { }
                //    }
                //};
                TreeBarItems.Add(treeBarItemViewModel);
            }

            IsLoading = false;
        }
        #endregion
    }
}
