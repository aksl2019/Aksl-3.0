
using Prism.Events;
using Prism.Mvvm;

namespace Contoso.Infrastructure.Events
{
    public class OnWorkspaceViewCloseEvent : PubSubEvent<OnWorkspaceViewCloseEvent>
    {
        public OnWorkspaceViewCloseEvent()
        {
        }

        public BindableBase ModulePermission { get; set; }

        public string Action { get; set; }
    }
}