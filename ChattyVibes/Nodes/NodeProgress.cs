using ST.Library.UI.NodeEditor;
using System.Drawing;
using System;

namespace ChattyVibes.Nodes
{
    internal class NodeProgress : STNodeControl
    {
        private int _value = 50;
        public int Value
        {
            get { return _value; }
            set
            {
                _value = value.Clamp(_min, _max);
                Invalidate();
            }
        }
        private int _min = 0;
        public int MinValue
        {
            get { return _min; }
            set
            {
                _min = value;

                if (_value < value)
                    _value = value;

                Invalidate();
            }
        }
        private int _max = 100;
        public int MaxValue
        {
            get { return _max; }
            set
            {
                _max = value;

                if (_value > value)
                    _value = value;

                Invalidate();
            }
        }

        private bool m_bMouseDown;

        public event EventHandler ValueChanged;
        protected virtual void OnValueChanged(EventArgs e) =>
            ValueChanged?.Invoke(this, e);

        protected override void OnPaint(DrawingTools dt)
        {
            base.OnPaint(dt);
            dt.Graphics.FillRectangle(Brushes.Gray, ClientRectangle);
            dt.Graphics.FillRectangle(Brushes.CornflowerBlue, 0, 0, (int)((float)_value / _max * Width), Height);
            m_sf.Alignment = StringAlignment.Near;
            dt.Graphics.DrawString(Text, Font, Brushes.White, ClientRectangle, m_sf);
            m_sf.Alignment = StringAlignment.Far;
            dt.Graphics.DrawString(((float)_value / _max).ToString("F2"), Font, Brushes.White, ClientRectangle, m_sf);
        }

        protected override void OnMouseDown(System.Windows.Forms.MouseEventArgs e)
        {
            base.OnMouseDown(e);
            m_bMouseDown = true;
        }

        protected override void OnMouseUp(System.Windows.Forms.MouseEventArgs e)
        {
            base.OnMouseUp(e);
            m_bMouseDown = false;
        }

        protected override void OnMouseMove(System.Windows.Forms.MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (!m_bMouseDown)
                return;

            int v = (int)((float)e.X / Width * _max);

            if (v < _min)
                v = _min;

            if (v >= _max)
                v = _max;

            _value = v;
            OnValueChanged(new EventArgs());
            Invalidate();
        }
    }
}
