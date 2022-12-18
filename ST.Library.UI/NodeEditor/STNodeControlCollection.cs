using System;
using System.Collections;

namespace ST.Library.UI.NodeEditor
{
    public class STNodeControlCollection: IList, ICollection, IEnumerable
    {
        /*
         * In order to ensure security in STNode, only inheritors can access the collection
         */
        private int _Count;
        public int Count { get { return _Count; } }
        private STNodeControl[] m_controls;
        private STNode m_owner;

        internal STNodeControlCollection(STNode owner) {
            m_owner = owner ?? throw new ArgumentNullException("owner cannot be null");
            m_controls = new STNodeControl[4];
        }

        public int Add(STNodeControl control) {
            if (control == null)
                throw new ArgumentNullException("Add object cannot be null");

            EnsureSpace(1);
            int nIndex = IndexOf(control);

            if (-1 == nIndex) {
                nIndex = _Count;
                control.Owner = m_owner;
                m_controls[_Count++] = control;
                Redraw();
            }

            return nIndex;
        }

        public void AddRange(STNodeControl[] controls) {
            if (controls == null)
                throw new ArgumentNullException("Add object cannot be null");

            EnsureSpace(controls.Length);

            foreach (var op in controls) {
                if (op == null)
                    throw new ArgumentNullException("Add object cannot be null");

                if (-1 == IndexOf(op)) {
                    op.Owner = m_owner;
                    m_controls[_Count++] = op;
                }
            }

            Redraw();
        }

        public void Clear() {
            for (int i = 0; i < _Count; i++)
                m_controls[i].Owner = null;

            _Count = 0;
            m_controls = new STNodeControl[4];
            Redraw();
        }

        public bool Contains(STNodeControl option) {
            return IndexOf(option) != -1;
        }

        public int IndexOf(STNodeControl option) {
            return Array.IndexOf<STNodeControl>(m_controls, option);
        }

        public void Insert(int index, STNodeControl control) {
            if (index < 0 || index >= _Count)
                throw new IndexOutOfRangeException("index out of bounds");

            if (control == null)
                throw new ArgumentNullException("Insert object cannot be null");

            EnsureSpace(1);

            for (int i = _Count; i > index; i--)
                m_controls[i] = m_controls[i - 1];

            control.Owner = m_owner;
            m_controls[index] = control;
            _Count++;
            Redraw();
        }

        public bool IsFixedSize {
            get { return false; }
        }

        public bool IsReadOnly {
            get { return false; }
        }

        public void Remove(STNodeControl control) {
            int nIndex = IndexOf(control);

            if (nIndex != -1)
                RemoveAt(nIndex);
        }

        public void RemoveAt(int index) {
            if (index < 0 || index >= _Count)
                throw new IndexOutOfRangeException("index out of bounds");

            _Count--;
            m_controls[index].Owner = null;

            for (int i = index, Len = _Count; i < Len; i++)
                m_controls[i] = m_controls[i + 1];

            Redraw();
        }

        public STNodeControl this[int index] {
            get {
                if (index < 0 || index >= _Count)
                    throw new IndexOutOfRangeException("index out of bounds");

                return m_controls[index];
            }
            set { throw new InvalidOperationException("Do not reassign elements"); }
        }

        public void CopyTo(Array array, int index) {
            if (array == null)
                throw new ArgumentNullException("array cannot be empty");

            m_controls.CopyTo(array, index);
        }

        public bool IsSynchronized {
            get { return true; }
        }

        public object SyncRoot {
            get { return this; }
        }

        public IEnumerator GetEnumerator() {
            for (int i = 0, Len = _Count; i < Len; i++)
                yield return m_controls[i];
        }

        /// <summary>
        /// Confirm whether the space is sufficient, if the space is insufficient, expand the capacity
        /// </summary>
        /// <param name="elements">Number of elements to increase</param>
        private void EnsureSpace(int elements) {
            if (elements + _Count > m_controls.Length) {
                STNodeControl[] arrTemp = new STNodeControl[Math.Max(m_controls.Length * 2, elements + _Count)];
                m_controls.CopyTo(arrTemp, 0);
                m_controls = arrTemp;
            }
        }

        protected void Redraw() {
            if (m_owner != null && m_owner.Owner != null) {
                //m_owner.BuildSize();
                m_owner.Owner.Invalidate(m_owner.Owner.CanvasToControl(m_owner.Rectangle));
            }
        }

        //===================================================================================
        int IList.Add(object value) {
            return Add((STNodeControl)value);
        }

        void IList.Clear() {
            Clear();
        }

        bool IList.Contains(object value) {
            return Contains((STNodeControl)value);
        }

        int IList.IndexOf(object value) {
            return IndexOf((STNodeControl)value);
        }

        void IList.Insert(int index, object value) {
            Insert(index, (STNodeControl)value);
        }

        bool IList.IsFixedSize {
            get { return IsFixedSize; }
        }

        bool IList.IsReadOnly {
            get { return IsReadOnly; }
        }

        void IList.Remove(object value) {
            Remove((STNodeControl)value);
        }

        void IList.RemoveAt(int index) {
            RemoveAt(index);
        }

        object IList.this[int index] {
            get { return this[index]; }
            set { this[index] = (STNodeControl)value; }
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
    }
}
