using ST.Library.UI.NodeEditor;

namespace ChattyVibes.Nodes.NumberNode.UIntNode
{
    [STNode("/Number/UInt", "LauraRozier", "", "", "UInt input node")]
    internal class UIntInputNode : BaseUIntNode
    {
        private STNodeOption m_op_out;

        private uint _value = 0u;
        [STNodeProperty("Value", "The input value")]
        public uint Value
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
            Title = "UInt";

            m_op_out = OutputOptions.Add("0", typeof(uint), false);

            m_op_out.TransferData(_value);
        }
    }
}
