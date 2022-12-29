using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Threading;
using System.ComponentModel;
using System.Reflection;
using System.IO.Compression;
using System.Drawing.Text;
/*
MIT License

Copyright (c) 2021 DebugST@crystal_lz

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/
/*
 * create: 2020-12-08
 * modify: 2021-04-12
 * Author: Crystal_lz
 * blog: http://st233.com
 * Gitee: https://gitee.com/DebugST
 * Github: https://github.com/DebugST
 */
namespace ST.Library.UI.NodeEditor
{
    public class STNodeEditor : Control
    {
        private const UInt32 WM_MOUSEHWHEEL = 0x020E;
        protected static readonly Type m_type_node = typeof(STNode);

        #region protected enum,struct --------------------------------------------------------------------------------------

        protected enum CanvasAction     //Which of the following actions is indicated by the current mouse movement operation
        {
            None,                       //none
            MoveNode,                   //Move Node
            ConnectOption,              //Connect Option
            SelectRectangle,            //Selecting a rectangular area
            DrawMarkDetails             //Drawing marker info details
        }

        protected struct MagnetInfo
        {
            public bool XMatched;       //Is there a magnet matching on the X axis
            public bool YMatched;
            public int X;               //Match the number on the X axis
            public int Y;
            public int OffsetX;         //The relative offset between the current node X position and the matching X
            public int OffsetY;
        }

        #endregion

        #region Properties ------------------------------------------------------------------------------------------------------

        private float _CanvasOffsetX;
        /// <summary>
        /// Get the offset position of the canvas origin relative to the X direction of the control
        /// </summary>
        [Browsable(false)]
        public float CanvasOffsetX {
            get { return _CanvasOffsetX; }
        }

        private float _CanvasOffsetY;
        /// <summary>
        /// Get the offset position of the canvas origin relative to the Y direction of the control
        /// </summary>
        [Browsable(false)]
        public float CanvasOffsetY {
            get { return _CanvasOffsetY; }
        }

        private PointF _CanvasOffset;
        /// <summary>
        /// Get the offset position of the canvas origin relative to the control
        /// </summary>
        [Browsable(false)]
        public PointF CanvasOffset {
            get {
                _CanvasOffset.X = _CanvasOffsetX;
                _CanvasOffset.Y = _CanvasOffsetY;
                return _CanvasOffset;
            }
        }

        private Rectangle _CanvasValidBounds;
        /// <summary>
        /// Get the active area in the canvas that is used
        /// </summary>
        [Browsable(false)]
        public Rectangle CanvasValidBounds {
            get { return _CanvasValidBounds; }
        }

        private float _CanvasScale = 1;
        /// <summary>
        /// Get the zoom ratio of the canvas
        /// </summary>
        [Browsable(false)]
        public float CanvasScale {
            get { return _CanvasScale; }
        }

        private float _Curvature = 0.3F;
        /// <summary>
        /// Gets or sets the curvature of the line between Option
        /// </summary>
        [Browsable(false)]
        public float Curvature {
            get { return _Curvature; }
            set {
                if (value < 0)
                    value = 0;

                if (value > 1)
                    value = 1;

                _Curvature = value;

                if (m_dic_gp_info.Count != 0)
                    BuildLinePath();
            }
        }

        private bool _ShowMagnet = true;
        /// <summary>
        /// Get or set whether to enable the magnet effect when moving the Node in the canvas
        /// </summary>
        [Description("Get or set whether to enable the magnet effect when moving the Node in the canvas"), DefaultValue(true)]
        public bool ShowMagnet {
            get { return _ShowMagnet; }
            set { _ShowMagnet = value; }
        }

        private bool _ShowBorder = true;
        /// <summary>
        /// Get or set whether to display the Node border in the mobile canvas
        /// </summary>
        [Description("Get or set whether to display the Node border in the mobile canvas"), DefaultValue(true)]
        public bool ShowBorder {
            get { return _ShowBorder; }
            set {
                _ShowBorder = value;
                Invalidate();
            }
        }

        private bool _ShowGrid = true;
        /// <summary>
        /// Get or set whether to draw background grid lines in the canvas
        /// </summary>
        [Description("Get or set whether to draw background grid lines in the canvas"), DefaultValue(true)]
        public bool ShowGrid {
            get { return _ShowGrid; }
            set {
                _ShowGrid = value;
                Invalidate();
            }
        }

        private bool _ShowLocation = true;
        /// <summary>
        /// Get or set whether to display the Node position information beyond the viewing angle at the edge of the canvas
        /// </summary>
        [Description("Get or set whether to display the Node position information beyond the viewing angle at the edge of the canvas"), DefaultValue(true)]
        public bool ShowLocation {
            get { return _ShowLocation; }
            set {
                _ShowLocation = value;
                Invalidate();
            }
        }

        private readonly STNodeCollection _Nodes;
        /// <summary>
        /// Get the Node collection in the canvas
        /// </summary>
        [Browsable(false)]
        public STNodeCollection Nodes {
            get {
                return _Nodes;
            }
        }

        private STNode _ActiveNode;
        /// <summary>
        /// Get the active Node selected in the current canvas
        /// </summary>
        [Browsable(false)]
        public STNode ActiveNode {
            get { return _ActiveNode; }
            //set {
            //    if (value == _ActiveSelectedNode) return;
            //    if (_ActiveSelectedNode != null) _ActiveSelectedNode.OnLostFocus(EventArgs.Empty);
            //    _ActiveSelectedNode = value;
            //    _ActiveSelectedNode.IsActive = true;
            //    this.Invalidate();
            //    this.OnSelectedChanged(EventArgs.Empty);
            //}
        }

        private STNode _HoverNode;
        /// <summary>
        /// Get the Node hovered by the mouse in the current canvas
        /// </summary>
        [Browsable(false)]
        public STNode HoverNode {
            get { return _HoverNode; }
        }

        //========================================color================================
        private Color _GridColor = Color.Black;
        /// <summary>
        /// Get or set the grid line color when drawing the canvas background
        /// </summary>
        [Description("Get or set the grid line color when drawing the canvas background"), DefaultValue(typeof(Color), "Black")]
        public Color GridColor {
            get { return _GridColor; }
            set {
                _GridColor = value;
                Invalidate();
            }
        }

        private Color _BorderColor = Color.Black;
        /// <summary>
        /// Get or set the border color of Node in the canvas
        /// </summary>
        [Description("Get or set the border color of Node in the canvas"), DefaultValue(typeof(Color), "Black")]
        public Color BorderColor {
            get { return _BorderColor; }
            set {
                _BorderColor = value;
                m_img_border?.Dispose();
                m_img_border = CreateBorderImage(value);
                Invalidate();
            }
        }

        private Color _BorderHoverColor = Color.Gray;
        /// <summary>
        /// Get or set the border color of the hovering Node in the canvas
        /// </summary>
        [Description("Get or set the border color of the hovering Node in the canvas"), DefaultValue(typeof(Color), "Gray")]
        public Color BorderHoverColor {
            get { return _BorderHoverColor; }
            set {
                _BorderHoverColor = value;
                m_img_border_hover?.Dispose();
                m_img_border_hover = CreateBorderImage(value);
                Invalidate();
            }
        }

        private Color _BorderSelectedColor = Color.Orange;
        /// <summary>
        /// Get or set the border color of the selected Node in the canvas
        /// </summary>
        [Description("Get or set the border color of the selected Node in the canvas"), DefaultValue(typeof(Color), "Orange")]
        public Color BorderSelectedColor {
            get { return _BorderSelectedColor; }
            set {
                _BorderSelectedColor = value;
                m_img_border_selected?.Dispose();
                m_img_border_selected = CreateBorderImage(value);
                Invalidate();
            }
        }

        private Color _BorderActiveColor = Color.OrangeRed;
        /// <summary>
        /// Get or set the border color of the active Node in the canvas
        /// </summary>
        [Description("Get or set the border color of the active Node in the canvas"), DefaultValue(typeof(Color), "OrangeRed")]
        public Color BorderActiveColor {
            get { return _BorderActiveColor; }
            set {
                _BorderActiveColor = value;
                m_img_border_active?.Dispose();
                m_img_border_active = CreateBorderImage(value);
                Invalidate();
            }
        }

        private Color _MarkForeColor = Color.White;
        /// <summary>
        /// Gets or sets the foreground color used by the canvas to draw Node mark details
        /// </summary>
        [Description("Gets or sets the foreground color used by the canvas to draw Node mark details"), DefaultValue(typeof(Color), "White")]
        public Color MarkForeColor {
            get { return _MarkForeColor; }
            set {
                _MarkForeColor = value;
                Invalidate();
            }
        }

        private Color _MarkBackColor = Color.FromArgb(180, Color.Black);
        /// <summary>
        /// Gets or sets the background color used by the canvas to draw Node mark details
        /// </summary>
        [Description("Gets or sets the background color used by the canvas to draw Node mark details")]
        public Color MarkBackColor {
            get { return _MarkBackColor; }
            set {
                _MarkBackColor = value;
                Invalidate();
            }
        }

        private Color _MagnetColor = Color.Lime;
        /// <summary>
        /// Get or set the color of the magnet mark when moving the Node in the canvas
        /// </summary>
        [Description("Get or set the color of the magnet mark when moving the Node in the canvas"), DefaultValue(typeof(Color), "Lime")]
        public Color MagnetColor {
            get { return _MagnetColor; }
            set { _MagnetColor = value; }
        }

        private Color _SelectedRectangleColor = Color.DodgerBlue;
        /// <summary>
        /// Get or set the color of the selection rectangle in the canvas
        /// </summary>
        [Description("Get or set the color of the selection rectangle in the canvas"), DefaultValue(typeof(Color), "DodgerBlue")]
        public Color SelectedRectangleColor {
            get { return _SelectedRectangleColor; }
            set { _SelectedRectangleColor = value; }
        }

        private Color _HighLineColor = Color.Cyan;
        /// <summary>
        /// Get or set the color of the highlighted connection in the canvas
        /// </summary>
        [Description("Get or set the color of the highlighted connection in the canvas"), DefaultValue(typeof(Color), "Cyan")]
        public Color HighLineColor {
            get { return _HighLineColor; }
            set { _HighLineColor = value; }
        }

        private Color _LocationForeColor = Color.Red;
        /// <summary>
        /// Get or set the foreground color of the hint area at the edge of the canvas
        /// </summary>
        [Description("Get or set the foreground color of the hint area at the edge of the canvas"), DefaultValue(typeof(Color), "Red")]
        public Color LocationForeColor {
            get { return _LocationForeColor; }
            set {
                _LocationForeColor = value;
                Invalidate();
            }
        }

        private Color _LocationBackColor = Color.FromArgb(120, Color.Black);
        /// <summary>
        /// Get or set the background color of the hint area at the edge of the canvas
        /// </summary>
        [Description("Get or set the background color of the hint area at the edge of the canvas")]
        public Color LocationBackColor {
            get { return _LocationBackColor; }
            set {
                _LocationBackColor = value;
                Invalidate();
            }
        }

        private Color _UnknownTypeColor = Color.Gray;
        /// <summary>
        /// Get or set the color that should be used when the Option data type in Node cannot be determined in the canvas
        /// </summary>
        [Description("Get or set the color that should be used when the Option data type in Node cannot be determined in the canvas"), DefaultValue(typeof(Color), "Gray")]
        public Color UnknownTypeColor {
            get { return _UnknownTypeColor; }
            set {
                _UnknownTypeColor = value;
                Invalidate();
            }
        }

        private readonly Dictionary<Type, Color> _TypeColor = new Dictionary<Type, Color>();
        /// <summary>
        /// Get or set the preset color of the Option data type in Node in the canvas
        /// </summary>
        [Browsable(false)]
        public Dictionary<Type, Color> TypeColor {
            get { return _TypeColor; }
        }

        private bool _modified = false;
        [Browsable(false)]
        public bool Modified
        {
            get { return _modified; }
            set { _modified = value; }
        }

        #endregion

