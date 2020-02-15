using System;

using Prism.Events;

namespace Contoso.Infrastructure.Events
{
    public class OnConfirmShowEvent : PubSubEvent<OnConfirmShowEvent>
    {
        public OnConfirmShowEvent()
        { }

        public string MessageTitle { get; set; }

        public string Message { get; set; }

        public Action<bool?> ConfirmAction { get; set; }
    }
}
