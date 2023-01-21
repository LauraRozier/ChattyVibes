using ST.Library.UI.NodeEditor;

namespace ChattyVibes.Nodes.StringNode
{
    [STNode("/String", "LauraRozier", "", "", "String input node")]
    internal sealed class StringInputNode : StringNode
    {
        private STNodeOption m_op_out;

        private string _value = string.Empty;
        [STNodeProperty("Value", "The input value")]
        public string Value
        {
            get { return _value; }
            set
            {
                _value = value;
                SetOptionText(m_op_out, _value);
                m_op_out.TransferData(value);
            }
        }

        protected override void OnCreate()
        {
            base.OnCreate();
            Title = "String";

            m_op_out = OutputOptions.Add(string.Empty, typeof(string), false);

            m_op_out.TransferData(_value);
        }
    }
}
