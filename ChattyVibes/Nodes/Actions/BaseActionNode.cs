using System.Drawing;

namespace ChattyVibes.Nodes.Actions
{
    internal abstract class BaseActionNode : FlowNode
    {
        protected override void OnCreate()
        {
            _direction = FlowDirection.In;
            base.OnCreate();
            TitleColor = Color.FromArgb(200, FrmBindingGraphs.C_COLOR_ACTION);
        }
    }
}
