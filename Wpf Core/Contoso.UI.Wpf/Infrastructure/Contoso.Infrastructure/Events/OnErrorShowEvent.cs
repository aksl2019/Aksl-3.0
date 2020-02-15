using System;

using Prism.Events;

namespace Contoso.Infrastructure.Events
{
    public class OnErrorShowEvent : PubSubEvent<OnErrorShowEvent>
    {
        public OnErrorShowEvent()
        { }

        public string Message { get; set; }

        public Exception ExceptionObj { get; set; }
    }
}
