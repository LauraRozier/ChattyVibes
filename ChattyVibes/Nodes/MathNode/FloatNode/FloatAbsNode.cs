using ST.Library.UI.NodeEditor;
using System;
using System.Globalization;

namespace ChattyVibes.Nodes.MathNode.FloatNode
{
    [STNode("/Math/Float", "LauraRozier", "", "", "Returns the absolute value of a specified number.")]
    internal class FloatAbsNode : Nodes.FloatNode
    {
        private float _val = 0.0f;

        private STNodeOption m_in;
        private STNodeOption m_out;

        protected override void OnCreate()
        {
            base.OnCreate();
            Title = "Float Abs";

            m_in = InputOptions.Add("0", typeof(float), true);
            m_out = OutputOptions.Add("0", typeof(float), false);

            m_in.DataTransfer += new STNodeOptionEventHandler(m_in_DataTransfer);

            ProcessResult();
        }

        private void m_in_DataTransfer(object sender, STNodeOptionEventArgs e)
        {
            if (e.Status == ConnectionStatus.Connected && e.TargetOption.Data != null)
                _val = (float)e.TargetOption.Data;
            else
                _val = 0.0f;

            ProcessResult();
        }

        private void ProcessResult()
        {
            float result = Math.Abs(_val);
            SetOptionText(m_in, _val.ToString("G", CultureInfo.InvariantCulture));
            SetOptionText(m_out, result.ToString("G", CultureInfo.InvariantCulture));
            m_out.TransferData(result);
        }
    }
}
