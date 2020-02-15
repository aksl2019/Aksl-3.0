
using Prism.Events;
using Prism.Mvvm;

namespace Contoso.Infrastructure.Events
{
    public class OnBuildWorkspaceViewEvent : PubSubEvent<OnBuildWorkspaceViewEvent>
    {
        public OnBuildWorkspaceViewEvent()
        {
        }

        //public string ModuleName { get; set; }

        //public string ViewName { get; set; }

        public MenuItem CurrentMenuItem { get; set; }
    }
}