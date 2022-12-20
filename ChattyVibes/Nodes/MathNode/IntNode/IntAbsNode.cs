using ST.Library.UI.NodeEditor;
using System;

namespace ChattyVibes.Nodes.MathNode.IntNode
{
    [STNode("/Math/Int", "LauraRozier", "", "", "Returns the absolute value of a specified number.")]
    internal class IntAbsNode : BaseIntNode
    {
        private int _val = 0;

        private STNodeOption m_in;
        private STNodeOption m_out;

        protected override void OnCreate()
        {
            base.OnCreate();
            Title = "Int Abs";

            m_in = InputOptions.Add("0", typeof(int), true);
            m_out = OutputOptions.Add("0", typeof(int), false);

            m_in.DataTransfer += new STNodeOptionEventHandler(m_in_DataTransfer);

            ProcessResult();
        }

        private void m_in_DataTransfer(object sender, STNodeOptionEventArgs e)
        {
            if (e.Status == ConnectionStatus.Connected && e.TargetOption.Data != null)
                _val = (int)e.TargetOption.Data;
            else
                _val = 0;

            ProcessResult();
        }

        private void ProcessResult()
        {
            int result = Math.Abs(_val);
            SetOptionText(m_in, _val.ToString());
            SetOptionText(m_out, result.ToString());
            m_out.TransferData(result);
        }
    }
}
