using System;
using System.ComponentModel;
using System.Drawing;

namespace ST.Library.UI.NodeEditor
{
    public enum ConnectionStatus
    {
        /// <summary>
        /// no owner
        /// </summary>
        [Description("no owner")]
        NoOwner,
        /// <summary>
        /// same owner
        /// </summary>
        [Description("same owner")]
        SameOwner,
        /// <summary>
        /// Both are input or output options
        /// </summary>
        [Description("Both are input or output options")]
        SameInputOrOutput,
        /// <summary>
        /// different data types
        /// </summary>
        [Description("different data types")]
        ErrorType,
        /// <summary>
        /// single link node
        /// </summary>
        [Description("single link node")]
        SingleOption,
        /// <summary>
        /// circular path
        /// </summary>
        [Description("circular path")]
        Loop,
        /// <summary>
        /// existing connection
        /// </summary>
        [Description("existing connection")]
        Exists,
        /// <summary>
        /// blank option
        /// </summary>
        [Description("blank option")]
        EmptyOption,
        /// <summary>
        /// already connected
        /// </summary>
        [Description("already connected")]
        Connected,
        /// <summary>
        /// disconnected
        /// </summary>
        [Description("disconnected")]
        DisConnected,
        /// <summary>
        /// node is locked
        /// </summary>
        [Description("node is locked")]
        Locked,
        /// <summary>
        /// operation denied
        /// </summary>
        [Description("operation denied")]
        Reject,
        /// <summary>
        /// being connected
        /// </summary>
        [Description("being connected")]
        Connecting,
        /// <summary>
        /// disconnecting
        /// </summary>
        [Description("disconnecting")]
        DisConnecting,
        /// <summary>
        /// Invalid data type
        /// </summary>
        [Description("Invalid data type")]
        InvalidType
    }

    public enum AlertLocation
    {
        Left,
        Top,
        Right,
        Bottom,
        Center,
        LeftTop,
        RightTop,
        RightBottom,
        LeftBottom,
    }

    public struct DrawingTools
    {
        public Graphics Graphics;
        public Pen Pen;
        public SolidBrush SolidBrush;
    }

    public enum CanvasMoveArgs //Parameters required when moving the canvas View->MoveCanvas()
    {
        Left = 1, //means move only the X coordinate
        Top = 2,  //means move only the Y coordinate
        All = 4   //Indicates that X Y moves simultaneously
    }

    public struct NodeFindInfo
    {
        public STNode Node;
        public STNodeOption NodeOption;
        public string Mark;
        public string[] MarkLines;
    }

    public struct ConnectionInfo
    {
        public STNodeOption Input;
        public STNodeOption Output;
    }

    public delegate void STNodeOptionEventHandler(object sender, STNodeOptionEventArgs e);

    public class STNodeOptionEventArgs : EventArgs
    {
        private STNodeOption _TargetOption;
        /// <summary>
        /// The corresponding Option that triggered this event
        /// </summary>
        public STNodeOption TargetOption {
            get { return _TargetOption; }
        }

        private ConnectionStatus _Status;
        /// <summary>
        /// Connection status between options
        /// </summary>
        public ConnectionStatus Status {
            get { return _Status; }
            internal set { _Status = value; }
        }

        private bool _IsSponsor;
        /// <summary>
        /// Whether it is the initiator of this behavior
        /// </summary>
        public bool IsSponsor {
            get { return _IsSponsor; }
        }

        public STNodeOptionEventArgs(bool isSponsor, STNodeOption opTarget, ConnectionStatus cr) {
            _IsSponsor = isSponsor;
            _TargetOption = opTarget;
            _Status = cr;
        }
    }

    public delegate void STNodeEditorEventHandler(object sender, STNodeEditorEventArgs e);
    public delegate void STNodeEditorOptionEventHandler(object sender, STNodeEditorOptionEventArgs e);


    public class STNodeEditorEventArgs : EventArgs
    {
        private STNode _Node;

        public STNode Node {
            get { return _Node; }
        }

        public STNodeEditorEventArgs(STNode node) {
            _Node = node;
        }
    }

    public class STNodeEditorOptionEventArgs : STNodeOptionEventArgs
    {
        private STNodeOption _CurrentOption;
        /// <summary>
        /// Option to actively trigger events
        /// </summary>
        public STNodeOption CurrentOption {
            get { return _CurrentOption; }
        }

        private bool _Continue = true;
        /// <summary>
        /// Whether to continue to operate downwards Used for Begin (Connecting/DisConnecting) whether to continue to operate backwards
        /// </summary>
        public bool Continue {
            get { return _Continue; }
            set { _Continue = value; }
        }

        public STNodeEditorOptionEventArgs(STNodeOption opTarget, STNodeOption opCurrent, ConnectionStatus cr) : base(false, opTarget, cr) {
            _CurrentOption = opCurrent;
        }
    }
}
