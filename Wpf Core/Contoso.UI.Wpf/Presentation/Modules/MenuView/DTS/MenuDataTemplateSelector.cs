using System.Windows;
using System.Windows.Controls;

using Contoso.Modules.MenuUI.ViewModels;

namespace Contoso.Modules.LayoutUI.DataTemplateSelectors
{
    public class MenuDataTemplateSelector : DataTemplateSelector
    {
        public MenuDataTemplateSelector() { }

        public DataTemplate CommandTemplate { set; get; }

        public DataTemplate SeparatorTemplate { set; get; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is MenuItemViewModel menuItemViewModel)
            {
                if (menuItemViewModel.IsSeparator)
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
