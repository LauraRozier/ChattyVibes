using ST.Library.UI.NodeEditor;
using System.Drawing;

namespace ChattyVibes.Nodes.MathNode.UIntNode
{
    [STNode("/Math/UInt", "LauraRozier", "", "", "This node can get two numbers multiply result")]
    internal class UIntMultiplyNode : Nodes.UIntNode
    {
        private uint _aVal = 0u;
        private uint _bVal = 0u;

        private STNodeOption m_in_A;
        private STNodeOption m_in_B;
        private STNodeOption m_out;

        protected override void OnCreate()
        {
            base.OnCreate();
            Title = "UInt Multiply";

            m_in_A = InputOptions.Add("", typeof(uint), true);
            m_in_B = InputOptions.Add("", typeof(uint), true);
            m_out = OutputOptions.Add("", typeof(uint), false);

            m_in_A.DataTransfer += new STNodeOptionEventHandler(m_in_num_DataTransfer);
            m_in_B.DataTransfer += new STNodeOptionEventHandler(m_in_num_DataTransfer);

            ProcessResult();
        }

        void m_in_num_DataTransfer(object sender, STNodeOptionEventArgs e)
        {
            if (e.Status == ConnectionStatus.Connected && e.TargetOption.Data != null)
            {
                if (sender == m_in_A)
                    _aVal = (uint)e.TargetOption.Data;
                else
                    _bVal = (uint)e.TargetOption.Data;
            }
            else
            {
                if (sender == m_in_A)
                    _aVal = 0u;
                else
                    _bVal = 0u;
            }

            ProcessResult();
        }

        protected override void OnDrawOptionText(DrawingTools dt, STNodeOption op)
        {
            base.OnDrawOptionText(dt, op);

            if (op == m_in_A)
                dt.Graphics.DrawString("*", Font, Brushes.White, new Rectangle(
                    op.TextRectangle.X, op.TextRectangle.Y + 10,
                    op.TextRectangle.Width, op.TextRectangle.Height
                ), _sf);
        }

        private void ProcessResult()
        {
            uint result = _aVal * _bVal;
            SetOptionText(m_in_A, _aVal.ToString());
            SetOptionText(m_in_B, _bVal.ToString());
            SetOptionText(m_out, result.ToString());
            m_out.TransferData(result);
        }
    }
}
