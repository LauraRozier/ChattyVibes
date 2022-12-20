using ST.Library.UI.NodeEditor;
using System.Drawing;

namespace ChattyVibes.Nodes.CharNode
{
    internal abstract class CharNode : STNode
    {
        protected override void OnCreate()
        {
            base.OnCreate();
            TitleColor = Color.FromArgb(200, Constants.C_COLOR_CHAR);
        }
    }
}
