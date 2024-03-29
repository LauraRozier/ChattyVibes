﻿using ST.Library.UI.NodeEditor;

namespace ChattyVibes.Nodes.StringNode
{
    [STNode("/String", "LauraRozier", "", "", "String append node")]
    internal sealed class StringAppendNode : StringNode
    {
        private string _valA = string.Empty;
        private string _valB = string.Empty;

        private STNodeOption m_op_inA;
        private STNodeOption m_op_inB;
        private STNodeOption m_op_out;

        protected override void OnCreate()
        {
            base.OnCreate();
            Title = "String Append";

            m_op_inA = InputOptions.Add("A", typeof(string), true);
            m_op_inB = InputOptions.Add("B", typeof(string), true);
            m_op_out = OutputOptions.Add(string.Empty, typeof(string), false);

            m_op_inA.DataTransfer += new STNodeOptionEventHandler(m_in_num_DataTransfer);
            m_op_inB.DataTransfer += new STNodeOptionEventHandler(m_in_num_DataTransfer);
            m_op_out.TransferData(string.Empty);
        }

        private void m_in_num_DataTransfer(object sender, STNodeOptionEventArgs e)
        {
            if (e.Status == ConnectionStatus.Connected && e.TargetOption.Data != null)
            {
                if (sender == m_op_inA)
                    _valA = (string)e.TargetOption.Data;
                else
                    _valB = (string)e.TargetOption.Data;
            }
            else
            {
                if (sender == m_op_inA)
                    _valA = string.Empty;
                else
                    _valB = string.Empty;
            }

            string result = _valA + _valB;
            SetOptionText(m_op_out, result);
            m_op_out.TransferData(result);
        }
    }
}
