using ST.Library.UI.NodeEditor;

namespace ChattyVibes.Nodes.StringNode
{
    [STNode("/String", "LauraRozier", "", "", "String split node")]
    internal sealed class StringSplitNode : StringNode
    {
        private string _str = string.Empty;
        private char _sep = char.MinValue;
        [STNodeProperty("Separator", "The character to split on")]
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
            Title = "String Split";

            m_op_str_in = InputOptions.Add("String", typeof(string), true);
            m_op_sep_in = InputOptions.Add("Separator", typeof(char), true);
            m_op_out = OutputOptions.Add("OUT", typeof(string[]), false);

            m_op_str_in.DataTransfer += new STNodeOptionEventHandler(m_in_DataTransfer);
            m_op_sep_in.DataTransfer += new STNodeOptionEventHandler(m_in_DataTransfer);
            m_op_out.TransferData(null);
        }

        private void m_in_DataTransfer(object sender, STNodeOptionEventArgs e)
        {
            if (e.Status == ConnectionStatus.Connected && e.TargetOption.Data != null)
            {
                if (sender == m_op_str_in)
                    _str = (string)e.TargetOption.Data;
                else
                    _sep = (char)e.TargetOption.Data;
            }
            else
            {
                if (sender == m_op_str_in)
                    _str = string.Empty;
                else
                    _sep = char.MinValue;
            }

            ProcessResult();
        }

        private void ProcessResult()
        {
            if (string.IsNullOrEmpty(_str) || char.IsControl(_sep))
            {
                m_op_out.TransferData(null);
                return;
            }

            string[] result = _str.Split(
                new char[1] { _sep },
                System.StringSplitOptions.RemoveEmptyEntries
            );
            m_op_out.TransferData(result);
        }
    }
}
