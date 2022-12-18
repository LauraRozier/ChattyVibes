using System;
using TwitchLib.Client.Events;

namespace ChattyVibes.Events
{
    internal class TwitchOnChatMsgEvent : BaseEvent
    {
        private static readonly object _evtKey = new object();

        public event EventHandler<OnMessageReceivedArgs> RaiseEvent
        {
            add { _events.AddHandler(_evtKey, value); }
            remove { _events.RemoveHandler(_evtKey, value); }
        }

        public void OnEvent(object sender, OnMessageReceivedArgs e)
        {
            EventHandler<OnMessageReceivedArgs> eventHandler =
                (EventHandler<OnMessageReceivedArgs>)_events[_evtKey];
            eventHandler?.Invoke(sender, e);
        }
    }
}
