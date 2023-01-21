using ChattyVibes.Events;
using ST.Library.UI.NodeEditor;
using System;
using TwitchLib.Client.Events;

namespace ChattyVibes.Nodes.EventNode.TwitchNode
{
    [STNode("/Events/Twitch", "LauraRozier", "", "", "Twitch OnChatMessage event node")]
    internal sealed class OnChatMsgEventNode : EventNode
    {
        private string _channel = string.Empty;
        [STNodeProperty("Channel", "(Optional) The channel to handle the event for")]
        public string Channel
        {
            get { return _channel; }
            set
            {
                _channel = value;
                Invalidate();
            }
        }

        private STNodeOption m_op_in_channel;
        private STNodeOption m_op_Bits_out;
        private STNodeOption m_op_Channel_out;
        private STNodeOption m_op_CustomRewardId_out;
        private STNodeOption m_op_DisplayName_out;
        private STNodeOption m_op_Id_out;
        private STNodeOption m_op_IsBroadcaster_out;
        private STNodeOption m_op_IsFirstMessage_out;
        private STNodeOption m_op_IsHighlighted_out;
        private STNodeOption m_op_IsMe_out;
        private STNodeOption m_op_IsModerator_out;
        private STNodeOption m_op_IsPartner_out;
        private STNodeOption m_op_IsStaff_out;
        private STNodeOption m_op_IsSubscriber_out;
        private STNodeOption m_op_IsTurbo_out;
        private STNodeOption m_op_IsVip_out;
        private STNodeOption m_op_Message_out;
        private STNodeOption m_op_SubscribedMonthCount_out;
        private STNodeOption m_op_TmiSentTs_out;

        protected override void BindEvent()
        {
            ((TwitchOnChatMsgEvent)MainForm.EventFactory.GetEvent(EventType.TwitchOnChatMsg)).RaiseEvent +=
                OnEventNode_RaiseEvent;
        }

        protected override void UnbindEvent()
        {
            ((TwitchOnChatMsgEvent)MainForm.EventFactory.GetEvent(EventType.TwitchOnChatMsg)).RaiseEvent -=
                OnEventNode_RaiseEvent;
        }

        protected override void OnCreate()
        {
            base.OnCreate();
            Title = "On Chat Message";

            m_op_in_channel = InputOptions.Add("Channel", typeof(string), true);
            m_op_Bits_out = OutputOptions.Add("Bits", typeof(int), false);
            m_op_Channel_out = OutputOptions.Add("Channel", typeof(string), false);
            m_op_CustomRewardId_out = OutputOptions.Add("Custom Reward ID", typeof(string), false);
            m_op_DisplayName_out = OutputOptions.Add("Display Name", typeof(string), false);
            m_op_Id_out = OutputOptions.Add("Message ID", typeof(string), false);
            m_op_IsBroadcaster_out = OutputOptions.Add("Is Broadcaster", typeof(bool), false);
            m_op_IsFirstMessage_out = OutputOptions.Add("Is First Message", typeof(bool), false);
            m_op_IsHighlighted_out = OutputOptions.Add("Is Highlighted", typeof(bool), false);
            m_op_IsMe_out = OutputOptions.Add("Is Me", typeof(bool), false);
            m_op_IsModerator_out = OutputOptions.Add("Is Moderator", typeof(bool), false);
            m_op_IsPartner_out = OutputOptions.Add("Is Partner", typeof(bool), false);
            m_op_IsStaff_out = OutputOptions.Add("Is Staff", typeof(bool), false);
            m_op_IsSubscriber_out = OutputOptions.Add("Is Subscriber", typeof(bool), false);
            m_op_IsTurbo_out = OutputOptions.Add("Is Turbo", typeof(bool), false);
            m_op_IsVip_out = OutputOptions.Add("Is Vip", typeof(bool), false);
            m_op_Message_out = OutputOptions.Add("Message", typeof(string), false);
            m_op_SubscribedMonthCount_out = OutputOptions.Add("Subscribed Month Count", typeof(int), false);
            m_op_TmiSentTs_out = OutputOptions.Add("Timestamp", typeof(DateTime), false);

            m_op_in_channel.DataTransfer += new STNodeOptionEventHandler(m_op_in_DataTransfer);
        }

        private void m_op_in_DataTransfer(object sender, STNodeOptionEventArgs e)
        {
            if (e.Status == ConnectionStatus.Connected && e.TargetOption.Data != null)
                Channel = (string)e.TargetOption.Data;
            else
                Channel = string.Empty;
        }

        private void OnEventNode_RaiseEvent(object sender, OnMessageReceivedArgs e)
        {
            if (!string.IsNullOrWhiteSpace(_channel))
                if (!_channel.Equals(e.ChatMessage.Channel))
                    return;

            m_op_Bits_out.TransferData(e.ChatMessage.Bits);
            m_op_Channel_out.TransferData(e.ChatMessage.Channel);
            m_op_CustomRewardId_out.TransferData(e.ChatMessage.CustomRewardId);
            m_op_DisplayName_out.TransferData(e.ChatMessage.DisplayName);
            m_op_Id_out.TransferData(e.ChatMessage.Id);
            m_op_IsBroadcaster_out.TransferData(e.ChatMessage.IsBroadcaster);
            m_op_IsFirstMessage_out.TransferData(e.ChatMessage.IsFirstMessage);
            m_op_IsHighlighted_out.TransferData(e.ChatMessage.IsHighlighted);
            m_op_IsMe_out.TransferData(e.ChatMessage.IsMe);
            m_op_IsModerator_out.TransferData(e.ChatMessage.IsModerator);
            m_op_IsPartner_out.TransferData(e.ChatMessage.IsPartner);
            m_op_IsStaff_out.TransferData(e.ChatMessage.IsStaff);
            m_op_IsSubscriber_out.TransferData(e.ChatMessage.IsSubscriber);
            m_op_IsTurbo_out.TransferData(e.ChatMessage.IsTurbo);
            m_op_IsVip_out.TransferData(e.ChatMessage.IsVip);
            m_op_Message_out.TransferData(e.ChatMessage.Message);
            m_op_SubscribedMonthCount_out.TransferData(e.ChatMessage.SubscribedMonthCount);

            long timestamp = long.Parse(e.ChatMessage.TmiSentTs);
            DateTime dt = DateTimeOffset.FromUnixTimeMilliseconds(timestamp).LocalDateTime;
            m_op_TmiSentTs_out.TransferData(dt);

            Trigger();
        }
    }
}
