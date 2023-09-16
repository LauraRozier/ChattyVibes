using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
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
 * create: 2021-12-08
 * modify: 2021-03-02
 * Author: Crystal_lz
 * blog: http://st233.com
 * Gitee: https://gitee.com/DebugST
 * Github: https://github.com/DebugST
 */
namespace ST.Library.UI.NodeEditor
{
    public abstract class STNode
    {
        private STNodeEditor _Owner;
        /// <summary>
        /// Get the current Node owner
        /// </summary>
        public STNodeEditor Owner {
            get { return _Owner; }
            internal set {
                if (value == _Owner)
                    return;

                if (_Owner != null) {
                    foreach (STNodeOption op in _InputOptions.ToArray())
                        op.DisConnectionAll();

                    foreach (STNodeOption op in _OutputOptions.ToArray())
                        op.DisConnectionAll();
                }

                _Owner = value;

                if (!_AutoSize)
                    SetOptionsLocation();

                BuildSize(true, true, false);
                OnOwnerChanged();
            }
        }

        private bool _IsSelected;
        /// <summary>
        /// Get or set whether Node is selected
        /// </summary>
        public bool IsSelected {
            get { return _IsSelected; }
            set {
                if (value == _IsSelected)
                    return;

                _IsSelected = value;
                Invalidate();
                OnSelectedChanged();

                _Owner?.OnSelectedChanged(EventArgs.Empty);
            }
        }

        private bool _IsActive;
        /// <summary>
        /// Get whether the Node is active
        /// </summary>
        public bool IsActive {
            get { return _IsActive; }
            internal set {
                if (value == _IsActive)
                    return;

                _IsActive = value;
                OnActiveChanged();
            }
        }

        private Color _TitleColor;
        /// <summary>
        /// Get or set title background color
        /// </summary>
        public Color TitleColor {
            get { return _TitleColor; }
            protected set {
                _TitleColor = value;
                Invalidate(new Rectangle(0, 0, _Width, _TitleHeight));
            }
        }

        private Color _MarkColor;
        /// <summary>
        /// Get or set the background color of the marker information
        /// </summary>
        public Color MarkColor {
            get { return _MarkColor; }
            protected set {
                _MarkColor = value;
                Invalidate(_MarkRectangle);
            }
        }

        private Color _ForeColor = Color.White;
        /// <summary>
        /// Get or set the current Node foreground color
        /// </summary>
        public Color ForeColor {
            get { return _ForeColor; }
            protected set {
                _ForeColor = value;
                Invalidate();
            }
        }

        private Color _BackColor;
        /// <summary>
        /// Get or set the current Node background color
        /// </summary>
        public Color BackColor {
            get { return _BackColor; }
            protected set {
                _BackColor = value;
                Invalidate();
            }
        }

        private string _Title;
        /// <summary>
        /// Get or set the Node title
        /// </summary>
        public string Title {
            get { return _Title; }
            protected set {
                _Title = value;

                if (_AutoSize)
                    BuildSize(true, true, true);
                //this.Invalidate(this.TitleRectangle);
            }
        }

        private string _Mark;
        /// <summary>
        /// Get or set Node flag information
        /// </summary>
        public string Mark {
            get { return _Mark; }
            set {
                _Mark = value;

                if (value == null)
                    _MarkLines = null;
                else
                    _MarkLines = (from s in value.Split('\n') select s.Trim()).ToArray();

                Invalidate(new Rectangle(-5, -5, _MarkRectangle.Width + 10, _MarkRectangle.Height + 10));
            }
        }

        private string[] _MarkLines;//Store row data separately without splitting each time in drawing
        /// <summary>
        /// Get Node tag information line data
        /// </summary>
        public string[] MarkLines {
            get { return _MarkLines; }
        }

        private int _Left;
        /// <summary>
        /// Get or set the left coordinate of Node
        /// </summary>
        public int Left {
            get { return _Left; }
            set {
                if (_LockLocation || value == _Left)
                    return;

                _Left = value;
                SetOptionsLocation();
                BuildSize(false, true, false);
                OnMove(EventArgs.Empty);

                if (_Owner != null) {
                    _Owner.BuildLinePath();
                    _Owner.BuildBounds();
                }
            }
        }

        private int _Top;
        /// <summary>
        /// Get or set the upper coordinates of Node
        /// </summary>
        public int Top {
            get { return _Top; }
            set {
                if (_LockLocation || value == _Top)
                    return;

                _Top = value;
                SetOptionsLocation();
                BuildSize(false, true, false);
                OnMove(EventArgs.Empty);

                if (_Owner != null) {
                    _Owner.BuildLinePath();
                    _Owner.BuildBounds();
                }
            }
        }

        private int _Width = 100;
        /// <summary>
        /// Get or set the Node width. This value cannot be set when AutoSize is set
        /// </summary>
        public int Width {
            get { return _Width; }
            protected set {
                if (value < 50)
                    return;

                if (_AutoSize || value == _Width)
                    return;

                _Width = value;
                SetOptionsLocation();
                BuildSize(false, true, false);
                OnResize(EventArgs.Empty);

                if (_Owner != null) {
                    _Owner.BuildLinePath();
                    _Owner.BuildBounds();
                }

                Invalidate();
            }
        }

