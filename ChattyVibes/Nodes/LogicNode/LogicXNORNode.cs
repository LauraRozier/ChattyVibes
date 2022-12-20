using ST.Library.UI.NodeEditor;

namespace ChattyVibes.Nodes.LogicNode
{
    [STNode("/Logic", "LauraRozier", "", "", "Boolean logic XNOR node")]
    internal class LogicXNORNode : LogicNode
    {
        protected override void OnCreate()
        {
            base.OnCreate();
            Title = "Logic XNOR";
        }

        protected override void Compare()
        {
            _equal = !(_aVal ^ _bVal);
            SendResult();
        }
    }
}
