using Buttplug.Client;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using TwitchLib.Client.Events;
using TwitchLib.Communication.Events;
using System.Linq;
using System.Threading.Tasks;

namespace ChattyVibes.Events
{
    internal enum EventType
    {
        // Twitch events
        TwitchOnConnected,
        TwitchOnDisconnected,
        TwitchOnJoinedChannel,
        TwitchOnChatMsg,
        TwitchOnWhisperMsg,
        TwitchOnNewSub,
        TwitchOnGiftSub,
        TwitchOnContinuedGiftSub,
        TwitchOnCommunitySub,
        TwitchOnPrimeSub,
        TwitchOnResub,
        TwitchOnVIPsReceived,
        TwitchOnModeratorsReceived,

        // Buttplug.IO events
        ButtplugDeviceAdded,
        ButtplugDeviceRemoved,

        // App events

    }

    internal class EventFactory
    {
        private struct QueueItem
        {
            public object Sender;
            public object Args;
        }

        private volatile bool _shouldStop = false;
        private volatile bool _isClearingQueue = false;
        private readonly Dictionary<EventType, BaseEvent> _events = new Dictionary<EventType, BaseEvent>();
        private Dictionary<EventType, ConcurrentQueue<QueueItem>> _queues = new Dictionary<EventType, ConcurrentQueue<QueueItem>>
        {
            { EventType.TwitchOnConnected, new ConcurrentQueue<QueueItem>() },
            { EventType.TwitchOnDisconnected, new ConcurrentQueue<QueueItem>() },
            { EventType.TwitchOnJoinedChannel, new ConcurrentQueue<QueueItem>() },
            { EventType.TwitchOnChatMsg, new ConcurrentQueue<QueueItem>() },
            { EventType.TwitchOnWhisperMsg, new ConcurrentQueue<QueueItem>() },
            { EventType.TwitchOnNewSub, new ConcurrentQueue<QueueItem>() },
            { EventType.TwitchOnGiftSub, new ConcurrentQueue<QueueItem>() },
            { EventType.TwitchOnContinuedGiftSub, new ConcurrentQueue<QueueItem>() },
            { EventType.TwitchOnCommunitySub, new ConcurrentQueue<QueueItem>() },
            { EventType.TwitchOnPrimeSub, new ConcurrentQueue<QueueItem>() },
            { EventType.TwitchOnResub, new ConcurrentQueue<QueueItem>() },
            { EventType.TwitchOnVIPsReceived, new ConcurrentQueue<QueueItem>() },
            { EventType.TwitchOnModeratorsReceived, new ConcurrentQueue<QueueItem>() },

            { EventType.ButtplugDeviceAdded, new ConcurrentQueue<QueueItem>() },
            { EventType.ButtplugDeviceRemoved, new ConcurrentQueue<QueueItem>() }
        };
        private readonly Thread _worker;

        public BaseEvent GetEvent(EventType eventType) =>
            _events?[eventType];

        public void Enqueue(EventType type, object sender, object eventArgs) =>
            _queues?[type]?.Enqueue(new QueueItem { Sender = sender, Args = eventArgs });

        public void Clear(EventType type)
        {
            if (_queues == null)
                return;

            _isClearingQueue = true;

            while (!_queues[type].IsEmpty)
                _queues[type].TryDequeue(out _);

            _isClearingQueue = false;
        }

        internal async Task Cleanup()
        {
            _shouldStop = true;

            while (_worker.IsAlive)
                await Task.Delay(10);
        }

        public EventFactory()
        {
            _events.Add(EventType.TwitchOnConnected, new TwitchOnConnectedEvent());
            _events.Add(EventType.TwitchOnDisconnected, new TwitchOnDisconnectedEvent());
            _events.Add(EventType.TwitchOnJoinedChannel, new TwitchOnJoinedChannelEvent());
            _events.Add(EventType.TwitchOnChatMsg, new TwitchOnChatMsgEvent());
            _events.Add(EventType.TwitchOnWhisperMsg, new TwitchOnWhisperMsgEvent());
            _events.Add(EventType.TwitchOnNewSub, new TwitchOnNewSubEvent());
            _events.Add(EventType.TwitchOnGiftSub, new TwitchOnGiftSubEvent());
            _events.Add(EventType.TwitchOnContinuedGiftSub, new TwitchOnContinuedGiftSubEvent());
            _events.Add(EventType.TwitchOnCommunitySub, new TwitchOnCommunitySubEvent());
            _events.Add(EventType.TwitchOnPrimeSub, new TwitchOnPrimeSubEvent());
            _events.Add(EventType.TwitchOnResub, new TwitchOnResubEvent());
            _events.Add(EventType.TwitchOnVIPsReceived, new TwitchOnVIPsReceivedEvent());
            _events.Add(EventType.TwitchOnModeratorsReceived, new TwitchOnModeratorsReceivedEvent());

            _events.Add(EventType.ButtplugDeviceAdded, new ButtplugDeviceAddedEvent());
            _events.Add(EventType.ButtplugDeviceRemoved, new ButtplugDeviceRemovedEvent());

            _worker = new Thread(new ThreadStart(HandleQueue)) { IsBackground = true };
            _worker.Start();
        }

