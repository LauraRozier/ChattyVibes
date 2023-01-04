using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Drawing;
using System.Runtime.InteropServices;
using System.ComponentModel;

namespace ST.Library.UI.NodeEditor
{
    public class STNodeEditorPannel : Control
    {
        private bool _LeftLayout = true;
        /// <summary>
        /// Get or set whether it is the left layout
        /// </summary>
        [Description("Get or set whether it is the left layout"), DefaultValue(true)]
        public bool LeftLayout {
            get { return _LeftLayout; }
            set {
                if (value == _LeftLayout)
                    return;

                _LeftLayout = value;
                SetLocation();
                Invalidate();
            }
        }

        private Color _SplitLineColor = Color.Black;
        /// <summary>
        /// Get or set the dividing line color
        /// </summary>
        [Description("Get or set the dividing line color"), DefaultValue(typeof(Color), "Black")]
        public Color SplitLineColor {
            get { return _SplitLineColor; }
            set { _SplitLineColor = value; }
        }

        private Color _HandleLineColor = Color.Gray;
        /// <summary>
        /// Get or set the dividing line handle color
        /// </summary>
        [Description("Get or set the dividing line handle color"), DefaultValue(typeof(Color), "Gray")]
        public Color HandleLineColor {
            get { return _HandleLineColor; }
            set { _HandleLineColor = value; }
        }

        private bool _ShowScale = true;
        /// <summary>
        /// Get or set the display ratio when the editor is zoomed
        /// </summary>
        [Description("Get or set the display ratio when the editor is zoomed"), DefaultValue(true)]
        public bool ShowScale {
            get { return _ShowScale; }
            set { _ShowScale = value; }
        }

        private bool _ShowConnectionStatus = true;
        /// <summary>
        /// Get or set whether to display the status when the node is connected
        /// </summary>
        [Description("Get or set whether to display the status when the node is connected"), DefaultValue(true)]
        public bool ShowConnectionStatus {
            get { return _ShowConnectionStatus; }
            set { _ShowConnectionStatus = value; }
        }

        private int _X;
        /// <summary>
        /// Get or set the horizontal width of the dividing line
        /// </summary>
        [Description("Get or set the horizontal width of the dividing line"), DefaultValue(201)]
        public int X {
            get { return _X; }
            set {
                if (value < 122)
                    value = 122;
                else if (value > Width - 122)
                    value = Width - 122;

                if (_X == value)
                    return;

                _X = value;
                SetLocation();
            }
        }

        private int _Y;
        /// <summary>
        /// Get or set the vertical height of the dividing line
        /// </summary>
        [Description("Get or set the vertical height of the dividing line")]
        public int Y {
            get { return _Y; }
            set {
                if (value < 122)
                    value = 122;
                else if (value > Height - 122)
                    value = Height - 122;

                if (_Y == value)
                    return;

                _Y = value;
                SetLocation();
            }
        }

        /// <summary>
        /// Get the STNodeEditor in the panel
        /// </summary>
        [Description("Get the STNodeEditor in the panel"), Browsable(false)]
        public STNodeEditor Editor {
            get { return m_editor; }
        }

        /// <summary>
        /// Get the STNodeTreeView in the panel
        /// </summary>
        [Description("Get the STNodeTreeView in the panel"), Browsable(false)]
        public STNodeTreeView TreeView {
            get { return m_tree; }
        }

        /// <summary>
        /// Get the STNodePropertyGrid in the panel
        /// </summary>
        [Description("Get the STNodePropertyGrid in the panel"), Browsable(false)]
        public STNodePropertyGrid PropertyGrid {
            get { return m_grid; }
        }

        private bool m_is_mx;
        private bool m_is_my;
        private readonly Pen m_pen;

        private bool m_nInited;
        private readonly Dictionary<ConnectionStatus, string> m_dic_status_key = new Dictionary<ConnectionStatus, string>();

        private readonly STNodeEditor m_editor;
        private readonly STNodeTreeView m_tree;
        private readonly STNodePropertyGrid m_grid;

        public override Size MinimumSize {
            get { return base.MinimumSize; }
            set {
                value = new Size(250, 250);
                base.MinimumSize = value;
            }
        }

        [DllImport("user32.dll")]
        private static extern bool MoveWindow(IntPtr hWnd, int x, int y, int w, int h, bool bRedraw);

        public STNodeEditorPannel() {
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.ResizeRedraw, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);

            m_editor = new STNodeEditor();
            m_tree = new STNodeTreeView();
            m_grid = new STNodePropertyGrid { Text = "NodeProperty" };
            Controls.Add(m_editor);
            Controls.Add(m_tree);
            Controls.Add(m_grid);
            Size = new Size(500, 500);
            MinimumSize = new Size(250, 250);
            BackColor = Color.FromArgb(255, 34, 34, 34);

            m_pen = new Pen(BackColor, 3);

            Type t = typeof(ConnectionStatus);

            foreach (var f in t.GetFields()) {
                if (!f.FieldType.IsEnum)
                    continue;

                foreach (var a in f.GetCustomAttributes(true)) {
                    if (!(a is DescriptionAttribute))
                        continue;

                    m_dic_status_key.Add((ConnectionStatus)f.GetValue(f), ((DescriptionAttribute)a).Description);
                }
            }

            m_editor.ActiveChanged += (s, e) => m_grid.SetNode(m_editor.ActiveNode);
            m_editor.CanvasScaled += (s, e) => {
                if (_ShowScale)
                    m_editor.ShowAlert(m_editor.CanvasScale.ToString("F2"), Color.White, Color.FromArgb(127, 255, 255, 0));
            };
            m_editor.OptionConnected += (s, e) => {
                if (_ShowConnectionStatus)
                    m_editor.ShowAlert(m_dic_status_key[e.Status], Color.White, e.Status == ConnectionStatus.Connected ? Color.FromArgb(125, Color.Lime) : Color.FromArgb(125, Color.Red));
            };
        }