        private int _Height = 40;
        /// <summary>
        /// Get or set the height of Node. This value cannot be set when AutoSize is set
        /// </summary>
        public int Height {
            get { return _Height; }
            protected set {
                if (value < 40)
                    return;

                if (_AutoSize || value == _Height)
                    return;

                _Height = value;
                SetOptionsLocation();
                BuildSize(false, true, false);
                OnResize(EventArgs.Empty);

                if (_Owner != null) {
                    _Owner.BuildLinePath();
                    _Owner.BuildBounds();
                }

                Invalidate();
            }
        }

        private int _ItemHeight = 20;
        /// <summary>
        /// Get or set the height of each option of Node
        /// </summary>
        public int ItemHeight {
            get { return _ItemHeight; }
            protected set {
                if (value < 16)
                    value = 16;

                if (value > 200)
                    value = 200;

                if (value == _ItemHeight)
                    return;

                _ItemHeight = value;

                if (_AutoSize) {
                    BuildSize(true, false, true);
                } else {
                    SetOptionsLocation();
                    _Owner?.Invalidate();
                }
            }
        }

        private bool _AutoSize = true;
        /// <summary>
        /// Get or set whether Node automatically calculates width and height
        /// </summary>
        public bool AutoSize {
            get { return _AutoSize; }
            protected set { _AutoSize = value; }
        }
        /// <summary>
        /// Get the coordinates of the right edge of Node
        /// </summary>
        public int Right {
            get { return _Left + _Width; }
        }
        /// <summary>
        /// Get the coordinates of the lower edge of Node
        /// </summary>
        public int Bottom {
            get { return _Top + _Height; }
        }
        /// <summary>
        /// Get the rectangular area of Node
        /// </summary>
        public Rectangle Rectangle {
            get {
                return new Rectangle(_Left, _Top, _Width, _Height);
            }
        }
        /// <summary>
        /// Get the title rectangle area of Node
        /// </summary>
        public Rectangle TitleRectangle {
            get {
                return new Rectangle(_Left, _Top, _Width, _TitleHeight);
            }
        }

        private Rectangle _MarkRectangle;
        /// <summary>
        /// Get the Node to mark the rectangular area
        /// </summary>
        public Rectangle MarkRectangle {
            get { return _MarkRectangle; }
        }

        private int _TitleHeight = 20;
        /// <summary>
        /// Get or set the height of the Node title
        /// </summary>
        public int TitleHeight {
            get { return _TitleHeight; }
            protected set { _TitleHeight = value; }
        }

        private readonly STNodeOptionCollection _InputOptions;
        /// <summary>
        /// Get a collection of input options
        /// </summary>
        public STNodeOptionCollection InputOptions {
            get { return _InputOptions; }
        }
        /// <summary>
        /// Get the number of input option sets
        /// </summary>
        public int InputOptionsCount { get { return _InputOptions.Count; } }

        private readonly STNodeOptionCollection _OutputOptions;
        /// <summary>
        /// Get output options
        /// </summary>
        protected internal STNodeOptionCollection OutputOptions {
            get { return _OutputOptions; }
        }
        /// <summary>
        /// Get the number of output options
        /// </summary>
        public int OutputOptionsCount { get { return _OutputOptions.Count; } }

        private readonly STNodeControlCollection _Controls;
        /// <summary>
        /// Get the collection of controls contained in Node
        /// </summary>
        protected STNodeControlCollection Controls {
            get { return _Controls; }
        }
        /// <summary>
        /// Get the number of controls contained in Node
        /// </summary>
        public int ControlsCount { get { return _Controls.Count; } }
        /// <summary>
        /// Get Node coordinate position
        /// </summary>
        public Point Location {
            get { return new Point(_Left, _Top); }
            set {
                Left = value.X;
                Top = value.Y;
            }
        }
        /// <summary>
        /// Get Node size
        /// </summary>
        public Size Size {
            get { return new Size(_Width, _Height); }
            set {
                Width = value.Width;
                Height = value.Height;
            }
        }

        private Font _Font;
        /// <summary>
        /// Get or set Node font
        /// </summary>
        protected Font Font {
            get { return _Font; }
            set {
                if (value == _Font)
                    return;

                _Font.Dispose();
                _Font = value;
            }
        }

        private bool _LockOption;
        /// <summary>
        /// Get or set whether to lock the Option option. After locking, it will no longer accept connections
        /// </summary>
        public bool LockOption {
            get { return _LockOption; }
            set {
                _LockOption = value;
                Invalidate(new Rectangle(0, 0, _Width, _TitleHeight));
            }
        }

        private bool _LockLocation;
        /// <summary>
        /// Get or set whether to lock the Node position. After locking, it cannot be moved
        /// </summary>
        public bool LockLocation {
            get { return _LockLocation; }
            set {
                _LockLocation = value;
                Invalidate(new Rectangle(0, 0, _Width, _TitleHeight));
            }
        }

        private ContextMenuStrip _ContextMenuStrip;
        /// <summary>
        /// Get or set the current Node context menu
        /// </summary>
        public ContextMenuStrip ContextMenuStrip {
            get { return _ContextMenuStrip; }
            set { _ContextMenuStrip = value; }
        }

        private object _Tag;
        /// <summary>
        /// Get or set user-defined saved data
        /// </summary>
        public object Tag {
            get { return _Tag; }
            set { _Tag = value; }
        }

