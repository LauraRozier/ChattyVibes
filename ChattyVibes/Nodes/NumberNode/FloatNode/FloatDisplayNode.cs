using ST.Library.UI.NodeEditor;
using System.Globalization;

namespace ChattyVibes.Nodes.NumberNode.FloatNode
{
    [STNode("/Number/Float", "LauraRozier", "", "", "Float display node")]
    internal class FloatDisplayNode : BaseFloatNode
    {
        private float _value = 0.0f;

        private STNodeOption m_op_in;
        private STNodeOption m_op_out;

        protected override void OnCreate()
        {
            base.OnCreate();
            Title = "Float Display";

            m_op_in = InputOptions.Add("0", typeof(float), true);
            m_op_out = OutputOptions.Add("", typeof(float), false);

            m_op_in.DataTransfer += new STNodeOptionEventHandler(m_in_DataTransfer);
            m_op_out.TransferData(_value);
        }

        private void m_in_DataTransfer(object sender, STNodeOptionEventArgs e)
        {
            if (e.Status == ConnectionStatus.Connected && e.TargetOption.Data != null)
                _value = (float)e.TargetOption.Data;
            else
                _value = 0.0f;

            SetOptionText(m_op_in, _value.ToString("G", CultureInfo.InvariantCulture));
            m_op_out.TransferData(_value);
        }
    }
}
