using System;
using TwitchLib.Client.Events;

namespace ChattyVibes.Events
{
    internal class TwitchOnCommunitySubEvent : BaseEvent
    {
        private static readonly object _evtKey = new object();

        public event EventHandler<OnCommunitySubscriptionArgs> RaiseEvent
        {
            add { _events.AddHandler(_evtKey, value); }
            remove { _events.RemoveHandler(_evtKey, value); }
        }

        public void OnEvent(object sender, OnCommunitySubscriptionArgs e)
        {
            EventHandler<OnCommunitySubscriptionArgs> eventHandler =
                (EventHandler<OnCommunitySubscriptionArgs>)_events[_evtKey];
            eventHandler?.Invoke(sender, e);
        }
    }
}