        #region protected properties ----------------------------------------------------------------------------------------
        /// <summary>
        /// The real-time position of the current mouse in the control
        /// </summary>
        protected Point m_pt_in_control;
        /// <summary>
        /// The real-time position of the current mouse in the canvas
        /// </summary>
        protected PointF m_pt_in_canvas;
        /// <summary>
        /// The position on the control when the mouse is clicked
        /// </summary>
        protected Point m_pt_down_in_control;
        /// <summary>
        /// The position in the canvas when the mouse is clicked
        /// </summary>
        protected PointF m_pt_down_in_canvas;
        /// <summary>
        /// When the mouse is clicked to move the canvas, the coordinate position of the canvas when the mouse is clicked
        /// </summary>
        protected PointF m_pt_canvas_old;
        /// <summary>
        /// It is used to save the starting point coordinates of the Option under the save point during the connection process
        /// </summary>
        protected Point m_pt_dot_down;
        /// <summary>
        /// It is used to save the starting point Option under the mouse during the connection process.
        /// When the Mouse is up, determine whether to connect this node
        /// </summary>
        protected STNodeOption m_option_down;
        /// <summary>
        /// STNode under the current mouse point
        /// </summary>
        protected STNode m_node_down;
        /// <summary>
        /// Whether the current mouse is in the control
        /// </summary>
        protected bool m_mouse_in_control;

        #endregion

        public STNodeEditor() {
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.ResizeRedraw, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            _Nodes = new STNodeCollection(this);
            BackColor = Color.FromArgb(255, 34, 34, 34);
            MinimumSize = new Size(100, 100);
            Size = new Size(200, 200);
            AllowDrop = true;

            m_real_canvas_x = _CanvasOffsetX = 10;
            m_real_canvas_y = _CanvasOffsetY = 10;
        }

        #region private fields --------------------------------------------------------------------------------------

        private DrawingTools m_drawing_tools;
        private NodeFindInfo m_find = new NodeFindInfo();
        private MagnetInfo m_mi = new MagnetInfo();

        private RectangleF m_rect_select = new RectangleF();
        //Node border default pattern
        private Image m_img_border;
        private Image m_img_border_hover;
        private Image m_img_border_selected;
        private Image m_img_border_active;
        //It is used for the animation effect when the mouse scrolls or the touchpad moves the canvas.
        //This value is the real coordinate address to be moved to.
        //View->MoveCanvasThread()
        private float m_real_canvas_x;
        private float m_real_canvas_y;
        //When used to move nodes, save the initial coordinates of the selected nodes when the mouse is clicked
        private readonly Dictionary<STNode, Point> m_dic_pt_selected = new Dictionary<STNode, Point>();
        //Used for the magnet effect when moving nodes, the coordinates of the non-selected nodes that need to participate
        //in the magnet effect are calculated View->BuildMagnetLocation()
        private readonly List<int> m_lst_magnet_x = new List<int>();
        private readonly List<int> m_lst_magnet_y = new List<int>();
        //It is used for the magnet effect when moving the node, and the active selection node counts the coordinates that
        //need to participate in the magnet effect View->CheckMagnet()
        private readonly List<int> m_lst_magnet_mx = new List<int>();
        private readonly List<int> m_lst_magnet_my = new List<int>();
        //It is used to calculate the time trigger interval in mouse scrolling.
        //According to different intervals, the displacement of the canvas is different View->OnMouseWheel(), OnMouseHWheel()
        private DateTime m_dt_vw = DateTime.Now;
        private DateTime m_dt_hw = DateTime.Now;
        //Current behavior during mouse movement
        private CanvasAction m_ca;
        //Save selected nodes
        private readonly HashSet<STNode> m_hs_node_selected = new HashSet<STNode>();

        private bool m_is_process_mouse_event = true; //Whether to pass down mouse-related events (Node or NodeControls) such as disconnection-related operations should not be passed down
        private bool m_is_buildpath; //It is used to determine whether to re-establish the path of the cache connection during the redrawing process
        private readonly Pen m_p_line = new Pen(Color.Cyan, 2f); //for drawing connected lines
        private readonly Pen m_p_line_hover = new Pen(Color.Cyan, 4f); //Used to draw lines when the mouse is hovering
        private GraphicsPath m_gp_hover; //The connection path currently hovered over by the mouse
        private StringFormat m_sf = new StringFormat(); //Text Format Used to set the text format when Mark is drawn
        //Save the node relationship corresponding to each connecting line
        private readonly Dictionary<GraphicsPath, ConnectionInfo> m_dic_gp_info = new Dictionary<GraphicsPath, ConnectionInfo>();
        //Saves the position of Nodes that are outside the visual area
        private readonly List<Point> m_lst_node_out = new List<Point>();
        //The Node type loaded by the current editor is used to load nodes from files or data
        private readonly Dictionary<string, Type> m_dic_type = new Dictionary<string, Type>();

        private int m_time_alert;
        private int m_alpha_alert;
        private string m_str_alert;
        private Color m_forecolor_alert;
        private Color m_backcolor_alert;
        private DateTime m_dt_alert;
        private Rectangle m_rect_alert;
        private AlertLocation m_al;

        private sealed class EditorPictureBox : PictureBox
        {
            public int TopOffset { get; set; }
            public int LeftOffset { get; set; }
            internal bool SkipProcessing { get; set; } = false;

            public EditorPictureBox() : base()
            {
                Height = 120;
                Width = 120;
                BackColor = Color.FromArgb(65, 80, 80, 80);
                SizeMode = PictureBoxSizeMode.Zoom;
            }

            protected override void OnCreateControl()
            {
                var typedParent = (STNodeEditor)Parent;
                typedParent.Resize += new EventHandler(Parent_Changed);
                Top = Parent.Top + (Margin.Top + TopOffset);
                Left = Parent.Width - (Width + Margin.Right + LeftOffset);
            }

            internal void ProcessNodeChanges()
            {
                if (SkipProcessing)
                    return;

                var typedParent = (STNodeEditor)Parent;
                Rectangle rect = new Rectangle(0, 0, 10, 10);
                
                var childNodes = typedParent.Nodes.ToArray();

                if (childNodes.Length <= 0 || (childNodes.Length == 1 && childNodes[0].Owner == null))
                {
                    Image = null;
                    return;
                }

                try
                {
                    rect.Width = childNodes.Max(n => n.Right) + 10;
                    rect.Height = childNodes.Max(n => n.Bottom) + 10;
                    Image = ((STNodeEditor)Parent).GetCanvasImage(rect);
                }
                catch { }

                GC.Collect();
            }

            private void Parent_Changed(object sender, EventArgs e)
            {
                Top = Parent.Top + (Margin.Top + TopOffset);
                Left = Parent.Width - (Width + Margin.Right + LeftOffset);
            }
        }

        private sealed class EditorButton : Button
        {
            public int TopOffset { get; set; }
            public int LeftOffset { get; set; }

            public EditorButton() : base()
            {
                Height = 22;
                Width = 22;
                Font = new Font("courier new", 8.25f, FontStyle.Bold);
                FlatStyle = FlatStyle.Flat;
                FlatAppearance.BorderSize = 0;
                ForeColor = Color.White;
                BackColor = Color.FromArgb(255, 15, 15, 15);
            }

            protected override void OnCreateControl()
            {
                Parent.Resize += new EventHandler(Parent_Resized);
                Top = Parent.Top + (Margin.Top + TopOffset);
                Left = Parent.Width - (Width + Margin.Right + LeftOffset);
            }

            private void Parent_Resized(object sender, EventArgs e)
            {
                Top = Parent.Top + (Margin.Top + TopOffset);
                Left = Parent.Width - (Width + Margin.Right + LeftOffset);
            }
        }

        private EditorPictureBox m_minimap = new EditorPictureBox
        {
            TopOffset = 5,
            LeftOffset = 5,
        };
        /* Zoom buttons */
        private EditorButton m_btn_zoom_plus;
        private EditorButton m_btn_zoom_min;
        /* Move buttons */
        private EditorButton m_btn_move_up;
        private EditorButton m_btn_move_down;
        private EditorButton m_btn_move_left;
        private EditorButton m_btn_move_right;

        #endregion

        #region event ----------------------------------------------------------------------------------------------------
        /// <summary>
        /// Occurs when the active node changes
        /// </summary>
        [Description("Occurs when the active node changes")]
        public event EventHandler ActiveChanged;
        /// <summary>
        /// Occurs when the selected node changes
        /// </summary>
        [Description("Occurs when the selected node changes")]
        public event EventHandler SelectedChanged;
        /// <summary>
        /// Occurs when the hovered node changes
        /// </summary>
        [Description("Occurs when the hovered node changes")]
        public event EventHandler HoverChanged;
        /// <summary>
        /// Occurs when a node is added
        /// </summary>
        [Description("Occurs when a node is added")]
        public event STNodeEditorEventHandler NodeAdded;
        /// <summary>
        /// Occurs when a node is removed
        /// </summary>
        [Description("Occurs when a node is removed")]
        public event STNodeEditorEventHandler NodeRemoved;
        /// <summary>
        /// Occurs when moving the canvas origin
        /// </summary>
        [Description("Occurs when moving the canvas origin")]
        public event EventHandler CanvasMoved;
        /// <summary>
        /// Occurs when the canvas is zoomed
        /// </summary>
        [Description("Occurs when the canvas is zoomed")]
        public event EventHandler CanvasScaled;
        /// <summary>
        /// Occurs when connecting node options
        /// </summary>
        [Description("Occurs when connecting node options")]
        public event STNodeEditorOptionEventHandler OptionConnected;
        /// <summary>
        /// Occurs while connecting node options
        /// </summary>
        [Description("Occurs while connecting node options")]
        public event STNodeEditorOptionEventHandler OptionConnecting;
        /// <summary>
        /// Occurs when a node option is disconnected
        /// </summary>
        [Description("Occurs when a node option is disconnected")]
        public event STNodeEditorOptionEventHandler OptionDisConnected;
        /// <summary>
        /// Occurs while disconnecting node options
        /// </summary>
        [Description("Occurs while disconnecting node options")]
        public event STNodeEditorOptionEventHandler OptionDisConnecting;

        protected virtual internal void OnSelectedChanged(EventArgs e)
        {
            _modified = true;
            SelectedChanged?.Invoke(this, e);
            m_minimap.ProcessNodeChanges();
        }

        protected virtual void OnActiveChanged(EventArgs e)
        {
            _modified = true;
            ActiveChanged?.Invoke(this, e);
            m_minimap.ProcessNodeChanges();
        }

        protected virtual void OnHoverChanged(EventArgs e) =>
            HoverChanged?.Invoke(this, e);

        protected internal virtual void OnNodeAdded(STNodeEditorEventArgs e) {
            _modified = true;
            NodeAdded?.Invoke(this, e);
            m_minimap.ProcessNodeChanges();
        }

        protected internal virtual void OnNodeRemoved(STNodeEditorEventArgs e)
        {
            _modified = true;
            NodeRemoved?.Invoke(this, e);
            m_minimap.ProcessNodeChanges();
        }

        protected virtual void OnCanvasMoved(EventArgs e)
        {
            _modified = true;
            CanvasMoved?.Invoke(this, e);
        }

        protected virtual void OnCanvasScaled(EventArgs e)
        {
            _modified = true;
            CanvasScaled?.Invoke(this, e);
        }

        protected internal virtual void OnOptionConnected(STNodeEditorOptionEventArgs e)
        {
            _modified = true;
            OptionConnected?.Invoke(this, e);
            m_minimap.ProcessNodeChanges();
        }

        protected internal virtual void OnOptionDisConnected(STNodeEditorOptionEventArgs e)
        {
            _modified = true;
            OptionDisConnected?.Invoke(this, e);
            m_minimap.ProcessNodeChanges();
        }

        protected internal virtual void OnOptionConnecting(STNodeEditorOptionEventArgs e) =>
            OptionConnecting?.Invoke(this, e);

        protected internal virtual void OnOptionDisConnecting(STNodeEditorOptionEventArgs e) =>
            OptionDisConnecting?.Invoke(this, e);

        #endregion event

        #region override -----------------------------------------------------------------------------------------------------

