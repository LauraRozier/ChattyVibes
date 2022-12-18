using System;
using TwitchLib.Client.Events;

namespace ChattyVibes.Events
{
    internal class TwitchOnWhisperMsgEvent : BaseEvent
    {
        private static readonly object _evtKey = new object();

        public event EventHandler<OnWhisperReceivedArgs> RaiseEvent
        {
            add { _events.AddHandler(_evtKey, value); }
            remove { _events.RemoveHandler(_evtKey, value); }
        }

        public void OnEvent(object sender, OnWhisperReceivedArgs e)
        {
            EventHandler<OnWhisperReceivedArgs> eventHandler =
                (EventHandler<OnWhisperReceivedArgs>)_events[_evtKey];
            eventHandler?.Invoke(sender, e);
        }
    }
}
