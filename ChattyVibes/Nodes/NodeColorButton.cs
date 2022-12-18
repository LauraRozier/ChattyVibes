using ST.Library.UI.NodeEditor;
using System;
using System.Windows.Forms;

namespace ChattyVibes.Nodes
{
    internal class NodeColorButton : STNodeControl
    {
        public event EventHandler ValueChanged;
        protected virtual void OnValueChanged(EventArgs e) =>
            ValueChanged?.Invoke(this, e);

        protected override void OnMouseClick(System.Windows.Forms.MouseEventArgs e)
        {
            base.OnMouseClick(e);
            ColorDialog cd = new ColorDialog();

            if (cd.ShowDialog() != DialogResult.OK)
                return;

            //this._Color = cd.Color;
            BackColor = cd.Color;
            OnValueChanged(new EventArgs());
        }
    }
}
