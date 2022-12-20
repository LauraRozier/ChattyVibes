using ST.Library.UI.NodeEditor;

namespace ChattyVibes.Nodes.BranchNode
{
    [STNode("/Branch", "LauraRozier", "", "", "Float branch node")]
    internal sealed class FloatBranchNode : BranchNode
    {
        private float _val = 0.0f;

        protected override void OnCreate()
        {
            _type = typeof(float);
            base.OnCreate();
            Title = "Float Branch";
            AutoSize = false;
            Width = 152;
            Height = 80;

            m_op_in.DataTransfer += new STNodeOptionEventHandler(m_in_DataTransfer);
        }

        private void m_in_DataTransfer(object sender, STNodeOptionEventArgs e)
        {
            if (e.Status == ConnectionStatus.Connected && e.TargetOption.Data != null)
                _val = (float)e.TargetOption.Data;
            else
                _val = 0.0f;

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
