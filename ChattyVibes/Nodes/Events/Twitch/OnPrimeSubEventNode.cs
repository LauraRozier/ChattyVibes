using ChattyVibes.Events;
using ST.Library.UI.NodeEditor;
using System;
using TwitchLib.Client.Events;

namespace ChattyVibes.Nodes.Events.Twitch
{
    [STNode("/Events/Twitch", "LauraRozier", "", "", "Twitch OnPrimeSub event node")]
    internal class OnPrimeSubEventNode : BaseEventNode
    {
        private STNodeOption m_op_Channel_out;
        private STNodeOption m_op_DisplayName_out;
        private STNodeOption m_op_Id_out;
        private STNodeOption m_op_IsModerator_out;
        private STNodeOption m_op_IsPartner_out;
        private STNodeOption m_op_IsSubscriber_out;
        private STNodeOption m_op_IsTurbo_out;
        private STNodeOption m_op_MsgId_out;
        private STNodeOption m_op_Months_out;
        private STNodeOption m_op_ShouldShareStreak_out;
        private STNodeOption m_op_StreakMonths_out;
        private STNodeOption m_op_ResubMessage_out;
        private STNodeOption m_op_SubPlan_out;
        private STNodeOption m_op_SystemMsg_out;
        private STNodeOption m_op_UserId_out;
        private STNodeOption m_op_TmiSentTs_out;

        protected override void BindEvent()
        {
            ((TwitchOnPrimeSubEvent)MainForm.EventFactory.GetEvent(EventType.TwitchOnPrimeSub)).RaiseEvent +=
                OnEventNode_RaiseEvent;
        }

        protected override void UnbindEvent()
        {
            ((TwitchOnPrimeSubEvent)MainForm.EventFactory.GetEvent(EventType.TwitchOnPrimeSub)).RaiseEvent -=
                OnEventNode_RaiseEvent;
        }

        protected override void OnCreate()
        {
            base.OnCreate();
            Title = "On Prime Sub";

            m_op_Channel_out = OutputOptions.Add("Channel", typeof(string), false);
            m_op_DisplayName_out = OutputOptions.Add("Display Name", typeof(string), false);
            m_op_Id_out = OutputOptions.Add("ID", typeof(string), false);
            m_op_IsModerator_out = OutputOptions.Add("Is Moderator", typeof(bool), false);
            m_op_IsPartner_out = OutputOptions.Add("Is Partner", typeof(bool), false);
            m_op_IsSubscriber_out = OutputOptions.Add("Is Subscriber", typeof(bool), false);
            m_op_IsTurbo_out = OutputOptions.Add("Is Turbo", typeof(bool), false);
            m_op_MsgId_out = OutputOptions.Add("Message ID", typeof(string), false);
            m_op_Months_out = OutputOptions.Add("Cumulative Months", typeof(string), false);
            m_op_ShouldShareStreak_out = OutputOptions.Add("Should Share Streak", typeof(bool), false);
            m_op_StreakMonths_out = OutputOptions.Add("Streak Months", typeof(string), false);
            m_op_ResubMessage_out = OutputOptions.Add("Resub Message", typeof(string), false);
            m_op_SubPlan_out = OutputOptions.Add("Sub Plan", typeof(string), false);
            m_op_SystemMsg_out = OutputOptions.Add("System Message", typeof(string), false);
            m_op_UserId_out = OutputOptions.Add("UserId", typeof(string), false);
            m_op_TmiSentTs_out = OutputOptions.Add("Timestamp", typeof(DateTime), false);
        }

        private void OnEventNode_RaiseEvent(object sender, OnPrimePaidSubscriberArgs e)
        {
            m_op_Channel_out.TransferData(e.Channel);
            m_op_DisplayName_out.TransferData(e.PrimePaidSubscriber.DisplayName);
            m_op_Id_out.TransferData(e.PrimePaidSubscriber.Id);
            m_op_IsModerator_out.TransferData(e.PrimePaidSubscriber.IsModerator);
            m_op_IsPartner_out.TransferData(e.PrimePaidSubscriber.IsPartner);
            m_op_IsSubscriber_out.TransferData(e.PrimePaidSubscriber.IsSubscriber);
            m_op_IsTurbo_out.TransferData(e.PrimePaidSubscriber.IsTurbo);
            m_op_MsgId_out.TransferData(e.PrimePaidSubscriber.MsgId);
            m_op_Months_out.TransferData(e.PrimePaidSubscriber.MsgParamCumulativeMonths);
            m_op_ShouldShareStreak_out.TransferData(e.PrimePaidSubscriber.MsgParamShouldShareStreak);
            m_op_StreakMonths_out.TransferData(e.PrimePaidSubscriber.MsgParamStreakMonths);
            m_op_ResubMessage_out.TransferData(e.PrimePaidSubscriber.ResubMessage);
            m_op_SubPlan_out.TransferData(e.PrimePaidSubscriber.SubscriptionPlan.ToString());
            m_op_SystemMsg_out.TransferData(e.PrimePaidSubscriber.SystemMessage);
            m_op_UserId_out.TransferData(e.PrimePaidSubscriber.UserId);

            long timestamp = long.Parse(e.PrimePaidSubscriber.TmiSentTs);
            DateTime dt = DateTimeOffset.FromUnixTimeMilliseconds(timestamp).LocalDateTime;
            m_op_TmiSentTs_out.TransferData(dt);

            Trigger();
        }
    }
}
