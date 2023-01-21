using ST.Library.UI.NodeEditor;
using System;
using System.Drawing;

namespace ChattyVibes.Nodes.BranchNode
{
    internal abstract class ChoiseNode : STNode
    {
        protected Type _type;
        protected bool _condition = false;
        private StringFormat _sf = new StringFormat
        {
            LineAlignment = StringAlignment.Center,
            Alignment = StringAlignment.Near
        };

        private STNodeOption m_op_condition_in;
        protected STNodeOption m_op_true_in;
        protected STNodeOption m_op_false_in;
        protected STNodeOption m_op_true_out;
        protected STNodeOption m_op_false_out;

        protected abstract void HandleCondition();

        protected override void OnCreate()
        {
            base.OnCreate();
            TitleColor = Color.FromArgb(200, Constants.C_COLOR_BRANCH);

            m_op_condition_in = InputOptions.Add("Condition", typeof(bool), true);
            m_op_true_in = InputOptions.Add(string.Empty, _type, true);
            m_op_false_in = InputOptions.Add(string.Empty, _type, true);
            m_op_true_out = OutputOptions.Add(string.Empty, _type, false);
            m_op_false_out = OutputOptions.Add(string.Empty, _type, false);

            m_op_condition_in.DataTransfer += new STNodeOptionEventHandler(m_in_condition_DataTransfer);
        }

        private void m_in_condition_DataTransfer(object sender, STNodeOptionEventArgs e)
        {
            _condition = (e.Status == ConnectionStatus.Connected && e.TargetOption.Data != null)
                ? (bool)e.TargetOption.Data
                : false;
            HandleCondition();
        }

        protected override Point OnSetOptionDotLocation(STNodeOption op, Point pt, int nIndex)
        {
            if (op == m_op_true_out || op == m_op_false_out)
                return new Point(pt.X, pt.Y + 20);

            return base.OnSetOptionDotLocation(op, pt, nIndex);
        }

        protected override Rectangle OnSetOptionTextRectangle(STNodeOption op, Rectangle rect, int nIndex)
        {
            if (op == m_op_true_out || op == m_op_false_out)
                return new Rectangle(rect.X, rect.Y + 20, rect.Width, rect.Height);

            return base.OnSetOptionTextRectangle(op, rect, nIndex);
        }

        protected override void OnDrawOptionText(DrawingTools dt, STNodeOption op)
        {
            base.OnDrawOptionText(dt, op);
            Rectangle rect = new Rectangle(op.TextRectangle.X, op.TextRectangle.Y, Width, op.TextRectangle.Height);

            if (op == m_op_true_out)
            {
                dt.Graphics.DrawString("══════ True ═══════", Font, _condition ? Brushes.ForestGreen : Brushes.White, rect, _sf);
            }
            else if (op == m_op_false_out)
            {
                dt.Graphics.DrawString("══════ False ══════", Font, _condition ? Brushes.White : Brushes.ForestGreen, rect, _sf);
            }
        }
    }
}
