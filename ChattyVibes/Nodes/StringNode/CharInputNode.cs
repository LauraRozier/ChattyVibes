using ST.Library.UI.NodeEditor;
using System.Drawing;

namespace ChattyVibes.Nodes.StringNode
{
    [STNode("/String", "LauraRozier", "", "", "Char input node")]
    internal sealed class CharInputNode : StringNode
    {
        private STNodeOption m_op_out;

        private char _value = 'A';
        [STNodeProperty("Value", "The input value")]
        public char Value
        {
            get { return _value; }
            set
            {
                _value = value;
                SetOptionText(m_op_out, $"{_value}");
                m_op_out.TransferData(value);
            }
        }

        protected override void OnCreate()
        {
            base.OnCreate();
            TitleColor = Color.FromArgb(200, Constants.C_COLOR_CHAR);
            Title = "Char";

            m_op_out = OutputOptions.Add($"{_value}", typeof(char), false);

            m_op_out.TransferData(_value);
        }
    }
}
