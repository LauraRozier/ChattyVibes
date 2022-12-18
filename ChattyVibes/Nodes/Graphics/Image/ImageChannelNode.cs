using ST.Library.UI.NodeEditor;
using System.Drawing.Imaging;
using System.Drawing;
using System.Runtime.InteropServices;

namespace ChattyVibes.Nodes.Graphics
{
    //[STNode("/Graphics/Image", "LauraRozier", "", "", "This is an image channel node")]
    internal class ImageChannelNode : BaseImageNode
    {
        private STNodeOption m_in_img_in;
        private STNodeOption m_out_img_r;
        private STNodeOption m_out_img_g;
        private STNodeOption m_out_img_b;

        protected override void OnCreate()
        {
            base.OnCreate();
            Title = "Image Channel";

            m_in_img_in = InputOptions.Add("", typeof(Image), true);
            m_out_img_r = OutputOptions.Add("R", typeof(Image), false);
            m_out_img_g = OutputOptions.Add("G", typeof(Image), false);
            m_out_img_b = OutputOptions.Add("B", typeof(Image), false);

            m_in_img_in.DataTransfer += new STNodeOptionEventHandler(m_op_img_in_DataTransfer);

            m_op_img_out.TransferData(null);
            m_out_img_r.TransferData(null);
            m_out_img_g.TransferData(null);
            m_out_img_b.TransferData(null);
        }

        void m_op_img_in_DataTransfer(object sender, STNodeOptionEventArgs e)
        {
            if (e.Status != ConnectionStatus.Connected || e.TargetOption.Data == null)
            {
                m_op_img_out.TransferData(null);
                m_out_img_r.TransferData(null);
                m_out_img_g.TransferData(null);
                m_out_img_b.TransferData(null);
                m_img_draw = null;
            }
            else
            {
                Bitmap bmp = (Bitmap)e.TargetOption.Data;
                Bitmap bmp_r = new Bitmap(bmp.Width, bmp.Height);
                Bitmap bmp_g = new Bitmap(bmp.Width, bmp.Height);
                Bitmap bmp_b = new Bitmap(bmp.Width, bmp.Height);
                BitmapData bmpData = bmp.LockBits(new Rectangle(Point.Empty, bmp.Size), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
                BitmapData bmpData_r = bmp_r.LockBits(new Rectangle(Point.Empty, bmp.Size), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
                BitmapData bmpData_g = bmp_g.LockBits(new Rectangle(Point.Empty, bmp.Size), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
                BitmapData bmpData_b = bmp_b.LockBits(new Rectangle(Point.Empty, bmp.Size), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
                byte[] byColor = new byte[bmpData.Height * bmpData.Stride];
                byte[] byColor_r = new byte[byColor.Length];
                byte[] byColor_g = new byte[byColor.Length];
                byte[] byColor_b = new byte[byColor.Length];
                Marshal.Copy(bmpData.Scan0, byColor, 0, byColor.Length);

                for (int y = 0; y < bmpData.Height; y++)
                {
                    int ny = y * bmpData.Stride;

                    for (int x = 0; x < bmpData.Width; x++)
                    {
                        int nx = x << 2;
                        byColor_b[ny + nx] = byColor[ny + nx];
                        byColor_g[ny + nx + 1] = byColor[ny + nx + 1];
                        byColor_r[ny + nx + 2] = byColor[ny + nx + 2];
                        byColor_r[ny + nx + 3] = byColor_g[ny + nx + 3] = byColor_b[ny + nx + 3] = byColor[ny + nx + 3];
                    }
                }

                bmp.UnlockBits(bmpData);
                Marshal.Copy(byColor_r, 0, bmpData_r.Scan0, byColor_r.Length);
                Marshal.Copy(byColor_g, 0, bmpData_g.Scan0, byColor_g.Length);
                Marshal.Copy(byColor_b, 0, bmpData_b.Scan0, byColor_b.Length);
                bmp_r.UnlockBits(bmpData_r);
                bmp_g.UnlockBits(bmpData_g);
                bmp_b.UnlockBits(bmpData_b);
                m_op_img_out.TransferData(bmp);
                m_out_img_r.TransferData(bmp_r);
                m_out_img_g.TransferData(bmp_g);
                m_out_img_b.TransferData(bmp_b);
                m_img_draw = bmp;
            }
        }

        protected override void OnDrawBody(DrawingTools dt)
        {
            base.OnDrawBody(dt);
            Rectangle rect = new Rectangle(Left + 10, Top + 30, 120, 80);
            dt.Graphics.FillRectangle(Brushes.Gray, rect);

            if (m_img_draw != null)
                dt.Graphics.DrawImage(m_img_draw, rect);
        }
    }
}
