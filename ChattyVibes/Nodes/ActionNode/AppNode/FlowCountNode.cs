using ST.Library.UI.NodeEditor;

namespace ChattyVibes.Nodes.ActionNode.AppNode
{
    [STNode("/Actions/App", "LauraRozier", "", "", "Flow count node")]
    internal sealed class FlowCountNode : ActionNode
    {
        private int _count = 0;

        private STNodeOption m_op_count_out;

        protected override void OnFlowTrigger()
        {
            _count++;
            SetOptionText(m_op_count_out, _count.ToString());
            m_op_count_out.TransferData(_count);
        }

        protected override void OnCreate()
        {
            _direction = FlowDirection.Both;
            base.OnCreate();
            Title = "Flow Counter";

            m_op_count_out = OutputOptions.Add(string.Empty, typeof(int), false);

            SetOptionText(m_op_count_out, _count.ToString());
        }
    }
}
