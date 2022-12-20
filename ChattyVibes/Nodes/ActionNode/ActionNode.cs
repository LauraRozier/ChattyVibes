using System.Drawing;

namespace ChattyVibes.Nodes.ActionNode
{
    internal abstract class ActionNode : BaseFlowNode
    {
        protected override void OnCreate()
        {
            base.OnCreate();
            TitleColor = Color.FromArgb(200, Constants.C_COLOR_ACTION);
        }
    }
}
