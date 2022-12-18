using ST.Library.UI.NodeEditor;
using System.Drawing;
using System.Windows.Forms;
using System;

namespace ChattyVibes.Nodes
{
    internal class NodeButton : STNodeControl
    {
        private bool m_b_enter;
        private bool m_b_down;

        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);
            m_b_enter = true;
            Invalidate();
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            m_b_enter = false;
            Invalidate();
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            m_b_down = true;
            Invalidate();
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            m_b_down = false;
            Invalidate();
        }

        protected override void OnPaint(DrawingTools dt)
        {
            // base.OnPaint(dt);
            SolidBrush brush = dt.SolidBrush;
            brush.Color = BackColor;

            if (m_b_down)
                brush.Color = Color.SkyBlue;
            else if (m_b_enter)
                brush.Color = Color.DodgerBlue;

            dt.Graphics.FillRectangle(brush, 0, 0, Width, Height);
            dt.Graphics.DrawString(Text, Font, Brushes.White, ClientRectangle, m_sf);
        }
    }
}