        protected override void OnResize(EventArgs e) {
            base.OnResize(e);

            if (!m_nInited) {
                _Y = Height / 2;

                if (_LeftLayout)
                    _X = 201;
                else
                    _X = Width - 202;

                m_nInited = true;
                SetLocation();
                return;
            }

            SetLocation();
        }

        protected override void SetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified) {
            if (width < 250)
                width = 250;

            if (height < 250)
                height = 250;

            base.SetBoundsCore(x, y, width, height, specified);
        }

        protected override void OnPaint(PaintEventArgs e) {
            base.OnPaint(e);
            Graphics g = e.Graphics;
            m_pen.Width = 3;
            m_pen.Color = _SplitLineColor;
            g.DrawLine(m_pen, _X, 0, _X, Height);
            int nX;

            if (_LeftLayout) {
                g.DrawLine(m_pen, 0, _Y, _X - 1, _Y);
                nX = _X / 2;
            } else {
                g.DrawLine(m_pen, _X + 2, _Y, Width, _Y);
                nX = _X + (Width - _X) / 2;
            }

            m_pen.Width = 1;
            _HandleLineColor = Color.Gray;
            m_pen.Color = _HandleLineColor;
            g.DrawLine(m_pen, _X, _Y - 10, _X, _Y + 10);
            g.DrawLine(m_pen, nX - 10, _Y, nX + 10, _Y);
        }

        private void SetLocation() {
            if (_LeftLayout) {
                //m_tree.Location = Point.Empty;
                //m_tree.Size = new Size(m_sx - 1, m_sy - 1);
                MoveWindow(m_tree.Handle, 0, 0, _X - 1, _Y - 1, false);

                //m_grid.Location = new Point(0, m_sy + 2);
                //m_grid.Size = new Size(m_sx - 1, Height - m_sy - 2);
                MoveWindow(m_grid.Handle, 0, _Y + 2, _X - 1, Height - _Y - 2, false);

                //m_editor.Location = new Point(m_sx + 2, 0);
                //m_editor.Size = new Size(Width - m_sx - 2, Height);
                MoveWindow(m_editor.Handle, _X + 2, 0, Width - _X - 2, Height, false);
            } else {
                MoveWindow(m_editor.Handle, 0, 0, _X - 1, Height, false);
                MoveWindow(m_tree.Handle, _X + 2, 0, Width - _X - 2, _Y - 1, false);
                MoveWindow(m_grid.Handle, _X + 2, _Y + 2, Width - _X - 2, Height - _Y - 2, false);
            }
        }

        protected override void OnMouseDown(MouseEventArgs e) {
            base.OnMouseDown(e);
            m_is_mx = m_is_my = false;

            if (Cursor == Cursors.VSplit) {
                m_is_mx = true;
            } else if (Cursor == Cursors.HSplit) {
                m_is_my = true;
            }
        }

        protected override void OnMouseMove(MouseEventArgs e) {
            base.OnMouseMove(e);

            if (e.Button == MouseButtons.Left) {
                int nw = 122;// (int)(Width * 0.1f);
                int nh = 122;// (int)(Height * 0.1f);

                if (m_is_mx) {
                    _X = e.X;// -m_pt_down.X;

                    if (_X < nw)
                        _X = nw;
                    else if (_X + nw > Width)
                        _X = Width - nw;
                } else if (m_is_my) {
                    _Y = e.Y;
                    if (_Y < nh)
                        _Y = nh;
                    else if (_Y + nh > Height)
                        _Y = Height - nh;
                }

                //m_rx = Width - m_sx;// (float)m_sx / Width;
                //m_fh = (float)m_sy / Height;
                SetLocation();
                Invalidate();
                return;
            }

            if (Math.Abs(e.X - _X) < 2)
                Cursor = Cursors.VSplit;
            else if (Math.Abs(e.Y - _Y) < 2)
                Cursor = Cursors.HSplit;
            else
                Cursor = Cursors.Arrow;
        }

        protected override void OnMouseLeave(EventArgs e) {
            base.OnMouseLeave(e);
            m_is_mx = m_is_my = false;
            Cursor = Cursors.Arrow;
        }

        /// <summary>
        /// Add an STNode to the tree control
        /// </summary>
        /// <param name="stNodeType">STNode type</param>
        /// <returns>Is it added successfully?</returns>
        public bool AddSTNode(Type stNodeType) {
            return m_tree.AddNode(stNodeType);
        }

        /// <summary>
        /// Load STNode from assembly
        /// </summary>
        /// <param name="strFileName">Assembly path</param>
        /// <returns>The number of successful additions</returns>
        public int LoadAssembly(string strFileName) {
            m_editor.LoadAssembly(strFileName);
            return m_tree.LoadAssembly(strFileName);
        }

        /// <summary>
        /// Sets the text for the editor to display connection status
        /// </summary>
        /// <param name="status">Connection Status</param>
        /// <param name="strText">Corresponding display text</param>
        /// <returns>Old text</returns>
        public string SetConnectionStatusText(ConnectionStatus status, string strText) {
            if (m_dic_status_key.ContainsKey(status)) {
                string strOld = m_dic_status_key[status];
                m_dic_status_key[status] = strText;
                return strOld;
            }

            m_dic_status_key.Add(status, strText);
            return strText;
        }
    }
}
