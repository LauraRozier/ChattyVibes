using ST.Library.UI.NodeEditor;
using System;

namespace ChattyVibes.Nodes.StringNode
{
    [STNode("/String", "LauraRozier", "", "", "String replace node")]
    internal sealed class StringReplaceNode : StringNode
    {
        private string _str = "";
        private string _old = "";
        private string _new = "";

        private STNodeOption m_op_str_in;
        private STNodeOption m_op_old_in;
        private STNodeOption m_op_new_in;
        private STNodeOption m_op_out;

        protected override void OnCreate()
        {
            base.OnCreate();
            Title = "String Replace";

            m_op_str_in = InputOptions.Add("String", typeof(string), true);
            m_op_old_in = InputOptions.Add("Old", typeof(string), true);
            m_op_new_in = InputOptions.Add("New", typeof(string), true);
            m_op_out = OutputOptions.Add("", typeof(string), false);

            m_op_str_in.DataTransfer += new STNodeOptionEventHandler(m_in_DataTransfer);
            m_op_old_in.DataTransfer += new STNodeOptionEventHandler(m_in_DataTransfer);
            m_op_new_in.DataTransfer += new STNodeOptionEventHandler(m_in_DataTransfer);
            m_op_out.TransferData(_str);
        }

        private void m_in_DataTransfer(object sender, STNodeOptionEventArgs e)
        {
            if (e.Status == ConnectionStatus.Connected && e.TargetOption.Data != null)
            {
                if (sender == m_op_str_in)
                    _str = (string)e.TargetOption.Data;
                else if (sender == m_op_old_in)
                    _old = (string)e.TargetOption.Data;
                else
                    _new = (string)e.TargetOption.Data;
            }
            else
            {
                if (sender == m_op_str_in)
                    _str = "";
                else if (sender == m_op_old_in)
                    _old = "";
                else
                    _new = "";
            }

            string result = _str.Replace(_old, _new);
            SetOptionText(m_op_out, result);
            m_op_out.TransferData(result);
        }
    }
}
