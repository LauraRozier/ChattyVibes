using ST.Library.UI.NodeEditor;
using System.Drawing;

namespace ChattyVibes.Nodes.ArrayNode
{
    [STNode("/Array", "LauraRozier", "", "", "Int[] node")]
    internal class IntArrayNode : ArrayNode<int>
    {
        protected override void OnCreate()
        {
            base.OnCreate();
            Title = "Int[]";
            TitleColor = Color.FromArgb(200, Constants.C_COLOR_INT);
        }
    }
}
