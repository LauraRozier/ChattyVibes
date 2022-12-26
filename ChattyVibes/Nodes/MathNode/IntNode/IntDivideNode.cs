using ST.Library.UI.NodeEditor;
using System.Drawing;
using System.Globalization;

namespace ChattyVibes.Nodes.MathNode.IntNode
{
    [STNode("/Math/Int", "LauraRozier", "", "", "This node can get two numbers divide result")]
    internal class IntDivideNode : Nodes.IntNode
    {
        private int _aVal = 0;
        private int _bVal = 0;

        private STNodeOption m_in_A;
        private STNodeOption m_in_B;
        private STNodeOption m_out;

        protected override void OnCreate()
        {
            base.OnCreate();
            Title = "Int Divide";

            m_in_A = InputOptions.Add("0", typeof(int), true);
            m_in_B = InputOptions.Add("0", typeof(int), true);
            m_out = OutputOptions.Add("0", typeof(float), false);

            m_in_A.DataTransfer += new STNodeOptionEventHandler(m_in_DataTransfer);
            m_in_B.DataTransfer += new STNodeOptionEventHandler(m_in_DataTransfer);
            m_out.TransferData((float)_aVal);
        }

        void m_in_DataTransfer(object sender, STNodeOptionEventArgs e)
        {
            if (e.Status == ConnectionStatus.Connected && e.TargetOption.Data != null)
            {
                if (sender == m_in_A)
                    _aVal = (int)e.TargetOption.Data;
                else
                    _bVal = (int)e.TargetOption.Data;
            }
            else
            {
                if (sender == m_in_A)
                    _aVal = 0;
                else
                    _bVal = 0;
            }

            ProcessResult();
        }

        protected override void OnDrawOptionText(DrawingTools dt, STNodeOption op)
        {
            base.OnDrawOptionText(dt, op);

            if (op == m_in_A)
                dt.Graphics.DrawString("/", Font, Brushes.White, new Rectangle(
                    op.TextRectangle.X, op.TextRectangle.Y + 10,
                    op.TextRectangle.Width, op.TextRectangle.Height
                ), _sf);
        }

        private void ProcessResult()
        {
            float result = _bVal == 0 ? 0.0f : (float)_aVal / _bVal;
            SetOptionText(m_in_A, _aVal.ToString());
            SetOptionText(m_in_B, _bVal.ToString());
            SetOptionText(m_out, result.ToString("G", CultureInfo.InvariantCulture));
            m_out.TransferData(result);
        }
    }
}