        protected override void OnCreateControl() {
            m_drawing_tools = new DrawingTools() {
                Pen = new Pen(Color.Black, 1),
                SolidBrush = new SolidBrush(Color.Black)
            };
            m_img_border = CreateBorderImage(_BorderColor);
            m_img_border_active = CreateBorderImage(_BorderActiveColor);
            m_img_border_hover = CreateBorderImage(_BorderHoverColor);
            m_img_border_selected = CreateBorderImage(_BorderSelectedColor);
            base.OnCreateControl();
            new Thread(MoveCanvasThread) { IsBackground = true }.Start();
            new Thread(ShowAlertThread) { IsBackground = true }.Start();
            m_sf = new StringFormat
            {
                Alignment = StringAlignment.Near,
                FormatFlags = StringFormatFlags.NoWrap
            };
            m_sf.SetTabStops(0, new float[] { 40 });

            Controls.Add(m_minimap);

            /* Zoom buttons */
            m_btn_zoom_plus = new EditorButton
            {
                TopOffset = 135,
                LeftOffset = 71,
                Text = "+",
            };
            m_btn_zoom_plus.Click += new EventHandler(Btn_Zoom_Click);
            Controls.Add(m_btn_zoom_plus);

            m_btn_zoom_min = new EditorButton
            {
                TopOffset = 135,
                LeftOffset = 27,
                Text = "-",
            };
            m_btn_zoom_min.Click += new EventHandler(Btn_Zoom_Click);
            Controls.Add(m_btn_zoom_min);

            /* Move buttons */
            m_btn_move_up = new EditorButton
            {
                TopOffset = 167,
                LeftOffset = 49,
                Text = "▲",
            };
            m_btn_move_up.Click += new EventHandler(Btn_Move_Click);
            Controls.Add(m_btn_move_up);

            m_btn_move_left = new EditorButton
            {
                TopOffset = 189,
                LeftOffset = 71,
                Text = "◄",
            };
            m_btn_move_left.Click += new EventHandler(Btn_Move_Click);
            Controls.Add(m_btn_move_left);

            m_btn_move_down = new EditorButton
            {
                TopOffset = 211,
                LeftOffset = 49,
                Text = "▼",
            };
            m_btn_move_down.Click += new EventHandler(Btn_Move_Click);
            Controls.Add(m_btn_move_down);

            m_btn_move_right = new EditorButton
            {
                TopOffset = 189,
                LeftOffset = 27,
                Text = "►",
            };
            m_btn_move_right.Click += new EventHandler(Btn_Move_Click);
            Controls.Add(m_btn_move_right);
        }

        private void Btn_Zoom_Click(object sender, EventArgs e)
        {
            float factor = (sender == m_btn_zoom_plus) ? 0.1f : -0.1f;
            ScaleCanvas(_CanvasScale + factor, Width / 2, Height / 2);
        }

        private void Btn_Move_Click(object sender, EventArgs e)
        {
            if (sender == m_btn_move_up)
                MoveCanvas(_CanvasOffsetX, m_real_canvas_y + 40, true, CanvasMoveArgs.Top);
            else if (sender == m_btn_move_down)
                MoveCanvas(_CanvasOffsetX, m_real_canvas_y - 40, true, CanvasMoveArgs.Top);
            else if (sender == m_btn_move_left)
                MoveCanvas(m_real_canvas_x + 40, _CanvasOffsetY, true, CanvasMoveArgs.Left);
            else
                MoveCanvas(m_real_canvas_x - 40, _CanvasOffsetY, true, CanvasMoveArgs.Left);
        }

        protected override void WndProc(ref Message m) {
            base.WndProc(ref m);

            ushort LOWORD(IntPtr dwValue) =>
                (ushort)(((long)dwValue) & 0x0000FFFF);

            ushort HIWORD(IntPtr dwValue) =>
                (ushort)(((long)dwValue) >> 16);

            try {
                Point pt = new Point(((int)m.LParam) >> 16, (ushort)m.LParam);
                pt = PointToClient(pt);

                if (m.Msg == WM_MOUSEHWHEEL) { //get horizontal scroll message
                    MouseButtons mb = MouseButtons.None;
                    int n = (short)LOWORD(m.WParam);

                    if ((n & 0x0001) == 0x0001)
                        mb |= MouseButtons.Left;

                    if ((n & 0x0010) == 0x0010)
                        mb |= MouseButtons.Middle;

                    if ((n & 0x0002) == 0x0002)
                        mb |= MouseButtons.Right;

                    if ((n & 0x0020) == 0x0020)
                        mb |= MouseButtons.XButton1;

                    if ((n & 0x0040) == 0x0040)
                        mb |= MouseButtons.XButton2;

                    int shiftedWParam = (short)HIWORD(m.WParam);
                    OnMouseHWheel(new MouseEventArgs(mb, 0, pt.X, pt.Y, shiftedWParam));
                }
            } catch { /*add code*/ }
        }

        protected override void OnInvalidated(InvalidateEventArgs e)
        {
            base.OnInvalidated(e);

            if (Parent != null)
            {
                foreach (var item in Parent.Controls)
                {
                    if (item is STNodePropertyGrid)
                        (item as STNodePropertyGrid).Invalidate();
                }
            }
        }

        protected override void OnPaint(PaintEventArgs e) {
            base.OnPaint(e);
            Graphics g = e.Graphics;
            g.Clear(BackColor);
            g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
            m_drawing_tools.Graphics = g;

            if (_ShowGrid)
                OnDrawGrid(m_drawing_tools, Width, Height);

            g.TranslateTransform(_CanvasOffsetX, _CanvasOffsetY); //moving coordinate system
            g.ScaleTransform(_CanvasScale, _CanvasScale); //Scale the drawing surface

            OnDrawConnectedLine(m_drawing_tools);
            OnDrawNode(m_drawing_tools, ControlToCanvas(ClientRectangle));

            if (m_ca == CanvasAction.ConnectOption) { //if connecting
                m_drawing_tools.Pen.Color = _HighLineColor;
                g.SmoothingMode = SmoothingMode.HighQuality;

                if (m_option_down.IsInput)
                    DrawBezier(g, m_drawing_tools.Pen, m_pt_in_canvas, m_pt_dot_down, _Curvature);
                else
                    DrawBezier(g, m_drawing_tools.Pen, m_pt_dot_down, m_pt_in_canvas, _Curvature);
            }

            //Reset the drawing coordinates I think other decoration-related drawing except nodes should not be drawn
            //in the Canvas coordinate system, but should be drawn using the coordinates of the control,
            //otherwise it will be affected by the zoom ratio
            g.ResetTransform();

            switch (m_ca) {
                case CanvasAction.MoveNode: //Draw alignment guides during movement
                    if (_ShowMagnet && _ActiveNode != null)
                        OnDrawMagnet(m_drawing_tools, m_mi);

                    break;
                case CanvasAction.SelectRectangle: //Draw rectangle selection
                    OnDrawSelectedRectangle(m_drawing_tools, CanvasToControl(m_rect_select));
                    break;
                case CanvasAction.DrawMarkDetails: //Draw marker info details
                    if (!string.IsNullOrEmpty(m_find.Mark))
                        OnDrawMark(m_drawing_tools);

                    break;
            }

            if (_ShowLocation)
                OnDrawNodeOutLocation(m_drawing_tools, Size, m_lst_node_out);

            OnDrawAlert(g);
        }

        protected override void OnMouseDown(MouseEventArgs e) {
            base.OnMouseDown(e);
            Focus();
            m_ca = CanvasAction.None;
            m_mi.XMatched = m_mi.YMatched = false;
            m_pt_down_in_control = e.Location;
            m_pt_down_in_canvas.X = ((e.X - _CanvasOffsetX) / _CanvasScale);
            m_pt_down_in_canvas.Y = ((e.Y - _CanvasOffsetY) / _CanvasScale);
            m_pt_canvas_old.X = _CanvasOffsetX;
            m_pt_canvas_old.Y = _CanvasOffsetY;

            if (m_gp_hover != null && e.Button == MouseButtons.Right) { //Disconnect
                DisConnectionHover();
                m_is_process_mouse_event = false; //Terminate MouseClick and MouseUp down pass
                return;
            }

            NodeFindInfo nfi = FindNodeFromPoint(m_pt_down_in_canvas);

            if (!string.IsNullOrEmpty(nfi.Mark)) { //If you click on the tag information
                m_ca = CanvasAction.DrawMarkDetails;
                Invalidate();
                return;
            }

            if (nfi.NodeOption != null) { //If the connection point of the Option under the point
                StartConnect(nfi.NodeOption);
                return;
            }

            if (nfi.Node != null) {
                nfi.Node.OnMouseDown(new MouseEventArgs(e.Button, e.Clicks, (int)m_pt_down_in_canvas.X - nfi.Node.Left, (int)m_pt_down_in_canvas.Y - nfi.Node.Top, e.Delta));
                bool bCtrlDown = (Control.ModifierKeys & Keys.Control) == Keys.Control;

                if (bCtrlDown) {
                    if (nfi.Node.IsSelected) {
                        if (nfi.Node == _ActiveNode)
                            SetActiveNode(null);
                    } else {
                        nfi.Node.SetSelected(true, true);
                    }

                    return;
                } else if (!nfi.Node.IsSelected) {
                    foreach (var n in m_hs_node_selected.ToArray())
                        n.SetSelected(false, false);
                }

                nfi.Node.SetSelected(true, false); //Add to selected node
                SetActiveNode(nfi.Node);

                if (PointInRectangle(nfi.Node.TitleRectangle, m_pt_down_in_canvas.X, m_pt_down_in_canvas.Y)) {
                    if (e.Button == MouseButtons.Right) {
                        nfi.Node.ContextMenuStrip?.Show(PointToScreen(e.Location));
                    } else {
                        m_dic_pt_selected.Clear();
                        lock (m_hs_node_selected) {
                            foreach (STNode n in m_hs_node_selected) //Record the position of the selected node, which will be useful if you need to move the selected node
                                m_dic_pt_selected.Add(n, n.Location);
                        }
                        m_ca = CanvasAction.MoveNode; //If the title of the node is clicked, the node can be moved

                        if (_ShowMagnet && _ActiveNode != null)
                            BuildMagnetLocation(); //Create the coordinates required by the magnet, which will be useful if you need to move the selected node
                    }
                } else {
                    m_node_down = nfi.Node;
                }
            } else {
                SetActiveNode(null);

                foreach (var n in m_hs_node_selected.ToArray())
                    n.SetSelected(false, false); //Do not click anything to clear the selected nodes

                m_ca = CanvasAction.SelectRectangle; //Enter rectangular area selection mode
                m_rect_select.Width = m_rect_select.Height = 0;
                m_node_down = null;
            }
            //this.SetActiveNode(nfi.Node);
        }

        protected override void OnMouseMove(MouseEventArgs e) {
            base.OnMouseMove(e);
            m_pt_in_control = e.Location;
            m_pt_in_canvas.X = ((e.X - _CanvasOffsetX) / _CanvasScale);
            m_pt_in_canvas.Y = ((e.Y - _CanvasOffsetY) / _CanvasScale);

            if (m_node_down != null) {
                m_node_down.OnMouseMove(new MouseEventArgs(e.Button, e.Clicks,
                    (int)m_pt_in_canvas.X - m_node_down.Left,
                    (int)m_pt_in_canvas.Y - m_node_down.Top, e.Delta));
                return;
            }

            if (e.Button == MouseButtons.Middle) { //Middle mouse button to move the canvas
                _CanvasOffsetX = m_real_canvas_x = m_pt_canvas_old.X + (e.X - m_pt_down_in_control.X);
                _CanvasOffsetY = m_real_canvas_y = m_pt_canvas_old.Y + (e.Y - m_pt_down_in_control.Y);
                Invalidate();
                return;
            }

            if (e.Button == MouseButtons.Left) { //If the left mouse button is clicked, judge the behavior
                m_gp_hover = null;

                switch (m_ca) {
                    case CanvasAction.MoveNode: MoveNode(e.Location); return; //current moving node
                    case CanvasAction.ConnectOption: Invalidate(); return; //currently connected
                    case CanvasAction.SelectRectangle: //currently selecting
                        m_rect_select.X = m_pt_down_in_canvas.X < m_pt_in_canvas.X ? m_pt_down_in_canvas.X : m_pt_in_canvas.X;
                        m_rect_select.Y = m_pt_down_in_canvas.Y < m_pt_in_canvas.Y ? m_pt_down_in_canvas.Y : m_pt_in_canvas.Y;
                        m_rect_select.Width = Math.Abs(m_pt_in_canvas.X - m_pt_down_in_canvas.X);
                        m_rect_select.Height = Math.Abs(m_pt_in_canvas.Y - m_pt_down_in_canvas.Y);

                        foreach (STNode n in _Nodes)
                            n.SetSelected(m_rect_select.IntersectsWith(n.Rectangle), false);
                        
                        Invalidate();
                        return;
                }
            }

            //If there is no behavior, determine whether there are other objects under the mouse
            NodeFindInfo nfi = FindNodeFromPoint(m_pt_in_canvas);
            bool bRedraw = false;

            if (_HoverNode != nfi.Node) { //Mouse over Node
                nfi.Node?.OnMouseEnter(EventArgs.Empty);
                _HoverNode?.OnMouseLeave(new MouseEventArgs(e.Button, e.Clicks,
                        (int)m_pt_in_canvas.X - _HoverNode.Left,
                        (int)m_pt_in_canvas.Y - _HoverNode.Top, e.Delta));
                _HoverNode = nfi.Node;
                OnHoverChanged(EventArgs.Empty);
                bRedraw = true;
            }

            if (_HoverNode != null) {
                _HoverNode.OnMouseMove(new MouseEventArgs(e.Button, e.Clicks,
                    (int)m_pt_in_canvas.X - _HoverNode.Left,
                    (int)m_pt_in_canvas.Y - _HoverNode.Top, e.Delta));
                m_gp_hover = null;
            } else {
                GraphicsPath gp = null;
                foreach (var v in m_dic_gp_info) { //Determine whether the mouse hovers over the connection path
                    if (v.Key.IsOutlineVisible(m_pt_in_canvas, m_p_line_hover)) {
                        gp = v.Key;
                        break;
                    }
                }

                if (m_gp_hover != gp) {
                    m_gp_hover = gp;
                    bRedraw = true;
                }
            }

            if (bRedraw)
                Invalidate();
        }

