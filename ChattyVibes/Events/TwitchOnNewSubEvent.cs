using System;
using TwitchLib.Client.Events;

namespace ChattyVibes.Events
{
    internal class TwitchOnNewSubEvent : BaseEvent
    {
        private static readonly object _evtKey = new object();

        public event EventHandler<OnNewSubscriberArgs> RaiseEvent
        {
            add { _events.AddHandler(_evtKey, value); }
            remove { _events.RemoveHandler(_evtKey, value); }
        }

        public void OnEvent(object sender, OnNewSubscriberArgs e)
        {
            EventHandler<OnNewSubscriberArgs> eventHandler =
                (EventHandler<OnNewSubscriberArgs>)_events[_evtKey];
            eventHandler?.Invoke(sender, e);
        }
    }
}
