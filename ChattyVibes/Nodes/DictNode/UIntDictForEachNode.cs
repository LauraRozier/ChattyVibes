using ST.Library.UI.NodeEditor;
using System.Drawing;

namespace ChattyVibes.Nodes.DictNode
{
    [STNode("/Dict", "LauraRozier", "", "", "Dict<String,UInt> ForEach node")]
    internal class UIntDictForEachNode : DictForEachNode<string, uint>
    {
        protected override void OnCreate()
        {
            base.OnCreate();
            Title = "Dict<String,UInt> ForEach";
            TitleColor = Color.FromArgb(200, Constants.C_COLOR_UINT);
        }
    }
}
