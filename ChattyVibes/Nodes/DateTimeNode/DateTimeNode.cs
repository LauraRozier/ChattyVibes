using ST.Library.UI.NodeEditor;
using System.Drawing;

namespace ChattyVibes.Nodes.DateTimeNode
{
    internal abstract class DateTimeNode : STNode
    {
        protected override void OnCreate()
        {
            base.OnCreate();
            TitleColor = Color.FromArgb(200, Constants.C_COLOR_DATETIME);
        }
    }
}
