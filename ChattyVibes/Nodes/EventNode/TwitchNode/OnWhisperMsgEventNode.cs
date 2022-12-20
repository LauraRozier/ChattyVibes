using ChattyVibes.Events;
using ST.Library.UI.NodeEditor;
using TwitchLib.Client.Events;

namespace ChattyVibes.Nodes.EventNode.TwitchNode
{
    [STNode("/Events/Twitch", "LauraRozier", "", "", "Twitch OnWhisperMsg event node")]
    internal sealed class OnWhisperMsgEventNode : BaseEventNode
    {
        private STNodeOption m_op_BotUsername_out;
        private STNodeOption m_op_DisplayName_out;
        private STNodeOption m_op_IsTurbo_out;
        private STNodeOption m_op_Message_out;
        private STNodeOption m_op_MessageId_out;
        private STNodeOption m_op_UserId_out;

        protected override void BindEvent()
        {
            ((TwitchOnWhisperMsgEvent)MainForm.EventFactory.GetEvent(EventType.TwitchOnWhisperMsg)).RaiseEvent +=
                OnEventNode_RaiseEvent;
        }

        protected override void UnbindEvent()
        {
            ((TwitchOnWhisperMsgEvent)MainForm.EventFactory.GetEvent(EventType.TwitchOnWhisperMsg)).RaiseEvent -=
                OnEventNode_RaiseEvent;
        }

        protected override void OnCreate()
        {
            base.OnCreate();
            Title = "On Whisper Message";

            m_op_BotUsername_out = OutputOptions.Add("Bot Username", typeof(string), false);
            m_op_DisplayName_out = OutputOptions.Add("Display Name", typeof(string), false);
            m_op_IsTurbo_out = OutputOptions.Add("Is Turbo", typeof(bool), false);
            m_op_Message_out = OutputOptions.Add("Message", typeof(string), false);
            m_op_MessageId_out = OutputOptions.Add("Message ID", typeof(string), false);
            m_op_UserId_out = OutputOptions.Add("User ID", typeof(string), false);
        }

        private void OnEventNode_RaiseEvent(object sender, OnWhisperReceivedArgs e)
        {
            m_op_BotUsername_out.TransferData(e.WhisperMessage.BotUsername);
            m_op_DisplayName_out.TransferData(e.WhisperMessage.DisplayName);
            m_op_IsTurbo_out.TransferData(e.WhisperMessage.IsTurbo);
            m_op_Message_out.TransferData(e.WhisperMessage.Message);
            m_op_MessageId_out.TransferData(e.WhisperMessage.MessageId);
            m_op_UserId_out.TransferData(e.WhisperMessage.UserId);
            Trigger();
        }
    }
}
