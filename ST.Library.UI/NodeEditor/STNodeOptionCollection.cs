﻿using System;

using System.Collections;

namespace ST.Library.UI.NodeEditor
{
    public class STNodeOptionCollection : IList, ICollection, IEnumerable
    {
        /*
         * Although the collection provides a complete data interface such as: Add, Remove,...
         * But try not to use some removal operations such as: Remove, RemoveAt, Clear, this[index] = value,...
         * Because in my definition, the Owner of each Option is strictly bound, and some operations such as removal
         * or replacement will affect the change of the Owner
         * So all original connections will be disconnected and the Disconnect event will be triggered
         * In order to ensure security in STNode, only inheritors can access the collection
         */
        private int _Count;
        public int Count { get { return _Count; } }
        private STNodeOption[] m_options;
        private STNode m_owner;

        private bool m_isInput; //Whether the current collection is storing the input point

        internal STNodeOptionCollection(STNode owner, bool isInput) {
            m_owner = owner ?? throw new ArgumentNullException("owner cannot be empty");
            m_isInput = isInput;
            m_options = new STNodeOption[4];
        }

        public STNodeOption Add(string strText, Type dataType, bool bSingle) {
            //not do this code -> out of bounds
            //return m_options[Add(new STNodeOption(strText, dataType, bSingle))];
            int nIndex = Add(new STNodeOption(strText, dataType, bSingle));
            return m_options[nIndex];
        }

        public int Add(STNodeOption option) {
            if (option == null)
                throw new ArgumentNullException("Add object cannot be empty");

            EnsureSpace(1);
            int nIndex = option == STNodeOption.Empty ? -1 : IndexOf(option);

            if (-1 == nIndex) {
                nIndex = _Count;
                option.Owner = m_owner;
                option.IsInput = m_isInput;
                m_options[_Count++] = option;
                Invalidate();
            }

            return nIndex;
        }

        public void AddRange(STNodeOption[] options) {
            if (options == null)
                throw new ArgumentNullException("Add object cannot be empty");

            EnsureSpace(options.Length);

            foreach (var op in options) {
                if (op == null)
                    throw new ArgumentNullException("Add object cannot be empty");

                if (-1 == IndexOf(op)) {
                    op.Owner = m_owner;
                    op.IsInput = m_isInput;
                    m_options[_Count++] = op;
                }
            }

            Invalidate();
        }

        public void Clear() {
            for (int i = 0; i < _Count; i++)
                m_options[i].Owner = null;

            _Count = 0;
            m_options = new STNodeOption[4];
            Invalidate();
        }

        public bool Contains(STNodeOption option) {
            return IndexOf(option) != -1;
        }

        public int IndexOf(STNodeOption option) {
            return Array.IndexOf<STNodeOption>(m_options, option);
        }

        public void Insert(int index, STNodeOption option) {
            if (index < 0 || index >= _Count)
                throw new IndexOutOfRangeException("index out of bounds");

            if (option == null)
                throw new ArgumentNullException("Insert object cannot be empty");

            EnsureSpace(1);

            for (int i = _Count; i > index; i--)
                m_options[i] = m_options[i - 1];

            option.Owner = m_owner;
            m_options[index] = option;
            _Count++;
            Invalidate();
        }

        public bool IsFixedSize {
            get { return false; }
        }

        public bool IsReadOnly {
            get { return false; }
        }

        public void Remove(STNodeOption option) {
            int nIndex = IndexOf(option);

            if (nIndex != -1)
                RemoveAt(nIndex);
        }

        public void RemoveAt(int index) {
            if (index < 0 || index >= _Count)
                throw new IndexOutOfRangeException("index out of bounds");

            _Count--;
            m_options[index].Owner = null;

            for (int i = index, Len = _Count; i < Len; i++)
                m_options[i] = m_options[i + 1];

            Invalidate();
        }

        public STNodeOption this[int index] {
            get {
                if (index < 0 || index >= _Count)
                    throw new IndexOutOfRangeException("index out of bounds");

                return m_options[index];
            }
            set { throw new InvalidOperationException("Do not reassign elements"); }
        }

        public void CopyTo(Array array, int index) {
            if (array == null)
                throw new ArgumentNullException("array cannot be empty");

            m_options.CopyTo(array, index);
        }

        public bool IsSynchronized {
            get { return true; }
        }

        public object SyncRoot {
            get { return this; }
        }

        public IEnumerator GetEnumerator() {
            for (int i = 0, Len = _Count; i < Len; i++)
                yield return m_options[i];
        }

        /// <summary>
        /// Confirm whether the space is sufficient, if the space is insufficient, expand the capacity
        /// </summary>
        /// <param name="elements">Need to increase the number</param>
        private void EnsureSpace(int elements) {
            if (elements + _Count > m_options.Length) {
                STNodeOption[] arrTemp = new STNodeOption[Math.Max(m_options.Length * 2, elements + _Count)];
                m_options.CopyTo(arrTemp, 0);
                m_options = arrTemp;
            }
        }

        protected void Invalidate() {
            if (m_owner != null && m_owner.Owner != null) {
                m_owner.BuildSize(true, true, true);
                //m_owner.Invalidate();//.Owner.Invalidate();
            }
        }

        //===================================================================================
        int IList.Add(object value) {
            return Add((STNodeOption)value);
        }

        void IList.Clear() {
            Clear();
        }

        bool IList.Contains(object value) {
            return Contains((STNodeOption)value);
        }

        int IList.IndexOf(object value) {
            return IndexOf((STNodeOption)value);
        }

        void IList.Insert(int index, object value) {
            Insert(index, (STNodeOption)value);
        }

        bool IList.IsFixedSize {
            get { return IsFixedSize; }
        }

        bool IList.IsReadOnly {
            get { return IsReadOnly; }
        }

        void IList.Remove(object value) {
            Remove((STNodeOption)value);
        }

        void IList.RemoveAt(int index) {
            RemoveAt(index);
        }

        object IList.this[int index] {
            get { return this[index]; }
            set { this[index] = (STNodeOption)value; }
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

        public STNodeOption[] ToArray() {
            STNodeOption[] ops = new STNodeOption[_Count];

            for (int i = 0; i < ops.Length; i++)
                ops[i] = m_options[i];

            return ops;
        }
    }
}
