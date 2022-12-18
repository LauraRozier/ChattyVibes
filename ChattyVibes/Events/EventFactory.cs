using System.Collections.Generic;

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

        // Buttplug.IO events
        ButtplugConnected,
        ButtplugDisconnected,
        ButtplugDeviceAdded,
        ButtplugDeviceRemoved,

        // App events

    }

    internal class EventFactory
    {
        private readonly Dictionary<EventType, BaseEvent> _events = new Dictionary<EventType, BaseEvent>();

        public BaseEvent GetEvent(EventType eventType) =>
            _events[eventType];

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

            //_events.Add(EventType.ButtplugConnected, new ButtplugConnectedEvent());
            //_events.Add(EventType.ButtplugDisconnected, new ButtplugDisconnectedEvent());
            //_events.Add(EventType.ButtplugDeviceAdded, new ButtplugDeviceAddedEvent());
            //_events.Add(EventType.ButtplugDeviceRemoved, new ButtplugDeviceRemovedEvent());
        }
    }
}
