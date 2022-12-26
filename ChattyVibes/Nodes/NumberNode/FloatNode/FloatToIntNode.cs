using ST.Library.UI.NodeEditor;
using System;
using System.Drawing;
using System.Globalization;

namespace ChattyVibes.Nodes.NumberNode.FloatNode
{
    [STNode("/Number/Float", "LauraRozier", "", "", "Float to Int node")]
    internal class FloatToIntNode : ConvertNode
    {
        private float _val;

        protected override void OnCreate()
        {
            base.OnCreate();
            Title = "Float to Int";
            TitleColor = Color.FromArgb(200, Constants.C_COLOR_FLOAT);

            m_in = InputOptions.Add("0", typeof(float), true);
            m_out = OutputOptions.Add("0", typeof(int), false);

            m_in.DataTransfer += new STNodeOptionEventHandler(m_in_DataTransfer);
            m_out.TransferData(0);
        }

        protected override void m_in_DataTransfer(object sender, STNodeOptionEventArgs e)
        {
            if (e.Status == ConnectionStatus.Connected && e.TargetOption.Data != null)
                _val = (float)e.TargetOption.Data;
            else
                _val = 0.0f;

            int result = (int)Math.Round(_val);
            SetOptionText(m_in, _val.ToString("G", CultureInfo.InvariantCulture));
            SetOptionText(m_out, result.ToString());
            m_out.TransferData(result);
        }
    }
}
