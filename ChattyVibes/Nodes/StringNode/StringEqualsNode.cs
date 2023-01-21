using ST.Library.UI.NodeEditor;

namespace ChattyVibes.Nodes.StringNode
{
    [STNode("/String", "LauraRozier", "", "", "String equals node")]
    internal sealed class StringEqualsNode : StringNode
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
            Title = "String Equal";

            m_op_a_in = InputOptions.Add("A", typeof(string), true);
            m_op_b_in = InputOptions.Add("B", typeof(string), true);
            m_op_out = OutputOptions.Add("False", typeof(bool), false);

            m_op_a_in.DataTransfer += new STNodeOptionEventHandler(m_in_DataTransfer);
            m_op_b_in.DataTransfer += new STNodeOptionEventHandler(m_in_DataTransfer);
            m_op_out.TransferData(false);
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
                SetOptionText(m_op_out, "False");
                m_op_out.TransferData(false);
                return;
            }

            bool result = _str_a.Equals(_str_b);
            SetOptionText(m_op_out, result ? "True" : "False");
            m_op_out.TransferData(result);
        }
    }
}
