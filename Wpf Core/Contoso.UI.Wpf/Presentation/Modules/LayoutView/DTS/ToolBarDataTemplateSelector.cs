using System.Windows;
using System.Windows.Controls;

using Tree.UI.Modules.LayoutUI.ViewModels;

namespace Tree.UI.Modules.LayoutUI.DataTemplateSelectors
{
    public class ToolBarDataTemplateSelector : DataTemplateSelector
    {
        public ToolBarDataTemplateSelector() { }

        public DataTemplate CommandTemplate { set; get; }

        public DataTemplate SeparatorTemplate { set; get; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            var toolBarItemViewModel = item as ToolBarItemViewModel;
            if (toolBarItemViewModel != null)
            {
                if (toolBarItemViewModel.IsSeparator)
                {
                    return SeparatorTemplate;
                }
                else
                {
                    return CommandTemplate;
                }
            }
            return base.SelectTemplate(item, container);
        }
    }
}
