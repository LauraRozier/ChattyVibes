using ST.Library.UI.NodeEditor;

namespace ChattyVibes.Nodes.NumberNode.UIntNode
{
    [STNode("/Number/UInt", "LauraRozier", "", "", "UInt display node")]
    internal class UIntDisplayNode : Nodes.UIntNode
    {
        private uint _value = 0u;

        private STNodeOption m_op_in;
        private STNodeOption m_op_out;

        protected override void OnCreate()
        {
            base.OnCreate();
            Title = "UInt Display";

            m_op_in = InputOptions.Add("0", typeof(uint), true);
            m_op_out = OutputOptions.Add(string.Empty, typeof(uint), false);

            m_op_in.DataTransfer += new STNodeOptionEventHandler(m_in_DataTransfer);
            m_op_out.TransferData(_value);
        }

        private void m_in_DataTransfer(object sender, STNodeOptionEventArgs e)
        {
            if (e.Status == ConnectionStatus.Connected && e.TargetOption.Data != null)
                _value = (uint)e.TargetOption.Data;
            else
                _value = 0u;

            SetOptionText(m_op_in, _value.ToString());
            m_op_out.TransferData(_value);
        }
    }
}
