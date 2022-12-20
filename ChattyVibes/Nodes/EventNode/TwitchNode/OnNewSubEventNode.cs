using ChattyVibes.Events;
using ST.Library.UI.NodeEditor;
using System;
using TwitchLib.Client.Events;

namespace ChattyVibes.Nodes.EventNode.TwitchNode
{
    [STNode("/Events/Twitch", "LauraRozier", "", "", "Twitch OnNewSub event node")]
    internal sealed class OnNewSubEventNode : BaseEventNode
    {
        private STNodeOption m_op_Channel_out;
        private STNodeOption m_op_DisplayName_out;
        private STNodeOption m_op_Id_out;
        private STNodeOption m_op_IsModerator_out;
        private STNodeOption m_op_IsPartner_out;
        private STNodeOption m_op_IsSubscriber_out;
        private STNodeOption m_op_IsTurbo_out;
        private STNodeOption m_op_MsgId_out;
        private STNodeOption m_op_CumMonths_out;
        private STNodeOption m_op_ShouldShareStreak_out;
        private STNodeOption m_op_StreakMonths_out;
        private STNodeOption m_op_ResubMessage_out;
        private STNodeOption m_op_SubPlan_out;
        private STNodeOption m_op_SystemMsg_out;
        private STNodeOption m_op_UserId_out;
        private STNodeOption m_op_TmiSentTs_out;

        protected override void BindEvent()
        {
            ((TwitchOnNewSubEvent)MainForm.EventFactory.GetEvent(EventType.TwitchOnNewSub)).RaiseEvent +=
                OnEventNode_RaiseEvent;
        }

        protected override void UnbindEvent()
        {
            ((TwitchOnNewSubEvent)MainForm.EventFactory.GetEvent(EventType.TwitchOnNewSub)).RaiseEvent -=
                OnEventNode_RaiseEvent;
        }

        protected override void OnCreate()
        {
            base.OnCreate();
            Title = "On New Sub";

            m_op_Channel_out = OutputOptions.Add("Channel", typeof(string), false);
            m_op_DisplayName_out = OutputOptions.Add("Display Name", typeof(string), false);
            m_op_Id_out = OutputOptions.Add("ID", typeof(string), false);
            m_op_IsModerator_out = OutputOptions.Add("Is Moderator", typeof(bool), false);
            m_op_IsPartner_out = OutputOptions.Add("Is Partner", typeof(bool), false);
            m_op_IsSubscriber_out = OutputOptions.Add("Is Subscriber", typeof(bool), false);
            m_op_IsTurbo_out = OutputOptions.Add("Is Turbo", typeof(bool), false);
            m_op_MsgId_out = OutputOptions.Add("Message ID", typeof(string), false);
            m_op_CumMonths_out = OutputOptions.Add("Cumulative Months", typeof(int), false);
            m_op_ShouldShareStreak_out = OutputOptions.Add("Should Share Streak", typeof(bool), false);
            m_op_StreakMonths_out = OutputOptions.Add("Streak Months", typeof(int), false);
            m_op_ResubMessage_out = OutputOptions.Add("Resub Message", typeof(string), false);
            m_op_SubPlan_out = OutputOptions.Add("Sub Plan", typeof(Enum), false);
            m_op_SystemMsg_out = OutputOptions.Add("System Message", typeof(string), false);
            m_op_UserId_out = OutputOptions.Add("UserId", typeof(string), false);
            m_op_TmiSentTs_out = OutputOptions.Add("Timestamp", typeof(DateTime), false);
        }

        private void OnEventNode_RaiseEvent(object sender, OnNewSubscriberArgs e)
        {
            m_op_Channel_out.TransferData(e.Channel);
            m_op_DisplayName_out.TransferData(e.Subscriber.DisplayName);
            m_op_Id_out.TransferData(e.Subscriber.Id);
            m_op_IsModerator_out.TransferData(e.Subscriber.IsModerator);
            m_op_IsPartner_out.TransferData(e.Subscriber.IsPartner);
            m_op_IsSubscriber_out.TransferData(e.Subscriber.IsSubscriber);
            m_op_IsTurbo_out.TransferData(e.Subscriber.IsTurbo);
            m_op_MsgId_out.TransferData(e.Subscriber.MsgId);

            if (!int.TryParse(e.Subscriber.MsgParamCumulativeMonths, out int cumMonths))
                cumMonths = 0;

            m_op_CumMonths_out.TransferData(cumMonths);
            m_op_ShouldShareStreak_out.TransferData(e.Subscriber.MsgParamShouldShareStreak);

            if (!int.TryParse(e.Subscriber.MsgParamStreakMonths, out int streakMonths))
                streakMonths = 0;

            m_op_StreakMonths_out.TransferData(streakMonths);
            m_op_ResubMessage_out.TransferData(e.Subscriber.ResubMessage);
            m_op_SubPlan_out.TransferData(e.Subscriber.SubscriptionPlan);
            m_op_SystemMsg_out.TransferData(e.Subscriber.SystemMessage);
            m_op_UserId_out.TransferData(e.Subscriber.UserId);

            long timestamp = long.Parse(e.Subscriber.TmiSentTs);
            DateTime dt = DateTimeOffset.FromUnixTimeMilliseconds(timestamp).LocalDateTime;
            m_op_TmiSentTs_out.TransferData(dt);

            Trigger();
        }
    }
}
