using ChattyVibes.Events;
using ST.Library.UI.NodeEditor;
using TwitchLib.Client.Events;
using TwitchLib.Communication.Events;

namespace ChattyVibes.Nodes.Events.Twitch
{
    [STNode("/Events/Twitch", "LauraRozier", "", "", "Twitch OnDisconnected event node")]
    internal class OnDisconnectedEventNode : BaseEventNode
    {
        protected override void BindEvent()
        {
            ((TwitchOnDisconnectedEvent)MainForm.EventFactory.GetEvent(EventType.TwitchOnDisconnected)).RaiseEvent +=
                OnEventNode_RaiseEvent;
        }

        protected override void UnbindEvent()
        {
            ((TwitchOnDisconnectedEvent)MainForm.EventFactory.GetEvent(EventType.TwitchOnDisconnected)).RaiseEvent -=
                OnEventNode_RaiseEvent;
        }

        protected override void OnCreate()
        {
            base.OnCreate();
            Title = "On OnDisconnected";
        }

        private void OnEventNode_RaiseEvent(object sender, OnDisconnectedEventArgs e)
        {
            Trigger();
        }
    }
}
