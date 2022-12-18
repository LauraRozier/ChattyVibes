using System;
using TwitchLib.Client.Events;

namespace ChattyVibes.Events
{
    internal class TwitchOnPrimeSubEvent : BaseEvent
    {
        private static readonly object _evtKey = new object();

        public event EventHandler<OnPrimePaidSubscriberArgs> RaiseEvent
        {
            add { _events.AddHandler(_evtKey, value); }
            remove { _events.RemoveHandler(_evtKey, value); }
        }

        public void OnEvent(object sender, OnPrimePaidSubscriberArgs e)
        {
            EventHandler<OnPrimePaidSubscriberArgs> eventHandler =
                (EventHandler<OnPrimePaidSubscriberArgs>)_events[_evtKey];
            eventHandler?.Invoke(sender, e);
        }
    }
}
