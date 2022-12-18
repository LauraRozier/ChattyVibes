using System;
using System.Windows.Forms;
using System.Drawing;
using ST.Library.UI.NodeEditor;

namespace ST.Library.UI
{
    internal class FrmSTNodePropertyInput : Form
    {
        private STNodePropertyDescriptor m_descriptor;
        private Rectangle m_rect;
        private Pen m_pen;
        private SolidBrush m_brush;
        private TextBox m_tbx;

        public FrmSTNodePropertyInput(STNodePropertyDescriptor descriptor) {
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.ResizeRedraw, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);

            m_rect = descriptor.RectangleR;
            m_descriptor = descriptor;
            ShowInTaskbar = false;
            FormBorderStyle = FormBorderStyle.None;
            BackColor = descriptor.Control.AutoColor ? descriptor.Node.TitleColor : descriptor.Control.ItemSelectedColor;
            m_pen = new Pen(descriptor.Control.ForeColor, 1);
            m_brush = new SolidBrush(BackColor);
        }

        protected override void OnLoad(EventArgs e) {
            base.OnLoad(e);
            Point pt = m_descriptor.Control.PointToScreen(m_rect.Location);
            pt.Y += m_descriptor.Control.ScrollOffset;
            Location = pt;
            Size = new Size(m_rect.Width + m_rect.Height, m_rect.Height);

            m_tbx = new TextBox();
            m_tbx.Font = m_descriptor.Control.Font;
            m_tbx.ForeColor = m_descriptor.Control.ForeColor;
            m_tbx.BackColor = Color.FromArgb(255, m_descriptor.Control.ItemValueBackColor);
            m_tbx.BorderStyle = BorderStyle.None;

            m_tbx.Size = new Size(Width - 4 - m_rect.Height, Height - 2);
            m_tbx.Text = m_descriptor.GetStringFromValue();
            Controls.Add(m_tbx);
            m_tbx.Location = new Point(2, (Height - m_tbx.Height) / 2);
            m_tbx.SelectAll();
            m_tbx.LostFocus += (s, ea) => Close();
            m_tbx.KeyDown += new KeyEventHandler(tbx_KeyDown);
        }

        protected override void OnPaint(PaintEventArgs e) {
            base.OnPaint(e);
            Graphics g = e.Graphics;
            m_brush.Color = m_tbx.BackColor;
            g.FillRectangle(m_brush, 1, 1, Width - 2 - m_rect.Height, Height - 2);
            m_brush.Color = m_descriptor.Control.ForeColor;
            //Enter
            g.FillPolygon(m_brush, new Point[]{
                new Point(Width - 21, Height - 2),
                new Point(Width - 14, Height - 2),
                new Point(Width - 14, Height - 8)
            });
            g.DrawLine(m_pen, Width - 14, Height - 3, Width - 4, Height - 3);
            g.DrawLine(m_pen, Width - 4, Height - 3, Width - 4, 14);
            g.DrawLine(m_pen, Width - 8, 13, Width - 4, 13);
            //----
            g.DrawLine(m_pen, Width - 19, 11, Width - 4, 11);
            //E
            g.DrawLine(m_pen, Width - 19, 3, Width - 16, 3);
            g.DrawLine(m_pen, Width - 19, 6, Width - 16, 6);
            g.DrawLine(m_pen, Width - 19, 9, Width - 16, 9);
            g.DrawLine(m_pen, Width - 19, 3, Width - 19, 9);
            //S
            g.DrawLine(m_pen, Width - 13, 3, Width - 10, 3);
            g.DrawLine(m_pen, Width - 13, 6, Width - 10, 6);
            g.DrawLine(m_pen, Width - 13, 9, Width - 10, 9);
            g.DrawLine(m_pen, Width - 13, 3, Width - 13, 6);
            g.DrawLine(m_pen, Width - 10, 6, Width - 10, 9);
            //C
            g.DrawLine(m_pen, Width - 7, 3, Width - 4, 3);
            g.DrawLine(m_pen, Width - 7, 9, Width - 4, 9);
            g.DrawLine(m_pen, Width - 7, 3, Width - 7, 9);
        }

        void tbx_KeyDown(object sender, KeyEventArgs e) {
            if (e.KeyCode == Keys.Escape)
                Close();

            if (e.KeyCode != Keys.Enter)
                return;

            try {
                m_descriptor.SetValue(((TextBox)sender).Text, null);
                m_descriptor.Control.Invalidate();//add rect;
            } catch (Exception ex) {
                m_descriptor.OnSetValueError(ex);
            }

            Close();
        }

        private void InitializeComponent() {
            SuspendLayout();
            // FrmSTNodePropertyInput
            ClientSize = new Size(292, 273);
            Name = "FrmSTNodePropertyInput";
            ResumeLayout(false);
        }
    }
}
