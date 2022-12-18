using ST.Library.UI.NodeEditor;
using System.Drawing;
using SysColor = System.Drawing.Color;

namespace ChattyVibes.Nodes.Graphics
{
    internal abstract class BaseImageNode : STNode
    {
        protected Image m_img_draw;
        protected STNodeOption m_op_img_out;

        protected override void OnCreate()
        {
            base.OnCreate();
            m_op_img_out = OutputOptions.Add("", typeof(Image), false);
            AutoSize = false;
            Width = 160;
            Height = 120;
            TitleColor = SysColor.FromArgb(200, FrmBindingGraphs.C_COLOR_IMAGE);
        }
    }
}
