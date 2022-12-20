using ChattyVibes.Events;
using ST.Library.UI.NodeEditor;
using TwitchLib.Client.Events;

namespace ChattyVibes.Nodes.EventNode.TwitchNode
{
    [STNode("/Events/Twitch", "LauraRozier", "", "", "Twitch OnConnected event node")]
    internal sealed class OnConnectedEventNode : BaseEventNode
    {
        private STNodeOption m_op_AutoJoinChannel_out;
        private STNodeOption m_op_BotUsername_out;

        protected override void BindEvent()
        {
            ((TwitchOnConnectedEvent)MainForm.EventFactory.GetEvent(EventType.TwitchOnConnected)).RaiseEvent +=
                OnEventNode_RaiseEvent;
        }

        protected override void UnbindEvent()
        {
            ((TwitchOnConnectedEvent)MainForm.EventFactory.GetEvent(EventType.TwitchOnConnected)).RaiseEvent -=
                OnEventNode_RaiseEvent;
        }

        protected override void OnCreate()
        {
            base.OnCreate();
            Title = "On Connected";

            m_op_AutoJoinChannel_out = OutputOptions.Add("Auto-join Channel", typeof(string), false);
            m_op_BotUsername_out = OutputOptions.Add("Bot Username", typeof(string), false);
        }

        private void OnEventNode_RaiseEvent(object sender, OnConnectedArgs e)
        {
            m_op_AutoJoinChannel_out.TransferData(e.AutoJoinChannel);
            m_op_BotUsername_out.TransferData(e.BotUsername);
            Trigger();
        }
    }
}
