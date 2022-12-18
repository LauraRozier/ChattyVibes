using System;
using System.Collections;

namespace ST.Library.UI.NodeEditor
{
    public class STNodeCollection : IList, ICollection, IEnumerable
    {
        private int _Count;
        public int Count { get { return _Count; } }
        private STNode[] m_nodes;
        private STNodeEditor m_owner;

        internal STNodeCollection(STNodeEditor owner) {
            if (owner == null)
                throw new ArgumentNullException("owner cannot be empty");

            m_owner = owner;
            m_nodes = new STNode[4];
        }

        public void MoveToEnd(STNode node) {
            if (_Count < 1)
                return;

            if (m_nodes[_Count - 1] == node)
                return;

            bool bFound = false;

            for (int i = 0; i < _Count - 1; i++) {
                if (m_nodes[i] == node)
                    bFound = true;

                if (bFound)
                    m_nodes[i] = m_nodes[i + 1];
            }

            m_nodes[_Count - 1] = node;
        }

        public int Add(STNode node) {
            if (node == null)
                throw new ArgumentNullException("Add object cannot be null");

            EnsureSpace(1);
            int nIndex = IndexOf(node);

            if (-1 == nIndex) {
                nIndex = _Count;
                node.Owner = m_owner;
                //node.BuildSize(true, true, false);
                m_nodes[_Count++] = node;
                m_owner.BuildBounds();
                m_owner.OnNodeAdded(new STNodeEditorEventArgs(node));
                m_owner.Invalidate();
                //m_owner.Invalidate(m_owner.CanvasToControl(new Rectangle(node.Left - 5, node.Top - 5, node.Width + 10, node.Height + 10)));
                //Console.WriteLine(node.Rectangle);
            }

            return nIndex;
        }

        public void AddRange(STNode[] nodes) {
            if (nodes == null)
                throw new ArgumentNullException("Add object cannot be null");

            EnsureSpace(nodes.Length);

            foreach (var n in nodes) {
                if (n == null)
                    throw new ArgumentNullException("Add object cannot be null");

                if (-1 == IndexOf(n)) {
                    n.Owner = m_owner;
                    m_nodes[_Count++] = n;
                }

                m_owner.OnNodeAdded(new STNodeEditorEventArgs(n));
            }

            m_owner.Invalidate();
            m_owner.BuildBounds();
        }

        public void Clear() {
            for (int i = 0; i < _Count; i++) {
                m_nodes[i].Owner = null;

                foreach (STNodeOption op in m_nodes[i].InputOptions)
                    op.DisConnectionAll();

                foreach (STNodeOption op in m_nodes[i].OutputOptions)
                    op.DisConnectionAll();

                m_owner.OnNodeRemoved(new STNodeEditorEventArgs(m_nodes[i]));
                m_owner.InternalRemoveSelectedNode(m_nodes[i]);
            }

            _Count = 0;
            m_nodes = new STNode[4];
            m_owner.SetActiveNode(null);
            m_owner.BuildBounds();
            m_owner.ScaleCanvas(1, 0, 0); //Coordinate system regression when no node exists
            m_owner.MoveCanvas(10, 10, true, CanvasMoveArgs.All);
            m_owner.Invalidate(); //If the canvas position and zoom are in the initial state, the above two lines of code will not cause the control to redraw
        }

        public bool Contains(STNode node) {
            return IndexOf(node) != -1;
        }

        public int IndexOf(STNode node) {
            return Array.IndexOf<STNode>(m_nodes, node);
        }

        public void Insert(int nIndex, STNode node) {
            if (nIndex < 0 || nIndex >= _Count)
                throw new IndexOutOfRangeException("index out of bounds");

            if (node == null)
                throw new ArgumentNullException("Insert object cannot be empty");

            EnsureSpace(1);

            for (int i = _Count; i > nIndex; i--)
                m_nodes[i] = m_nodes[i - 1];

            node.Owner = m_owner;
            m_nodes[nIndex] = node;
            _Count++;
            //node.BuildSize(true, true,false);
            m_owner.Invalidate();
            m_owner.BuildBounds();
        }

