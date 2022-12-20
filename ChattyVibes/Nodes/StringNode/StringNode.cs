using ST.Library.UI.NodeEditor;
using System.Drawing;

namespace ChattyVibes.Nodes.StringNode
{
    internal abstract class StringNode : STNode
    {
        protected override void OnCreate()
        {
            base.OnCreate();
            TitleColor = Color.FromArgb(200, Constants.C_COLOR_STRING);
        }
    }
}
