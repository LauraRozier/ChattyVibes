using ST.Library.UI.NodeEditor;
using System.Drawing;
using TwitchLib.Client.Enums;

namespace ChattyVibes.Nodes.EnumNode
{
    [STNode("/Enum", "LauraRozier", "", "", "SubscriptionPlan value node")]
    internal sealed class SubscriptionPlanInputNode : EnumNode
    {
        private SubscriptionPlan _value = SubscriptionPlan.NotSet;
        [STNodeProperty("Value", "The boolean value")]
        public SubscriptionPlan Value
        {
            get { return _value; }
            set
            {
                _value = value;
                m_ctrl_select.Enum = value;
                Invalidate();
                m_op_out.TransferData(value);
            }
        }

        private NodeSelectEnumBox m_ctrl_select;

        private STNodeOption m_op_out;

        protected override void OnCreate()
        {
            base.OnCreate();
            Title = "SubscriptionPlan";

            m_op_out = OutputOptions.Add(string.Empty, typeof(SubscriptionPlan), false);

            m_op_out.TransferData(_value);

            m_ctrl_select = new NodeSelectEnumBox
            {
                DisplayRectangle = new Rectangle(12, 1, 120, 18),
                Enum = _value
            };
            m_ctrl_select.ValueChanged += (s, e) =>
            {
                _value = (SubscriptionPlan)m_ctrl_select.Enum;
                Invalidate();
                m_op_out.TransferData(_value);
            };
            Controls.Add(m_ctrl_select);
        }
    }
}
