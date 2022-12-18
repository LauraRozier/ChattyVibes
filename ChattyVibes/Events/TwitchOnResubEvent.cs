using System;
using TwitchLib.Client.Events;

namespace ChattyVibes.Events
{
    internal class TwitchOnResubEvent : BaseEvent
    {
        private static readonly object _evtKey = new object();

        public event EventHandler<OnReSubscriberArgs> RaiseEvent
        {
            add { _events.AddHandler(_evtKey, value); }
            remove { _events.RemoveHandler(_evtKey, value); }
        }

        public void OnEvent(object sender, OnReSubscriberArgs e)
        {
            EventHandler<OnReSubscriberArgs> eventHandler =
                (EventHandler<OnReSubscriberArgs>)_events[_evtKey];
            eventHandler?.Invoke(sender, e);
        }
    }
}
