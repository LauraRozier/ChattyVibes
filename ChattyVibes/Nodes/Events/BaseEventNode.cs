using System.Drawing;

namespace ChattyVibes.Nodes.Events
{
    internal abstract class BaseEventNode : FlowNode
    {
        protected abstract void BindEvent();

        protected abstract void UnbindEvent();

        ~BaseEventNode()
        {
            UnbindEvent();
        }

        protected override void OnCreate()
        {
            _direction = FlowDirection.Out;
            base.OnCreate();
            TitleColor = Color.FromArgb(200, FrmBindingGraphs.C_COLOR_EVENT);
            BindEvent();
        }
    }
}