        private Guid _Guid;
        /// <summary>
        /// Get a globally unique identifier
        /// </summary>
        public Guid Guid {
            get { return _Guid; }
        }

        private bool _LetGetOptions = false;
        /// <summary>
        /// Get or set whether to allow external access to STNodeOption
        /// </summary>
        public bool LetGetOptions {
            get { return _LetGetOptions; }
            protected set { _LetGetOptions = value; }
        }

        private static Point m_static_pt_init = new Point(10, 10);

        public STNode() {
            _Title = "Untitled";
            _MarkRectangle.Height = _Height;
            _Left = _MarkRectangle.X = m_static_pt_init.X;
            _Top = m_static_pt_init.Y;
            _MarkRectangle.Y = _Top - 30;
            _InputOptions = new STNodeOptionCollection(this, true);
            _OutputOptions = new STNodeOptionCollection(this, false);
            _Controls = new STNodeControlCollection(this);
            _BackColor = Color.FromArgb(200, 64, 64, 64);
            _TitleColor = Color.FromArgb(200, Color.DodgerBlue);
            _MarkColor = Color.FromArgb(200, Color.Brown);
            _Font = new Font("courier new", 8.25f);

            m_sf = new StringFormat
            {
                Alignment = StringAlignment.Near,
                LineAlignment = StringAlignment.Center,
                FormatFlags = StringFormatFlags.NoWrap
            };
            m_sf.SetTabStops(0, new float[] { 40 });
            m_static_pt_init.X += 10;
            m_static_pt_init.Y += 10;
            _Guid = Guid.NewGuid();
            OnCreate();
        }

        //private int m_nItemHeight = 30;
        protected StringFormat m_sf;
        /// <summary>
        /// Active controls in the current Node
        /// </summary>
        protected STNodeControl m_ctrl_active;
        /// <summary>
        /// The hovering control in the current Node
        /// </summary>
        protected STNodeControl m_ctrl_hover;
        /// <summary>
        /// The control under the mouse point in the current Node
        /// </summary>
        protected STNodeControl m_ctrl_down;

        protected internal void BuildSize(bool bBuildNode, bool bBuildMark, bool bRedraw) {
            if (_Owner == null)
                return;

            using (Graphics g = _Owner.CreateGraphics()) {
                if (_AutoSize && bBuildNode) {
                    Size sz = GetDefaultNodeSize(g);

                    if (_Width != sz.Width || _Height != sz.Height) {
                        _Width = sz.Width;
                        _Height = sz.Height;
                        SetOptionsLocation();
                        OnResize(EventArgs.Empty);
                    }
                }

                if (bBuildMark && !string.IsNullOrEmpty(_Mark))
                    _MarkRectangle = OnBuildMarkRectangle(g);
            }

            if (bRedraw)
                _Owner.Invalidate();
        }

        internal Dictionary<string, byte[]> OnSaveNode() {
            Dictionary<string, byte[]> dic = new Dictionary<string, byte[]>
            {
                { "Guid", _Guid.ToByteArray() },
                { "Left", BitConverter.GetBytes(_Left) },
                { "Top", BitConverter.GetBytes(_Top) },
                { "Width", BitConverter.GetBytes(_Width) },
                { "Height", BitConverter.GetBytes(_Height) },
                { "AutoSize", new byte[] { (byte)(_AutoSize ? 1 : 0) } }
            };

            if (_Mark != null)
                dic.Add("Mark", Encoding.UTF8.GetBytes(_Mark));

            dic.Add("LockOption", new byte[] { (byte)(_LockOption ? 1 : 0) });
            dic.Add("LockLocation", new byte[] { (byte)(_LockLocation ? 1 : 0) });
            Type t = GetType();

            foreach (var p in t.GetProperties()) {
                var attrs = p.GetCustomAttributes(true);

                foreach (var a in attrs) {
                    if (!(a is STNodePropertyAttribute))
                        continue;

                    var attr = a as STNodePropertyAttribute;
                    object obj = Activator.CreateInstance(attr.DescriptorType);

                    if (!(obj is STNodePropertyDescriptor))
                        throw new InvalidOperationException("[STNodePropertyAttribute.Type] parameter value must be [STNodePropertyDescriptor] or it's subclass type");

                    var desc = (STNodePropertyDescriptor)Activator.CreateInstance(attr.DescriptorType);
                    desc.Node = this;
                    desc.PropertyInfo = p;
                    byte[] byData = desc.GetBytesFromValue();

                    if (byData == null)
                        continue;

                    dic.Add(p.Name, byData);
                }
            }

            OnSaveNode(dic);
            return dic;
        }

        internal byte[] GetSaveData() {
            List<byte> lst = new List<byte>();
            Type t = GetType();
            byte[] byData = Encoding.UTF8.GetBytes(t.Module.Name + "|" + t.FullName);
            lst.Add((byte)byData.Length);
            lst.AddRange(byData);
            byData = Encoding.UTF8.GetBytes(t.GUID.ToString());
            lst.Add((byte)byData.Length);
            lst.AddRange(byData);

            var dic = OnSaveNode();

            if (dic != null) {
                foreach (var v in dic) {
                    byData = Encoding.UTF8.GetBytes(v.Key);
                    lst.AddRange(BitConverter.GetBytes(byData.Length));
                    lst.AddRange(byData);
                    lst.AddRange(BitConverter.GetBytes(v.Value.Length));
                    lst.AddRange(v.Value);
                }
            }

            return lst.ToArray();
        }

