using ST.Library.UI.NodeEditor;
using System.Drawing;
using System.Windows.Forms;

namespace ChattyVibes.Nodes.Graphics
{
    //[STNode("/Graphics/Image", "LauraRozier", "", "", "This is an image node")]
    internal class ImageInputNode : BaseImageNode
    {
        private string _FileName;
        [STNodeProperty("Image", "Click to select a image", DescriptorType = typeof(OpenFileDescriptor))]
        public string FileName
        {
            get { return _FileName; }
            set
            {
                Image img = null;

                if (!string.IsNullOrEmpty(value))
                    img = Image.FromFile(value);

                if (m_img_draw != null)
                    m_img_draw.Dispose();

                m_img_draw = img;
                _FileName = value;
                m_op_img_out.TransferData(m_img_draw, true);
                Invalidate();
            }
        }

        protected override void OnCreate()
        {
            base.OnCreate();
            Title = "Image";

            m_op_img_out.TransferData(m_img_draw, true);
        }

        protected override void OnDrawBody(DrawingTools dt)
        {
            base.OnDrawBody(dt);
            Rectangle rect = new Rectangle(Left + 10, Top + 30, 140, 80);
            dt.Graphics.FillRectangle(Brushes.Gray, rect);

            if (m_img_draw != null)
                dt.Graphics.DrawImage(m_img_draw, rect);
        }
    }

    internal class OpenFileDescriptor : STNodePropertyDescriptor
    {
        private Rectangle m_rect_open;
        private StringFormat _sf = new StringFormat
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center
            };

        protected override void OnSetItemLocation()
        {
            base.OnSetItemLocation();
            m_rect_open = new Rectangle(
                RectangleR.Right - 20,
                RectangleR.Top,
                20,
                RectangleR.Height
            );
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            if (m_rect_open.Contains(e.Location))
            {
                OpenFileDialog ofd = new OpenFileDialog { Filter = "*.jpg|*.jpg|*.png|*.png" };

                if (ofd.ShowDialog() != DialogResult.OK)
                    return;

                SetValue(ofd.FileName);
            }
            else
            {
                base.OnMouseClick(e);
            }
        }

        protected override void OnDrawValueRectangle(DrawingTools dt)
        {
            base.OnDrawValueRectangle(dt);
            dt.Graphics.FillRectangle(Brushes.Gray, m_rect_open);
            dt.Graphics.DrawString("+", Control.Font, Brushes.White, m_rect_open, _sf);
        }
    }
}
