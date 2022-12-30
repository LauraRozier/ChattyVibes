using System;
using TwitchLib.Client.Events;

namespace ChattyVibes.Events
{
    internal class TwitchOnModeratorsReceivedEvent : BaseEvent
    {
        private static readonly object _evtKey = new object();

        public event EventHandler<OnModeratorsReceivedArgs> RaiseEvent
        {
            add { _events.AddHandler(_evtKey, value); }
            remove { _events.RemoveHandler(_evtKey, value); }
        }

        public void OnEvent(object sender, OnModeratorsReceivedArgs e)
        {
            EventHandler<OnModeratorsReceivedArgs> eventHandler =
                (EventHandler<OnModeratorsReceivedArgs>)_events[_evtKey];
            eventHandler?.Invoke(sender, e);
        }
    }
}