        #region protected
        /// <summary>
        /// Occurs when Node is constructed
        /// </summary>
        protected virtual void OnCreate() { }
        /// <summary>
        /// Draw the entire Node
        /// </summary>
        /// <param name="dt">Drawing tool</param>
        protected internal virtual void OnDrawNode(DrawingTools dt) {
            dt.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;

            //Fill background
            if (_BackColor.A != 0) {
                dt.SolidBrush.Color = _BackColor;
                dt.Graphics.FillRectangle(dt.SolidBrush, _Left, _Top + _TitleHeight, _Width, Height - _TitleHeight);
            }

            OnDrawTitle(dt);
            OnDrawBody(dt);
        }
        /// <summary>
        /// Draw the Node header section
        /// </summary>
        /// <param name="dt">Drawing tool</param>
        protected virtual void OnDrawTitle(DrawingTools dt) {
            m_sf.Alignment = StringAlignment.Center;
            m_sf.LineAlignment = StringAlignment.Center;
            Graphics g = dt.Graphics;
            SolidBrush brush = dt.SolidBrush;

            if (_TitleColor.A != 0) {
                brush.Color = _TitleColor;
                g.FillRectangle(brush, TitleRectangle);
            }

            if (_LockOption) {
                //dt.Pen.Color = this.ForeColor;
                brush.Color = _ForeColor;
                int n = _Top + _TitleHeight / 2 - 5;
                g.FillRectangle(dt.SolidBrush, _Left + 4, n + 0, 2, 4);
                g.FillRectangle(dt.SolidBrush, _Left + 6, n + 0, 2, 2);
                g.FillRectangle(dt.SolidBrush, _Left + 8, n + 0, 2, 4);
                g.FillRectangle(dt.SolidBrush, _Left + 3, n + 4, 8, 6);
                //g.DrawLine(dt.Pen, _Left + 6, n + 5, _Left + 6, n + 7);
                //g.DrawRectangle(dt.Pen, _Left + 3, n + 0, 6, 3);
                //g.DrawRectangle(dt.Pen, _Left + 2, n + 3, 8, 6);
                //g.DrawLine(dt.Pen, _Left + 6, n + 5, _Left + 6, n + 7);

            }

            if (_LockLocation) {
                //dt.Pen.Color = ForeColor;
                brush.Color = _ForeColor;
                int n = _Top + _TitleHeight / 2 - 5;
                g.FillRectangle(brush, Right - 9, n, 4, 4);
                g.FillRectangle(brush, Right - 11, n + 4, 8, 2);
                g.FillRectangle(brush, Right - 8, n + 6, 2, 4);
                //g.DrawLine(dt.Pen, Right - 10, n + 6, Right - 4, n + 6);
                //g.DrawLine(dt.Pen, Right - 10, n, Right - 4, n);
                //g.DrawLine(dt.Pen, Right - 11, n + 6, Right - 3, n + 6);
                //g.DrawLine(dt.Pen, Right - 7, n + 7, Right - 7, n + 9);
            }

            if (!string.IsNullOrEmpty(_Title) && _ForeColor.A != 0) {
                brush.Color = _ForeColor;
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                g.DrawString(_Title, _Font, brush, TitleRectangle, m_sf);
            }
        }
        /// <summary>
        /// 绘制Node主体部分 除去标题部分
        /// </summary>
        /// <param name="dt">Drawing tool</param>
        protected virtual void OnDrawBody(DrawingTools dt) {
            foreach (STNodeOption op in _InputOptions) {
                if (op == STNodeOption.Empty)
                    continue;

                OnDrawOptionDot(dt, op);
                OnDrawOptionText(dt, op);
            }

            foreach (STNodeOption op in _OutputOptions) {
                if (op == STNodeOption.Empty)
                    continue;

                OnDrawOptionDot(dt, op);
                OnDrawOptionText(dt, op);
            }
            if (_Controls.Count != 0) { //Draw child controls
                //Align the coordinate origin with the node
                //dt.Graphics.ResetTransform();
                dt.Graphics.TranslateTransform(this._Left, this._Top + this._TitleHeight);
                Point pt = Point.Empty;         //The current amount of offset required 
                Point pt_last = Point.Empty;    //The coordinates of the last control relative to the node

                foreach (STNodeControl v in this._Controls) {
                    if (!v.Visable) continue;
                    pt.X = v.Left - pt_last.X;
                    pt.Y = v.Top - pt_last.Y;
                    pt_last = v.Location;
                    dt.Graphics.TranslateTransform(pt.X, pt.Y); //Move the origin coordinates to the control position
                    dt.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;
                    v.OnPaint(dt);
                }

                //dt.Graphics.TranslateTransform(-pt_last.X, -pt_last.Y); restore coordinates
                dt.Graphics.TranslateTransform(-this._Left - pt_last.X, -this._Top - this._TitleHeight - pt_last.Y);
            }
        }

