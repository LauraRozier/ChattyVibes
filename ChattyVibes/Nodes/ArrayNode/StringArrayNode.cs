using ST.Library.UI.NodeEditor;
using System.Drawing;

namespace ChattyVibes.Nodes.ArrayNode
{
    [STNode("/Array", "LauraRozier", "", "", "String[] node")]
    internal class StringArrayNode : ArrayNode<string>
    {
        protected override void OnCreate()
        {
            base.OnCreate();
            Title = "String[]";
            TitleColor = Color.FromArgb(200, Constants.C_COLOR_STRING);
        }
    }
}
