using ST.Library.UI.NodeEditor;

namespace ChattyVibes.Nodes.LogicNode
{
    [STNode("/Logic", "LauraRozier", "", "", "Boolean logic NOR node")]
    internal class LogicNORNode : LogicNode
    {
        protected override void OnCreate()
        {
            base.OnCreate();
            Title = "Logic NOR";
        }

        protected override void Compare()
        {
            _equal = !(_aVal | _bVal);
            SendResult();
        }
    }
}