        /// <summary>
        /// Draw marker information
        /// </summary>
        /// <param name="dt">Drawing tool</param>
        protected internal virtual void OnDrawMark(DrawingTools dt) {
            if (string.IsNullOrEmpty(_Mark))
                return;

            Graphics g = dt.Graphics;
            SolidBrush brush = dt.SolidBrush;
            m_sf.LineAlignment = StringAlignment.Center;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;
            brush.Color = _MarkColor;
            g.FillRectangle(brush, _MarkRectangle); //fill background color

            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality; //Determine the size required for text drawing
            var sz = g.MeasureString(Mark, Font, _MarkRectangle.Width);
            brush.Color = _ForeColor;

            if (sz.Height > _ItemHeight || sz.Width > _MarkRectangle.Width) { //If it exceeds the drawing area, draw the part
                Rectangle rect = new Rectangle(_MarkRectangle.Left + 2, _MarkRectangle.Top + 2, _MarkRectangle.Width - 20, 16);
                m_sf.Alignment = StringAlignment.Near;
                g.DrawString(_MarkLines[0], _Font, brush, rect, m_sf);
                m_sf.Alignment = StringAlignment.Far;
                rect.Width = _MarkRectangle.Width - 5;
                g.DrawString("+", _Font, brush, rect, m_sf); // + Indicates beyond the drawing area
            } else {
                m_sf.Alignment = StringAlignment.Near;
                g.DrawString(_MarkLines[0].Trim(), _Font, brush, _MarkRectangle, m_sf);
            }
        }

        /// <summary>
        /// Points to draw option lines
        /// </summary>
        /// <param name="dt">Drawing tool</param>
        /// <param name="op">Specific option</param>
        protected virtual void OnDrawOptionDot(DrawingTools dt, STNodeOption op) {
            Graphics g = dt.Graphics;
            Pen pen = dt.Pen;
            SolidBrush brush = dt.SolidBrush;
            var t = typeof(object);

            if (op.DotColor != Color.Transparent) { //set color
                brush.Color = op.DotColor;
            } else {
                if (op.DataType == t)
                    pen.Color = Owner.UnknownTypeColor;
                else
                    brush.Color = Owner.TypeColor.ContainsKey(op.DataType) ? Owner.TypeColor[op.DataType] : Owner.UnknownTypeColor;
            }

            if (op.IsSingle) { //single connection round
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;

                if (op.DataType == t) //unknown type draw otherwise fill
                    g.DrawEllipse(pen, op.DotRectangle.X, op.DotRectangle.Y, op.DotRectangle.Width - 1, op.DotRectangle.Height - 1);
                else
                    g.FillEllipse(brush, op.DotRectangle);
            } else { //Multi-connection Rectangular
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;

                if (op.DataType == t)
                    g.DrawRectangle(pen, op.DotRectangle.X, op.DotRectangle.Y, op.DotRectangle.Width - 1, op.DotRectangle.Height - 1);
                else
                    g.FillRectangle(brush, op.DotRectangle);
            }
        }

        /// <summary>
        /// Draw the text of the option
        /// </summary>
        /// <param name="dt">Drawing tool</param>
        /// <param name="op">Specific option</param>
        protected virtual void OnDrawOptionText(DrawingTools dt, STNodeOption op) {
            Graphics g = dt.Graphics;
            SolidBrush brush = dt.SolidBrush;
            m_sf.Alignment = op.IsInput ? StringAlignment.Near : StringAlignment.Far;
            brush.Color = op.TextColor;
            g.DrawString(op.Text, Font, brush, op.TextRectangle, m_sf);
        }

        /// <summary>
        /// Occurs when calculating the position of the Option connection point
        /// </summary>
        /// <param name="op">Option to be calculated</param>
        /// <param name="pt">Automatically calculated position</param>
        /// <param name="nIndex">The index of the current Option</param>
        /// <returns>New location</returns>
        protected virtual Point OnSetOptionDotLocation(STNodeOption op, Point pt, int nIndex) {
            return pt;
        }

        /// <summary>
        /// Occurs when calculating the Option text area
        /// </summary>
        /// <param name="op">Option to be calculated</param>
        /// <param name="rect">Automatically calculated area</param>
        /// <param name="nIndex">The index of the current Option</param>
        /// <returns>New area</returns>
        protected virtual Rectangle OnSetOptionTextRectangle(STNodeOption op, Rectangle rect, int nIndex) {
            return rect;
        }

        /// <summary>
        /// Get the default size required by the current STNode
        /// The returned size does not limit the drawing area, you can still draw outside this area
        /// But it will not be accepted by STNodeEditor and trigger the corresponding event
        /// </summary>
        /// <param name="g">Drawing panel</param>
        /// <returns>Calculated size</returns>
        protected virtual Size GetDefaultNodeSize(Graphics g) {
            int nInputHeight = 0,
                nOutputHeight = 0;

            foreach (STNodeOption op in _InputOptions)
                nInputHeight += _ItemHeight;

            foreach (STNodeOption op in _OutputOptions)
                nOutputHeight += _ItemHeight;

            int nHeight = _TitleHeight + (nInputHeight > nOutputHeight ? nInputHeight : nOutputHeight);

            SizeF szf_input = SizeF.Empty, szf_output = SizeF.Empty;

            foreach (STNodeOption v in _InputOptions) {
                if (string.IsNullOrEmpty(v.Text))
                    continue;

                SizeF szf = g.MeasureString(v.Text, _Font);

                if (szf.Width > szf_input.Width)
                    szf_input = szf;
            }

            foreach (STNodeOption v in _OutputOptions) {
                if (string.IsNullOrEmpty(v.Text))
                    continue;

                SizeF szf = g.MeasureString(v.Text, _Font);

                if (szf.Width > szf_output.Width)
                    szf_output = szf;
            }

            int nWidth = (int)(szf_input.Width + szf_output.Width + 25);

            if (!string.IsNullOrEmpty(_Title))
                szf_input = g.MeasureString(_Title, _Font);

            if (szf_input.Width + 30 > nWidth)
                nWidth = (int)szf_input.Width + 30;

            return new Size(nWidth, nHeight);
        }

