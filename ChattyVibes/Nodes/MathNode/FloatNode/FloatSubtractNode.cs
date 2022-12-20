using ST.Library.UI.NodeEditor;
using System.Drawing;
using System.Globalization;

namespace ChattyVibes.Nodes.MathNode.FloatNode
{
    [STNode("/Math/Float", "LauraRozier", "", "", "This node can get two floats subtract result")]
    internal class FloatSubtractNode : BaseFloatNode
    {
        private float _aVal = 0.0f;
        private float _bVal = 0.0f;

        private STNodeOption m_in_A;
        private STNodeOption m_in_B;
        private STNodeOption m_out;

        protected override void OnCreate()
        {
            base.OnCreate();
            Title = "Float Subtract";

            m_in_A = InputOptions.Add("", typeof(float), true);
            m_in_B = InputOptions.Add("", typeof(float), true);
            m_out = OutputOptions.Add("", typeof(float), false);

            m_in_A.DataTransfer += new STNodeOptionEventHandler(m_in_num_DataTransfer);
            m_in_B.DataTransfer += new STNodeOptionEventHandler(m_in_num_DataTransfer);

            ProcessResult();
        }

        void m_in_num_DataTransfer(object sender, STNodeOptionEventArgs e)
        {
            if (e.Status == ConnectionStatus.Connected && e.TargetOption.Data != null)
            {
                if (sender == m_in_A)
                    _aVal = (float)e.TargetOption.Data;
                else
                    _bVal = (float)e.TargetOption.Data;
            }
            else
            {
                if (sender == m_in_A)
                    _aVal = 0.0f;
                else
                    _bVal = 0.0f;
            }

            ProcessResult();
        }

        protected override void OnDrawOptionText(DrawingTools dt, STNodeOption op)
        {
            base.OnDrawOptionText(dt, op);

            if (op == m_in_A)
                dt.Graphics.DrawString("-", Font, Brushes.White, new Rectangle(
                    op.TextRectangle.X, op.TextRectangle.Y + 10,
                    op.TextRectangle.Width, op.TextRectangle.Height
                ), _sf);
        }

        private void ProcessResult()
        {
            float result = _aVal - _bVal;
            SetOptionText(m_in_A, _aVal.ToString("G", CultureInfo.InvariantCulture));
            SetOptionText(m_in_B, _bVal.ToString("G", CultureInfo.InvariantCulture));
            SetOptionText(m_out, result.ToString("G", CultureInfo.InvariantCulture));
            m_out.TransferData(result);
        }
    }
}
