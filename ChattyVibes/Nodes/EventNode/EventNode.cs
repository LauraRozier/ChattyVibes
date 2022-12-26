using System.Drawing;

namespace ChattyVibes.Nodes.EventNode
{
    internal abstract class EventNode : FlowNode
    {
        protected override void OnCreate()
        {
            base.OnCreate();
            TitleColor = Color.FromArgb(200, Constants.C_COLOR_EVENT);
        }
    }
}
