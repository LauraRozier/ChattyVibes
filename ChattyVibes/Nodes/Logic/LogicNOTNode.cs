using ST.Library.UI.NodeEditor;
using System.Drawing;

namespace ChattyVibes.Nodes.Bool
{
    [STNode("/Logic", "LauraRozier", "", "", "Boolean logic NOT node")]
    internal class LogicNOTNode : STNode
    {
        private bool _value = true;

        private STNodeOption m_op_in;
        private STNodeOption m_op_out;

        private StringFormat _sf = new StringFormat
        {
            LineAlignment = StringAlignment.Center,
            Alignment = StringAlignment.Center
        };

        protected override void OnCreate()
        {
            base.OnCreate();
            TitleColor = Color.FromArgb(200, FrmBindingGraphs.C_COLOR_LOGIC);
            Title = "Logic NOT";
            AutoSize = false;
            Width = 80;
            Height = 40;

            m_op_in = InputOptions.Add("", typeof(bool), true);
            m_op_out = OutputOptions.Add("", typeof(bool), false);

            m_op_in.DataTransfer += new STNodeOptionEventHandler(m_op_bool_in_DataTransfer);
            m_op_out.TransferData(_value);
        }

        private void m_op_bool_in_DataTransfer(object sender, STNodeOptionEventArgs e)
        {
            if (e.Status == ConnectionStatus.Connected && e.TargetOption.Data != null)
                _value = !((bool)e.TargetOption.Data);
            else
                _value = true;

            m_op_out.TransferData(_value);
            Invalidate();
        }

        protected override void OnDrawOptionText(DrawingTools dt, STNodeOption op)
        {
            base.OnDrawOptionText(dt, op);

            if (op == m_op_out)
            {
                Rectangle rect = new Rectangle
                {
                    X = op.TextRectangle.X,
                    Y = op.TextRectangle.Y,
                    Width = op.TextRectangle.Width,
                    Height = op.TextRectangle.Height
                };
                dt.Graphics.DrawString(_value ? "True" : "False", Font, Brushes.White, rect, _sf);
            }
        }
    }
}
