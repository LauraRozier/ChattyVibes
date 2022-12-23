using ST.Library.UI.NodeEditor;
using System.Drawing;

namespace ChattyVibes.Nodes.ListNode
{
    //[STNode("/List", "LauraRozier", "", "", "List<UInt> ForEach node")]
    internal class UIntListForEachNode : ListForEachNode<uint>
    {
        protected override void OnCreate()
        {
            base.OnCreate();
            Title = "List<UInt> ForEach";
            TitleColor = Color.FromArgb(200, Constants.C_COLOR_UINT);
        }
    }
}
