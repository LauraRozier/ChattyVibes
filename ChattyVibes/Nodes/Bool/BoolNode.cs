using ST.Library.UI.NodeEditor;
using System.Drawing;

namespace ChattyVibes.Nodes.Bool
{
    internal class BoolNode : STNode
    {
        protected override void OnCreate()
        {
            base.OnCreate();
            TitleColor = Color.FromArgb(200, FrmBindingGraphs.C_COLOR_BOOL);
        }
    }
}
