using ST.Library.UI.NodeEditor;
using System.Drawing;
using System.Windows.Forms;

namespace ChattyVibes.Nodes.EventNode.AppNode
{
    [STNode("/Events/App", "LauraRozier", "", "", "Manual event trigger node")]
    internal sealed class ManualEventTrigger : EventNode
    {
        protected override void OnCreate()
        {
            _direction = FlowDirection.Both;
            base.OnCreate();
            Title = "Manual Event Trigger";
            AutoSize = false;
            Width = 170;
            Height = 50;

            var ctrl = new NodeButton
            {
                Text = "Trigger",
                Location = new Point(48, 4)
            };
            ctrl.MouseUp += new MouseEventHandler((s,e) => Trigger());
            Controls.Add(ctrl);
        }
    }
}