        protected override void OnMouseUp(MouseEventArgs e) {
            base.OnMouseUp(e);
            var nfi = FindNodeFromPoint(m_pt_in_canvas);

            switch (m_ca) { //Judging behavior when the mouse is lifted
                case CanvasAction.MoveNode: //If the Node is being moved, re-record the current position
                    foreach (STNode n in m_dic_pt_selected.Keys.ToList())
                        m_dic_pt_selected[n] = n.Location;

                    break;
                case CanvasAction.ConnectOption: //If connected, end the connection
                    if (e.Location == m_pt_down_in_control)
                        break;

                    if (nfi.NodeOption != null) {
                        if (m_option_down.IsInput)
                            nfi.NodeOption.ConnectOption(m_option_down);
                        else
                            m_option_down.ConnectOption(nfi.NodeOption);
                    }

                    break;
            }

            if (m_is_process_mouse_event && _ActiveNode != null) {
                var mea = new MouseEventArgs(e.Button, e.Clicks,
                    (int)m_pt_in_canvas.X - _ActiveNode.Left,
                    (int)m_pt_in_canvas.Y - _ActiveNode.Top, e.Delta);
                _ActiveNode.OnMouseUp(mea);
                m_node_down = null;
            }
            m_is_process_mouse_event = true; //Currently no event delivery is performed for the disconnection operation, and the event will be accepted next time
            m_ca = CanvasAction.None;
            Invalidate();
        }

        protected override void OnMouseEnter(EventArgs e) {
            base.OnMouseEnter(e);
            m_mouse_in_control = true;
        }

        protected override void OnMouseLeave(EventArgs e) {
            base.OnMouseLeave(e);
            m_mouse_in_control = false;
            _HoverNode?.OnMouseLeave(e);
            _HoverNode = null;
            Invalidate();
        }

        protected override void OnMouseWheel(MouseEventArgs e) {
            base.OnMouseWheel(e);

            if ((Control.ModifierKeys & Keys.Control) == Keys.Control) {
                float f = _CanvasScale + (e.Delta < 0 ? -0.1f : 0.1f);
                ScaleCanvas(f, Width / 2, Height / 2);
            } else {
                if (!m_mouse_in_control)
                    return;

                _ = FindNodeFromPoint(m_pt_in_canvas);

                if (_HoverNode != null) {
                    _HoverNode.OnMouseWheel(new MouseEventArgs(e.Button, e.Clicks,
                        (int)m_pt_in_canvas.X - _HoverNode.Left,
                        (int)m_pt_in_canvas.Y - _HoverNode.Top, e.Delta));
                    return;
                }

                int t = (int)DateTime.Now.Subtract(m_dt_vw).TotalMilliseconds;

                if (t <= 30)
                    t = 40;
                else if (t <= 100)
                    t = 20;
                else if (t <= 150)
                    t = 10;
                else if (t <= 300)
                    t = 4;
                else
                    t = 2;

                MoveCanvas(_CanvasOffsetX, m_real_canvas_y + (e.Delta < 0 ? -t : t), true, CanvasMoveArgs.Top);//process mouse mid
                m_dt_vw = DateTime.Now;
            }
        }

        protected virtual void OnMouseHWheel(MouseEventArgs e) {
            if ((Control.ModifierKeys & Keys.Control) == Keys.Control)
                return;

            if (!m_mouse_in_control)
                return;

            if (_HoverNode != null) {
                _HoverNode.OnMouseWheel(new MouseEventArgs(e.Button, e.Clicks,
                    (int)m_pt_in_canvas.X - _HoverNode.Left,
                    (int)m_pt_in_canvas.Y - _HoverNode.Top, e.Delta));
                return;
            }

            int t = (int)DateTime.Now.Subtract(m_dt_hw).TotalMilliseconds;

            if (t <= 30)
                t = 40;
            else if (t <= 100)
                t = 20;
            else if (t <= 150)
                t = 10;
            else if (t <= 300)
                t = 4;
            else
                t = 2;

            MoveCanvas(m_real_canvas_x + (e.Delta > 0 ? -t : t), _CanvasOffsetY, true, CanvasMoveArgs.Left);
            m_dt_hw = DateTime.Now;
        }

        //===========================for node other event==================================
        protected override void OnMouseClick(MouseEventArgs e) {
            base.OnMouseClick(e);

            if (_ActiveNode != null && m_is_process_mouse_event) {
                if (!PointInRectangle(_ActiveNode.Rectangle, m_pt_in_canvas.X, m_pt_in_canvas.Y))
                    return;

                _ActiveNode.OnMouseClick(new MouseEventArgs(e.Button, e.Clicks,
                    (int)m_pt_down_in_canvas.X - _ActiveNode.Left,
                    (int)m_pt_down_in_canvas.Y - _ActiveNode.Top, e.Delta));
            }
        }

        protected override void OnKeyDown(KeyEventArgs e) {
            base.OnKeyDown(e);
            _ActiveNode?.OnKeyDown(e);
        }

        protected override void OnKeyUp(KeyEventArgs e) {
            base.OnKeyUp(e);
            _ActiveNode?.OnKeyUp(e);
            m_node_down = null;
        }

        protected override void OnKeyPress(KeyPressEventArgs e) {
            base.OnKeyPress(e);
            _ActiveNode?.OnKeyPress(e);
        }

        #endregion

        protected override void OnDragEnter(DragEventArgs drgevent) {
            base.OnDragEnter(drgevent);

            if (DesignMode)
                return;
            
            drgevent.Effect = drgevent.Data.GetDataPresent("STNodeType") ? DragDropEffects.Copy : DragDropEffects.None;
        }

        protected override void OnDragDrop(DragEventArgs drgevent) {
            base.OnDragDrop(drgevent);

            if (DesignMode)
                return;

            if (drgevent.Data.GetDataPresent("STNodeType")) {
                object data = drgevent.Data.GetData("STNodeType");

                if (!(data is Type))
                    return;

                var t = (Type)data;

                if (!t.IsSubclassOf(typeof(STNode)))
                    return;

                STNode node = (STNode)Activator.CreateInstance((t));
                Point pt = new Point(drgevent.X, drgevent.Y);
                pt = PointToClient(pt);
                pt = ControlToCanvas(pt);
                node.Left = pt.X; node.Top = pt.Y;
                Nodes.Add(node);
            }
        }

        #region protected ----------------------------------------------------------------------------------------------------
        /// <summary>
        /// Occurs when background gridlines are drawn
        /// </summary>
        /// <param name="dt">drawing tool</param>
        /// <param name="nWidth">Need to draw width</param>
        /// <param name="nHeight">Need to draw height</param>
        protected virtual void OnDrawGrid(DrawingTools dt, int nWidth, int nHeight) {
            Graphics g = dt.Graphics;

            using (Pen p_2 = new Pen(Color.FromArgb(65, _GridColor))) {
                using (Pen p_1 = new Pen(Color.FromArgb(30, _GridColor))) {
                    float nIncrement = (20 * _CanvasScale); //The spacing between the grids is drawn according to the scale
                    int n = 5 - (int)(_CanvasOffsetX / nIncrement);

                    for (float f = _CanvasOffsetX % nIncrement; f < nWidth; f += nIncrement)
                        g.DrawLine((n++ % 5 == 0 ? p_2 : p_1), f, 0, f, nHeight);

                    n = 5 - (int)(_CanvasOffsetY / nIncrement);

                    for (float f = _CanvasOffsetY % nIncrement; f < nHeight; f += nIncrement)
                        g.DrawLine((n++ % 5 == 0 ? p_2 : p_1), 0, f, nWidth, f);

                    //origin two antennas
                    p_1.Color = Color.FromArgb(_Nodes.Count == 0 ? 255 : 120, _GridColor);
                    g.DrawLine(p_1, _CanvasOffsetX, 0, _CanvasOffsetX, nHeight);
                    g.DrawLine(p_1, 0, _CanvasOffsetY, nWidth, _CanvasOffsetY);
                }
            }
        }

        /// <summary>
        /// Occurs when a Node is drawn
        /// </summary>
        /// <param name="dt">drawing tool</param>
        /// <param name="rect">Visual canvas area size</param>
        protected virtual void OnDrawNode(DrawingTools dt, Rectangle rect) {
            m_lst_node_out.Clear(); //Clear the coordinates of Node beyond the visual area

            foreach (STNode n in _Nodes) {
                if (n.Owner == null)
                    continue;

                if (_ShowBorder)
                    OnDrawNodeBorder(dt, n);

                n.OnDrawNode(dt); //Call Node to draw the body part by itself

                if (!string.IsNullOrEmpty(n.Mark))
                    n.OnDrawMark(dt); //Call Node to draw the Mark area by itself

                if (!rect.IntersectsWith(n.Rectangle))
                    m_lst_node_out.Add(n.Location); //Determine whether this Node is beyond the visual area
            }
        }

        /// <summary>
        /// Occurs when the Node border is drawn
        /// </summary>
        /// <param name="dt">drawing tool</param>
        /// <param name="node">target node</param>
        protected virtual void OnDrawNodeBorder(DrawingTools dt, STNode node) {
            Image img_border;

            if (_ActiveNode == node)
                img_border = m_img_border_active;
            else if (node.IsSelected)
                img_border = m_img_border_selected;
            else if (_HoverNode == node)
                img_border = m_img_border_hover;
            else
                img_border = m_img_border;

            RenderBorder(dt.Graphics, node.Rectangle, img_border);

            if (!string.IsNullOrEmpty(node.Mark))
                RenderBorder(dt.Graphics, node.MarkRectangle, img_border);
        }

        /// <summary>
        /// Occurs when drawing connected paths
        /// </summary>
        /// <param name="dt">drawing tool</param>
        protected virtual void OnDrawConnectedLine(DrawingTools dt) {
            Graphics g = dt.Graphics;
            g.SmoothingMode = SmoothingMode.HighQuality;
            m_p_line_hover.Color = Color.FromArgb(50, 0, 0, 0);
            var t = typeof(object);

            foreach (STNode n in _Nodes) {
                foreach (STNodeOption op in n.OutputOptions) {
                    if (op == STNodeOption.Empty)
                        continue;

                    if (op.DotColor != Color.Transparent) {//determine line color
                        m_p_line.Color = op.DotColor;
                    } else {
                        m_p_line.Color = (op.DataType == t)
                            ? _UnknownTypeColor
                            : _TypeColor.ContainsKey(op.DataType)
                                ? _TypeColor[op.DataType]
                                : _UnknownTypeColor;//value can not be null
                    }

                    foreach (var v in op.ConnectedOption) {
                        DrawBezier(g, m_p_line_hover, op.DotLeft + op.DotSize, op.DotTop + op.DotSize / 2,
                            v.DotLeft - 1, v.DotTop + v.DotSize / 2, _Curvature);
                        DrawBezier(g, m_p_line, op.DotLeft + op.DotSize, op.DotTop + op.DotSize / 2,
                            v.DotLeft - 1, v.DotTop + v.DotSize / 2, _Curvature);

                        if (m_is_buildpath) { //If the current drawing needs to re-establish the connected path cache
                            GraphicsPath gp = CreateBezierPath(op.DotLeft + op.DotSize, op.DotTop + op.DotSize / 2,
                                v.DotLeft - 1, v.DotTop + v.DotSize / 2, _Curvature);
                            m_dic_gp_info.Add(gp, new ConnectionInfo() { Output = op, Input = v });
                        }
                    }
                }
            }

            m_p_line_hover.Color = _HighLineColor;

            if (m_gp_hover != null) //If there is currently a hovered connection path, it will be highlighted
                try { g.DrawPath(m_p_line_hover, m_gp_hover); } catch { }

            m_is_buildpath = false; //Reset the flag, the next time you draw, the path cache will not be rebuilt
        }

