using System.Drawing;

namespace ChattyVibes.Nodes.ActionNode
{
    internal abstract class ActionNode : FlowNode
    {
        protected override void OnCreate()
        {
            if (_direction == FlowDirection.None)
                _direction = FlowDirection.In;

            base.OnCreate();
            TitleColor = Color.FromArgb(200, Constants.C_COLOR_ACTION);
        }
    }
}
