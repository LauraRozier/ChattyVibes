using ST.Library.UI.NodeEditor;
using System.Drawing;

namespace ChattyVibes.Nodes.MathNode.UIntNode
{
    [STNode("/Math/UInt", "LauraRozier", "", "", "Compare two numbers")]
    internal class UIntCompareNode : CompareNode<uint>
    {
        protected override void OnCreate()
        {
            _defaultVal = 0u;
            base.OnCreate();
            Title = "UInt Compare";
            TitleColor = Color.FromArgb(200, Constants.C_COLOR_UINT);
        }
    }
}
