using ST.Library.UI.NodeEditor;
using System.Drawing;

namespace ChattyVibes.Nodes.ListNode
{
    //[STNode("/List", "LauraRozier", "", "", "List<Int> ForEach node")]
    internal class IntListForEachNode : ListForEachNode<int>
    {
        protected override void OnCreate()
        {
            base.OnCreate();
            Title = "List<Int> ForEach";
            TitleColor = Color.FromArgb(200, Constants.C_COLOR_INT);
        }
    }
}
