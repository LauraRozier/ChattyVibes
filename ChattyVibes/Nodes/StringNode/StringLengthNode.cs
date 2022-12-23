using ST.Library.UI.NodeEditor;

namespace ChattyVibes.Nodes.StringNode
{
    [STNode("/String", "LauraRozier", "", "", "String length node")]
    internal sealed class StringLengthNode : StringNode
    {
        private string _value = "";

        private STNodeOption m_op_in;
        private STNodeOption m_op_out;

        protected override void OnCreate()
        {
            base.OnCreate();
            Title = "String Length";

            m_op_in = InputOptions.Add("", typeof(string), true);
            m_op_out = OutputOptions.Add("0", typeof(int), false);

            m_op_in.DataTransfer += new STNodeOptionEventHandler(m_in_DataTransfer);
            m_op_out.TransferData(_value);
        }

        private void m_in_DataTransfer(object sender, STNodeOptionEventArgs e)
        {
            if (e.Status == ConnectionStatus.Connected && e.TargetOption.Data != null)
                _value = (string)e.TargetOption.Data;
            else
                _value = "";

            SetOptionText(m_op_out, _value.Length.ToString());
            m_op_out.TransferData(_value.Length);
        }
    }
}
