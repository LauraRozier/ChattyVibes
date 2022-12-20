using ST.Library.UI.NodeEditor;
using System.Drawing;

namespace ChattyVibes.Nodes.EnumNode
{
    internal abstract class EnumNode : STNode
    {
        protected override void OnCreate()
        {
            base.OnCreate();
            TitleColor = Color.FromArgb(200, Constants.C_COLOR_ENUM);
        }
    }
}