        /// <summary>
        /// Calculate the rectangular area required by the current Mark
        /// The returned size does not limit the drawing area, you can still draw outside this area
        /// But it will not be accepted by STNodeEditor and trigger the corresponding event
        /// </summary>
        /// <param name="g">Drawing panel</param>
        /// <returns>Calculated area</returns>
        protected virtual Rectangle OnBuildMarkRectangle(Graphics g) {
            //if (string.IsNullOrEmpty(this._Mark)) return Rectangle.Empty;
            return new Rectangle(_Left, _Top - 30, _Width, 20);
        }

        /// <summary>
        /// When it needs to be saved, what data does this Node need to save additionally?
        /// Note: No serialization will be performed when saving, and the Node will only be recreated through the empty parameter constructor when restoring
        ///       Then call OnLoadNode() to restore the saved data
        /// </summary>
        /// <param name="dic">Data to save</param>
        protected virtual void OnSaveNode(Dictionary<string, byte[]> dic) { }

        /// <summary>
        /// When restoring the node, the data returned by OnSaveNode() will be passed back to this function
        /// </summary>
        /// <param name="dic">Data at the time of saving</param>
        protected internal virtual void OnLoadNode(Dictionary<string, byte[]> dic) {
            if (dic.ContainsKey("AutoSize"))
                _AutoSize = dic["AutoSize"][0] == 1;

            if (dic.ContainsKey("LockOption")) 
               _LockOption = dic["LockOption"][0] == 1;

            if (dic.ContainsKey("LockLocation"))
                _LockLocation = dic["LockLocation"][0] == 1;

            if (dic.ContainsKey("Guid"))
                _Guid = new Guid(dic["Guid"]);

            if (dic.ContainsKey("Left"))
                _Left = BitConverter.ToInt32(dic["Left"], 0);

            if (dic.ContainsKey("Top"))
                _Top = BitConverter.ToInt32(dic["Top"], 0);

            if (dic.ContainsKey("Width") && !_AutoSize)
                _Width = BitConverter.ToInt32(dic["Width"], 0);

            if (dic.ContainsKey("Height") && !_AutoSize)
                _Height = BitConverter.ToInt32(dic["Height"], 0);

            if (dic.ContainsKey("Mark"))
                Mark = Encoding.UTF8.GetString(dic["Mark"]);

            Type t = GetType();

            foreach (var p in t.GetProperties()) {
                var attrs = p.GetCustomAttributes(true);

                foreach (var a in attrs) {
                    if (!(a is STNodePropertyAttribute))
                        continue;

                    var attr = a as STNodePropertyAttribute;
                    object obj = Activator.CreateInstance(attr.DescriptorType);

                    if (!(obj is STNodePropertyDescriptor))
                        throw new InvalidOperationException("[STNodePropertyAttribute.Type] parameter value must be [STNodePropertyDescriptor] or it's subclass type");

                    var desc = (STNodePropertyDescriptor)Activator.CreateInstance(attr.DescriptorType);
                    desc.Node = this;
                    desc.PropertyInfo = p;

                    try {
                        if (dic.ContainsKey(p.Name))
                            desc.SetValue(dic[p.Name]);
                    } catch (Exception ex) {
                        string strErr = "Attributes[" + Title + "." + p.Name + "] The value cannot be restored. You can ensure that the binary data is correct when saving and loading by rewriting [STNodePropertyAttribute.GetBytesFromValue(), STNodePropertyAttribute.GetValueFromBytes(byte[])]";
                        Exception e = ex;

                        while (e != null) {
                            strErr += "\r\n----\r\n[" + e.GetType().Name + "] -> " + e.Message;
                            e = e.InnerException;
                        }

                        throw new InvalidOperationException(strErr, ex);
                    }
                }
            }
        }

        /// <summary>
        /// Occurs when the editor has loaded all nodes
        /// </summary>
        protected internal virtual void OnEditorLoadCompleted() { }

        /// <summary>
        /// Set the text information of Option
        /// </summary>
        /// <param name="op">Target option</param>
        /// <param name="strText">Text</param>
        /// <returns>Whether succeed</returns>
        protected bool SetOptionText(STNodeOption op, string strText) {
            if (op.Owner != this)
                return false;

            op.Text = strText;
            return true;
        }

        /// <summary>
        /// Set the color of Option text information
        /// </summary>
        /// <param name="op">Target option</param>
        /// <param name="clr">Color</param>
        /// <returns>Whether succeed</returns>
        protected bool SetOptionTextColor(STNodeOption op, Color clr) {
            if (op.Owner != this)
                return false;

            op.TextColor = clr;
            return true;
        }

