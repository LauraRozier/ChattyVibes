using ST.Library.UI.NodeEditor;
using System;

namespace ChattyVibes.Nodes.BranchNode
{
    [STNode("/Branch", "LauraRozier", "", "", "Enum branch node")]
    internal sealed class EnumBranchNode : BranchNode
    {
        private Enum _val = null;

        protected override void OnCreate()
        {
            _type = typeof(Enum);
            base.OnCreate();
            Title = "Enum Branch";
            AutoSize = false;
            Width = 152;
            Height = 80;

            m_op_in.DataTransfer += new STNodeOptionEventHandler(m_in_DataTransfer);
        }

        private void m_in_DataTransfer(object sender, STNodeOptionEventArgs e)
        {
            if (e.Status == ConnectionStatus.Connected && e.TargetOption.Data != null)
                _val = (Enum)e.TargetOption.Data;
            else
                _val = null;

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
