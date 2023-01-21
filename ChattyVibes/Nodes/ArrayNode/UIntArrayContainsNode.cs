using ST.Library.UI.NodeEditor;
using System.Drawing;

namespace ChattyVibes.Nodes.ArrayNode
{
    [STNode("/Array", "LauraRozier", "", "", "UInt[] Contains node")]
    internal class UIntArrayContainsNode : ArrayContainsNode<uint>
    {
        protected override void OnCreate()
        {
            base.OnCreate();
            Title = "UInt[] Contains";
            TitleColor = Color.FromArgb(200, Constants.C_COLOR_UINT);
        }
    }
}
