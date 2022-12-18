using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace ST.Library.UI.NodeEditor
{
    internal class FrmNodePreviewPanel : Form
    {
        public Color BorderColor { get; set; }
        public bool AutoBorderColor { get; set; }

        private bool m_bRight;
        private Point m_ptHandle;
        private int m_nHandleSize;
        private Rectangle m_rect_handle;
        private Rectangle m_rect_panel;
        private Rectangle m_rect_exclude;
        private Region m_region;
        private Type m_type;
        private STNode m_node;
        private STNodeEditor m_editor;
        private STNodePropertyGrid m_property;

        private Pen m_pen = new Pen(Color.Black);
        private SolidBrush m_brush = new SolidBrush(Color.Black);
        private static FrmNodePreviewPanel m_last_frm;

        [DllImport("user32.dll")]
        private static extern int SetWindowRgn(IntPtr hWnd, IntPtr hRgn, bool bRedraw);

        public FrmNodePreviewPanel(Type stNodeType, Point ptHandle, int nHandleSize, bool bRight, STNodeEditor editor, STNodePropertyGrid propertyGrid) {
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.ResizeRedraw, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);

            if (m_last_frm != null)
                m_last_frm.Close();

            m_last_frm = this;

            m_editor = editor;
            m_property = propertyGrid;
            m_editor.Size = new Size(200, 200);
            m_property.Size = new Size(200, 200);
            m_editor.Location = new Point(1 + (bRight ? nHandleSize : 0), 1);
            m_property.Location = new Point(m_editor.Right, 1);
            m_property.InfoFirstOnDraw = true;
            Controls.Add(m_editor);
            Controls.Add(m_property);
            ShowInTaskbar = false;
            FormBorderStyle = FormBorderStyle.None;
            Size = new Size(402 + nHandleSize, 202);

            m_type = stNodeType;
            m_ptHandle = ptHandle;
            m_nHandleSize = nHandleSize;
            m_bRight = bRight;

            AutoBorderColor = true;
            BorderColor = Color.DodgerBlue;
        }

        protected override void OnLoad(EventArgs e) {
            base.OnLoad(e);
            m_node = (STNode)Activator.CreateInstance(m_type);
            m_node.Left = 20; m_node.Top = 20;
            m_editor.Nodes.Add(m_node);
            m_property.SetNode(m_node);

            m_rect_panel = new Rectangle(0, 0, 402, 202);
            m_rect_handle = new Rectangle(m_ptHandle.X, m_ptHandle.Y, m_nHandleSize, m_nHandleSize);
            m_rect_exclude = new Rectangle(0, m_nHandleSize, m_nHandleSize, Height - m_nHandleSize);

            if (m_bRight) {
                Left = m_ptHandle.X;
                m_rect_panel.X = m_ptHandle.X + m_nHandleSize;
            } else {
                Left = m_ptHandle.X - Width + m_nHandleSize;
                m_rect_exclude.X = Width - m_nHandleSize;
                m_rect_panel.X = Left;
            }

            if (m_ptHandle.Y + Height > Screen.GetWorkingArea(this).Bottom) {
                Top = m_ptHandle.Y - Height + m_nHandleSize;
                m_rect_exclude.Y -= m_nHandleSize;
            } else Top = m_ptHandle.Y;

            m_rect_panel.Y = Top;
            m_region = new Region(new Rectangle(Point.Empty, Size));
            m_region.Exclude(m_rect_exclude);

            using (Graphics g = CreateGraphics()) {
                IntPtr h = m_region.GetHrgn(g);
                FrmNodePreviewPanel.SetWindowRgn(Handle, h, false);
                m_region.ReleaseHrgn(h);
            }

            MouseLeave += Event_MouseLeave;
            m_editor.MouseLeave += Event_MouseLeave;
            m_property.MouseLeave += Event_MouseLeave;
            BeginInvoke(new MethodInvoker(() => { m_property.Focus(); }));
        }

        protected override void OnClosing(CancelEventArgs e) {
            base.OnClosing(e);
            Controls.Clear();
            m_editor.Nodes.Clear();
            m_editor.MouseLeave -= Event_MouseLeave;
            m_property.MouseLeave -= Event_MouseLeave;
            m_last_frm = null;
        }

        void Event_MouseLeave(object sender, EventArgs e) {
            Point pt = Control.MousePosition;

            if (m_rect_panel.Contains(pt) || m_rect_handle.Contains(pt))
                return;

            Close();
        }

        protected override void OnPaint(PaintEventArgs e) {
            base.OnPaint(e);
            Graphics g = e.Graphics;
            m_pen.Color = AutoBorderColor ? m_node.TitleColor : BorderColor;
            m_brush.Color = m_pen.Color;
            g.DrawRectangle(m_pen, 0, 0, Width - 1, Height - 1);
            g.FillRectangle(m_brush, m_rect_exclude.X - 1, m_rect_exclude.Y - 1, m_rect_exclude.Width + 2, m_rect_exclude.Height + 2);

            Rectangle rect = RectangleToClient(m_rect_handle);
            rect.Y = (m_nHandleSize - 14) / 2;
            rect.X += rect.Y + 1;
            rect.Width = rect.Height = 14;
            m_pen.Width = 2;
            g.DrawLine(m_pen, rect.X + 4, rect.Y + 3, rect.X + 10, rect.Y + 3);
            g.DrawLine(m_pen, rect.X + 4, rect.Y + 6, rect.X + 10, rect.Y + 6);
            g.DrawLine(m_pen, rect.X + 4, rect.Y + 11, rect.X + 10, rect.Y + 11);
            g.DrawLine(m_pen, rect.X + 7, rect.Y + 7, rect.X + 7, rect.Y + 10);
            m_pen.Width = 1;
            g.DrawRectangle(m_pen, rect.X, rect.Y, rect.Width - 1, rect.Height - 1);
        }
    }
}
