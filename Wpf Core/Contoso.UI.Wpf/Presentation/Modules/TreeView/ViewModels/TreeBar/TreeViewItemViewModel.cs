using System.Collections.ObjectModel;
using System.Linq;

using Prism.Events;
using Prism.Mvvm;

using Contoso.Infrastructure;
using Contoso.Infrastructure.Events;

namespace Contoso.Modules.TreeBar.ViewModels
{
    public class TreeBarItemViewModel : BindableBase
    {
        #region Members
        protected readonly IEventAggregator _eventAggregator;
        protected readonly TreeBarItemViewModel _parent;
        private readonly MenuItem _menuItem;
        #endregion

        #region Constructors
        public TreeBarItemViewModel(IEventAggregator eventAggregator, MenuItem menuItem) : this(eventAggregator, menuItem, null)
        {
        }

        public TreeBarItemViewModel(IEventAggregator eventAggregator, MenuItem menuItem, TreeBarItemViewModel parent)
        {
            _eventAggregator = eventAggregator;
            _menuItem = menuItem;
            _parent = parent;

            _children = new ObservableCollection<TreeBarItemViewModel>((from child in _menuItem.SubMenus
                                                                        select new TreeBarItemViewModel(eventAggregator, child, this)).ToList<TreeBarItemViewModel>());
        }
        #endregion

        #region Properties
        public string Title => _menuItem.Title;

        public TreeBarItemViewModel Parent => _parent;

        protected ObservableCollection<TreeBarItemViewModel> _children;
        public ObservableCollection<TreeBarItemViewModel> Children => _children;

        protected bool Isleaf => _children.Count <= 0;

        protected bool _isExpanded;
        public bool IsExpanded
        {
            get => _isExpanded;
            set
            {
                SetProperty<bool>(ref _isExpanded, value);

                if (_isExpanded && Parent != null)
                {
                    Parent.IsExpanded = true;
                }
            }
        }

        protected bool _isSelected;
        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                if (SetProperty<bool>(ref _isSelected, value))
                {
                    if (Isleaf && _isSelected)
                    {
                        _eventAggregator.GetEvent<OnBuildWorkspaceViewEvent>().Publish(new OnBuildWorkspaceViewEvent { CurrentMenuItem = _menuItem });
                    }
                }
            }
        }
        #endregion
    }
}