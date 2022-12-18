using System;
using TwitchLib.Client.Events;

namespace ChattyVibes.Events
{
    internal class TwitchOnJoinedChannelEvent : BaseEvent
    {
        private static readonly object _evtKey = new object();

        public event EventHandler<OnJoinedChannelArgs> RaiseEvent
        {
            add { _events.AddHandler(_evtKey, value); }
            remove { _events.RemoveHandler(_evtKey, value); }
        }

        public void OnEvent(object sender, OnJoinedChannelArgs e)
        {
            EventHandler<OnJoinedChannelArgs> eventHandler =
                (EventHandler<OnJoinedChannelArgs>)_events[_evtKey];
            eventHandler?.Invoke(sender, e);
        }
    }
}
