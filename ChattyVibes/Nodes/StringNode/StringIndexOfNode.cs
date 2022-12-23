using ST.Library.UI.NodeEditor;

namespace ChattyVibes.Nodes.StringNode
{
    [STNode("/String", "LauraRozier", "", "", "String index of node")]
    internal sealed class StringIndexOfNode : StringNode
    {
        private string _haystack = "";
        private string _needle = "";

        private STNodeOption m_op_haystack_in;
        private STNodeOption m_op_needle_in;
        private STNodeOption m_op_out;

        protected override void OnCreate()
        {
            base.OnCreate();
            Title = "String Index Of";

            m_op_haystack_in = InputOptions.Add("Haystack", typeof(string), true);
            m_op_needle_in = InputOptions.Add("Needle", typeof(string), true);
            m_op_out = OutputOptions.Add("-1", typeof(int), false);

            m_op_haystack_in.DataTransfer += new STNodeOptionEventHandler(m_in_DataTransfer);
            m_op_needle_in.DataTransfer += new STNodeOptionEventHandler(m_in_DataTransfer);
            m_op_out.TransferData(-1);
        }

        private void m_in_DataTransfer(object sender, STNodeOptionEventArgs e)
        {
            if (e.Status == ConnectionStatus.Connected && e.TargetOption.Data != null)
            {
                if (sender == m_op_haystack_in)
                    _haystack = (string)e.TargetOption.Data;
                else
                    _needle = (string)e.TargetOption.Data;
            }
            else
            {
                if (sender == m_op_haystack_in)
                    _haystack = "";
                else
                    _needle = "";
            }

            int result = _haystack.IndexOf(_needle);
            SetOptionText(m_op_out, result.ToString());
            m_op_out.TransferData(result);
        }
    }
}