        public bool IsFixedSize {
            get { return false; }
        }

        public bool IsReadOnly {
            get { return false; }
        }

        public void Remove(STNode node) {
            int nIndex = IndexOf(node);

            if (nIndex != -1)
                RemoveAt(nIndex);
        }

        public void RemoveAt(int nIndex) {
            if (nIndex < 0 || nIndex >= _Count)
                throw new IndexOutOfRangeException("index out of bounds");

            m_nodes[nIndex].Owner = null;
            m_owner.InternalRemoveSelectedNode(m_nodes[nIndex]);

            if (m_owner.ActiveNode == m_nodes[nIndex])
                m_owner.SetActiveNode(null);

            m_owner.OnNodeRemoved(new STNodeEditorEventArgs(m_nodes[nIndex]));
            _Count--;

            for (int i = nIndex, Len = _Count; i < Len; i++)
                m_nodes[i] = m_nodes[i + 1];

            if (this._Count == 0) { //Coordinate system regression when no node exists
                m_owner.ScaleCanvas(1, 0, 0);
                m_owner.MoveCanvas(10, 10, true, CanvasMoveArgs.All);
            } else {
                m_owner.Invalidate();
                m_owner.BuildBounds();
            }
        }

        public STNode this[int nIndex] {
            get {
                if (nIndex < 0 || nIndex >= _Count)
                    throw new IndexOutOfRangeException("index out of bounds");

                return m_nodes[nIndex];
            }
            set { throw new InvalidOperationException("Do not reassign elements"); }
        }

        public void CopyTo(Array array, int index) {
            if (array == null)
                throw new ArgumentNullException("array cannot be empty");

            m_nodes.CopyTo(array, index);
        }

        public bool IsSynchronized {
            get { return true; }
        }

        public object SyncRoot {
            get { return this; }
        }

        public IEnumerator GetEnumerator() {
            for (int i = 0, Len = _Count; i < Len; i++)
                yield return m_nodes[i];
        }

        /// <summary>
        /// Confirm whether the space is sufficient, if the space is insufficient, expand the capacity
        /// </summary>
        /// <param name="elements">The number of elements/param>
        private void EnsureSpace(int elements) {
            if (elements + _Count > m_nodes.Length) {
                STNode[] arrTemp = new STNode[Math.Max(m_nodes.Length * 2, elements + _Count)];
                m_nodes.CopyTo(arrTemp, 0);
                m_nodes = arrTemp;
            }
        }

        //============================================================================
        int IList.Add(object value) {
            return Add((STNode)value);
        }

        void IList.Clear() {
            Clear();
        }

        bool IList.Contains(object value) {
            return Contains((STNode)value);
        }

        int IList.IndexOf(object value) {
            return IndexOf((STNode)value);
        }

        void IList.Insert(int index, object value) {
            Insert(index, (STNode)value);
        }

        bool IList.IsFixedSize {
            get { return IsFixedSize; }
        }

        bool IList.IsReadOnly {
            get { return IsReadOnly; }
        }

        void IList.Remove(object value) {
            Remove((STNode)value);
        }

        void IList.RemoveAt(int index) {
            RemoveAt(index);
        }

        object IList.this[int index] {
            get { return this[index]; }
            set { this[index] = (STNode)value; }
        }

        void ICollection.CopyTo(Array array, int index) {
            CopyTo(array, index);
        }

        int ICollection.Count {
            get { return _Count; }
        }

        bool ICollection.IsSynchronized {
            get { return IsSynchronized; }
        }

        object ICollection.SyncRoot {
            get { return SyncRoot; }
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }

        public STNode[] ToArray() {
            STNode[] nodes = new STNode[_Count];

            for (int i = 0; i < nodes.Length; i++)
                nodes[i] = m_nodes[i];

            return nodes;
        }
    }
}
