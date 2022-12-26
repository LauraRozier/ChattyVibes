using ST.Library.UI.NodeEditor;
using System.Drawing;
using System.Windows.Forms;

namespace ChattyVibes.Nodes.UtilNode
{
    [STNode("/Util", "LauraRozier", "", "", "This is a comment node")]
    internal sealed class CommentNode : STNode
    {
        private string _string = "";
        [STNodeProperty("String", "The comment to show")]
        public string String
        {
            get { return _string; }
            set
            {
                _string = value;
                Invalidate();
            }
        }
        private readonly static Size DefaultSize = new Size(100, 22);
        private readonly static Size MaxSize = new Size(350, 16);

        protected override void OnCreate()
        {
            base.OnCreate();
            Title = "Comment";
            TitleColor = Color.FromArgb(200, Constants.C_COLOR_COMMENT);
            AutoSize = false;
            Size = DefaultSize;
            m_sf = new StringFormat { LineAlignment = StringAlignment.Near };
            InputOptions.Add(STNodeOption.Empty);
        }

        protected override void OnDrawBody(DrawingTools dt)
        {
            string replacedText = _string.Replace("\\r", "\r").Replace("\\n", "\n");
            Size textSize = TextRenderer.MeasureText(replacedText, Font, MaxSize, TextFormatFlags.WordBreak);
            Rectangle textRect = new Rectangle(
                Location.X,
                Location.Y + 22,
                textSize.Width,
                textSize.Height
            );
            Width = textRect.Width;
            Height = textRect.Height + 20;
            dt.Graphics.DrawString(replacedText, Font, Brushes.White, textRect, m_sf);
            base.OnDrawBody(dt);
        }
    }
}
