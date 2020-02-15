using Prism.Events;

using Contoso.Infrastructure.Models;

namespace Contoso.Infrastructure.Events
{
    public class OnBuildTreeBarEvent : PubSubEvent<MenuItem>
    {
        public OnBuildTreeBarEvent()
        {
        }
    }
}
