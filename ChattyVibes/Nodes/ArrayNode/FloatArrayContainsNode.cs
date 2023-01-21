using ST.Library.UI.NodeEditor;
using System.Drawing;

namespace ChattyVibes.Nodes.ArrayNode
{
    [STNode("/Array", "LauraRozier", "", "", "Float[] Contains node")]
    internal class FloatArrayContainsNode : ArrayContainsNode<float>
    {
        protected override void OnCreate()
        {
            base.OnCreate();
            Title = "Float[] Contains";
            TitleColor = Color.FromArgb(200, Constants.C_COLOR_FLOAT);
        }
    }
}
