using ST.Library.UI.NodeEditor;
using System.Drawing;

namespace ChattyVibes.Nodes.ListNode
{
    //[STNode("/List", "LauraRozier", "", "", "List<String> ForEach node")]
    internal class StringListForEachNode : ListForEachNode<string>
    {
        protected override void OnCreate()
        {
            base.OnCreate();
            Title = "List<String> ForEach";
            TitleColor = Color.FromArgb(200, Constants.C_COLOR_STRING);
        }
    }
}
