using ST.Library.UI.NodeEditor;
using TwitchLib.Client.Enums;

namespace ChattyVibes.Nodes.EnumNode
{
    [STNode("/Enum", "LauraRozier", "", "", "CommercialLength display node")]
    internal sealed class CommercialLengthDisplayNode : EnumNode
    {
        private CommercialLength _value = CommercialLength.Seconds60;

        private STNodeOption m_op_in;
        private STNodeOption m_op_out;

        protected override void OnCreate()
        {
            base.OnCreate();
            Title = "CommercialLength Display";

            m_op_in = InputOptions.Add("Seconds60", typeof(CommercialLength), true);
            m_op_out = OutputOptions.Add(string.Empty, typeof(CommercialLength), false);

            m_op_in.DataTransfer += new STNodeOptionEventHandler(m_op_in_DataTransfer);
            m_op_out.TransferData(_value);
        }

        private void m_op_in_DataTransfer(object sender, STNodeOptionEventArgs e)
        {
            if (e.Status == ConnectionStatus.Connected && e.TargetOption.Data != null)
                _value = (CommercialLength)e.TargetOption.Data;
            else
                _value = CommercialLength.Seconds60;

            SetOptionText(m_op_in, _value.ToString());
            m_op_out.TransferData(_value);
        }
    }
}
