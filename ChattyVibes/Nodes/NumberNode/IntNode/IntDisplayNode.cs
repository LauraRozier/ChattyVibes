using ST.Library.UI.NodeEditor;

namespace ChattyVibes.Nodes.NumberNode.IntNode
{
    [STNode("/Number/Int", "LauraRozier", "", "", "Int display node")]
    internal class IntDisplayNode : Nodes.IntNode
    {
        private int _value = 0;

        private STNodeOption m_op_in;
        private STNodeOption m_op_out;

        protected override void OnCreate()
        {
            base.OnCreate();
            Title = "Int Display";

            m_op_in = InputOptions.Add("0", typeof(int), true);
            m_op_out = OutputOptions.Add(string.Empty, typeof(int), false);

            m_op_in.DataTransfer += new STNodeOptionEventHandler(m_in_DataTransfer);
            m_op_out.TransferData(_value);
        }

        private void m_in_DataTransfer(object sender, STNodeOptionEventArgs e)
        {
            if (e.Status == ConnectionStatus.Connected && e.TargetOption.Data != null)
                _value = (int)e.TargetOption.Data;
            else
                _value = 0;

            SetOptionText(m_op_in, _value.ToString());
            m_op_out.TransferData(_value);
        }
    }
}
