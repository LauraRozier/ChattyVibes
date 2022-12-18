using ST.Library.UI.NodeEditor;

namespace ChattyVibes.Nodes.Number.Int
{
    [STNode("/Number/Int", "LauraRozier", "", "", "Int input node")]
    internal class IntInputNode : IntNode
    {
        private STNodeOption m_op_out;

        private int _value = 0;
        [STNodeProperty("Value", "The input value")]
        public int Value
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
            Title = "Int";

            m_op_out = OutputOptions.Add("0", typeof(int), false);

            m_op_out.TransferData(_value);
        }
    }
}
