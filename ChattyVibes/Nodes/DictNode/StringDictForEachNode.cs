using ST.Library.UI.NodeEditor;
using System.Drawing;

namespace ChattyVibes.Nodes.DictNode
{
    //[STNode("/Dict", "LauraRozier", "", "", "Dict<String,String> ForEach node")]
    internal class StringDictForEachNode : DictForEachNode<string, string>
    {
        protected override void OnCreate()
        {
            base.OnCreate();
            Title = "Dict<String,String> ForEach";
            TitleColor = Color.FromArgb(200, Constants.C_COLOR_STRING);
        }
    }
}
