using ST.Library.UI.NodeEditor;

namespace ChattyVibes.Nodes.BranchNode
{
    [STNode("/Branch", "LauraRozier", "", "", "Float choice node")]
    internal sealed class FloatChoiceNode : ChoiceNode
    {
        private float _tval = 0.0f;
        private float _fval = 0.0f;

        protected override void OnCreate()
        {
            _type = typeof(float);
            base.OnCreate();
            Title = "Float Choice";
            AutoSize = false;
            Width = 152;
            Height = 80;

            m_op_true_in.DataTransfer += new STNodeOptionEventHandler(m_in_DataTransfer);
            m_op_false_in.DataTransfer += new STNodeOptionEventHandler(m_in_DataTransfer);
        }

        private void m_in_DataTransfer(object sender, STNodeOptionEventArgs e)
        {
            if (e.Status == ConnectionStatus.Connected && e.TargetOption.Data != null)
            {
                if (sender == m_op_true_in)
                    _tval = (float)e.TargetOption.Data;
                else
                    _fval = (float)e.TargetOption.Data;
            }
            else
            {
                if (sender == m_op_true_in)
                    _tval = 0.0f;
                else
                    _fval = 0.0f;
            }

            HandleCondition();
        }

        protected override void HandleCondition()
        {
            if (_condition)
                m_op_true_out.TransferData(_tval);
            else
                m_op_false_out.TransferData(_fval);
        }
    }
}
