using System;
using System.Windows.Forms;
using System.Drawing;
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
 * time: 2021-01-06
 * Author: Crystal_lz
 * blog: st233.com
 * Github: DebugST.github.io
 */
namespace ST.Library.UI.NodeEditor
{
    public class STNodeControl
    {
        private STNode _Owner;
        public STNode Owner {
            get { return _Owner; }
            internal set { _Owner = value; }
        }

        private int _Left;
        public int Left {
            get { return _Left; }
            set {
                _Left = value;
                OnMove(EventArgs.Empty);
                Invalidate();
            }
        }

        private int _Top;
        public int Top {
            get { return _Top; }
            set {
                _Top = value;
                OnMove(EventArgs.Empty);
                Invalidate();
            }
        }

        private int _Width;
        public int Width {
            get { return _Width; }
            set {
                _Width = value;
                OnResize(EventArgs.Empty);
                Invalidate();
            }
        }

        private int _Height;
        public int Height {
            get { return _Height; }
            set {
                _Height = value;
                OnResize(EventArgs.Empty);
                Invalidate();
            }
        }

        public int Right { get { return _Left + _Width; } }

        public int Bottom { get { return _Top + _Height; } }

        public Point Location {
            get { return new Point(_Left, _Top); }
            set {
                Left = value.X;
                Top = value.Y;
            }
        }

        public Size Size {
            get { return new Size(_Width, _Height); }
            set {
                Width = value.Width;
                Height = value.Height;
            }
        }

        public Rectangle DisplayRectangle {
            get { return new Rectangle(_Left, _Top, _Width, _Height); }
            set {
                Left = value.X;
                Top = value.Y;
                Width = value.Width;
                Height = value.Height;
            }
        }

        public Rectangle ClientRectangle {
            get { return new Rectangle(0, 0, _Width, _Height); }
        }

        private Color _BackColor = Color.FromArgb(127, 0, 0, 0);
        public Color BackColor {
            get { return _BackColor; }
            set {
                _BackColor = value;
                Invalidate();
            }
        }

        private Color _ForeColor = Color.White;
        public Color ForeColor {
            get { return _ForeColor; }
            set {
                _ForeColor = value;
                Invalidate();
            }
        }

        private string _Text = "STNCTRL";
        public string Text {
            get { return _Text; }
            set {
                _Text = value;
                Invalidate();
            }
        }

        private Font _Font;
        public Font Font {
            get { return _Font; }
            set {
                if (value == _Font)
                    return;

                _Font = value ?? throw new ArgumentNullException("Value cannot be empty");
                Invalidate();
            }
        }

        private bool _Enabled = true;
        public bool Enabled {
            get { return _Enabled; }
            set {
                if (value == _Enabled)
                    return;

                _Enabled = value;
                Invalidate();
            }
        }

        private bool _Visable = true;
        public bool Visable {
            get { return _Visable; }
            set {
                if (value == _Visable)
                    return;

                _Visable = value;
                Invalidate();
            }
        }

        protected StringFormat m_sf;

        public STNodeControl() {
            m_sf = new StringFormat
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center
            };
            _Font = new Font("courier new", 8.25f);
            Width = 75;
            Height = 23;
        }

        protected internal virtual void OnPaint(DrawingTools dt) {
            Graphics g = dt.Graphics;
            SolidBrush brush = dt.SolidBrush;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
            brush.Color = _BackColor;
            g.FillRectangle(brush, 0, 0, Width, Height);

            if (!string.IsNullOrEmpty(_Text)) {
                brush.Color = _ForeColor;
                g.DrawString(_Text, _Font, brush, ClientRectangle, m_sf);
            }

            Paint?.Invoke(this, new STNodeControlPaintEventArgs(dt));
        }

        public void Invalidate() {
            if (_Owner == null)
                return;

            _Owner.Invalidate(new Rectangle(_Left, _Top + _Owner.TitleHeight, Width, Height));
        }

        public void Invalidate(Rectangle rect) {
            if (_Owner == null)
                return;

            _Owner.Invalidate(RectangleToParent(rect));
        }

        public Rectangle RectangleToParent(Rectangle rect) {
            return new Rectangle(_Left, _Top + _Owner.TitleHeight, Width, Height);
        }

        public event EventHandler GotFocus;
        public event EventHandler LostFocus;
        public event EventHandler MouseEnter;
        public event EventHandler MouseLeave;
        public event MouseEventHandler MouseDown;
        public event MouseEventHandler MouseMove;
        public event MouseEventHandler MouseUp;
        public event MouseEventHandler MouseClick;
        public event MouseEventHandler MouseWheel;
        public event EventHandler MouseHWheel;

        public event KeyEventHandler KeyDown;
        public event KeyEventHandler KeyUp;
        public event KeyPressEventHandler KeyPress;

        public event EventHandler Move;
        public event EventHandler Resize;

        public event STNodeControlPaintEventHandler Paint;

        protected internal virtual void OnGotFocus(EventArgs e) {
            GotFocus?.Invoke(this, e);
        }

        protected internal virtual void OnLostFocus(EventArgs e) {
            LostFocus?.Invoke(this, e);
        }

        protected internal virtual void OnMouseEnter(EventArgs e) {
            MouseEnter?.Invoke(this, e);
        }

        protected internal virtual void OnMouseLeave(EventArgs e) {
            MouseLeave?.Invoke(this, e);
        }

        protected internal virtual void OnMouseDown(MouseEventArgs e) {
            MouseDown?.Invoke(this, e);
        }

        protected internal virtual void OnMouseMove(MouseEventArgs e) {
            MouseMove?.Invoke(this, e);
        }

        protected internal virtual void OnMouseUp(MouseEventArgs e) {
            MouseUp?.Invoke(this, e);
        }

        protected internal virtual void OnMouseClick(MouseEventArgs e) {
            MouseClick?.Invoke(this, e);
        }

        protected internal virtual void OnMouseWheel(MouseEventArgs e) {
            MouseWheel?.Invoke(this, e);
        }

        protected internal virtual void OnMouseHWheel(MouseEventArgs e) {
            MouseHWheel?.Invoke(this, e);
        }

        protected internal virtual void OnKeyDown(KeyEventArgs e) {
            KeyDown?.Invoke(this, e);
        }

        protected internal virtual void OnKeyUp(KeyEventArgs e) {
            KeyUp?.Invoke(this, e);
        }

        protected internal virtual void OnKeyPress(KeyPressEventArgs e) {
            KeyPress?.Invoke(this, e);
        }

        protected internal virtual void OnMove(EventArgs e) {
            Move?.Invoke(this, e);
        }

        protected internal virtual void OnResize(EventArgs e) {
            Resize?.Invoke(this, e);
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

    public delegate void STNodeControlPaintEventHandler(object sender, STNodeControlPaintEventArgs e);

    public class STNodeControlPaintEventArgs : EventArgs
    {
        /// <summary>
        /// Drawing tool
        /// </summary>
        public DrawingTools DrawingTools { get; private set; }

        public STNodeControlPaintEventArgs(DrawingTools dt) {
            DrawingTools = dt;
        }
    }
}
