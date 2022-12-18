using ST.Library.UI.NodeEditor;
using System.Drawing;
using SysColor = System.Drawing.Color;

namespace ChattyVibes.Nodes.Graphics.Color
{
    //[STNode("/Graphics/Color", "LauraRozier", "", "", "This is a color output node")]
    internal class ColorOutputNode : STNode
    {
        private SysColor _color = SysColor.LightGray;
        [STNodeProperty("Color", "The color value", DescriptorType = typeof(DescriptorForColor))]
        public SysColor Color
        {
            get { return _color; }
            set
            {
                _color = value;
                Invalidate();
            }
        }

        private STNodeOption m_in_color;

        protected override void OnCreate()
        {
            base.OnCreate();
            AutoSize = false;
            Width = 140;
            Height = 140;
            TitleColor = SysColor.FromArgb(200, FrmBindingGraphs.C_COLOR_COLOR);
            Title = "Color Output";

            m_in_color = InputOptions.Add("Color", typeof(SysColor), true);
            m_in_color.DataTransfer += new STNodeOptionEventHandler(m_in_color_DataTransfer);
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
                Color = (SysColor)e.TargetOption.Data;
            else
                Color = SysColor.LightGray;
        }
    }
}