        /// <summary>
        /// Set the color of the Option connection point
        /// </summary>
        /// <param name="op">Target option</param>
        /// <param name="clr">Color</param>
        /// <returns>Whether succeed</returns>
        protected bool SetOptionDotColor(STNodeOption op, Color clr) {
            if (op.Owner != this)
                return false;

            op.DotColor = clr;
            return false;
        }

        //[event]===========================[event]==============================[event]============================[event]

        protected internal virtual void OnGotFocus(EventArgs e) { }

        protected internal virtual void OnLostFocus(EventArgs e) { }

        protected internal virtual void OnMouseEnter(EventArgs e) { }

        protected internal virtual void OnMouseDown(MouseEventArgs e) {
            Point pt = e.Location;
            pt.Y -= _TitleHeight;
            for (int i = _Controls.Count - 1; i >= 0; i--) {
                var c = _Controls[i];

                if (c.DisplayRectangle.Contains(pt)) {
                    if (!c.Enabled)
                        return;

                    if (!c.Visable)
                        continue;

                    c.OnMouseDown(new MouseEventArgs(e.Button, e.Clicks, e.X - c.Left, pt.Y - c.Top, e.Delta));
                    m_ctrl_down = c;

                    if (m_ctrl_active != c) {
                        c.OnGotFocus(EventArgs.Empty);
                        m_ctrl_active?.OnLostFocus(EventArgs.Empty);
                        m_ctrl_active = c;
                    }

                    return;
                }
            }

            m_ctrl_active?.OnLostFocus(EventArgs.Empty);
            m_ctrl_active = null;
        }

        protected internal virtual void OnMouseMove(MouseEventArgs e) {
            Point pt = e.Location;
            pt.Y -= _TitleHeight;

            if (m_ctrl_down != null) {
                if (m_ctrl_down.Enabled && m_ctrl_down.Visable)
                    m_ctrl_down.OnMouseMove(new MouseEventArgs(e.Button, e.Clicks, e.X - m_ctrl_down.Left, pt.Y - m_ctrl_down.Top, e.Delta));

                return;
            }

            for (int i = _Controls.Count - 1; i >= 0; i--) {
                var c = _Controls[i];

                if (c.DisplayRectangle.Contains(pt)) {
                    if (m_ctrl_hover != _Controls[i]) {
                        c.OnMouseEnter(EventArgs.Empty);
                        m_ctrl_hover?.OnMouseLeave(EventArgs.Empty);
                        m_ctrl_hover = c;
                    }

                    m_ctrl_hover.OnMouseMove(new MouseEventArgs(e.Button, e.Clicks, e.X - c.Left, pt.Y - c.Top, e.Delta));
                    return;
                }
            }

            m_ctrl_hover?.OnMouseLeave(EventArgs.Empty);
            m_ctrl_hover = null;
        }

        protected internal virtual void OnMouseUp(MouseEventArgs e) {
            Point pt = e.Location;
            pt.Y -= _TitleHeight;

            if (m_ctrl_down != null && m_ctrl_down.Enabled && m_ctrl_down.Visable)
                m_ctrl_down.OnMouseUp(new MouseEventArgs(e.Button, e.Clicks, e.X - m_ctrl_down.Left, pt.Y - m_ctrl_down.Top, e.Delta));

            //if (m_ctrl_active != null) {
            //    m_ctrl_active.OnMouseUp(new MouseEventArgs(e.Button, e.Clicks,
            //        e.X - m_ctrl_active.Left, pt.Y - m_ctrl_active.Top, e.Delta));
            //}
            m_ctrl_down = null;
        }

        protected internal virtual void OnMouseLeave(EventArgs e) {
            if (m_ctrl_hover != null && m_ctrl_hover.Enabled && m_ctrl_hover.Visable)
                m_ctrl_hover.OnMouseLeave(e);

            m_ctrl_hover = null;
        }

        protected internal virtual void OnMouseClick(MouseEventArgs e) {
            Point pt = e.Location;
            pt.Y -= _TitleHeight;

            if (m_ctrl_active != null && m_ctrl_active.Enabled && m_ctrl_active.Visable)
                m_ctrl_active.OnMouseClick(new MouseEventArgs(e.Button, e.Clicks, e.X - m_ctrl_active.Left, pt.Y - m_ctrl_active.Top, e.Delta));
        }

        protected internal virtual void OnMouseWheel(MouseEventArgs e) {
            Point pt = e.Location;
            pt.Y -= _TitleHeight;

            if (m_ctrl_hover != null && m_ctrl_hover.Enabled && m_ctrl_hover.Visable)
                m_ctrl_hover.OnMouseWheel(new MouseEventArgs(e.Button, e.Clicks, e.X - m_ctrl_hover.Left, pt.Y - m_ctrl_hover.Top, e.Delta));
        }

        protected internal virtual void OnMouseHWheel(MouseEventArgs e) {
            if (m_ctrl_hover != null && m_ctrl_hover.Enabled && m_ctrl_hover.Visable)
                m_ctrl_hover.OnMouseHWheel(e);
        }

        protected internal virtual void OnKeyDown(KeyEventArgs e) {
            if (m_ctrl_active != null && m_ctrl_active.Enabled && m_ctrl_active.Visable)
                m_ctrl_active.OnKeyDown(e);
        }

