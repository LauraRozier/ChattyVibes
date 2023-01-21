using ST.Library.UI.NodeEditor;

namespace ChattyVibes.Nodes.BranchNode
{
    [STNode("/Branch", "LauraRozier", "", "", "String choise node")]
    internal sealed class StringChoiseNode : ChoiseNode
    {
        private string _tval = string.Empty;
        private string _fval = string.Empty;

        protected override void OnCreate()
        {
            _type = typeof(string);
            base.OnCreate();
            Title = "String Choise";
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
                    _tval = (string)e.TargetOption.Data;
                else
                    _fval = (string)e.TargetOption.Data;
            }
            else
            {
                if (sender == m_op_true_in)
                    _tval = string.Empty;
                else
                    _fval = string.Empty;
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
