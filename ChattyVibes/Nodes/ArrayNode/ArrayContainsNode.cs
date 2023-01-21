using ST.Library.UI.NodeEditor;
using System;

namespace ChattyVibes.Nodes.ArrayNode
{
    internal abstract class ArrayContainsNode<T> : STNode
    {
        protected Type C_OBJ_TYPE = typeof(object);

        protected T[] _data = null;
        protected T _needle = default(T);
        protected STNodeOption m_op_haystack_in;
        protected STNodeOption m_op_needle_in;
        protected STNodeOption m_op_out;

        protected override void OnCreate()
        {
            base.OnCreate();

            m_op_haystack_in = InputOptions.Add("Haystack", typeof(T[]), true);
            m_op_needle_in = InputOptions.Add("Needle", typeof(T), true);
            m_op_out = OutputOptions.Add("False", typeof(bool), false);

            m_op_haystack_in.DataTransfer += new STNodeOptionEventHandler(m_op_DataTransfer);
            m_op_needle_in.DataTransfer += new STNodeOptionEventHandler(m_op_DataTransfer);
        }

        private void m_op_DataTransfer(object sender, STNodeOptionEventArgs e)
        {
            if (e.Status == ConnectionStatus.Connected && e.TargetOption.Data != null)
            {
                if (sender == m_op_haystack_in)
                    _data = (T[])e.TargetOption.Data;
                else
                    _needle = (T)e.TargetOption.Data;
            }
            else
            {
                if (sender == m_op_haystack_in)
                    _data = null;
                else
                    _needle = default(T);
            }

            if (_data != null)
            {
                foreach (var item in _data)
                {
                    if (item.Equals(_needle))
                    {
                        m_op_out.TransferData(true);
                        return;
                    }
                }
            }

            m_op_out.TransferData(false);
        }
    }
}
