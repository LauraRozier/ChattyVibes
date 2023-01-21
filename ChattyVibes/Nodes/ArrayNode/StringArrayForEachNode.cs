using ST.Library.UI.NodeEditor;
using System.Drawing;

namespace ChattyVibes.Nodes.ArrayNode
{
    [STNode("/Array", "LauraRozier", "", "", "String[] ForEach node")]
    internal class StringArrayForEachNode : ArrayForEachNode<string>
    {
        protected override void OnCreate()
        {
            base.OnCreate();
            Title = "String[] ForEach";
            TitleColor = Color.FromArgb(200, Constants.C_COLOR_STRING);
        }
    }
}
