using ST.Library.UI.NodeEditor;
using System.Drawing;

namespace ChattyVibes.Nodes.ArrayNode
{
    [STNode("/Array", "LauraRozier", "", "", "Float[] ForEach node")]
    internal class FloatArrayForEachNode : ArrayForEachNode<float>
    {
        protected override void OnCreate()
        {
            base.OnCreate();
            Title = "Float[] ForEach";
            TitleColor = Color.FromArgb(200, Constants.C_COLOR_FLOAT);
        }
    }
}
