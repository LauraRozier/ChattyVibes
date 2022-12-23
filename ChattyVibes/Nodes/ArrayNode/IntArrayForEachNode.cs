using ST.Library.UI.NodeEditor;
using System.Drawing;

namespace ChattyVibes.Nodes.ArrayNode
{
    [STNode("/Array", "LauraRozier", "", "", "Int[] ForEach node")]
    internal class IntArrayForEachNode : ArrayForEachNode<int>
    {
        protected override void OnCreate()
        {
            base.OnCreate();
            Title = "Int[] ForEach";
            TitleColor = Color.FromArgb(200, Constants.C_COLOR_INT);
        }
    }
}
