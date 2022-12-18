using ST.Library.UI.NodeEditor;
using System.Drawing;

namespace ChattyVibes.Nodes.Math.Int
{
    [STNode("/Math/Int", "LauraRozier", "", "", "Compare two numbers")]
    internal class IntCompareNode : CompareNode<int>
    {
        protected override void OnCreate()
        {
            _defaultVal = 0;
            base.OnCreate();
            Title = "Int Compare";
            TitleColor = Color.FromArgb(200, FrmBindingGraphs.C_COLOR_INT);
        }
    }
}
