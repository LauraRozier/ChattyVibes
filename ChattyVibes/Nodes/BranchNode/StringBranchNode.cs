using ST.Library.UI.NodeEditor;

namespace ChattyVibes.Nodes.BranchNode
{
    [STNode("/Branch", "LauraRozier", "", "", "String branch node")]
    internal sealed class StringBranchNode : BranchNode
    {
        private string _val = "";

        protected override void OnCreate()
        {
            _type = typeof(string);
            base.OnCreate();
            Title = "String Branch";
            AutoSize = false;
            Width = 152;
            Height = 80;

            m_op_in.DataTransfer += new STNodeOptionEventHandler(m_in_DataTransfer);
        }

        private void m_in_DataTransfer(object sender, STNodeOptionEventArgs e)
        {
            if (e.Status == ConnectionStatus.Connected && e.TargetOption.Data != null)
                _val = (string)e.TargetOption.Data;
            else
                _val = "";

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
