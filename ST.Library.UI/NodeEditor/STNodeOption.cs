using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;

namespace ST.Library.UI.NodeEditor
{
    public class STNodeOption
    {
        #region Properties

        public static readonly STNodeOption Empty = new STNodeOption();

        private STNode _Owner;
        /// <summary>
        /// Get the Node to which the current Option belongs
        /// </summary>
        public STNode Owner {
            get { return _Owner; }
            internal set {
                if (value == _Owner)
                    return;

                if (_Owner != null)
                    DisConnectionAll(); //Disconnect all current connections when owner changes

                _Owner = value;
            }
        }

        private bool _IsSingle;
        /// <summary>
        /// Get whether the current Option can only be connected once
        /// </summary>
        public bool IsSingle {
            get { return _IsSingle; }
        }

        private bool _IsInput;
        /// <summary>
        /// Get whether the current Option is an input option
        /// </summary>
        public bool IsInput {
            get { return _IsInput; }
            internal set { _IsInput = value; }
        }

        private Color _TextColor = Color.White;
        /// <summary>
        /// Get or set the current Option text color
        /// </summary>
        public Color TextColor {
            get { return _TextColor; }
            internal set {
                if (value == _TextColor)
                    return;

                _TextColor = value;
                Invalidate();
            }
        }

        private Color _DotColor = Color.Transparent;
        /// <summary>
        /// Gets or sets the color of the current Option connection point
        /// </summary>
        public Color DotColor {
            get { return _DotColor; }
            internal set {
                if (value == _DotColor)
                    return;

                _DotColor = value;
                Invalidate();
            }
        }

        private string _Text;
        /// <summary>
        /// Get or set the current Option display text
        /// This property cannot be modified when AutoSize is set
        /// </summary>
        public string Text {
            get { return _Text; }
            internal set {
                if (value == _Text)
                    return;

                _Text = value;

                if (_Owner == null)
                    return;

                _Owner.BuildSize(true, true, true);
            }
        }

        private int _DotLeft;
        /// <summary>
        /// Get the left coordinate of the current Option connection point
        /// </summary>
        public int DotLeft {
            get { return _DotLeft; }
            internal set { _DotLeft = value; }
        }

        private int _DotTop;
        /// <summary>
        /// Get the upper coordinates of the current Option connection point
        /// </summary>
        public int DotTop {
            get { return _DotTop; }
            internal set { _DotTop = value; }
        }

        private int _DotSize;
        /// <summary>
        /// Get the width of the current Option join point
        /// </summary>
        public int DotSize {
            get { return _DotSize; }
            protected set { _DotSize = value; }
        }

        private Rectangle _TextRectangle;
        /// <summary>
        /// Get the current Option text area
        /// </summary>
        public Rectangle TextRectangle {
            get { return _TextRectangle; }
            internal set { _TextRectangle = value; }
        }

        private object _Data;
        /// <summary>
        /// Get or set the data contained in the current Option
        /// </summary>
        public object Data {
            get { return _Data; }
            set {
                if (value != null) {
                    if (_DataType == null)
                        return;

                    var t = value.GetType();

                    if (t != _DataType && !t.IsSubclassOf(_DataType))
                        throw new ArgumentException("Invalid data type The data type must be the specified data type or a subclass thereof");
                }

                _Data = value;
            }
        }

        private Type _DataType;
        /// <summary>
        /// Get the current Option data type
        /// </summary>
        public Type DataType {
            get { return _DataType; }
            set { _DataType = value; }
        }

        //private Rectangle _DotRectangle;
        /// <summary>
        /// Get the region of the current Option join point
        /// </summary>
        public Rectangle DotRectangle {
            get { return new Rectangle(_DotLeft, _DotTop, _DotSize, _DotSize); }
        }

        /// <summary>
        /// Get the number of current Option connected
        /// </summary>
        public int ConnectionCount {
            get { return m_hs_connected.Count; }
        }

