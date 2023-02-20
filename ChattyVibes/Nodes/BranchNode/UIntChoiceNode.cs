using ST.Library.UI.NodeEditor;

namespace ChattyVibes.Nodes.BranchNode
{
    [STNode("/Branch", "LauraRozier", "", "", "UInt choice node")]
    internal sealed class UIntChoiceNode : ChoiceNode
    {
        private uint _tval = 0u;
        private uint _fval = 0u;

        protected override void OnCreate()
        {
            _type = typeof(uint);
            base.OnCreate();
            Title = "UInt Choice";
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
                    _tval = (uint)e.TargetOption.Data;
                else
                    _fval = (uint)e.TargetOption.Data;
            }
            else
            {
                if (sender == m_op_true_in)
                    _tval = 0u;
                else
                    _fval = 0u;
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
