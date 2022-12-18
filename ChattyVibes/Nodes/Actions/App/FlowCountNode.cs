using ST.Library.UI.NodeEditor;
using System.Drawing;

namespace ChattyVibes.Nodes.Actions.App
{
    [STNode("/Actions/App", "LauraRozier", "", "", "Flow count node")]
    internal class FlowCountNode : FlowNode
    {
        private int _count = 0;
        private StringFormat _sf = new StringFormat
        {
            LineAlignment = StringAlignment.Center,
            Alignment = StringAlignment.Center
        };

        protected override void OnFlowTrigger()
        {
            _count++;
            Invalidate();
        }

        protected override void OnCreate()
        {
            _direction = FlowDirection.Both;
            base.OnCreate();
            Title = "Flow Counter";
            TitleColor = Color.FromArgb(200, FrmBindingGraphs.C_COLOR_ACTION);
            AutoSize = false;
            Width = 150;
            Height = 40;
        }

        protected override void OnDrawOptionText(DrawingTools dt, STNodeOption op)
        {
            base.OnDrawOptionText(dt, op);
            Rectangle rect = new Rectangle
            {
                X = op.TextRectangle.X,
                Y = op.TextRectangle.Y,
                Width = op.TextRectangle.Width,
                Height = op.TextRectangle.Height
            };
            dt.Graphics.DrawString(_count.ToString(), Font, Brushes.White, rect, _sf);
        }
    }
}
