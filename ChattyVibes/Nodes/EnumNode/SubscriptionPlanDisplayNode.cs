using ST.Library.UI.NodeEditor;
using TwitchLib.Client.Enums;

namespace ChattyVibes.Nodes.EnumNode
{
    [STNode("/Enum", "LauraRozier", "", "", "SubscriptionPlan display node")]
    internal sealed class SubscriptionPlanDisplayNode : EnumNode
    {
        private SubscriptionPlan _value = SubscriptionPlan.NotSet;

        private STNodeOption m_op_in;
        private STNodeOption m_op_out;

        protected override void OnCreate()
        {
            base.OnCreate();
            Title = "SubscriptionPlan Display";

            m_op_in = InputOptions.Add("NotSet", typeof(SubscriptionPlan), true);
            m_op_out = OutputOptions.Add(string.Empty, typeof(SubscriptionPlan), false);

            m_op_in.DataTransfer += new STNodeOptionEventHandler(m_op_in_DataTransfer);
            m_op_out.TransferData(_value);
        }

        private void m_op_in_DataTransfer(object sender, STNodeOptionEventArgs e)
        {
            if (e.Status == ConnectionStatus.Connected && e.TargetOption.Data != null)
                _value = (SubscriptionPlan)e.TargetOption.Data;
            else
                _value = SubscriptionPlan.NotSet;

            SetOptionText(m_op_in, _value.ToString());
            m_op_out.TransferData(_value);
        }
    }
}
