using ST.Library.UI.NodeEditor;
using System;
using System.Drawing;

namespace ChattyVibes.Nodes.Time
{
    [STNode("/Time", "LauraRozier", "", "", "Show time node")]
    internal class DateTimeDisplayNode : STNode
    {
        private DateTime _value = default(DateTime);
        private STNodeOption m_op_in;
        private STNodeOption m_op_out;

        protected override void OnCreate()
        {
            base.OnCreate();
            Title = "DateTime Display";
            TitleColor = Color.FromArgb(200, FrmBindingGraphs.C_COLOR_DATETIME);

            m_op_in = InputOptions.Add(_value.ToString(), typeof(DateTime), true);
            m_op_out = OutputOptions.Add("", typeof(DateTime), false);

            m_op_in.DataTransfer += new STNodeOptionEventHandler(op_DataTransfer);
            m_op_out.TransferData(_value);
        }

        void op_DataTransfer(object sender, STNodeOptionEventArgs e)
        {
            if (e.Status == ConnectionStatus.Connected && e.TargetOption.Data != null)
                _value = (DateTime)e.TargetOption.Data;
            else
                _value = default(DateTime);

            SetOptionText(m_op_in, _value.ToString());
            m_op_out.TransferData(_value);
        }
    }
}
