using ST.Library.UI.NodeEditor;
using System.Drawing;

namespace ChattyVibes.Nodes.ArrayNode
{
    [STNode("/Array", "LauraRozier", "", "", "UInt[] node")]
    internal class UIntArrayNode : ArrayNode<uint>
    {
        protected override void OnCreate()
        {
            base.OnCreate();
            Title = "UInt[]";
            TitleColor = Color.FromArgb(200, Constants.C_COLOR_UINT);
        }
    }
}
