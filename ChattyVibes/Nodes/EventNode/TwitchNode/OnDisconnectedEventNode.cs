using ChattyVibes.Events;
using ST.Library.UI.NodeEditor;
using TwitchLib.Communication.Events;

namespace ChattyVibes.Nodes.EventNode.TwitchNode
{
    [STNode("/Events/Twitch", "LauraRozier", "", "", "Twitch OnDisconnected event node")]
    internal sealed class OnDisconnectedEventNode : EventNode
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
