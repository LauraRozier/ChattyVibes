using ST.Library.UI.NodeEditor;

namespace ChattyVibes.Nodes.LogicNode
{
    [STNode("/Logic", "LauraRozier", "", "", "Boolean logic AND node")]
    internal class LogicANDNode : LogicNode
    {
        protected override void OnCreate()
        {
            base.OnCreate();
            Title = "Logic AND";
        }

        protected override void Compare()
        {
            _equal = _aVal & _bVal;
            SendResult();
        }
    }
}
