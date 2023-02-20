using ST.Library.UI.NodeEditor;

namespace ChattyVibes.Nodes.BranchNode
{
    [STNode("/Branch", "LauraRozier", "", "", "Boolean choice node")]
    internal sealed class BoolChoiceNode : ChoiceNode
    {
        private bool _tval = false;
        private bool _fval = false;

        protected override void OnCreate()
        {
            _type = typeof(bool);
            base.OnCreate();
            Title = "Bool Choice";
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
                    _tval = (bool)e.TargetOption.Data;
                else
                    _fval = (bool)e.TargetOption.Data;
            }
            else
            {
                if (sender == m_op_true_in)
                    _tval = false;
                else
                    _fval = false;
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
