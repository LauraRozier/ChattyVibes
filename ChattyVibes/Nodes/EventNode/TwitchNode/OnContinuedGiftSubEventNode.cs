using ChattyVibes.Events;
using ST.Library.UI.NodeEditor;
using System;
using TwitchLib.Client.Events;

namespace ChattyVibes.Nodes.EventNode.TwitchNode
{
    [STNode("/Events/Twitch", "LauraRozier", "", "", "Twitch ContinuedGiftSub event node")]
    internal sealed class OnContinuedGiftSubEventNode : EventNode
    {
        private STNodeOption m_op_Channel_out;
        private STNodeOption m_op_DisplayName_out;
        private STNodeOption m_op_Flags_out;
        private STNodeOption m_op_Id_out;
        private STNodeOption m_op_IsModerator_out;
        private STNodeOption m_op_IsSubscriber_out;
        private STNodeOption m_op_MsgId_out;
        private STNodeOption m_op_SystemMsg_out;
        private STNodeOption m_op_UserId_out;
        private STNodeOption m_op_TmiSentTs_out;

        protected override void BindEvent()
        {
            ((TwitchOnContinuedGiftSubEvent)MainForm.EventFactory.GetEvent(EventType.TwitchOnContinuedGiftSub)).RaiseEvent +=
                OnEventNode_RaiseEvent;
        }

        protected override void UnbindEvent()
        {
            ((TwitchOnContinuedGiftSubEvent)MainForm.EventFactory.GetEvent(EventType.TwitchOnContinuedGiftSub)).RaiseEvent -=
                OnEventNode_RaiseEvent;
        }

        protected override void OnCreate()
        {
            base.OnCreate();
            Title = "On Continued Gift Sub";

            m_op_Channel_out = OutputOptions.Add("Channel", typeof(string), false);
            m_op_DisplayName_out = OutputOptions.Add("Display Name", typeof(string), false);
            m_op_Flags_out = OutputOptions.Add("Flags", typeof(string), false);
            m_op_Id_out = OutputOptions.Add("ID", typeof(string), false);
            m_op_IsModerator_out = OutputOptions.Add("Is Moderator", typeof(bool), false);
            m_op_IsSubscriber_out = OutputOptions.Add("Is Subscriber", typeof(bool), false);
            m_op_MsgId_out = OutputOptions.Add("Message ID", typeof(string), false);
            m_op_SystemMsg_out = OutputOptions.Add("System Message", typeof(string), false);
            m_op_UserId_out = OutputOptions.Add("User ID", typeof(string), false);
            m_op_TmiSentTs_out = OutputOptions.Add("Timestamp", typeof(DateTime), false);
        }

        private void OnEventNode_RaiseEvent(object sender, OnContinuedGiftedSubscriptionArgs e)
        {
            m_op_Channel_out.TransferData(e.Channel);
            m_op_DisplayName_out.TransferData(e.ContinuedGiftedSubscription.DisplayName);
            m_op_Flags_out.TransferData(e.ContinuedGiftedSubscription.Flags);
            m_op_Id_out.TransferData(e.ContinuedGiftedSubscription.Id);
            m_op_IsModerator_out.TransferData(e.ContinuedGiftedSubscription.IsModerator);
            m_op_IsSubscriber_out.TransferData(e.ContinuedGiftedSubscription.IsSubscriber);
            m_op_MsgId_out.TransferData(e.ContinuedGiftedSubscription.MsgId);
            m_op_SystemMsg_out.TransferData(e.ContinuedGiftedSubscription.SystemMsg);
            m_op_UserId_out.TransferData(e.ContinuedGiftedSubscription.UserId);

            long timestamp = long.Parse(e.ContinuedGiftedSubscription.TmiSentTs);
            DateTime dt = DateTimeOffset.FromUnixTimeMilliseconds(timestamp).LocalDateTime;
            m_op_TmiSentTs_out.TransferData(dt);

            Trigger();
        }
    }
}
