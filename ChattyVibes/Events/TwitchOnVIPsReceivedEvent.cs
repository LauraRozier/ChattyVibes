using System;
using TwitchLib.Client.Events;

namespace ChattyVibes.Events
{
    internal class TwitchOnVIPsReceivedEvent : BaseEvent
    {
        private static readonly object _evtKey = new object();

        public event EventHandler<OnVIPsReceivedArgs> RaiseEvent
        {
            add { _events.AddHandler(_evtKey, value); }
            remove { _events.RemoveHandler(_evtKey, value); }
        }

        public void OnEvent(object sender, OnVIPsReceivedArgs e)
        {
            EventHandler<OnVIPsReceivedArgs> eventHandler =
                (EventHandler<OnVIPsReceivedArgs>)_events[_evtKey];
            eventHandler?.Invoke(sender, e);
        }
    }
}
