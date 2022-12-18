using ST.Library.UI.NodeEditor;

namespace ChattyVibes.Nodes.Branch
{
    [STNode("/Branch", "LauraRozier", "", "", "UInt branch node")]
    internal class UIntBranchNode : BranchNode
    {
        private uint _val = 0u;

        protected override void OnCreate()
        {
            _type = typeof(uint);
            base.OnCreate();
            Title = "UInt Branch";
            AutoSize = false;
            Width = 152;
            Height = 80;

            m_op_in.DataTransfer += new STNodeOptionEventHandler(m_in_DataTransfer);
        }

        private void m_in_DataTransfer(object sender, STNodeOptionEventArgs e)
        {
            if (e.Status == ConnectionStatus.Connected && e.TargetOption.Data != null)
                _val = (uint)e.TargetOption.Data;
            else
                _val = 0u;

            HandleCondition();
        }

        protected override void HandleCondition()
        {
            if (_condition)
                m_op_true_out.TransferData(_val);
            else
                m_op_false_out.TransferData(_val);
        }
    }
}
