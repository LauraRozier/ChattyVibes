using ST.Library.UI.NodeEditor;
using System.Drawing;

namespace ChattyVibes.Nodes.ArrayNode
{
    [STNode("/Array", "LauraRozier", "", "", "UInt[] ForEach node")]
    internal class UIntArrayForEachNode : ArrayForEachNode<uint>
    {
        protected override void OnCreate()
        {
            base.OnCreate();
            Title = "UInt[] ForEach";
            TitleColor = Color.FromArgb(200, Constants.C_COLOR_UINT);
        }
    }
}
