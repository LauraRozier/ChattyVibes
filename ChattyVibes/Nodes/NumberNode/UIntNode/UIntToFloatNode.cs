using ST.Library.UI.NodeEditor;
using System.Drawing;
using System.Globalization;

namespace ChattyVibes.Nodes.NumberNode.UIntNode
{
    [STNode("/Number/UInt", "LauraRozier", "", "", "UInt to Float node")]
    internal class UIntToFloatNode : ConvertNode
    {
        private uint _val;

        protected override void OnCreate()
        {
            base.OnCreate();
            Title = "UInt to Float";
            TitleColor = Color.FromArgb(200, Constants.C_COLOR_UINT);

            m_in = InputOptions.Add("0", typeof(uint), true);
            m_out = OutputOptions.Add("0", typeof(float), false);

            m_in.DataTransfer += new STNodeOptionEventHandler(m_in_DataTransfer);
            m_out.TransferData(0.0f);
        }

        protected override void m_in_DataTransfer(object sender, STNodeOptionEventArgs e)
        {
            if (e.Status == ConnectionStatus.Connected && e.TargetOption.Data != null)
                _val = (uint)e.TargetOption.Data;
            else
                _val = 0u;

            float result = (float)_val;
            SetOptionText(m_in, _val.ToString());
            SetOptionText(m_out, result.ToString("G", CultureInfo.InvariantCulture));
            m_out.TransferData(result);
        }
    }
}
