using ST.Library.UI.NodeEditor;

namespace ChattyVibes.Nodes.StringNode
{
    [STNode("/String", "LauraRozier", "", "", "String to upper node")]
    internal sealed class StringToUpperNode : StringNode
    {
        private string _value = "";

        private STNodeOption m_op_in;
        private STNodeOption m_op_out;

        protected override void OnCreate()
        {
            base.OnCreate();
            Title = "String To Upper";

            m_op_in = InputOptions.Add("", typeof(string), true);
            m_op_out = OutputOptions.Add("", typeof(string), false);

            m_op_in.DataTransfer += new STNodeOptionEventHandler(m_in_DataTransfer);
            m_op_out.TransferData(_value);
        }

        private void m_in_DataTransfer(object sender, STNodeOptionEventArgs e)
        {
            if (e.Status == ConnectionStatus.Connected && e.TargetOption.Data != null)
                _value = ((string)e.TargetOption.Data).ToUpper();
            else
                _value = "";

            SetOptionText(m_op_in, _value);
            m_op_out.TransferData(_value);
        }
    }
}
