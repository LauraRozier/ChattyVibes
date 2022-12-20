using ST.Library.UI.NodeEditor;

namespace ChattyVibes.Nodes.CharNode
{
    [STNode("/Char", "LauraRozier", "", "", "Char input node")]
    internal sealed class CharInputNode : CharNode
    {
        private STNodeOption m_op_out;

        private char _value = default;
        [STNodeProperty("Value", "The input value")]
        public char Value
        {
            get { return _value; }
            set
            {
                _value = value;
                SetOptionText(m_op_out, _value.ToString());
                m_op_out.TransferData(value);
            }
        }

        protected override void OnCreate()
        {
            base.OnCreate();
            Title = "Char";

            m_op_out = OutputOptions.Add("", typeof(char), false);

            m_op_out.TransferData(_value);
        }
    }
}
