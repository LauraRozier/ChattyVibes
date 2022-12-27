using System.Drawing;

namespace ChattyVibes.Nodes.EventNode
{
    internal abstract class EventNode : FlowNode
    {
        protected abstract void BindEvent();

        protected abstract void UnbindEvent();

        ~EventNode()
        {
            UnbindEvent();
        }

        protected override void OnCreate()
        {
            _direction = FlowDirection.Out;
            base.OnCreate();
            TitleColor = Color.FromArgb(200, Constants.C_COLOR_EVENT);
            BindEvent();
        }
    }
}
