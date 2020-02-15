
using Prism.Events;
using Prism.Mvvm;

namespace Contoso.Infrastructure.Events
{
    public class OnContentChangedViewEvent : PubSubEvent<OnContentChangedViewEvent>
    {
        public OnContentChangedViewEvent()
        {
        }

        public MenuItem CurrentMenuItem { get; set; }
    }
}