using ST.Library.UI.NodeEditor;

namespace ChattyVibes.Nodes.StringNode
{
    [STNode("/String", "LauraRozier", "", "", "String compare node")]
    internal sealed class StringCompareNode : StringNode
    {
        private string _str_a = string.Empty;
        private string _str_b = string.Empty;
        [STNodeProperty("B", "The second string")]
        public string Needle
        {
            get { return _str_b; }
            set
            {
                _str_b = value;
                ProcessResult();
            }
        }

        private STNodeOption m_op_a_in;
        private STNodeOption m_op_b_in;
        private STNodeOption m_op_out;

        protected override void OnCreate()
        {
            base.OnCreate();
            Title = "String Compare";

            m_op_a_in = InputOptions.Add("A", typeof(string), true);
            m_op_b_in = InputOptions.Add("B", typeof(string), true);
            m_op_out = OutputOptions.Add("-1", typeof(int), false);

            m_op_a_in.DataTransfer += new STNodeOptionEventHandler(m_in_DataTransfer);
            m_op_b_in.DataTransfer += new STNodeOptionEventHandler(m_in_DataTransfer);
            m_op_out.TransferData(-1);
        }

        private void m_in_DataTransfer(object sender, STNodeOptionEventArgs e)
        {
            if (e.Status == ConnectionStatus.Connected && e.TargetOption.Data != null)
            {
                if (sender == m_op_a_in)
                    _str_a = (string)e.TargetOption.Data;
                else
                    _str_b = (string)e.TargetOption.Data;
            }
            else
            {
                if (sender == m_op_a_in)
                    _str_a = string.Empty;
                else
                    _str_b = string.Empty;
            }

            ProcessResult();
        }

        private void ProcessResult()
        {
            if (string.IsNullOrEmpty(_str_a) || string.IsNullOrEmpty(_str_b))
            {
                SetOptionText(m_op_out, "-1");
                m_op_out.TransferData(-1);
                return;
            }

            int result = _str_a.CompareTo(_str_b);
            SetOptionText(m_op_out, result.ToString());
            m_op_out.TransferData(result);
        }
    }
}
