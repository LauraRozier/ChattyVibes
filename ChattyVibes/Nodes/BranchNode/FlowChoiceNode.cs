using ST.Library.UI.NodeEditor;

namespace ChattyVibes.Nodes.BranchNode
{
    [STNode("/Branch", "LauraRozier", "", "", "Flow choice node")]
    internal sealed class FlowChoiceNode : ChoiceNode
    {
        private object _tval = null;
        private object _fval = null;

        protected override void OnCreate()
        {
            _type = typeof(object);
            base.OnCreate();
            Title = "Flow Choice";
            AutoSize = false;
            Width = 152;
            Height = 80;

            m_op_true_in.DataTransfer += new STNodeOptionEventHandler(m_in_DataTransfer);
            m_op_false_in.DataTransfer += new STNodeOptionEventHandler(m_in_DataTransfer);
        }

        private void m_in_DataTransfer(object sender, STNodeOptionEventArgs e)
        {
            if (e.Status == ConnectionStatus.Connected)
            {
                if (sender == m_op_true_in)
                    _tval = e.TargetOption.Data;
                else
                    _fval = e.TargetOption.Data;
            }
            else
            {
                if (sender == m_op_true_in)
                    _tval = null;
                else
                    _fval = null;
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
