using ST.Library.UI.NodeEditor;
using System;
using System.Collections.Generic;

namespace ChattyVibes.Nodes.DictNode
{
    internal abstract class DictForEachNode<TKey, TVal> : STNode
    {
        protected Type C_OBJ_TYPE = typeof(object);

        protected Dictionary<TKey, TVal> _data = null;
        protected STNodeOption m_op_flow_in;
        protected STNodeOption m_op_array_in;
        protected STNodeOption m_op_flow_out_item;
        protected STNodeOption m_op_key_out_item;
        protected STNodeOption m_op_val_out_item;
        protected STNodeOption m_op_flow_out_done;

        protected override void OnCreate()
        {
            base.OnCreate();

            m_op_flow_in = InputOptions.Add(">", C_OBJ_TYPE, false);
            m_op_array_in = InputOptions.Add("IN", typeof(Dictionary<TKey, TVal>), false);
            m_op_flow_out_done = OutputOptions.Add("Done >", C_OBJ_TYPE, false);
            m_op_flow_out_item = OutputOptions.Add("Item >", C_OBJ_TYPE, false);
            m_op_key_out_item = OutputOptions.Add("Item Key", typeof(TKey), false);
            m_op_val_out_item = OutputOptions.Add("Item Value", typeof(TVal), false);

            m_op_flow_in.DataTransfer += new STNodeOptionEventHandler(m_op_DataTransfer);
            m_op_flow_in.DataTransfer += new STNodeOptionEventHandler(m_op_DataTransfer);
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
                _data = e.TargetOption.Data == null ? null : (Dictionary<TKey, TVal>)e.TargetOption.Data;
            }
        }

        private void OnFlowTrigger()
        {
            foreach (var item in _data)
            {
                m_op_key_out_item.TransferData(item.Key);
                m_op_val_out_item.TransferData(item.Value);
                m_op_flow_out_item.TransferData(new object());
                m_op_flow_out_item.TransferData(null);
            }

            m_op_flow_out_done.TransferData(new object());
            m_op_flow_out_done.TransferData(null); // We reset with null to avoid triggering on changes
        }
    }
}