        /// <summary>
        /// Get the Option collection connected to the current Option
        /// </summary>
        public HashSet<STNodeOption> ConnectedOption {
            get { return m_hs_connected; }
        }

        #endregion Properties
        /// <summary>
        /// save connected points
        /// </summary>
        protected HashSet<STNodeOption> m_hs_connected;

        #region Constructor

        private STNodeOption() { }

        /// <summary>
        /// Constructs an Option
        /// </summary>
        /// <param name="strText">display text</param>
        /// <param name="dataType">type of data</param>
        /// <param name="bSingle">Is it a single connection</param>
        public STNodeOption(string strText, Type dataType, bool bSingle) {
            _DataType = dataType ?? throw new ArgumentNullException("The specified data type cannot be empty");
            _DotSize = 10;
            m_hs_connected = new HashSet<STNodeOption>();
            _Text = strText;
            _IsSingle = bSingle;
        }

        #endregion Constructor

        #region Event

        /// <summary>
        /// Occurs when connected
        /// </summary>
        public event STNodeOptionEventHandler Connected;
        /// <summary>
        /// Occurs when a connection starts to occur
        /// </summary>
        public event STNodeOptionConnectionEventHandler Connecting;
        /// <summary>
        /// Occurs when the connection is disconnected
        /// </summary>
        public event STNodeOptionEventHandler DisConnected;
        /// <summary>
        /// Occurs when the connection starts to drop
        /// </summary>
        public event STNodeOptionConnectionEventHandler DisConnecting;
        /// <summary>
        /// Occurs when data is passed
        /// </summary>
        public event STNodeOptionEventHandler DataTransfer;

        #endregion Event

        #region protected
        /// <summary>
        /// redraw the entire control
        /// </summary>
        protected void Invalidate() {
            if (_Owner == null)
                return;

            _Owner.Invalidate();
        }

        /*
         * At first I thought that only input type options should have events because inputs are passive and outputs are active
         * But later found that, for example, the output node in STNodeHub uses events
         * Just in case, the code here is commented, and it is not very problematic.
         * The output option does not register the event, and the effect is the same.
         */
        protected internal virtual void OnConnected(STNodeOptionEventArgs e) =>
            Connected?.Invoke(this, e);

        protected internal virtual bool OnConnecting(STNodeOptionEventArgs e) =>
            Connecting?.Invoke(this, e) ?? true;

        protected internal virtual void OnDisConnected(STNodeOptionEventArgs e) =>
            DisConnected?.Invoke(this, e);

        protected internal virtual bool OnDisConnecting(STNodeOptionEventArgs e) =>
            DisConnecting?.Invoke(this, e) ?? true;

        protected internal virtual void OnDataTransfer(STNodeOptionEventArgs e) =>
            DataTransfer?.Invoke(this, e);

        protected void STNodeEditorConnected(STNodeEditorOptionEventArgs e) {
            if (_Owner == null)
                return;

            if (_Owner.Owner == null)
                return;

            _Owner.Owner.OnOptionConnected(e);
        }

        protected void STNodeEidtorDisConnected(STNodeEditorOptionEventArgs e) {
            if (_Owner == null)
                return;

            if (_Owner.Owner == null)
                return;

            _Owner.Owner.OnOptionDisConnected(e);
        }

        /// <summary>
        /// The current Option starts connecting to the target Option
        /// </summary>
        /// <param name="op">Option to be connected</param>
        /// <returns>Is it allowed to continue</returns>
        protected virtual bool ConnectingOption(STNodeOption op) {
            if (_Owner == null)
                return false;

            if (_Owner.Owner == null)
                return false;

            STNodeEditorOptionEventArgs e = new STNodeEditorOptionEventArgs(op, this, ConnectionStatus.Connecting);
            _Owner.Owner.OnOptionConnecting(e);
            return e.Continue
                && OnConnecting(new STNodeOptionEventArgs(true, op, ConnectionStatus.Connecting))
                && op.OnConnecting(new STNodeOptionEventArgs(false, this, ConnectionStatus.Connecting));
        }

