using ST.Library.UI.NodeEditor;
using System.Drawing;

namespace ChattyVibes.Nodes.NumberNode.IntNode
{
    [STNode("/Number/Int", "LauraRozier", "", "", "Int to UInt node")]
    internal class IntToUIntNode : BaseConvertNode
    {
        private int _val;

        protected override void OnCreate()
        {
            base.OnCreate();
            Title = "Int to UInt";
            TitleColor = Color.FromArgb(200, Constants.C_COLOR_INT);

            m_in = InputOptions.Add("0", typeof(int), true);
            m_out = OutputOptions.Add("0", typeof(uint), false);

            m_in.DataTransfer += new STNodeOptionEventHandler(m_in_DataTransfer);
            m_out.TransferData(0u);
        }

        protected override void m_in_DataTransfer(object sender, STNodeOptionEventArgs e)
        {
            if (e.Status == ConnectionStatus.Connected && e.TargetOption.Data != null)
                _val = (int)e.TargetOption.Data;
            else
                _val = 0;

            uint result = (uint)_val;
            SetOptionText(m_in, _val.ToString());
            SetOptionText(m_out, result.ToString());
            m_out.TransferData(result);
        }
    }
}
