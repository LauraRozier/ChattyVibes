using Buttplug;
using System;

namespace ChattyVibes.Events
{
    internal class ButtplugDeviceRemovedEvent : BaseEvent
    {
        private static readonly object _evtKey = new object();

        public event EventHandler<DeviceRemovedEventArgs> RaiseEvent
        {
            add { _events.AddHandler(_evtKey, value); }
            remove { _events.RemoveHandler(_evtKey, value); }
        }

        public void OnEvent(object sender, DeviceRemovedEventArgs e)
        {
            EventHandler<DeviceRemovedEventArgs> eventHandler =
                (EventHandler<DeviceRemovedEventArgs>)_events[_evtKey];
            eventHandler?.Invoke(sender, e);
        }
    }
}
