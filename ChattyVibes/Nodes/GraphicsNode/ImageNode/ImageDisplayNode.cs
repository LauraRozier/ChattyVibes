using ST.Library.UI.NodeEditor;
using System.Drawing;

namespace ChattyVibes.Nodes.GraphicsNode.ImageNode
{
    //[STNode("/Graphics/Image", "LauraRozier", "", "", "This is an image display node")]
    internal class ImageDisplayNode : STNode
    {
        private Image m_img_draw;
        private STNodeOption m_op_img_in;

        protected override void OnCreate()
        {
            base.OnCreate();
            AutoSize = false;
            Width = 160;
            Height = 120;
            //TitleColor = Color.FromArgb(200, FrmBindingGraphs.C_COLOR_IMAGE);
            Title = "Image Display";

            m_op_img_in = InputOptions.Add("", typeof(Image), true);

            m_op_img_in.DataTransfer += new STNodeOptionEventHandler(m_op_img_in_DataTransfer);
        }

        protected override void OnDrawBody(DrawingTools dt)
        {
            base.OnDrawBody(dt);
            Rectangle rect = new Rectangle(Left + 10, Top + 30, 140, 80);
            dt.Graphics.FillRectangle(Brushes.Gray, rect);

            if (m_img_draw != null)
                dt.Graphics.DrawImage(m_img_draw, rect);
        }

        void m_op_img_in_DataTransfer(object sender, STNodeOptionEventArgs e)
        {
            m_img_draw = (e.Status != ConnectionStatus.Connected || e.TargetOption.Data == null) ? null
                : (Bitmap)e.TargetOption.Data;
        }
    }
}
