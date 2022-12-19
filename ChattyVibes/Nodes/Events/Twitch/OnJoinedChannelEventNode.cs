using ChattyVibes.Events;
using ST.Library.UI.NodeEditor;
using TwitchLib.Client.Events;

namespace ChattyVibes.Nodes.Events.Twitch
{
    [STNode("/Events/Twitch", "LauraRozier", "", "", "Twitch OnJoinedChannel event node")]
    internal class OnJoinedChannelEventNode : BaseEventNode
    {
        private STNodeOption m_op_Channel_out;
        private STNodeOption m_op_BotUsername_out;

        protected override void BindEvent()
        {
            ((TwitchOnJoinedChannelEvent)MainForm.EventFactory.GetEvent(EventType.TwitchOnJoinedChannel)).RaiseEvent +=
                OnEventNode_RaiseEvent;
        }

        protected override void UnbindEvent()
        {
            ((TwitchOnJoinedChannelEvent)MainForm.EventFactory.GetEvent(EventType.TwitchOnJoinedChannel)).RaiseEvent -=
                OnEventNode_RaiseEvent;
        }

        protected override void OnCreate()
        {
            base.OnCreate();
            Title = "On Joined Channel";

            m_op_Channel_out = OutputOptions.Add("Channel", typeof(string), false);
            m_op_BotUsername_out = OutputOptions.Add("Bot Username", typeof(string), false);
        }

        private void OnEventNode_RaiseEvent(object sender, OnJoinedChannelArgs e)
        {
            m_op_Channel_out.TransferData(e.Channel);
            m_op_BotUsername_out.TransferData(e.BotUsername);
            Trigger();
        }
    }
}
