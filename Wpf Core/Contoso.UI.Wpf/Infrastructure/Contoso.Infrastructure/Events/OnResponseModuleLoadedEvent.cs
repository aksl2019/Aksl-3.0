
using Prism.Events;
using Prism.Mvvm;

namespace Contoso.Infrastructure.Events
{
    public class OnResponseModuleLoadedEvent : PubSubEvent<OnResponseModuleLoadedEvent>
    {
        public OnResponseModuleLoadedEvent()
        {
        }

        public string ModuleName { get; set; }

        public BindableBase ViewModel { get; set; }
    }
}