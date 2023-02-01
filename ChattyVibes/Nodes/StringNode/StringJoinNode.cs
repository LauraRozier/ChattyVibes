using ST.Library.UI.NodeEditor;

namespace ChattyVibes.Nodes.StringNode
{
    [STNode("/String", "LauraRozier", "", "", "String join node")]
    internal sealed class StringJoinNode : StringNode
    {
        private string[] _strArr = null;
        private char _sep = ',';
        [STNodeProperty("Separator", "The character to join with")]
        public char Separator
        {
            get { return _sep; }
            set
            {
                _sep = value;
                ProcessResult();
            }
        }

        private STNodeOption m_op_str_in;
        private STNodeOption m_op_sep_in;
        private STNodeOption m_op_out;

        protected override void OnCreate()
        {
            base.OnCreate();
            Title = "String Join";

            m_op_str_in = InputOptions.Add("Strings", typeof(string[]), true);
            m_op_sep_in = InputOptions.Add("Separator", typeof(char), true);
            m_op_out = OutputOptions.Add("OUT", typeof(string), false);

            m_op_str_in.DataTransfer += new STNodeOptionEventHandler(m_in_DataTransfer);
            m_op_sep_in.DataTransfer += new STNodeOptionEventHandler(m_in_DataTransfer);
            m_op_out.TransferData(string.Empty);
        }

        private void m_in_DataTransfer(object sender, STNodeOptionEventArgs e)
        {
            if (e.Status == ConnectionStatus.Connected && e.TargetOption.Data != null)
            {
                if (sender == m_op_str_in)
                    _strArr = (string[])e.TargetOption.Data;
                else
                    _sep = (char)e.TargetOption.Data;
            }
            else
            {
                if (sender == m_op_str_in)
                    _strArr = null;
                else
                    _sep = char.MinValue;
            }

            ProcessResult();
        }

        private void ProcessResult()
        {
            if (_strArr == null || char.IsControl(_sep))
            {
                m_op_out.TransferData(string.Empty);
                return;
            }

            string result = string.Join($"{_sep}", _strArr);
            m_op_out.TransferData(result);
        }
    }
}