        protected internal virtual void OnKeyUp(KeyEventArgs e) {
            if (m_ctrl_active != null && m_ctrl_active.Enabled && m_ctrl_active.Visable)
                m_ctrl_active.OnKeyUp(e);
        }

        protected internal virtual void OnKeyPress(KeyPressEventArgs e) {
            if (m_ctrl_active != null && m_ctrl_active.Enabled && m_ctrl_active.Visable)
                m_ctrl_active.OnKeyPress(e);
        }

        protected virtual void OnMove(EventArgs e) { /*this.SetOptionLocation();*/ }

        protected virtual void OnResize(EventArgs e) { /*this.SetOptionLocation();*/ }

        /// <summary>
        /// Occurs when the owner changes
        /// </summary>
        protected virtual void OnOwnerChanged() { }

        /// <summary>
        /// Occurs when the selected state changes
        /// </summary>
        protected virtual void OnSelectedChanged() { }

        /// <summary>
        /// Occurs when the activity state changes
        /// </summary>
        protected virtual void OnActiveChanged() { }

        #endregion protected
        /// <summary>
        /// Calculate the position of each Option
        /// </summary>
        protected virtual void SetOptionsLocation() {
            int nIndex = 0;
            Rectangle rect = new Rectangle(Left + 10, _Top + _TitleHeight, _Width - 20, _ItemHeight);

            foreach (STNodeOption op in _InputOptions) {
                if (op != STNodeOption.Empty) {
                    Point pt = OnSetOptionDotLocation(op, new Point(Left - op.DotSize / 2, rect.Y + (rect.Height - op.DotSize) / 2), nIndex);
                    op.TextRectangle = OnSetOptionTextRectangle(op, rect, nIndex);
                    op.DotLeft = pt.X;
                    op.DotTop = pt.Y;
                }

                rect.Y += _ItemHeight;
                nIndex++;
            }

            rect.Y = _Top + _TitleHeight;
            m_sf.Alignment = StringAlignment.Far;

            foreach (STNodeOption op in _OutputOptions) {
                if (op != STNodeOption.Empty) {
                    Point pt = OnSetOptionDotLocation(op, new Point(_Left + _Width - op.DotSize / 2, rect.Y + (rect.Height - op.DotSize) / 2), nIndex);
                    op.TextRectangle = OnSetOptionTextRectangle(op, rect, nIndex);
                    op.DotLeft = pt.X;
                    op.DotTop = pt.Y;
                }

                rect.Y += _ItemHeight;
                nIndex++;
            }
        }

        /// <summary>
        /// Redraw Node
        /// </summary>
        public void Invalidate() {
            _Owner?.Invalidate(_Owner.CanvasToControl(new Rectangle(_Left - 5, _Top - 5, _Width + 10, _Height + 10)));
        }

        /// <summary>
        /// Redraw the specified area of Node
        /// </summary>
        /// <param name="rect">Node specific area</param>
        public void Invalidate(Rectangle rect) {
            rect.X += _Left;
            rect.Y += _Top;

            if (_Owner != null) {
                rect = _Owner.CanvasToControl(rect);
                rect.Width += 1; rect.Height += 1;//Coordinate system conversion may cause loss of progress, plus one more pixel
                _Owner.Invalidate(rect);
            }
        }
        /// <summary>
        /// Get the input Option collection contained in this Node
        /// </summary>
        /// <returns>Option set</returns>
        public STNodeOption[] GetInputOptions() {
            if (!_LetGetOptions)
                return null;

            STNodeOption[] ops = new STNodeOption[_InputOptions.Count];

            for (int i = 0; i < _InputOptions.Count; i++)
                ops[i] = _InputOptions[i];

            return ops;
        }

        /// <summary>
        /// Get the output Option collection contained in this Node
        /// </summary>
        /// <returns>Option set</returns>
        public STNodeOption[] GetOutputOptions() {
            if (!_LetGetOptions)
                return null;

            STNodeOption[] ops = new STNodeOption[_OutputOptions.Count];

            for (int i = 0; i < _OutputOptions.Count; i++)
                ops[i] = _OutputOptions[i];

            return ops;
        }

        /// <summary>
        /// Set the selected state of Node
        /// </summary>
        /// <param name="bSelected">Is selected</param>
        /// <param name="bRedraw">Whether to redraw</param>
        public void SetSelected(bool bSelected, bool bRedraw) {
            if (_IsSelected == bSelected)
                return;

            _IsSelected = bSelected;

            if (_Owner != null) {
                if (bSelected)
                    _Owner.AddSelectedNode(this);
                else
                    _Owner.RemoveSelectedNode(this);
            }

            if (bRedraw)
                Invalidate();

            OnSelectedChanged();
            _Owner?.OnSelectedChanged(EventArgs.Empty);
        }

        public IAsyncResult BeginInvoke(Delegate method) =>
            BeginInvoke(method, null);

        public IAsyncResult BeginInvoke(Delegate method, params object[] args) {
            if (_Owner == null)
                return null;

            return _Owner.BeginInvoke(method, args);
        }

        public object Invoke(Delegate method) =>
            Invoke(method, null);

        public object Invoke(Delegate method, params object[] args) {
            if (_Owner == null)
                return null;

            return _Owner.Invoke(method, args);
        }
    }
}
