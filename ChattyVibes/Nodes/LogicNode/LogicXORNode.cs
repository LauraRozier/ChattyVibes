using ST.Library.UI.NodeEditor;

namespace ChattyVibes.Nodes.LogicNode
{
    [STNode("/Logic", "LauraRozier", "", "", "Boolean logic XOR node")]
    internal class LogicXORNode : LogicNode
    {
        protected override void OnCreate()
        {
            base.OnCreate();
            Title = "Logic XOR";
        }

        protected override void Compare()
        {
            _equal = _aVal ^ _bVal;
            SendResult();
        }
    }
}