        private void HandleQueue()
        {
            try
            {
                while (!_shouldStop)
                {
                    if (_isClearingQueue)
                    {
                        Thread.Sleep(10);
                        continue;
                    }

                    foreach (var item in _queues.Where(item => !item.Value.IsEmpty))
                    {
                        if (!item.Value.TryDequeue(out QueueItem qitem))
                            continue;

                        FireEvent(item.Key, qitem.Sender, qitem.Args);
                    }

                    Thread.Sleep(10);
                }
            }
            catch (ThreadAbortException)
            {
                _shouldStop = true;
            }
            finally
            {
                _isClearingQueue = true;
                var tmp = _queues;
                _queues = null;

                foreach (var queue in tmp.Where(eq => !eq.Value.IsEmpty).Select(eq => eq.Value))
                {
                    while (!queue.IsEmpty)
                        queue.TryDequeue(out _);
                }
            }
        }

        internal void FireEvent(EventType type, object sender, object args)
        {
            switch (type)
            {
                // Twitch
                case EventType.TwitchOnConnected:
                    {
                        ((TwitchOnConnectedEvent)_events[type]).OnEvent(sender, (OnConnectedArgs)args);
                        return;
                    }
                case EventType.TwitchOnDisconnected:
                    {
                        ((TwitchOnDisconnectedEvent)_events[type]).OnEvent(sender, (OnDisconnectedEventArgs)args);
                        return;
                    }
                case EventType.TwitchOnJoinedChannel:
                    {
                        ((TwitchOnJoinedChannelEvent)_events[type]).OnEvent(sender, (OnJoinedChannelArgs)args);
                        return;
                    }
                case EventType.TwitchOnChatMsg:
                    {
                        ((TwitchOnChatMsgEvent)_events[type]).OnEvent(sender, (OnMessageReceivedArgs)args);
                        return;
                    }
                case EventType.TwitchOnWhisperMsg:
                    {
                        ((TwitchOnWhisperMsgEvent)_events[type]).OnEvent(sender, (OnWhisperReceivedArgs)args);
                        return;
                    }
                case EventType.TwitchOnNewSub:
                    {
                        ((TwitchOnNewSubEvent)_events[type]).OnEvent(sender, (OnNewSubscriberArgs)args);
                        return;
                    }
                case EventType.TwitchOnGiftSub:
                    {
                        ((TwitchOnGiftSubEvent)_events[type]).OnEvent(sender, (OnGiftedSubscriptionArgs)args);
                        break;
                    }
                case EventType.TwitchOnContinuedGiftSub:
                    {
                        ((TwitchOnContinuedGiftSubEvent)_events[type]).OnEvent(sender, (OnContinuedGiftedSubscriptionArgs)args);
                        return;
                    }
                case EventType.TwitchOnCommunitySub:
                    {
                        ((TwitchOnCommunitySubEvent)_events[type]).OnEvent(sender, (OnCommunitySubscriptionArgs)args);
                        return;
                    }
                case EventType.TwitchOnPrimeSub:
                    {
                        ((TwitchOnPrimeSubEvent)_events[type]).OnEvent(sender, (OnPrimePaidSubscriberArgs)args);
                        return;
                    }
                case EventType.TwitchOnResub:
                    {
                        ((TwitchOnResubEvent)_events[type]).OnEvent(sender, (OnReSubscriberArgs)args);
                        return;
                    }
                case EventType.TwitchOnVIPsReceived:
                    {
                        ((TwitchOnVIPsReceivedEvent)_events[type]).OnEvent(sender, (OnVIPsReceivedArgs)args);
                        return;
                    }
                case EventType.TwitchOnModeratorsReceived:
                    {
                        ((TwitchOnModeratorsReceivedEvent)_events[type]).OnEvent(sender, (OnModeratorsReceivedArgs)args);
                        return;
                    }
                // Buttplug.IO
                case EventType.ButtplugDeviceAdded:
                    {
                        ((ButtplugDeviceAddedEvent)_events[type]).OnEvent(sender, (DeviceAddedEventArgs)args);
                        return;
                    }
                case EventType.ButtplugDeviceRemoved:
                    {
                        ((ButtplugDeviceRemovedEvent)_events[type]).OnEvent(sender, (DeviceRemovedEventArgs)args);
                        return;
                    }
            }
        }
    }
}
