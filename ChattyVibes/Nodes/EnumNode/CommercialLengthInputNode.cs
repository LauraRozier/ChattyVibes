using ST.Library.UI.NodeEditor;
using System.Drawing;
using TwitchLib.Client.Enums;

namespace ChattyVibes.Nodes.EnumNode
{
    [STNode("/Enum", "LauraRozier", "", "", "CommercialLength value node")]
    internal sealed class CommercialLengthInputNode : EnumNode
    {
        private CommercialLength _value = CommercialLength.Seconds60;
        [STNodeProperty("Value", "The boolean value")]
        public CommercialLength Value
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
            Title = "CommercialLength";

            m_op_out = OutputOptions.Add("", typeof(CommercialLength), false);

            m_op_out.TransferData(_value);

            m_ctrl_select = new NodeSelectEnumBox
            {
                DisplayRectangle = new Rectangle(12, 1, 120, 18),
                Enum = _value
            };
            m_ctrl_select.ValueChanged += (s, e) =>
            {
                _value = (CommercialLength)m_ctrl_select.Enum;
                Invalidate();
                m_op_out.TransferData(_value);
            };
            Controls.Add(m_ctrl_select);
        }
    }
}