        /// <summary>
        /// The current Option starts disconnecting the target Option
        /// </summary>
        /// <param name="op">Option to be disconnected</param>
        /// <returns>Is it allowed to continue</returns>
        protected virtual bool DisConnectingOption(STNodeOption op) {
            if (_Owner == null)
                return false;

            if (_Owner.Owner == null)
                return false;

            STNodeEditorOptionEventArgs e = new STNodeEditorOptionEventArgs(op, this, ConnectionStatus.DisConnecting);
            _Owner.Owner.OnOptionDisConnecting(e);
            return e.Continue
                && OnDisConnecting(new STNodeOptionEventArgs(true, op, ConnectionStatus.DisConnecting))
                && op.OnDisConnecting(new STNodeOptionEventArgs(false, this, ConnectionStatus.DisConnecting));
        }

        #endregion protected

        #region public
        /// <summary>
        /// The current Option to connect to the target Option
        /// </summary>
        /// <param name="op">Option to be connected</param>
        /// <returns>connection result</returns>
        public virtual ConnectionStatus ConnectOption(STNodeOption op) {
            if (!ConnectingOption(op)) {
                STNodeEditorConnected(new STNodeEditorOptionEventArgs(op, this, ConnectionStatus.Reject));
                return ConnectionStatus.Reject;
            }

            var v = CanConnect(op);

            if (v != ConnectionStatus.Connected) {
                STNodeEditorConnected(new STNodeEditorOptionEventArgs(op, this, v));
                return v;
            }

            v = op.CanConnect(this);

            if (v != ConnectionStatus.Connected) {
                STNodeEditorConnected(new STNodeEditorOptionEventArgs(op, this, v));
                return v;
            }

            op.AddConnection(this, false);
            AddConnection(op, true);
            ControlBuildLinePath();

            STNodeEditorConnected(new STNodeEditorOptionEventArgs(op, this, v));
            return v;
        }

        /// <summary>
        /// Detect whether the current Option can connect to the target Option
        /// </summary>
        /// <param name="op">Option to be connected</param>
        /// <returns>Test results</returns>
        public virtual ConnectionStatus CanConnect(STNodeOption op) {
            if (this == STNodeOption.Empty || op == STNodeOption.Empty)
                return ConnectionStatus.EmptyOption;

            if (_IsInput == op.IsInput)
                return ConnectionStatus.SameInputOrOutput;

            if (op.Owner == null || _Owner == null)
                return ConnectionStatus.NoOwner;

            if (op.Owner == _Owner)
                return ConnectionStatus.SameOwner;

            if (_Owner.LockOption || op._Owner.LockOption)
                return ConnectionStatus.Locked;

            if (_IsSingle && m_hs_connected.Count == 1)
                return ConnectionStatus.SingleOption;

            if (op.IsInput && STNodeEditor.CanFindNodePath(op.Owner, _Owner))
                return ConnectionStatus.Loop;

            if (m_hs_connected.Contains(op))
                return ConnectionStatus.Exists;

            if (_IsInput && op._DataType != _DataType && !op._DataType.IsSubclassOf(_DataType))
                return ConnectionStatus.ErrorType;

            return ConnectionStatus.Connected;
        }

