using ST.Library.UI.NodeEditor;
using System.Drawing;

namespace ChattyVibes.Nodes.LogicNode
{
    internal abstract class LogicNode : STNode
    {
        protected bool _aVal = false;
        protected bool _bVal = false;
        protected bool _equal = false;

        protected STNodeOption m_op_a_in;
        protected STNodeOption m_op_b_in;
        protected STNodeOption m_op_out;

        protected abstract void Compare();

        protected override void OnCreate()
        {
            base.OnCreate();
            TitleColor = Color.FromArgb(200, Constants.C_COLOR_LOGIC);

            m_op_a_in = InputOptions.Add("A", typeof(bool), true);
            m_op_b_in = InputOptions.Add("B", typeof(bool), true);
            m_op_out = OutputOptions.Add("", typeof(bool), false);

            m_op_a_in.DataTransfer += new STNodeOptionEventHandler(m_in_DataTransfer);
            m_op_b_in.DataTransfer += new STNodeOptionEventHandler(m_in_DataTransfer);
            Compare();
        }

        private void m_in_DataTransfer(object sender, STNodeOptionEventArgs e)
        {
            if (e.Status == ConnectionStatus.Connected && e.TargetOption.Data != null)
            {
                if (sender == m_op_a_in)
                    _aVal = (bool)e.TargetOption.Data;
                else
                    _bVal = (bool)e.TargetOption.Data;
            }
            else
            {
                if (sender == m_op_a_in)
                    _aVal = false;
                else
                    _bVal = false;
            }

            Compare();
        }

        protected void SendResult()
        {
            SetOptionText(m_op_out, _equal ? "True" : "False");
            m_op_out.TransferData(_equal);
        }
    }
}
