using ST.Library.UI.NodeEditor;
using System.Drawing;

namespace ChattyVibes.Nodes.ListNode
{
    [STNode("/List", "LauraRozier", "", "", "List<Float> ForEach node")]
    internal class FloatListForEachNode : ListForEachNode<float>
    {
        protected override void OnCreate()
        {
            base.OnCreate();
            Title = "List<Float> ForEach";
            TitleColor = Color.FromArgb(200, Constants.C_COLOR_FLOAT);
        }
    }
}
