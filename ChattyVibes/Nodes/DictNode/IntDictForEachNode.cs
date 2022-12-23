using ST.Library.UI.NodeEditor;
using System.Drawing;

namespace ChattyVibes.Nodes.DictNode
{
    //[STNode("/Dict", "LauraRozier", "", "", "Dict<String,Int> ForEach node")]
    internal class IntDictForEachNode : DictForEachNode<string, int>
    {
        protected override void OnCreate()
        {
            base.OnCreate();
            Title = "Dict<String,Int> ForEach";
            TitleColor = Color.FromArgb(200, Constants.C_COLOR_INT);
        }
    }
}
