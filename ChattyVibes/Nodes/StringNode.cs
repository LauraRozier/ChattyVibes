using ST.Library.UI.NodeEditor;
using System.Drawing;

namespace ChattyVibes.Nodes
{
    internal class StringNode : STNode
    {
        protected override void OnCreate()
        {
            base.OnCreate();
            TitleColor = Color.FromArgb(200, FrmBindingGraphs.C_COLOR_STRING);
        }
    }
}
