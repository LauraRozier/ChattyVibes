using System;
using TwitchLib.Client.Events;

namespace ChattyVibes.Events
{
    internal class TwitchOnGiftSubEvent : BaseEvent
    {
        private static readonly object _evtKey = new object();

        public event EventHandler<OnGiftedSubscriptionArgs> RaiseEvent
        {
            add { _events.AddHandler(_evtKey, value); }
            remove { _events.RemoveHandler(_evtKey, value); }
        }

        public void OnEvent(object sender, OnGiftedSubscriptionArgs e)
        {
            EventHandler<OnGiftedSubscriptionArgs> eventHandler =
                (EventHandler<OnGiftedSubscriptionArgs>)_events[_evtKey];
            eventHandler?.Invoke(sender, e);
        }
    }
}
