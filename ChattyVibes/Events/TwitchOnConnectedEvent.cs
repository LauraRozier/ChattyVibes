using System;
using TwitchLib.Client.Events;

namespace ChattyVibes.Events
{
    internal class TwitchOnConnectedEvent : BaseEvent
    {
        private static readonly object _evtKey = new object();

        public event EventHandler<OnConnectedArgs> RaiseEvent
        {
            add { _events.AddHandler(_evtKey, value); }
            remove { _events.RemoveHandler(_evtKey, value); }
        }

        public void OnEvent(object sender, OnConnectedArgs e)
        {
            EventHandler<OnConnectedArgs> eventHandler =
                (EventHandler<OnConnectedArgs>)_events[_evtKey];
            eventHandler?.Invoke(sender, e);
        }
    }
}
