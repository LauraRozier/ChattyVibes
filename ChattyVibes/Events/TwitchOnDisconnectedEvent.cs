using System;
using TwitchLib.Communication.Events;

namespace ChattyVibes.Events
{
    internal class TwitchOnDisconnectedEvent : BaseEvent
    {
        private static readonly object _evtKey = new object();

        public event EventHandler<OnDisconnectedEventArgs> RaiseEvent
        {
            add { _events.AddHandler(_evtKey, value); }
            remove { _events.RemoveHandler(_evtKey, value); }
        }

        public void OnEvent(object sender, OnDisconnectedEventArgs e)
        {
            EventHandler<OnDisconnectedEventArgs> eventHandler =
                (EventHandler<OnDisconnectedEventArgs>)_events[_evtKey];
            eventHandler?.Invoke(sender, e);
        }
    }
}
