using ST.Library.UI.NodeEditor;
using System.Drawing;
using System.Windows.Forms;

namespace ChattyVibes.Nodes
{
    internal class DescriptorForColor : STNodePropertyDescriptor
    {
        private Rectangle m_rect;

        protected override void OnSetItemLocation()
        {
            base.OnSetItemLocation();
            Rectangle rect = RectangleR;
            m_rect = new Rectangle(rect.Right - 25, rect.Top + 5, 19, 12);
        }

        protected override string GetStringFromValue()
        {
            Color clr = (Color)GetValue(null);
            return clr.A + "," + clr.R + "," + clr.G + "," + clr.B;
        }

        protected override object GetValueFromString(string strText)
        {
            string[] strClr = strText.Split(',');
            return Color.FromArgb(
                int.Parse(strClr[0]), // A
                int.Parse(strClr[1]), // R
                int.Parse(strClr[2]), // G
                int.Parse(strClr[3])  // B
            );
        }

        protected override void OnDrawValueRectangle(DrawingTools dt)
        {
            base.OnDrawValueRectangle(dt);
            dt.SolidBrush.Color = (Color)GetValue(null);
            dt.Graphics.FillRectangle(dt.SolidBrush, m_rect);
            dt.Graphics.DrawRectangle(Pens.Black, m_rect);
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            if (m_rect.Contains(e.Location))
            {
                ColorDialog cd = new ColorDialog();

                if (cd.ShowDialog() != DialogResult.OK)
                    return;

                SetValue(cd.Color, null);
                Invalidate();
                return;
            }

            base.OnMouseClick(e);
        }
    }
}
