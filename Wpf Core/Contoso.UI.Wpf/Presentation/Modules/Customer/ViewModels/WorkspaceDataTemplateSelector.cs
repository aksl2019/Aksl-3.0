using System.Windows;
using System.Windows.Controls;

namespace Contoso.Modules.Customer.ViewModels
{
    public class WorkspaceDataTemplateSelector : DataTemplateSelector
    {
        public WorkspaceDataTemplateSelector() { }

        public DataTemplate CustomerListTemplate { set; get; }

        public DataTemplate CustomerGroupTemplate { set; get; }

        public DataTemplate CustomerTemplate { set; get; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is Contoso.Modules.Customer.ViewModels.WorkspaceViewModel workspaceViewModel)
            {
                if (workspaceViewModel is CustomerListViewModel)
                {
                    return CustomerListTemplate;
                }
               else  if (workspaceViewModel is CustomerGroupViewModel)
                {
                    return CustomerGroupTemplate;
                }
                else
                {
                    return CustomerTemplate;
                }
            }

            return base.SelectTemplate(item, container);
        }
    }
}
