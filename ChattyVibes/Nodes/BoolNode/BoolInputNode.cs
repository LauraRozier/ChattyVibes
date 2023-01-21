using ST.Library.UI.NodeEditor;
using System.Drawing;

namespace ChattyVibes.Nodes.BoolNode
{
    [STNode("/Bool", "LauraRozier", "", "", "Boolean value node")]
    internal sealed class BoolInputNode : BoolNode
    {
        private bool _value = false;
        [STNodeProperty("Value", "The boolean value")]
        public bool Value
        {
            get { return _value; }
            set
            {
                _value = value;
                m_ctrl_checkbox.Checked = value;
                Invalidate();
                m_op_out.TransferData(value);
            }
        }

        private NodeCheckBox m_ctrl_checkbox;

        private STNodeOption m_op_out;

        protected override void OnCreate()
        {
            base.OnCreate();
            Title = "Boolean";
            AutoSize = false;
            Size = new Size(100, 40);

            m_op_out = OutputOptions.Add(string.Empty, typeof(bool), false);

            m_ctrl_checkbox = new NodeCheckBox
            {
                Text = "Value",
                DisplayRectangle = new Rectangle(10, 0, 60, 20)
            };
            m_ctrl_checkbox.ValueChanged += (s, e) =>
            {
                _value = m_ctrl_checkbox.Checked;
                m_op_out.TransferData(_value);
            };
            Controls.Add(m_ctrl_checkbox);

            m_op_out.TransferData(_value);
        }
    }
}
