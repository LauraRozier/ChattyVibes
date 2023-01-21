using Buttplug.Client;
using System;

namespace ChattyVibes.Events
{
    internal class ButtplugDeviceAddedEvent : BaseEvent
    {
        private static readonly object _evtKey = new object();

        public event EventHandler<DeviceAddedEventArgs> RaiseEvent
        {
            add { _events.AddHandler(_evtKey, value); }
            remove { _events.RemoveHandler(_evtKey, value); }
        }

        public void OnEvent(object sender, DeviceAddedEventArgs e)
        {
            EventHandler<DeviceAddedEventArgs> eventHandler =
                (EventHandler<DeviceAddedEventArgs>)_events[_evtKey];
            eventHandler?.Invoke(sender, e);
        }
    }
}
