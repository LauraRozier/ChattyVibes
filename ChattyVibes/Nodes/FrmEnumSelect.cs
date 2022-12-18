using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.Windows.Forms;

namespace ChattyVibes.Nodes
{
    internal class FrmEnumSelect : Form
    {
        private Point m_pt;
        private int m_nWidth;
        private float m_scale;
        private List<Enum> m_lst = new List<Enum>();
        private StringFormat _sf = new StringFormat { LineAlignment = StringAlignment.Center };
        private bool m_bClosed;

        public Enum Enum { get; set; }

        public FrmEnumSelect(Enum e, Point pt, int nWidth, float scale)
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);

            foreach (Enum v in e.GetType().GetEnumValues())
                m_lst.Add(v);

            Enum = e;
            m_pt = pt;
            m_scale = scale;
            m_nWidth = nWidth;

            ShowInTaskbar = false;
            BackColor = Color.FromArgb(255, 34, 34, 34);
            FormBorderStyle = FormBorderStyle.None;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            Location = m_pt;
            Width = (int)(m_nWidth * m_scale);
            Height = (int)(m_lst.Count * 20 * m_scale);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            e.Graphics.ScaleTransform(m_scale, m_scale);
            Rectangle rect = new Rectangle(0, 0, Width, 20);

            foreach (var v in m_lst)
            {
                var vDisplayName = v.GetAttribute<DisplayAttribute>();
                string name = vDisplayName?.Name ?? v.ToString();

                e.Graphics.DrawString(name, Font, Brushes.White, rect, _sf);
                rect.Y += rect.Height;
            }
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);
            int nIndex = e.Y / (int)(20 * m_scale);

            if (nIndex >= 0 && nIndex < m_lst.Count)
                Enum = m_lst[nIndex];

            DialogResult = DialogResult.OK;
            m_bClosed = true;
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);

            if (m_bClosed)
                return;

            Close();
        }
    }
}
