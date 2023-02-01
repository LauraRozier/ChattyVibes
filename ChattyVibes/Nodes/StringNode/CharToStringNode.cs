using ST.Library.UI.NodeEditor;
using System.Drawing;

namespace ChattyVibes.Nodes.StringNode
{
    [STNode("/String", "LauraRozier", "", "", "Char to String node")]
    internal sealed class CharToStringNode : StringNode
    {
        private STNodeOption m_op_in;
        private STNodeOption m_op_out;

        protected override void OnCreate()
        {
            base.OnCreate();
            TitleColor = Color.FromArgb(200, Constants.C_COLOR_CHAR);
            Title = "Char To String";

            m_op_in = InputOptions.Add("IN", typeof(char), false);
            m_op_out = OutputOptions.Add("OUT", typeof(string), false);

            m_op_in.DataTransfer += new STNodeOptionEventHandler(m_in_DataTransfer);
            m_op_out.TransferData(string.Empty);
        }

        private void m_in_DataTransfer(object sender, STNodeOptionEventArgs e)
        {
            if (e.Status == ConnectionStatus.Connected && e.TargetOption.Data != null)
                m_op_out.TransferData($"{(char)e.TargetOption.Data}");
            else
                m_op_out.TransferData(string.Empty);
        }
    }
}