        /// <summary>
        /// Occurs when drawing Mark details
        /// </summary>
        /// <param name="dt">drawing tool</param>
        protected virtual void OnDrawMark(DrawingTools dt) {
            Graphics g = dt.Graphics;
            SizeF sz = g.MeasureString(m_find.Mark, Font); //Confirm the required size of the text
            Rectangle rect = new Rectangle(m_pt_in_control.X + 15,
                m_pt_in_control.Y + 10, (int)sz.Width + 6,
                4 + (Font.Height + 4) * m_find.MarkLines.Length); //sz.Height does not consider the line spacing of the text, so the height is calculated by itself here

            if (rect.Right > Width)
                rect.X = Width - rect.Width;

            if (rect.Bottom > Height)
                rect.Y = Height - rect.Height;

            if (rect.X < 0)
                rect.X = 0;

            if (rect.Y < 0)
                rect.Y = 0;

            dt.SolidBrush.Color = _MarkBackColor;
            g.SmoothingMode = SmoothingMode.None;
            g.FillRectangle(dt.SolidBrush, rect); //draw the background area
            rect.Width--; rect.Height--;
            dt.Pen.Color = Color.FromArgb(255, _MarkBackColor);
            g.DrawRectangle(dt.Pen, rect);
            dt.SolidBrush.Color = _MarkForeColor;

            m_sf.LineAlignment = StringAlignment.Center;
            //g.SmoothingMode = SmoothingMode.HighQuality;
            rect.X += 2; rect.Width -= 3;
            rect.Height = Font.Height + 4;
            int nY = rect.Y + 2;

            for (int i = 0; i < m_find.MarkLines.Length; i++) { //draw text
                rect.Y = nY + i * (Font.Height + 4);
                g.DrawString(m_find.MarkLines[i], Font, dt.SolidBrush, rect, m_sf);
            }
        }

        /// <summary>
        /// Occurs when the alignment guide needs to be displayed when moving the Node
        /// </summary>
        /// <param name="dt">drawing tool</param>
        /// <param name="mi">Matching magnet information</param>
        protected virtual void OnDrawMagnet(DrawingTools dt, MagnetInfo mi) {
            if (_ActiveNode == null)
                return;

            Graphics g = dt.Graphics;
            Pen pen = m_drawing_tools.Pen;
            SolidBrush brush = dt.SolidBrush;
            pen.Color = _MagnetColor;
            brush.Color = Color.FromArgb(_MagnetColor.A / 3, _MagnetColor);
            g.SmoothingMode = SmoothingMode.None;
            /*
            int nL = _ActiveNode.Left,
                nMX = _ActiveNode.Left + _ActiveNode.Width / 2,
                nR = _ActiveNode.Right;
            int nT = _ActiveNode.Top,
                nMY = _ActiveNode.Top + _ActiveNode.Height / 2,
                nB = _ActiveNode.Bottom;
            */

            if (mi.XMatched)
                g.DrawLine(pen, CanvasToControl(mi.X, true), 0, CanvasToControl(mi.X, true), Height);

            if (mi.YMatched)
                g.DrawLine(pen, 0, CanvasToControl(mi.Y, false), Width, CanvasToControl(mi.Y, false));

            g.TranslateTransform(_CanvasOffsetX, _CanvasOffsetY); //moving coordinate system
            g.ScaleTransform(_CanvasScale, _CanvasScale); //Scale the drawing surface

            if (mi.XMatched) {
                foreach (STNode n in _Nodes) {
                    if (n.Left == mi.X || n.Right == mi.X || n.Left + n.Width / 2 == mi.X)
                        g.FillRectangle(brush, n.Rectangle);
                }
            }

            if (mi.YMatched) {
                foreach (STNode n in _Nodes) {
                    if (n.Top == mi.Y || n.Bottom == mi.Y || n.Top + n.Height / 2 == mi.Y) 
                        g.FillRectangle(brush, n.Rectangle);
                }
            }

            g.ResetTransform();
        }

        /// <summary>
        /// Draw the selected rectangular area
        /// </summary>
        /// <param name="dt">drawing tool</param>
        /// <param name="rectf">A rectangular area on the control</param>
        protected virtual void OnDrawSelectedRectangle(DrawingTools dt, RectangleF rectf) {
            Graphics g = dt.Graphics;
            SolidBrush brush = dt.SolidBrush;
            dt.Pen.Color = _SelectedRectangleColor;
            g.DrawRectangle(dt.Pen, rectf.Left, rectf.Y, rectf.Width, rectf.Height);
            brush.Color = Color.FromArgb(_SelectedRectangleColor.A / 3, _SelectedRectangleColor);
            g.FillRectangle(brush, CanvasToControl(m_rect_select));
        }

        /// <summary>
        /// Draw the prompt information of the Node position beyond the visual area
        /// </summary>
        /// <param name="dt">drawing tool</param>
        /// <param name="sz">tooltip margin</param>
        /// <param name="lstPts">Node position information beyond the visual area</param>
        protected virtual void OnDrawNodeOutLocation(DrawingTools dt, Size sz, List<Point> lstPts) {
            Graphics g = dt.Graphics;
            SolidBrush brush = dt.SolidBrush;
            brush.Color = _LocationBackColor;
            g.SmoothingMode = SmoothingMode.None;

            if (lstPts.Count == _Nodes.Count && _Nodes.Count != 0) //If the number of excesses is the same as the number of collections, all of them are exceeded Draw a circumscribed rectangle
                g.FillRectangle(brush, CanvasToControl(_CanvasValidBounds));

            g.FillRectangle(brush, 0, 0, 4, sz.Height); //Draw a four-sided background
            g.FillRectangle(brush, sz.Width - 4, 0, 4, sz.Height);
            g.FillRectangle(brush, 4, 0, sz.Width - 8, 4);
            g.FillRectangle(brush, 4, sz.Height - 4, sz.Width - 8, 4);
            brush.Color = _LocationForeColor;

            foreach (var v in lstPts) { //draw points
                var pt = CanvasToControl(v);

                if (pt.X < 0)
                    pt.X = 0;

                if (pt.Y < 0)
                    pt.Y = 0;

                if (pt.X > sz.Width)
                    pt.X = sz.Width - 4;

                if (pt.Y > sz.Height)
                    pt.Y = sz.Height - 4;

                g.FillRectangle(brush, pt.X, pt.Y, 4, 4);
            }
        }

        /// <summary>
        /// Draw hints
        /// </summary>
        /// <param name="dt">drawing tool</param>
        /// <param name="rect">area to be drawn</param>
        /// <param name="strText">need to draw text</param>
        /// <param name="foreColor">information foreground color</param>
        /// <param name="backColor">information background color</param>
        /// <param name="al">information location</param>
        protected virtual void OnDrawAlert(DrawingTools dt, Rectangle rect, string strText, Color foreColor, Color backColor, AlertLocation al) {
            if (m_alpha_alert == 0)
                return;

            Graphics g = dt.Graphics;
            SolidBrush brush = dt.SolidBrush;

            g.SmoothingMode = SmoothingMode.None;
            brush.Color = backColor;
            dt.Pen.Color = brush.Color;
            g.FillRectangle(brush, rect);
            g.DrawRectangle(dt.Pen, rect.Left, rect.Top, rect.Width - 1, rect.Height - 1);

            brush.Color = foreColor;
            m_sf.Alignment = StringAlignment.Center;
            m_sf.LineAlignment = StringAlignment.Center;
            g.SmoothingMode = SmoothingMode.HighQuality;
            g.DrawString(strText, Font, brush, rect, m_sf);
        }

        /// <summary>
        /// Obtain the rectangular area that needs to be drawn for prompt information
        /// </summary>
        /// <param name="g">drawing surface</param>
        /// <param name="strText">need to draw text</param>
        /// <param name="al">information location</param>
        /// <returns>rectangular area</returns>
        protected virtual Rectangle GetAlertRectangle(Graphics g, string strText, AlertLocation al) {
            SizeF szf = g.MeasureString(m_str_alert, Font);
            Size sz = new Size((int)Math.Round(szf.Width + 10), (int)Math.Round(szf.Height + 4));
            Rectangle rect = new Rectangle(4, Height - sz.Height - 4, sz.Width, sz.Height);

            switch (al) {
                case AlertLocation.Left:
                    rect.Y = (Height - sz.Height) >> 1;
                    break;
                case AlertLocation.Top:
                    rect.Y = 4;
                    rect.X = (Width - sz.Width) >> 1;
                    break;
                case AlertLocation.Right:
                    rect.X = Width - sz.Width - 4;
                    rect.Y = (Height - sz.Height) >> 1;
                    break;
                case AlertLocation.Bottom:
                    rect.X = (Width - sz.Width) >> 1;
                    break;
                case AlertLocation.Center:
                    rect.X = (Width - sz.Width) >> 1;
                    rect.Y = (Height - sz.Height) >> 1;
                    break;
                case AlertLocation.LeftTop:
                    rect.X = rect.Y = 4;
                    break;
                case AlertLocation.RightTop:
                    rect.Y = 4;
                    rect.X = Width - sz.Width - 4;
                    break;
                case AlertLocation.RightBottom:
                    rect.X = Width - sz.Width - 4;
                    break;
            }

            return rect;
        }

        #endregion protected

        #region internal

        public void BuildLinePath() {
            foreach (var v in m_dic_gp_info)
                v.Key.Dispose();

            m_dic_gp_info.Clear();
            m_is_buildpath = true;
            Invalidate();
        }

        internal void OnDrawAlert(Graphics g) {
            m_rect_alert = GetAlertRectangle(g, m_str_alert, m_al);
            Color clr_fore = Color.FromArgb((int)((float)m_alpha_alert / 255 * m_forecolor_alert.A), m_forecolor_alert);
            Color clr_back = Color.FromArgb((int)((float)m_alpha_alert / 255 * m_backcolor_alert.A), m_backcolor_alert);
            OnDrawAlert(m_drawing_tools, m_rect_alert, m_str_alert, clr_fore, clr_back, m_al);
        }

        internal void InternalAddSelectedNode(STNode node) {
            node.IsSelected = true;
            lock (m_hs_node_selected) m_hs_node_selected.Add(node);
        }

        internal void InternalRemoveSelectedNode(STNode node) {
            node.IsSelected = false;
            lock (m_hs_node_selected) m_hs_node_selected.Remove(node);
        }

        #endregion internal

        #region private -----------------------------------------------------------------------------------------------------

        private void MoveCanvasThread() {
            bool bRedraw;

            while (true) {
                bRedraw = false;

                if (m_real_canvas_x != _CanvasOffsetX) {
                    float nx = m_real_canvas_x - _CanvasOffsetX;
                    float n = Math.Abs(nx) / 10;
                    float nTemp = Math.Abs(nx);

                    if (nTemp <= 4)
                        n = 1;
                    else if (nTemp <= 12)
                        n = 2;
                    else if (nTemp <= 30)
                        n = 3;

                    if (nTemp < 1)
                        _CanvasOffsetX = m_real_canvas_x;
                    else
                        _CanvasOffsetX += nx > 0 ? n : -n;
                    bRedraw = true;
                }

                if (m_real_canvas_y != _CanvasOffsetY) {
                    float ny = m_real_canvas_y - _CanvasOffsetY;
                    float n = Math.Abs(ny) / 10;
                    float nTemp = Math.Abs(ny);

                    if (nTemp <= 4)
                        n = 1;
                    else if (nTemp <= 12)
                        n = 2;
                    else if (nTemp <= 30)
                        n = 3;

                    if (nTemp < 1)
                        _CanvasOffsetY = m_real_canvas_y;
                    else
                        _CanvasOffsetY += ny > 0 ? n : -n;

                    bRedraw = true;
                }

                if (bRedraw) {
                    m_pt_canvas_old.X = _CanvasOffsetX;
                    m_pt_canvas_old.Y = _CanvasOffsetY;
                    Invalidate();
                    Thread.Sleep(30);
                } else {
                    Thread.Sleep(100);
                }
            }
        }

