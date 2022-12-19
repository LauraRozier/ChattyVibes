using ST.Library.UI.NodeEditor;
using System.Drawing;

namespace ChattyVibes.Nodes.Bool
{
    [STNode("/Logic", "LauraRozier", "", "", "Boolean logic NOT node")]
    internal class LogicNOTNode : STNode
    {
        private bool _value = true;

        private STNodeOption m_op_in;
        private STNodeOption m_op_out;

        protected override void OnCreate()
        {
            base.OnCreate();
            TitleColor = Color.FromArgb(200, FrmBindingGraphs.C_COLOR_LOGIC);
            Title = "Logic NOT";

            m_op_in = InputOptions.Add("", typeof(bool), true);
            m_op_out = OutputOptions.Add("", typeof(bool), false);

            m_op_in.DataTransfer += new STNodeOptionEventHandler(m_op_bool_in_DataTransfer);
            m_op_out.TransferData(_value);
        }

        private void m_op_bool_in_DataTransfer(object sender, STNodeOptionEventArgs e)
        {
            if (e.Status == ConnectionStatus.Connected && e.TargetOption.Data != null)
                _value = !((bool)e.TargetOption.Data);
            else
                _value = true;

            SetOptionText(m_op_out, _value ? "True" : "False");
            m_op_out.TransferData(_value);
            Invalidate();
        }
    }
}
