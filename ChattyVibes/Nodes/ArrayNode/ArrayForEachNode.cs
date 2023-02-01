using ST.Library.UI.NodeEditor;
using System;

namespace ChattyVibes.Nodes.ArrayNode
{
    internal abstract class ArrayForEachNode<T> : STNode
    {
        protected Type C_OBJ_TYPE = typeof(object);

        protected T[] _data = null;
        protected STNodeOption m_op_flow_in;
        protected STNodeOption m_op_array_in;
        protected STNodeOption m_op_flow_item_out;
        protected STNodeOption m_op_val_out_item;
        protected STNodeOption m_op_flow_done_out;

        protected override void OnCreate()
        {
            base.OnCreate();

            m_op_flow_in = InputOptions.Add(">", C_OBJ_TYPE, false);
            m_op_array_in = InputOptions.Add("IN", typeof(T[]), false);
            m_op_flow_done_out = OutputOptions.Add("Done >", C_OBJ_TYPE, false);
            m_op_flow_item_out = OutputOptions.Add("Item >", C_OBJ_TYPE, false);
            m_op_val_out_item = OutputOptions.Add("Item Value", typeof(T), false);

            m_op_flow_in.DataTransfer += new STNodeOptionEventHandler(m_op_DataTransfer);
            m_op_array_in.DataTransfer += new STNodeOptionEventHandler(m_op_DataTransfer);
        }

        private void m_op_DataTransfer(object sender, STNodeOptionEventArgs e)
        {
            if (sender == m_op_flow_in)
            {
                if (e.TargetOption.Data != null)
                    OnFlowTrigger();
            }
            else
            {
                _data = e.TargetOption.Data == null ? null : (T[])e.TargetOption.Data;
            }
        }

        private void OnFlowTrigger()
        {
            if (_data != null)
            {
                foreach (var item in _data)
                {
                    m_op_val_out_item.TransferData(item);
                    m_op_flow_item_out.TransferData(new object());
                    m_op_flow_item_out.TransferData(null);
                }
            }

            m_op_flow_done_out.TransferData(new object());
            m_op_flow_done_out.TransferData(null); // We reset with null to avoid triggering on changes
        }
    }
}
