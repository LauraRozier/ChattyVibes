using ST.Library.UI.NodeEditor;
using System.Drawing;

namespace ChattyVibes.Nodes.ArrayNode
{
    [STNode("/Array", "LauraRozier", "", "", "Int[] Contains node")]
    internal class IntArrayContainsNode : ArrayContainsNode<int>
    {
        protected override void OnCreate()
        {
            base.OnCreate();
            Title = "Int[] Contains";
            TitleColor = Color.FromArgb(200, Constants.C_COLOR_INT);
        }
    }
}
