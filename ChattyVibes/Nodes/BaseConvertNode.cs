using ST.Library.UI.NodeEditor;

namespace ChattyVibes.Nodes
{
    internal abstract class BaseConvertNode : STNode
    {
        protected STNodeOption m_in;
        protected STNodeOption m_out;

        protected abstract void m_in_DataTransfer(object sender, STNodeOptionEventArgs e);
    }
}
