using System;
using System.ComponentModel;
using System.Drawing;

namespace ST.Library.UI.NodeEditor
{
    public enum ConnectionStatus
    {
        /// <summary>
        /// No Owner
        /// </summary>
        [Description("No Owner")]
        NoOwner,
        /// <summary>
        /// Same Owner
        /// </summary>
        [Description("Same Owner")]
        SameOwner,
        /// <summary>
        /// Both are input or output options
        /// </summary>
        [Description("Both are input or output options")]
        SameInputOrOutput,
        /// <summary>
        /// Different data types
        /// </summary>
        [Description("Different data types")]
        ErrorType,
        /// <summary>
        /// Single link node
        /// </summary>
        [Description("Single link node")]
        SingleOption,
        /// <summary>
        /// Circular path
        /// </summary>
        [Description("Circular path")]
        Loop,
        /// <summary>
        /// Existing connection
        /// </summary>
        [Description("Existing connection")]
        Exists,
        /// <summary>
        /// Blank option
        /// </summary>
        [Description("Blank option")]
        EmptyOption,
        /// <summary>
        /// Connected
        /// </summary>
        [Description("Connected")]
        Connected,
        /// <summary>
        /// Disconnected
        /// </summary>
        [Description("Disconnected")]
        DisConnected,
        /// <summary>
        /// Node is locked
        /// </summary>
        [Description("Node is locked")]
        Locked,
        /// <summary>
        /// Operation denied
        /// </summary>
        [Description("Operation denied")]
        Reject,
        /// <summary>
        /// Being connected
        /// </summary>
        [Description("Being connected")]
        Connecting,
        /// <summary>
        /// Disconnecting
        /// </summary>
        [Description("Disconnecting")]
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

    public delegate bool STNodeOptionConnectionEventHandler(object sender, STNodeOptionEventArgs e);

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
