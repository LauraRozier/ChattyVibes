using ST.Library.UI.NodeEditor;

namespace ChattyVibes.Nodes.LogicNode
{
    [STNode("/Logic", "LauraRozier", "", "", "Boolean logic OR node")]
    internal class LogicORNode : LogicNode
    {
        protected override void OnCreate()
        {
            base.OnCreate();
            Title = "Logic OR";
        }

        protected override void Compare()
        {
            _equal = _aVal | _bVal;
            SendResult();
        }
    }
}