        private void ShowAlertThread() {
            while (true) {
                int nTime = m_time_alert - (int)DateTime.Now.Subtract(m_dt_alert).TotalMilliseconds;

                if (nTime > 0) {
                    Thread.Sleep(nTime);
                    continue;
                }

                if (nTime < -1000) {
                    if (m_alpha_alert != 0) {
                        m_alpha_alert = 0;
                        Invalidate();
                    }

                    Thread.Sleep(100);
                } else {
                    m_alpha_alert = (int)(255 - (-nTime / 1000F) * 255);
                    Invalidate(m_rect_alert);
                    Thread.Sleep(50);
                }
            }
        }

        private Image CreateBorderImage(Color clr) {
            Image img = new Bitmap(12, 12);

            using (Graphics g = Graphics.FromImage(img)) {
                g.SmoothingMode = SmoothingMode.HighQuality;

                using (GraphicsPath gp = new GraphicsPath()) {
                    gp.AddEllipse(new Rectangle(0, 0, 11, 11));

                    using (PathGradientBrush b = new PathGradientBrush(gp)) {
                        b.CenterColor = Color.FromArgb(200, clr);
                        b.SurroundColors = new Color[] { Color.FromArgb(10, clr) };
                        g.FillPath(b, gp);
                    }
                }
            }
            return img;
        }

        private ConnectionStatus DisConnectionHover() {
            if (!m_dic_gp_info.ContainsKey(m_gp_hover))
                return ConnectionStatus.DisConnected;

            ConnectionInfo ci = m_dic_gp_info[m_gp_hover];
            var ret = ci.Output.DisConnectOption(ci.Input);
            //this.OnOptionDisConnected(new STNodeOptionEventArgs(ci.Output, ci.Input, ret));

            if (ret == ConnectionStatus.DisConnected) {
                m_dic_gp_info.Remove(m_gp_hover);
                m_gp_hover.Dispose();
                m_gp_hover = null;
                Invalidate();
            }

            return ret;
        }

        private void StartConnect(STNodeOption op) {
            if (op.IsInput) {
                m_pt_dot_down.X = op.DotLeft;
                m_pt_dot_down.Y = op.DotTop + 5;
            } else {
                m_pt_dot_down.X = op.DotLeft + op.DotSize;
                m_pt_dot_down.Y = op.DotTop + 5;
            }

            m_ca = CanvasAction.ConnectOption;
            m_option_down = op;
        }

        private void MoveNode(Point pt) {
            int nX = (int)((pt.X - m_pt_down_in_control.X) / _CanvasScale);
            int nY = (int)((pt.Y - m_pt_down_in_control.Y) / _CanvasScale);

            lock (m_hs_node_selected) {
                foreach (STNode v in m_hs_node_selected) {
                    v.Left = m_dic_pt_selected[v].X + nX;
                    v.Top = m_dic_pt_selected[v].Y + nY;
                }

                if (_ShowMagnet) {
                    MagnetInfo mi = CheckMagnet(_ActiveNode);

                    if (mi.XMatched)
                        foreach (STNode v in m_hs_node_selected)
                            v.Left -= mi.OffsetX;

                    if (mi.YMatched)
                        foreach (STNode v in m_hs_node_selected)
                            v.Top -= mi.OffsetY;
                }
            }
            
            Invalidate();
        }

        protected internal virtual void BuildBounds() {
            if (_Nodes.Count == 0) {
                _CanvasValidBounds = ControlToCanvas(DisplayRectangle);
                return;
            }

            int x = int.MaxValue;
            int y = int.MaxValue;
            int r = int.MinValue;
            int b = int.MinValue;

            foreach (STNode n in _Nodes) {
                if (x > n.Left)
                    x = n.Left;

                if (y > n.Top)
                    y = n.Top;

                if (r < n.Right)
                    r = n.Right;

                if (b < n.Bottom)
                    b = n.Bottom;
            }

            _CanvasValidBounds.X = x - 60;
            _CanvasValidBounds.Y = y - 60;
            _CanvasValidBounds.Width = r - x + 120;
            _CanvasValidBounds.Height = b - y + 120;
        }

        private bool PointInRectangle(Rectangle rect, float x, float y) {
            if (x < rect.Left || x > rect.Right || y < rect.Top || y > rect.Bottom)
                return false;

            return true;
        }

        private void BuildMagnetLocation() {
            m_lst_magnet_x.Clear();
            m_lst_magnet_y.Clear();

            foreach (STNode v in _Nodes) {
                if (v.IsSelected)
                    continue;

                m_lst_magnet_x.Add(v.Left);
                m_lst_magnet_x.Add(v.Left + v.Width / 2);
                m_lst_magnet_x.Add(v.Left + v.Width);
                m_lst_magnet_y.Add(v.Top);
                m_lst_magnet_y.Add(v.Top + v.Height / 2);
                m_lst_magnet_y.Add(v.Top + v.Height);
            }
        }

        private MagnetInfo CheckMagnet(STNode node) {
            m_mi.XMatched = m_mi.YMatched = false;
            m_lst_magnet_mx.Clear();
            m_lst_magnet_my.Clear();
            m_lst_magnet_mx.Add(node.Left + node.Width / 2);
            m_lst_magnet_mx.Add(node.Left);
            m_lst_magnet_mx.Add(node.Left + node.Width);
            m_lst_magnet_my.Add(node.Top + node.Height / 2);
            m_lst_magnet_my.Add(node.Top);
            m_lst_magnet_my.Add(node.Top + node.Height);

            bool bFlag = false;

            foreach (var mx in m_lst_magnet_mx) {
                foreach (var x in m_lst_magnet_x) {
                    if (Math.Abs(mx - x) <= 5) {
                        bFlag = true;
                        m_mi.X = x;
                        m_mi.OffsetX = mx - x;
                        m_mi.XMatched = true;
                        break;
                    }
                }

                if (bFlag)
                    break;
            }

            bFlag = false;

            foreach (var my in m_lst_magnet_my) {
                foreach (var y in m_lst_magnet_y) {
                    if (Math.Abs(my - y) <= 5) {
                        bFlag = true;
                        m_mi.Y = y;
                        m_mi.OffsetY = my - y;
                        m_mi.YMatched = true;
                        break;
                    }
                }

                if (bFlag)
                    break;
            }

            return m_mi;
        }

        private void DrawBezier(Graphics g, Pen p, PointF ptStart, PointF ptEnd, float f) {
            DrawBezier(g, p, ptStart.X, ptStart.Y, ptEnd.X, ptEnd.Y, f);
        }

        private void DrawBezier(Graphics g, Pen p, float x1, float y1, float x2, float y2, float f) {
            float n = (Math.Abs(x1 - x2) * f);

            if (_Curvature != 0 && n < 30)
                n = 30;

            g.DrawBezier(p,
                x1, y1,
                x1 + n, y1,
                x2 - n, y2,
                x2, y2);
        }

        private GraphicsPath CreateBezierPath(float x1, float y1, float x2, float y2, float f) {
            GraphicsPath gp = new GraphicsPath();
            float n = (Math.Abs(x1 - x2) * f);

            if (_Curvature != 0 && n < 30)
                n = 30;

            gp.AddBezier(
                x1, y1,
                x1 + n, y1,
                x2 - n, y2,
                x2, y2
                );
            return gp;
        }

        private void RenderBorder(Graphics g, Rectangle rect, Image img) {
            //Fill the four corners
            g.DrawImage(img, new Rectangle(rect.X - 5, rect.Y - 5, 5, 5),
                new Rectangle(0, 0, 5, 5), GraphicsUnit.Pixel);
            g.DrawImage(img, new Rectangle(rect.Right, rect.Y - 5, 5, 5),
                new Rectangle(img.Width - 5, 0, 5, 5), GraphicsUnit.Pixel);
            g.DrawImage(img, new Rectangle(rect.X - 5, rect.Bottom, 5, 5),
                new Rectangle(0, img.Height - 5, 5, 5), GraphicsUnit.Pixel);
            g.DrawImage(img, new Rectangle(rect.Right, rect.Bottom, 5, 5),
                new Rectangle(img.Width - 5, img.Height - 5, 5, 5), GraphicsUnit.Pixel);
            //four sides
            g.DrawImage(img, new Rectangle(rect.X - 5, rect.Y, 5, rect.Height),
                new Rectangle(0, 5, 5, img.Height - 10), GraphicsUnit.Pixel);
            g.DrawImage(img, new Rectangle(rect.X, rect.Y - 5, rect.Width, 5),
                new Rectangle(5, 0, img.Width - 10, 5), GraphicsUnit.Pixel);
            g.DrawImage(img, new Rectangle(rect.Right, rect.Y, 5, rect.Height),
                new Rectangle(img.Width - 5, 5, 5, img.Height - 10), GraphicsUnit.Pixel);
            g.DrawImage(img, new Rectangle(rect.X, rect.Bottom, rect.Width, 5),
                new Rectangle(5, img.Height - 5, img.Width - 10, 5), GraphicsUnit.Pixel);
        }

        /// <summary>
        /// Patch outdated node GUIDs with their new GUID
        /// </summary>
        /// <param name="guid">The old GUID</param>
        /// <param name="name">The FullName of the node</param>
        /// <returns>The proper GUID</returns>
        private string PatchOldData(string guid, string name)
        {
            if (!m_dic_type.ContainsKey(guid))
            {
                StringBuilder sb = new StringBuilder(name);

                // Fix a bad path
                if (name.EndsWith(".StringAppendNode"))
                    sb.Replace(".Number.Int.", ".StringNode.");
                if (name.EndsWith(".StringContainsNode"))
                    sb.Replace(".Number.Int.", ".StringNode.");
                if (name.EndsWith(".StringDisplayNode"))
                    sb.Replace(".Number.Int.", ".StringNode.");
                if (name.EndsWith(".LogicNOTNode"))
                    sb.Replace(".Bool.", ".LogicNode.");

                // Fix old paths
                if (name.EndsWith(".CommentNode"))
                    sb.Replace(".Nodes.", ".Nodes.UtilNode.");
                if (name.EndsWith(".STNodeHubSingle"))
                    sb.Replace(".Nodes.", ".Nodes.UtilNode.");
                if (name.EndsWith(".PassthroughNode"))
                    sb.Replace(".Nodes.", ".Nodes.UtilNode.");

                // Replace node root-paths
                sb.Replace(".Actions.", ".ActionNode.");
                sb.Replace(".Bool.", ".BoolNode.");
                sb.Replace(".Branch.", ".BranchNode.");
                sb.Replace(".Time.", ".DateTimeNode.");
                sb.Replace(".Events.", ".EventNode.");
                sb.Replace(".Logic.", ".LogicNode.");
                sb.Replace(".Math.", ".MathNode.");
                sb.Replace(".Number.", ".NumberNode.");
                sb.Replace(".String.", ".StringNode.");
                sb.Replace(".Graphics.", "GraphicsNode");
                // Replace node sub-paths
                sb.Replace(".App.", ".AppNode.");
                sb.Replace(".Buttplug.", ".IntifaceNode.");
                sb.Replace(".Twitch.", ".TwitchNode.");
                sb.Replace(".Color.", ".ColorNode.");
                sb.Replace(".Image.", ".ImageNode.");
                sb.Replace(".Float.", ".FloatNode.");
                sb.Replace(".Int.", ".IntNode.");
                sb.Replace(".UInt.", ".UIntNode.");
                sb.Replace(".ButtplugNode.", ".IntifaceNode.");

                string patchedName = sb.ToString();

                foreach (var t in m_dic_type)
                    if (t.Value.FullName.Equals(patchedName))
                        return t.Key;
            }

            return guid;
        }

        #endregion private

