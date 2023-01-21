using ST.Library.UI.NodeEditor;

namespace ChattyVibes.Nodes.BoolNode
{
    [STNode("/Bool", "LauraRozier", "", "", "Boolean display node")]
    internal sealed class BoolDisplayNode : BoolNode
    {
        private bool _value = false;

        private STNodeOption m_op_in;
        private STNodeOption m_op_out;

        protected override void OnCreate()
        {
            base.OnCreate();
            Title = "Bool Display";

            m_op_in = InputOptions.Add("False", typeof(bool), true);
            m_op_out = OutputOptions.Add(string.Empty, typeof(bool), false);

            m_op_in.DataTransfer += new STNodeOptionEventHandler(m_op_bool_in_DataTransfer);
            m_op_out.TransferData(_value);
        }

        private void m_op_bool_in_DataTransfer(object sender, STNodeOptionEventArgs e)
        {
            if (e.Status == ConnectionStatus.Connected && e.TargetOption.Data != null)
                _value = (bool)e.TargetOption.Data;
            else
                _value = false;

            SetOptionText(m_op_in, _value ? "True" : "False");
            m_op_out.TransferData(_value);
        }
    }
}
