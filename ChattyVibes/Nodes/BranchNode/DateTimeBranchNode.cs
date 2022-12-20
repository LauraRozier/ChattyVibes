using ST.Library.UI.NodeEditor;
using System;

namespace ChattyVibes.Nodes.BranchNode
{
    [STNode("/Branch", "LauraRozier", "", "", "DateTime branch node")]
    internal sealed class DateTimeBranchNode : BranchNode
    {
        private DateTime _val = default(DateTime);

        protected override void OnCreate()
        {
            _type = typeof(DateTime);
            base.OnCreate();
            Title = "DateTime Branch";
            AutoSize = false;
            Width = 152;
            Height = 80;

            m_op_in.DataTransfer += new STNodeOptionEventHandler(m_in_DataTransfer);
        }

        private void m_in_DataTransfer(object sender, STNodeOptionEventArgs e)
        {
            if (e.Status == ConnectionStatus.Connected && e.TargetOption.Data != null)
                _val = (DateTime)e.TargetOption.Data;
            else
                _val = default(DateTime);

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
