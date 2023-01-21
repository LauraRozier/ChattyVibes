using ST.Library.UI.NodeEditor;

namespace ChattyVibes.Nodes.StringNode
{
    [STNode("/String", "LauraRozier", "", "", "String remove node")]
    internal sealed class StringRemoveNode : StringNode
    {
        private string _str = string.Empty;
        private int _start = 0;
        private int _count = 0;

        private STNodeOption m_op_str_in;
        private STNodeOption m_op_start_in;
        private STNodeOption m_op_count_in;
        private STNodeOption m_op_out;

        protected override void OnCreate()
        {
            base.OnCreate();
            Title = "String Remove";

            m_op_str_in = InputOptions.Add("String", typeof(string), true);
            m_op_start_in = InputOptions.Add("Start", typeof(int), true);
            m_op_count_in = InputOptions.Add("Count", typeof(int), true);
            m_op_out = OutputOptions.Add(string.Empty, typeof(string), false);

            m_op_str_in.DataTransfer += new STNodeOptionEventHandler(m_in_DataTransfer);
            m_op_start_in.DataTransfer += new STNodeOptionEventHandler(m_in_DataTransfer);
            m_op_count_in.DataTransfer += new STNodeOptionEventHandler(m_in_DataTransfer);
            m_op_out.TransferData(_str);
        }

        private void m_in_DataTransfer(object sender, STNodeOptionEventArgs e)
        {
            if (e.Status == ConnectionStatus.Connected && e.TargetOption.Data != null)
            {
                if (sender == m_op_str_in)
                    _str = (string)e.TargetOption.Data;
                else if (sender == m_op_start_in)
                    _start = (int)e.TargetOption.Data;
                else
                    _count = (int)e.TargetOption.Data;
            }
            else
            {
                if (sender == m_op_str_in)
                    _str = string.Empty;
                else if (sender == m_op_start_in)
                    _start = 0;
                else
                    _count = 0;
            }

            string result = _str.Remove(_start, _count);
            SetOptionText(m_op_out, result);
            m_op_out.TransferData(result);
        }
    }
}
