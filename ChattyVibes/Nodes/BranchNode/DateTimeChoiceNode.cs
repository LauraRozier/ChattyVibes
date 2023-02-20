using ST.Library.UI.NodeEditor;
using System;

namespace ChattyVibes.Nodes.BranchNode
{
    [STNode("/Branch", "LauraRozier", "", "", "DateTime choice node")]
    internal sealed class DateTimeChoiceNode : ChoiceNode
    {
        private DateTime _tval = default(DateTime);
        private DateTime _fval = default(DateTime);

        protected override void OnCreate()
        {
            _type = typeof(DateTime);
            base.OnCreate();
            Title = "DateTime Choice";
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
                    _tval = (DateTime)e.TargetOption.Data;
                else
                    _fval = (DateTime)e.TargetOption.Data;
            }
            else
            {
                if (sender == m_op_true_in)
                    _tval = default(DateTime);
                else
                    _fval = default(DateTime);
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
