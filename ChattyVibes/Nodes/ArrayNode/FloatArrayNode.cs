using ST.Library.UI.NodeEditor;
using System.Drawing;

namespace ChattyVibes.Nodes.ArrayNode
{
    [STNode("/Array", "LauraRozier", "", "", "Float[] node")]
    internal class FloatArrayNode : ArrayNode<float>
    {
        protected override void OnCreate()
        {
            base.OnCreate();
            Title = "Float[]";
            TitleColor = Color.FromArgb(200, Constants.C_COLOR_FLOAT);
        }
    }
}