        #region public --------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Find by canvas coordinates
        /// </summary>
        /// <param name="pt">coordinates in the canvas</param>
        /// <returns>found data</returns>
        public NodeFindInfo FindNodeFromPoint(PointF pt) {
            m_find.Node = null;
            m_find.NodeOption = null;
            m_find.Mark = null;

            for (int i = _Nodes.Count - 1; i >= 0; i--) {
                if (!string.IsNullOrEmpty(_Nodes[i].Mark) && PointInRectangle(_Nodes[i].MarkRectangle, pt.X, pt.Y)) {
                    m_find.Mark = _Nodes[i].Mark;
                    m_find.MarkLines = _Nodes[i].MarkLines;
                    return m_find;
                }

                foreach (STNodeOption v in _Nodes[i].InputOptions) {
                    if (v == STNodeOption.Empty)
                        continue;

                    if (PointInRectangle(v.DotRectangle, pt.X, pt.Y))
                        m_find.NodeOption = v;
                }

                foreach (STNodeOption v in _Nodes[i].OutputOptions) {
                    if (v == STNodeOption.Empty)
                        continue;

                    if (PointInRectangle(v.DotRectangle, pt.X, pt.Y))
                        m_find.NodeOption = v;
                }

                if (PointInRectangle(_Nodes[i].Rectangle, pt.X, pt.Y))
                    m_find.Node = _Nodes[i];

                if (m_find.NodeOption != null || m_find.Node != null)
                    return m_find;
            }

            return m_find;
        }

        /// <summary>
        /// Get the selected Node collection
        /// </summary>
        /// <returns>Collection of Nodes</returns>
        public STNode[] GetSelectedNode() {
            return m_hs_node_selected.ToArray();
        }

        /// <summary>
        /// Convert canvas coordinates to control coordinates
        /// </summary>
        /// <param name="number">Item</param>
        /// <param name="isX">Is it the X coordinate</param>
        /// <returns>transformed coordinates</returns>
        public float CanvasToControl(float number, bool isX) {
            return (number * _CanvasScale) + (isX ? _CanvasOffsetX : _CanvasOffsetY);
        }

        /// <summary>
        /// Convert canvas coordinates to control coordinates
        /// </summary>
        /// <param name="pt">coordinate</param>
        /// <returns>transformed coordinates</returns>
        public PointF CanvasToControl(PointF pt) {
            pt.X = (pt.X * _CanvasScale) + _CanvasOffsetX;
            pt.Y = (pt.Y * _CanvasScale) + _CanvasOffsetY;
            //pt.X += _CanvasOffsetX;
            //pt.Y += _CanvasOffsetY;
            return pt;
        }

        /// <summary>
        /// Convert canvas coordinates to control coordinates
        /// </summary>
        /// <param name="pt">coordinate</param>
        /// <returns>transformed coordinates</returns>
        public Point CanvasToControl(Point pt) {
            pt.X = (int)(pt.X * _CanvasScale + _CanvasOffsetX);
            pt.Y = (int)(pt.Y * _CanvasScale + _CanvasOffsetY);
            //pt.X += (int)_CanvasOffsetX;
            //pt.Y += (int)_CanvasOffsetY;
            return pt;
        }

        /// <summary>
        /// Convert canvas coordinates to control coordinates
        /// </summary>
        /// <param name="rect">rectangular area</param>
        /// <returns>Converted rectangular area</returns>
        public Rectangle CanvasToControl(Rectangle rect) {
            rect.X = (int)((rect.X * _CanvasScale) + _CanvasOffsetX);
            rect.Y = (int)((rect.Y * _CanvasScale) + _CanvasOffsetY);
            rect.Width = (int)(rect.Width * _CanvasScale);
            rect.Height = (int)(rect.Height * _CanvasScale);
            //rect.X += (int)_CanvasOffsetX;
            //rect.Y += (int)_CanvasOffsetY;
            return rect;
        }

        /// <summary>
        /// Convert canvas coordinates to control coordinates
        /// </summary>
        /// <param name="rect">rectangular area</param>
        /// <returns>Converted rectangular area</returns>
        public RectangleF CanvasToControl(RectangleF rect) {
            rect.X = (rect.X * _CanvasScale) + _CanvasOffsetX;
            rect.Y = (rect.Y * _CanvasScale) + _CanvasOffsetY;
            rect.Width *= _CanvasScale;
            rect.Height *= _CanvasScale;
            //rect.X += _CanvasOffsetX;
            //rect.Y += _CanvasOffsetY;
            return rect;
        }

        /// <summary>
        /// Convert control coordinates to canvas coordinates
        /// </summary>
        /// <param name="number">Index</param>
        /// <param name="isX">Is it the X coordinate</param>
        /// <returns>transformed coordinates</returns>
        public float ControlToCanvas(float number, bool isX) {
            return (number - (isX ? _CanvasOffsetX : _CanvasOffsetY)) / _CanvasScale;
        }

        /// <summary>
        /// Convert control coordinates to canvas coordinates
        /// </summary>
        /// <param name="pt">coordinate</param>
        /// <returns>transformed coordinates</returns>
        public Point ControlToCanvas(Point pt) {
            pt.X = (int)((pt.X - _CanvasOffsetX) / _CanvasScale);
            pt.Y = (int)((pt.Y - _CanvasOffsetY) / _CanvasScale);
            return pt;
        }

        /// <summary>
        /// Convert control coordinates to canvas coordinates
        /// </summary>
        /// <param name="pt">coordinate</param>
        /// <returns>transformed coordinates</returns>
        public PointF ControlToCanvas(PointF pt) {
            pt.X = ((pt.X - _CanvasOffsetX) / _CanvasScale);
            pt.Y = ((pt.Y - _CanvasOffsetY) / _CanvasScale);
            return pt;
        }

        /// <summary>
        /// Convert control coordinates to canvas coordinates
        /// </summary>
        /// <param name="rect">rectangular area</param>
        /// <returns>converted area</returns>
        public Rectangle ControlToCanvas(Rectangle rect) {
            rect.X = (int)((rect.X - _CanvasOffsetX) / _CanvasScale);
            rect.Y = (int)((rect.Y - _CanvasOffsetY) / _CanvasScale);
            rect.Width = (int)(rect.Width / _CanvasScale);
            rect.Height = (int)(rect.Height / _CanvasScale);
            return rect;
        }

        /// <summary>
        /// Convert control coordinates to canvas coordinates
        /// </summary>
        /// <param name="rect">rectangular area</param>
        /// <returns>converted area</returns>
        public RectangleF ControlToCanvas(RectangleF rect) {
            rect.X = ((rect.X - _CanvasOffsetX) / _CanvasScale);
            rect.Y = ((rect.Y - _CanvasOffsetY) / _CanvasScale);
            rect.Width /= _CanvasScale;
            rect.Height /= _CanvasScale;
            return rect;
        }

        /// <summary>
        /// Move the canvas origin coordinates to the specified control coordinates
        /// Cannot move when there is no Node
        /// </summary>
        /// <param name="x">X coordinate</param>
        /// <param name="y">Y coordinate</param>
        /// <param name="bAnimation">Whether to start the animation effect during the movement</param>
        /// <param name="ma">Specify the coordinate parameters that need to be modified</param>
        public void MoveCanvas(float x, float y, bool bAnimation, CanvasMoveArgs ma) {
            if (_Nodes.Count == 0) {
                m_real_canvas_x = m_real_canvas_y = 10;
                return;
            }

            int l = (int)((_CanvasValidBounds.Left + 50) * _CanvasScale);
            int t = (int)((_CanvasValidBounds.Top + 50) * _CanvasScale);
            int r = (int)((_CanvasValidBounds.Right - 50) * _CanvasScale);
            int b = (int)((_CanvasValidBounds.Bottom - 50) * _CanvasScale);

            if (r + x < 0)
                x = -r;

            if (Width - l < x)
                x = Width - l;

            if (b + y < 0)
                y = -b;

            if (Height - t < y)
                y = Height - t;

            if (bAnimation) {
                if ((ma & CanvasMoveArgs.Left) == CanvasMoveArgs.Left)
                    m_real_canvas_x = x;

                if ((ma & CanvasMoveArgs.Top) == CanvasMoveArgs.Top)
                    m_real_canvas_y = y;
            } else {
                m_real_canvas_x = _CanvasOffsetX = x;
                m_real_canvas_y = _CanvasOffsetY = y;
            }

            OnCanvasMoved(EventArgs.Empty);
        }

        /// <summary>
        /// Zoom canvas
        /// Cannot scale when there is no Node
        /// </summary>
        /// <param name="f">Scaling ratio</param>
        /// <param name="x">The coordinates of the zoom center X located on the control</param>
        /// <param name="y">The coordinates where the zoom center Y is located on the control</param>
        public void ScaleCanvas(float f, float x, float y) {
            if (_Nodes.Count == 0) {
                _CanvasScale = 1F;
                return;
            }

            if (_CanvasScale == f)
                return;

            if (f < 0.5)
                f = 0.5f;
            else if (f > 3)
                f = 3;

            float x_c = ControlToCanvas(x, true);
            float y_c = ControlToCanvas(y, false);
            _CanvasScale = f;
            _CanvasOffsetX = m_real_canvas_x -= CanvasToControl(x_c, true) - x;
            _CanvasOffsetY = m_real_canvas_y -= CanvasToControl(y_c, false) - y;
            OnCanvasScaled(EventArgs.Empty);
            Invalidate();
        }

        /// <summary>
        /// Get the corresponding relationship of the currently connected Option
        /// </summary>
        /// <returns>collection of connection information</returns>
        public ConnectionInfo[] GetConnectionInfo() {
            return m_dic_gp_info.Values.ToArray();
        }

        /// <summary>
        /// Determine whether there is a connection path between two Nodes
        /// </summary>
        /// <param name="nodeStart">Start Node</param>
        /// <param name="nodeFind">Target Node</param>
        /// <returns>Returns true if the path exists, otherwise false</returns>
        public static bool CanFindNodePath(STNode nodeStart, STNode nodeFind) {
            HashSet<STNode> hs = new HashSet<STNode>();
            return STNodeEditor.CanFindNodePath(nodeStart, nodeFind, hs);
        }

