using System.Drawing;

namespace ChattyVibes.Nodes.ActionNode
{
    internal abstract class BaseActionNode : ActionNode
    {
        protected override void OnCreate()
        {
            _direction = FlowDirection.In;
            base.OnCreate();
        }
    }
}
