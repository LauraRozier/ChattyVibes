using ST.Library.UI.NodeEditor;
using System;

namespace ChattyVibes.Nodes.ArrayNode
{
    internal abstract class ArrayNode<T> : STNode
    {
        public int Length
        {
            get { return _data.Length; }
            set
            {
                Array.Resize(ref _data, value);
                m_op_array_out.TransferData(_data);
            }
        }
        protected T[] _data = null;

        protected STNodeOption m_op_array_out;

        protected override void OnCreate()
        {
            base.OnCreate();

            m_op_array_out = InputOptions.Add("OUT", typeof(T[]), false);

            m_op_array_out.TransferData(_data);
        }
    }
}
