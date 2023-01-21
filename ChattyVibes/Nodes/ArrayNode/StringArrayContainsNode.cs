using ST.Library.UI.NodeEditor;
using System.Drawing;

namespace ChattyVibes.Nodes.ArrayNode
{
    [STNode("/Array", "LauraRozier", "", "", "String[] Contains node")]
    internal class StringArrayContainsNode : ArrayContainsNode<string>
    {
        protected override void OnCreate()
        {
            base.OnCreate();
            Title = "String[] Contains";
            TitleColor = Color.FromArgb(200, Constants.C_COLOR_STRING);
        }
    }
}
