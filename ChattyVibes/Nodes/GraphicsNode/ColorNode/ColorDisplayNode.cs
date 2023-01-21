using ST.Library.UI.NodeEditor;
using System.Drawing;

namespace ChattyVibes.Nodes.GraphicsNode.ColorNode
{
    //[STNode("/Graphics/Color", "LauraRozier", "", "", "This is a color display node")]
    internal class ColorDisplayNode : STNode
    {
        private Color _color = Color.LightGray;

        private STNodeOption m_in_color;
        private STNodeOption m_out_color;

        protected override void OnCreate()
        {
            AutoSize = false;
            Width = 140;
            Height = 140;
            //TitleColor = SysColor.FromArgb(200, FrmBindingGraphs.C_COLOR_COLOR);
            Title = "Color Display";

            m_in_color = InputOptions.Add(string.Empty, typeof(Color), true);
            m_out_color = OutputOptions.Add(string.Empty, typeof(Color), false);

            m_in_color.DataTransfer += new STNodeOptionEventHandler(m_in_color_DataTransfer);
            m_out_color.TransferData(_color);
        }

        protected override void OnDrawBody(DrawingTools dt)
        {
            base.OnDrawBody(dt);
            Rectangle rect = new Rectangle(Left + 10, Top + 50, 120, 80);
            dt.Graphics.FillRectangle(new SolidBrush(_color), rect);
        }

        void m_in_color_DataTransfer(object sender, STNodeOptionEventArgs e)
        {
            if (e.Status == ConnectionStatus.Connected && e.TargetOption.Data != null)
                _color = (Color)e.TargetOption.Data;
            else
                _color = Color.LightGray;

            m_out_color.TransferData(_color);
            Invalidate();
        }
    }
}