        private static bool CanFindNodePath(STNode nodeStart, STNode nodeFind, HashSet<STNode> hs) {
            foreach (STNodeOption op_1 in nodeStart.OutputOptions) {
                foreach (STNodeOption op_2 in op_1.ConnectedOption) {
                    if (op_2.Owner == nodeFind)
                        return true;

                    if (hs.Add(op_2.Owner) && STNodeEditor.CanFindNodePath(op_2.Owner, nodeFind))
                        return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Get the image of the specified rectangular area in the canvas
        /// </summary>
        /// <param name="rect">A specified rectangular area in the canvas</param>
        /// <returns>image</returns>
        public Image GetCanvasImage(Rectangle rect) =>
            GetCanvasImage(rect, 1f);

        /// <summary>
        /// Get the image of the specified rectangular area in the canvas
        /// </summary>
        /// <param name="rect">A specified rectangular area in the canvas</param>
        /// <param name="fScale">Scaling ratio</param>
        /// <returns>image</returns>
        public Image GetCanvasImage(Rectangle rect, float fScale) {
            if (fScale < 0.5)
                fScale = 0.5f;
            else if (fScale > 3)
                fScale = 3;

            Image img = new Bitmap((int)(rect.Width * fScale), (int)(rect.Height * fScale));

            using (Graphics g = Graphics.FromImage(img)) {
                g.Clear(BackColor);
                g.ScaleTransform(fScale, fScale);
                m_drawing_tools.Graphics = g;

                if (_ShowGrid)
                    OnDrawGrid(m_drawing_tools, rect.Width, rect.Height);

                g.TranslateTransform(-rect.X, -rect.Y); //moving coordinate system
                OnDrawNode(m_drawing_tools, rect);
                OnDrawConnectedLine(m_drawing_tools);

                g.ResetTransform();

                if (_ShowLocation)
                    OnDrawNodeOutLocation(m_drawing_tools, img.Size, m_lst_node_out);
            }

            GC.Collect();
            return img;
        }

        /// <summary>
        /// Save the class content in the canvas to a file
        /// </summary>
        /// <param name="strFileName">file path</param>
        public void SaveCanvas(string strFileName) {
            using (FileStream fs = new FileStream(strFileName, FileMode.Create, FileAccess.Write)) {
                SaveCanvas(fs);
            }

            _modified = false;
        }

        /// <summary>
        /// Save the class content in the canvas to the data flow
        /// </summary>
        /// <param name="s">data stream object</param>
        public void SaveCanvas(Stream s) {
            Dictionary<STNodeOption, long> dic = new Dictionary<STNodeOption, long>();
            s.Write(new byte[] { (byte)'S', (byte)'T', (byte)'N', (byte)'D' }, 0, 4); //file head
            s.WriteByte(1);                                                           //ver

            using (GZipStream gs = new GZipStream(s, CompressionMode.Compress)) {
                gs.Write(BitConverter.GetBytes(_CanvasOffsetX), 0, 4);
                gs.Write(BitConverter.GetBytes(_CanvasOffsetY), 0, 4);
                gs.Write(BitConverter.GetBytes(_CanvasScale), 0, 4);
                gs.Write(BitConverter.GetBytes(_Nodes.Count), 0, 4);

                foreach (STNode node in _Nodes) {
                    try {
                        byte[] byNode = node.GetSaveData();
                        gs.Write(BitConverter.GetBytes(byNode.Length), 0, 4);
                        gs.Write(byNode, 0, byNode.Length);

                        foreach (STNodeOption op in node.InputOptions)
                            if (!dic.ContainsKey(op))
                                dic.Add(op, dic.Count);

                        foreach (STNodeOption op in node.OutputOptions)
                            if (!dic.ContainsKey(op))
                                dic.Add(op, dic.Count);
                    } catch (Exception ex) {
                        throw new Exception("Error getting node data - " + node.Title, ex);
                    }
                }

                gs.Write(BitConverter.GetBytes(m_dic_gp_info.Count), 0, 4);

                foreach (var v in m_dic_gp_info.Values)
                    gs.Write(BitConverter.GetBytes(((dic[v.Output] << 32) | dic[v.Input])), 0, 8);
            }
        }

        /// <summary>
        /// Get the content binary data in the canvas
        /// </summary>
        /// <returns>binary data</returns>
        public byte[] GetCanvasData() {
            using (MemoryStream ms = new MemoryStream()) {
                SaveCanvas(ms);
                return ms.ToArray();
            }
        }

        /// <summary>
        /// load assembly
        /// </summary>
        /// <param name="strFiles">assembly collection</param>
        /// <returns>The number of files of type STNode exists</returns>
        public int LoadAssembly(string[] strFiles) {
            int nCount = 0;

            foreach (var v in strFiles) {
                try {
                    if (LoadAssembly(v))
                        nCount++;
                } catch { }
            }

            return nCount;
        }

        /// <summary>
        /// load assembly
        /// </summary>
        /// <param name="strFile">Specify the file to be loaded</param>
        /// <returns>Is it loaded successfully?</returns>
        public bool LoadAssembly(string strFile) {
            bool bFound = false;
            Assembly asm = Assembly.LoadFrom(strFile);

            if (asm == null)
                return false;

            foreach (var t in asm.GetTypes()) {
                if (t.IsAbstract)
                    continue;

                if (t == m_type_node || t.IsSubclassOf(m_type_node)) {
                    if (m_dic_type.ContainsKey(t.GUID.ToString()))
                        continue;

                    m_dic_type.Add(t.GUID.ToString(), t);
                    bFound = true;
                }
            }

            return bFound;
        }

        /// <summary>
        /// Get the Node type loaded in the current editor
        /// </summary>
        /// <returns>collection of types</returns>
        public Type[] GetTypes() {
            return m_dic_type.Values.ToArray();
        }

        /// <summary>
        /// load data from file
        /// Note: This method does not clear the data in the canvas, but overlays the data
        /// </summary>
        /// <param name="strFileName">file path</param>
        public void LoadCanvas(string strFileName) {
            using (MemoryStream ms = new MemoryStream(File.ReadAllBytes(strFileName)))
                LoadCanvas(ms);

            _modified = false;
        }

        /// <summary>
        /// Load data from binary
        /// Note: This method does not clear the data in the canvas, but overlays the data
        /// </summary>
        /// <param name="byData">binary data</param>
        public void LoadCanvas(byte[] byData) {
            using (MemoryStream ms = new MemoryStream(byData))
                LoadCanvas(ms);
        }

        /// <summary>
        /// Load data from data stream
        /// Note: This method does not clear the data in the canvas, but overlays the data
        /// </summary>
        /// <param name="s">data stream object</param>
        public void LoadCanvas(Stream s) {
            byte[] byLen = new byte[4];
            s.Read(byLen, 0, 4);

            if (BitConverter.ToInt32(byLen, 0)!= BitConverter.ToInt32(new byte[] { (byte)'S', (byte)'T', (byte)'N', (byte)'D' }, 0))
                throw new InvalidDataException("unrecognized file type");

            if (s.ReadByte() != 1)
                throw new InvalidDataException("Unrecognized file version number");

            m_minimap.SkipProcessing = true;

            using (GZipStream gs = new GZipStream(s, CompressionMode.Decompress)) {
                gs.Read(byLen, 0, 4);
                float x = BitConverter.ToSingle(byLen, 0);
                gs.Read(byLen, 0, 4);
                float y = BitConverter.ToSingle(byLen, 0);
                gs.Read(byLen, 0, 4);
                float scale = BitConverter.ToSingle(byLen, 0);
                gs.Read(byLen, 0, 4);
                int nCount = BitConverter.ToInt32(byLen, 0);
                Dictionary<long, STNodeOption> dic = new Dictionary<long, STNodeOption>();
                HashSet<STNodeOption> hs = new HashSet<STNodeOption>();
                byte[] byData = null;

                for (int i = 0; i < nCount; i++) {
                    gs.Read(byLen, 0, byLen.Length);
                    int nLen = BitConverter.ToInt32(byLen, 0);
                    byData = new byte[nLen];
                    gs.Read(byData, 0, byData.Length);
                    STNode node = null;

                    try {
                        node = GetNodeFromData(byData);
                    } catch (Exception ex) {
                        m_minimap.SkipProcessing = false;
                        throw new Exception("An error occurred while loading nodes Possible data corruption\r\n" + ex.Message, ex);
                    }

                    try {
                        _Nodes.Add(node);
                    } catch (Exception ex) {
                        m_minimap.SkipProcessing = false;
                        throw new Exception("error loading node - " + node.Title, ex);
                    }

                    foreach (STNodeOption op in node.InputOptions)
                        if (hs.Add(op))
                            dic.Add(dic.Count, op);

                    foreach (STNodeOption op in node.OutputOptions)
                        if (hs.Add(op))
                            dic.Add(dic.Count, op);
                }

                gs.Read(byLen, 0, 4);
                nCount = BitConverter.ToInt32(byLen, 0);
                byData = new byte[8];

                for (int i = 0; i < nCount; i++) {
                    gs.Read(byData, 0, byData.Length);
                    long id = BitConverter.ToInt64(byData, 0);
                    long op_out = id >> 32;
                    long op_in = (int)id;
                    dic[op_out].ConnectOption(dic[op_in]);
                }

                ScaleCanvas(scale, 0, 0);
                MoveCanvas(x, y, false, CanvasMoveArgs.All);
            }

            BuildBounds();

            foreach (STNode node in _Nodes)
                node.OnEditorLoadCompleted();

            m_minimap.SkipProcessing = false;
            m_minimap.ProcessNodeChanges();
        }

        private STNode GetNodeFromData(byte[] byData) {
            int nIndex = 0;
            string strModel = Encoding.UTF8.GetString(byData, nIndex + 1, byData[nIndex]);
            nIndex += byData[nIndex] + 1;
            string strGUID = Encoding.UTF8.GetString(byData, nIndex + 1, byData[nIndex]);
            nIndex += byData[nIndex] + 1;
            Dictionary<string, byte[]> dic = new Dictionary<string, byte[]>();

            while (nIndex < byData.Length) {
                int nLen = BitConverter.ToInt32(byData, nIndex);
                nIndex += 4;
                string strKey = Encoding.UTF8.GetString(byData, nIndex, nLen);
                nIndex += nLen;
                nLen = BitConverter.ToInt32(byData, nIndex);
                nIndex += 4;
                byte[] byValue = new byte[nLen];
                Array.Copy(byData, nIndex, byValue, 0, nLen);
                nIndex += nLen;
                dic.Add(strKey, byValue);
            }

            string[] modelSplit = strModel.Split('|');
            strGUID = PatchOldData(strGUID, modelSplit[1]);

            if (!m_dic_type.ContainsKey(strGUID))
                throw new TypeLoadException("Could not find type {" + modelSplit[1] + "} In the assembly Make sure the assembly {" + modelSplit[0] + "} Has been correctly loaded by the editor The assembly can be loaded by calling LoadAssembly()");

            Type t = m_dic_type[strGUID];
            STNode node = (STNode)Activator.CreateInstance(t);
            node.OnLoadNode(dic);
            return node;
        }

        /// <summary>
        /// Show tooltip in canvas
        /// </summary>
        /// <param name="strText">information to display</param>
        /// <param name="foreColor">information foreground color</param>
        /// <param name="backColor">information background color</param>
        /// <param name="nTime">message duration</param>
        /// <param name="al">where the information should be displayed</param>
        /// <param name="bRedraw">Whether to redraw immediately</param>
        public void ShowAlert(string strText, Color foreColor, Color backColor, int nTime = 1000, AlertLocation al = AlertLocation.RightBottom, bool bRedraw = true) {
            m_str_alert = strText;
            m_forecolor_alert = foreColor;
            m_backcolor_alert = backColor;
            m_time_alert = nTime;
            m_dt_alert = DateTime.Now;
            m_alpha_alert = 255;
            m_al = al;

            if (bRedraw)
                Invalidate();
        }

        /// <summary>
        /// Set the active node in the canvas
        /// </summary>
        /// <param name="node">The node that needs to be set active</param>
        /// <returns>Active node before setting</returns>
        public STNode SetActiveNode(STNode node) {
            if (node != null && !_Nodes.Contains(node))
                return _ActiveNode;

            STNode ret = _ActiveNode;

            if (_ActiveNode != node) { //reset active selection node
                if (node != null) {
                    _Nodes.MoveToEnd(node);
                    node.IsActive = true;
                    node.SetSelected(true, false);
                    node.OnGotFocus(EventArgs.Empty);
                }

                if (_ActiveNode != null) {
                    _ActiveNode.IsActive /*= _ActiveNode.IsSelected*/ = false;
                    _ActiveNode.OnLostFocus(EventArgs.Empty);
                }

                _ActiveNode = node;
                Invalidate();
                OnActiveChanged(EventArgs.Empty);
                //OnSelectedChanged(EventArgs.Empty);
            }

            return ret;
        }

        /// <summary>
        /// Add a selected node to the canvas
        /// </summary>
        /// <param name="node">node to be selected</param>
        /// <returns>Is it added successfully?</returns>
        public bool AddSelectedNode(STNode node) {
            if (!_Nodes.Contains(node))
                return false;

            _modified = true;
            bool b = !node.IsSelected;
            node.IsSelected = true;

            lock (m_hs_node_selected)
                return m_hs_node_selected.Add(node) || b;
        }

        /// <summary>
        /// Remove a selected node to the canvas
        /// </summary>
        /// <param name="node">node to be removed</param>
        /// <returns>Is the removal successful?</returns>
        public bool RemoveSelectedNode(STNode node) {
            if (!_Nodes.Contains(node))
                return false;

            _modified = true;
            bool b = node.IsSelected;
            node.IsSelected = false;

            lock (m_hs_node_selected)
                return m_hs_node_selected.Remove(node) || b;
        }

        /// <summary>
        /// Add default datatype colors to editor
        /// </summary>
        /// <param name="t">type of data</param>
        /// <param name="clr">corresponding color</param>
        /// <param name="bReplace">Whether to replace the color if it already exists</param>
        /// <returns>The color after being set</returns>
        public Color SetTypeColor(Type t, Color clr, bool bReplace = false) {
            if (_TypeColor.ContainsKey(t)) {
                if (bReplace)
                    _TypeColor[t] = clr;
            } else {
                _TypeColor.Add(t, clr);
            }

            return _TypeColor[t];
        }

        public void ClearNodes()
        {
            m_minimap.SkipProcessing = true;
            _Nodes.Clear();
            m_minimap.SkipProcessing = false;
            m_minimap.ProcessNodeChanges();
        }

        #endregion public
    }
}
