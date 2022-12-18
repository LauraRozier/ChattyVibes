using System;
using TwitchLib.Client.Events;

namespace ChattyVibes.Events
{
    internal class TwitchOnContinuedGiftSubEvent : BaseEvent
    {
        private static readonly object _evtKey = new object();

        public event EventHandler<OnContinuedGiftedSubscriptionArgs> RaiseEvent
        {
            add { _events.AddHandler(_evtKey, value); }
            remove { _events.RemoveHandler(_evtKey, value); }
        }

        public void OnEvent(object sender, OnContinuedGiftedSubscriptionArgs e)
        {
            EventHandler<OnContinuedGiftedSubscriptionArgs> eventHandler =
                (EventHandler<OnContinuedGiftedSubscriptionArgs>)_events[_evtKey];
            eventHandler?.Invoke(sender, e);
        }
    }
}
