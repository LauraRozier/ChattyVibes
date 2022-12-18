using ST.Library.UI.NodeEditor;
using System.Drawing;

namespace ChattyVibes.Nodes.Number.Int
{
    [STNode("/String", "LauraRozier", "", "", "String contains node")]
    internal class StringContainsNode : StringNode
    {
        private string _haystack = "";
        private string _needle = "";
        [STNodeProperty("Needle", "The sub-text to find")]
        public string Needle
        {
            get { return _needle; }
            set
            {
                _needle = value;
                ProcessResult();
            }
        }
        private bool _caseSensitive = true;
        [STNodeProperty("Case-sensitive", "Whether the check is case-sensitive or not")]
        public bool CaseSensitive
        {
            get { return _caseSensitive; }
            set
            {
                _caseSensitive = value;
                m_ctrl_checkbox.Checked = value;
                Invalidate();
                ProcessResult();
            }
        }

        private NodeCheckBox m_ctrl_checkbox;

        private STNodeOption m_op_haystack_in;
        private STNodeOption m_op_needle_in;
        private STNodeOption m_op_sensitive_in;
        private STNodeOption m_op_out;

        protected override void OnCreate()
        {
            base.OnCreate();
            Title = "String Contains";

            m_op_haystack_in = InputOptions.Add("Haystack", typeof(string), true);
            m_op_needle_in = InputOptions.Add("Needle", typeof(string), true);
            m_op_sensitive_in = InputOptions.Add("", typeof(bool), true);
            m_op_out = OutputOptions.Add("False", typeof(bool), false);

            m_ctrl_checkbox = new NodeCheckBox
            {
                Text = "Case-Sensitive",
                DisplayRectangle = new Rectangle(10, 40, 120, 20),
                Checked = _caseSensitive
            };
            m_ctrl_checkbox.ValueChanged += (s, e) =>
            {
                _caseSensitive = m_ctrl_checkbox.Checked;
                ProcessResult();
            };
            Controls.Add(m_ctrl_checkbox);

            m_op_haystack_in.DataTransfer += new STNodeOptionEventHandler(m_in_DataTransfer);
            m_op_needle_in.DataTransfer += new STNodeOptionEventHandler(m_in_DataTransfer);
            m_op_sensitive_in.DataTransfer += new STNodeOptionEventHandler(m_in_DataTransfer);
            m_op_out.TransferData(false);
        }

        private void m_in_DataTransfer(object sender, STNodeOptionEventArgs e)
        {
            if (e.Status == ConnectionStatus.Connected && e.TargetOption.Data != null)
            {
                if (sender == m_op_haystack_in)
                    _haystack = (string)e.TargetOption.Data;
                else if (sender == m_op_needle_in)
                    _needle = (string)e.TargetOption.Data;
                else
                    CaseSensitive = (bool)e.TargetOption.Data;
            }
            else
            {
                if (sender == m_op_haystack_in)
                    _haystack = "";
                else if (sender == m_op_needle_in)
                    _needle = "";
            }

            ProcessResult();
        }

        private void ProcessResult()
        {
            if (_haystack == "" || _needle == "")
            {
                SetOptionText(m_op_out, "False");
                m_op_out.TransferData(false);
                return;
            }

            bool result;

            if (_caseSensitive)
            {
                result = _haystack.Contains(_needle);
            }
            else
            {
                string tmpHay = _haystack.ToUpper();
                string tmpNeedle = _needle.ToUpper();
                result = tmpHay.Contains(tmpNeedle);
            }

            SetOptionText(m_op_out, result ? "True" : "False");
            m_op_out.TransferData(result);
        }
    }
}