        /// <summary>
        /// The current Option disconnects the target Option
        /// </summary>
        /// <param name="op">Option to be disconnected</param>
        /// <returns></returns>
        public virtual ConnectionStatus DisConnectOption(STNodeOption op) {
            if (!DisConnectingOption(op)) {
                STNodeEidtorDisConnected(new STNodeEditorOptionEventArgs(op, this, ConnectionStatus.Reject));
                return ConnectionStatus.Reject;
            }

            if (op.Owner == null)
                return ConnectionStatus.NoOwner;

            if (_Owner == null)
                return ConnectionStatus.NoOwner;

            if (op.Owner.LockOption && _Owner.LockOption) {
                STNodeEidtorDisConnected(new STNodeEditorOptionEventArgs(op, this, ConnectionStatus.Locked));
                return ConnectionStatus.Locked;
            }

            op.RemoveConnection(this, false);
            RemoveConnection(op, true);
            ControlBuildLinePath();

            STNodeEidtorDisConnected(new STNodeEditorOptionEventArgs(op, this, ConnectionStatus.DisConnected));
            return ConnectionStatus.DisConnected;
        }

        /// <summary>
        /// Disconnect all connections from the current Option
        /// </summary>
        public void DisConnectionAll() {
            if (_DataType == null)
                return;

            var arr = m_hs_connected.ToArray();

            foreach (var v in arr)
                DisConnectOption(v);
        }

        /// <summary>
        /// Get the Option collection connected to the current Option
        /// </summary>
        /// <returns>If it is null, it means that there is no owner, otherwise it returns a collection</returns>
        public List<STNodeOption> GetConnectedOption() {
            if (_DataType == null)
                return null;

            if (!_IsInput)
                return m_hs_connected.ToList();

            List<STNodeOption> lst = new List<STNodeOption>();

            if (_Owner == null)
                return null;

            if (_Owner.Owner == null)
                return null;

            foreach (var v in _Owner.Owner.GetConnectionInfo())
                if (v.Output == this)
                    lst.Add(v.Input);

            return lst;
        }

        /// <summary>
        /// Post data to all Option connected to the current Option
        /// </summary>
        public void TransferData() {
            if (_DataType == null)
                return;

            foreach (var v in m_hs_connected)
                v.OnDataTransfer(new STNodeOptionEventArgs(true, this, ConnectionStatus.Connected));
        }

        /// <summary>
        /// Post data to all Option connected to the current Option
        /// </summary>
        /// <param name="data">data to be delivered</param>
        public void TransferData(object data) {
            if (_DataType == null)
                return;

            Data = data; //not this._Data

            foreach (var v in m_hs_connected)
                v.OnDataTransfer(new STNodeOptionEventArgs(true, this, ConnectionStatus.Connected));
        }

        /// <summary>
        /// Post data to all Option connected to the current Option
        /// </summary>
        /// <param name="data">data to be delivered</param>
        /// <param name="bDisposeOld">Whether to release old data</param>
        public void TransferData(object data, bool bDisposeOld) {
            if (bDisposeOld && _Data != null) {
                if (_Data is IDisposable d)
                    d.Dispose();

                _Data = null;
            }

            TransferData(data);
        }

        #endregion public

        #region internal

        private bool AddConnection(STNodeOption op, bool bSponsor) {
            if (_DataType == null)
                return false;

            bool b = m_hs_connected.Add(op);
            OnConnected(new STNodeOptionEventArgs(bSponsor, op, ConnectionStatus.Connected));

            if (_IsInput)
                OnDataTransfer(new STNodeOptionEventArgs(bSponsor, op, ConnectionStatus.Connected));

            return b;
        }

        private bool RemoveConnection(STNodeOption op, bool bSponsor) {
            if (_DataType == null)
                return false;

            bool b = false;

            if (m_hs_connected.Contains(op)) {
                b = m_hs_connected.Remove(op);

                if (_IsInput)
                    OnDataTransfer(new STNodeOptionEventArgs(bSponsor, op, ConnectionStatus.DisConnected));

                OnDisConnected(new STNodeOptionEventArgs(bSponsor, op, ConnectionStatus.Connected));
            }

            return b;
        }

        #endregion internal

        #region private

        private void ControlBuildLinePath() {
            if (Owner == null)
                return;

            if (Owner.Owner == null)
                return;

            Owner.Owner.BuildLinePath();
        }

        #endregion
    }
}
