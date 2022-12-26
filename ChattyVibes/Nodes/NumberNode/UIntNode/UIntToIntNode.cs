using ST.Library.UI.NodeEditor;
using System.Drawing;

namespace ChattyVibes.Nodes.NumberNode.UIntNode
{
    [STNode("/Number/UInt", "LauraRozier", "", "", "UInt to Int node")]
    internal class UIntToIntNode : ConvertNode
    {
        private uint _val;

        protected override void OnCreate()
        {
            base.OnCreate();
            Title = "UInt to Int";
            TitleColor = Color.FromArgb(200, Constants.C_COLOR_UINT);

            m_in = InputOptions.Add("0", typeof(uint), true);
            m_out = OutputOptions.Add("0", typeof(int), false);

            m_in.DataTransfer += new STNodeOptionEventHandler(m_in_DataTransfer);
            m_out.TransferData(0);
        }

        protected override void m_in_DataTransfer(object sender, STNodeOptionEventArgs e)
        {
            if (e.Status == ConnectionStatus.Connected && e.TargetOption.Data != null)
                _val = (uint)e.TargetOption.Data;
            else
                _val = 0u;

            int result = (int)_val;
            SetOptionText(m_in, _val.ToString());
            SetOptionText(m_out, result.ToString());
            m_out.TransferData(result);
        }
    }
}
