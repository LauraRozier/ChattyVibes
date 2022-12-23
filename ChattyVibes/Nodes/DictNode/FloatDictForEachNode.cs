using ST.Library.UI.NodeEditor;
using System.Drawing;

namespace ChattyVibes.Nodes.DictNode
{
    //[STNode("/Dict", "LauraRozier", "", "", "Dict<String,Float> ForEach node")]
    internal class FloatDictForEachNode : DictForEachNode<string, float>
    {
        protected override void OnCreate()
        {
            base.OnCreate();
            Title = "Dict<String,Float> ForEach";
            TitleColor = Color.FromArgb(200, Constants.C_COLOR_FLOAT);
        }
    }
}
