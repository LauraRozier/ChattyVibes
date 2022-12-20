using ST.Library.UI.NodeEditor;

namespace ChattyVibes.Nodes.LogicNode
{
    [STNode("/Logic", "LauraRozier", "", "", "Boolean logic NAND node")]
    internal class LogicNANDNode : LogicNode
    {
        protected override void OnCreate()
        {
            base.OnCreate();
            Title = "Logic NAND";
        }

        protected override void Compare()
        {
            _equal = !(_aVal & _bVal);
            SendResult();
        }
    }
}
