using ST.Library.UI.NodeEditor;

namespace ChattyVibes.Nodes.BranchNode
{
    [STNode("/Branch", "LauraRozier", "", "", "Int choise node")]
    internal sealed class IntChoiseNode : ChoiseNode
    {
        private int _tval = 0;
        private int _fval = 0;

        protected override void OnCreate()
        {
            _type = typeof(int);
            base.OnCreate();
            Title = "Int Choise";
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
                    _tval = (int)e.TargetOption.Data;
                else
                    _fval = (int)e.TargetOption.Data;
            }
            else
            {
                if (sender == m_op_true_in)
                    _tval = 0;
                else
                    _fval = 0;
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
