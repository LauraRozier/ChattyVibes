using ST.Library.UI.NodeEditor;
using System.Drawing;

namespace ChattyVibes.Nodes.GraphicsNode.ImageNode
{
    internal abstract class BaseImageNode : STNode
    {
        protected Image m_img_draw;
        protected STNodeOption m_op_img_out;

        protected override void OnCreate()
        {
            base.OnCreate();
            m_op_img_out = OutputOptions.Add(string.Empty, typeof(Image), false);
            AutoSize = false;
            Width = 160;
            Height = 120;
            //TitleColor = Color.FromArgb(200, FrmBindingGraphs.C_COLOR_IMAGE);
        }
    }
}
