using ST.Library.UI.NodeEditor;
using System.Drawing;
using SysColor = System.Drawing.Color;

namespace ChattyVibes.Nodes.Graphics
{
    //[STNode("/Graphics/Image", "LauraRozier", "", "", "This is an image size node")]
    internal class ImageSizeNode : STNode
    {
        private STNodeOption m_op_in;
        private STNodeOption m_op_width_out;
        private STNodeOption m_op_height_out;
        private int m_width = 0;
        private int m_height = 0;

        protected override void OnCreate()
        {
            base.OnCreate();
            Title = "Image Size";
            TitleColor = SysColor.FromArgb(200, FrmBindingGraphs.C_COLOR_IMAGE);

            m_op_in = InputOptions.Add("", typeof(Image), true);
            m_op_width_out = OutputOptions.Add("Width", typeof(int), false);
            m_op_height_out = OutputOptions.Add("Height", typeof(int), false);

            SetOptionText(m_op_in, $"W:{m_width} H:{m_height}");

            m_op_in.DataTransfer += new STNodeOptionEventHandler(m_op_in_DataTransfer);
            m_op_width_out.TransferData(m_width);
            m_op_height_out.TransferData(m_height);
        }

        void m_op_in_DataTransfer(object sender, STNodeOptionEventArgs e)
        {
            if (e.Status != ConnectionStatus.Connected || e.TargetOption.Data == null)
            {
                m_width = 0;
                m_height = 0;
            }
            else
            {
                Image img = (Image)e.TargetOption.Data;
                m_width = img.Width;
                m_height = img.Height;
            }

            SetOptionText(m_op_in, $"W:{m_width} H:{m_height}");
            m_op_width_out.TransferData(m_width);
            m_op_height_out.TransferData(m_height);
        }
    }
}
