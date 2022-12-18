using ST.Library.UI.NodeEditor;

namespace ChattyVibes.Nodes
{
    internal abstract class FlowNode : STNode
    {
        protected enum FlowDirection
        {
            In,
            Out,
            Both
        }

        protected FlowDirection _direction;
        private STNodeOption m_op_flow_in;
        protected STNodeOption m_op_flow_out;

        protected virtual void OnFlowTrigger() { }

        protected override void OnCreate()
        {
            base.OnCreate();

            if (_direction == FlowDirection.In || _direction == FlowDirection.Both)
            {
                m_op_flow_in = InputOptions.Add(">", typeof(object), false);
                m_op_flow_in.DataTransfer += m_op_flow_DataTransfer;
            }

            if (_direction == FlowDirection.Out || _direction == FlowDirection.Both)
                m_op_flow_out = OutputOptions.Add(">", typeof(object), false);
        }

        private void m_op_flow_DataTransfer(object sender, STNodeOptionEventArgs e)
        {
            if (e.TargetOption.Data != null)
                OnFlowTrigger();

            if (_direction == FlowDirection.Out || _direction == FlowDirection.Both)
                m_op_flow_out.TransferData(e.TargetOption.Data);
        }

        protected void Trigger()
        {
            m_op_flow_out.TransferData(new object());
            m_op_flow_out.TransferData(null); // We reset with null to avoid triggering on changes
        }
    }
}
