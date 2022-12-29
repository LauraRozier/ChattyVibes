using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;
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
 * create: 2021-01-28
 * modify: 2021-03-02
 * Author: Crystal_lz
 * blog: http://st233.com
 * Gitee: https://gitee.com/DebugST
 * Github: https://github.com/DebugST
 */
namespace ST.Library.UI.NodeEditor
{
    /// <summary>
    /// STNode node attribute editor
    /// </summary>
    public class STNodePropertyGrid : Control
    {
        #region properties ==========

        private STNode _STNode;
        /// <summary>
        /// The currently displayed STNode
        /// </summary>
        [Description("The currently displayed STNode"), Browsable(false)]
        public STNode STNode {
            get { return _STNode; }
        }

        private Color _ItemHoverColor = Color.FromArgb(50, 125, 125, 125);
        /// <summary>
        /// Get or set the background color when the property option is hovered by the mouse
        /// </summary>
        [Description("Get or set the background color when the property option is hovered by the mouse")]
        public Color ItemHoverColor {
            get { return _ItemHoverColor; }
            set { _ItemHoverColor = value; }
        }

        private Color _ItemSelectedColor = Color.DodgerBlue;
        /// <summary>
        /// Get or set the background color when the property option is selected. This property cannot be set when AutoColor is set.
        /// </summary>
        [Description("Get or set the background color when the property option is selected. This property cannot be set when AutoColor is set."), DefaultValue(typeof(Color), "DodgerBlue")]
        public Color ItemSelectedColor {
            get { return _ItemSelectedColor; }
            set {
                if (_AutoColor)
                    return;

                if (value == _ItemSelectedColor)
                    return;

                _ItemSelectedColor = value;
                Invalidate();
            }
        }

        private Color _ItemValueBackColor = Color.FromArgb(255, 80, 80, 80);
        /// <summary>
        /// Get or set the background color of the attribute option value
        /// </summary>
        [Description("Get or set the background color of the attribute option value")]
        public Color ItemValueBackColor {
            get { return _ItemValueBackColor; }
            set {
                _ItemValueBackColor = value;
                Invalidate();
            }
        }

        private Color _TitleColor = Color.FromArgb(127, 0, 0, 0);
        /// <summary>
        /// Get or set the default title background color
        /// </summary>
        [Description("Get or set the default title background color")]
        public Color TitleColor {
            get { return _TitleColor; }
            set {
                _TitleColor = value;

                if (!_ShowTitle)
                    return;

                Invalidate(m_rect_title);
            }
        }

        private Color _ErrorColor = Color.FromArgb(200, Color.Brown);
        /// <summary>
        /// Get or set the background color of the prompt message when the property is set incorrectly
        /// </summary>
        [Description("Get or set the background color of the prompt message when the property is set incorrectly")]
        public Color ErrorColor {
            get { return _ErrorColor; }
            set { _ErrorColor = value; }
        }

        private Color _DescriptionColor = Color.FromArgb(200, Color.DarkGoldenrod);
        /// <summary>
        /// Get or set the background color of attribute description information
        /// </summary>
        [Description("Get or set the background color of attribute description information")]
        public Color DescriptionColor {
            get { return _DescriptionColor; }
            set { _DescriptionColor = value; }
        }

        private bool _ShowTitle = true;
        /// <summary>
        /// Get or set whether to display the node title
        /// </summary>
        [Description("Get or set whether to display the node title")]
        public bool ShowTitle {
            get { return _ShowTitle; }
            set {
                _ShowTitle = value;
                SetItemRectangle();
                Invalidate();
            }
        }

        private bool _AutoColor = true;
        /// <summary>
        /// Get or set whether to automatically set the highlight color of the control according to the STNode
        /// </summary>
        [Description("Get or set whether to automatically set the highlight color of the control according to the STNode"), DefaultValue(true)]
        public bool AutoColor {
            get { return _AutoColor; }
            set { _AutoColor = value; }
        }

        private bool _InfoFirstOnDraw;
        /// <summary>
        /// Get or when the node is set, whether to draw the information panel first
        /// </summary>
        [Description("Get or when the node is set, whether to draw the information panel first"), DefaultValue(false)]
        public bool InfoFirstOnDraw {
            get { return _InfoFirstOnDraw; }
            set { _InfoFirstOnDraw = value; }
        }

        private bool _ReadOnlyModel;
        /// <summary>
        /// Get or set whether the current property editor is in read-only mode
        /// </summary>
        [Description("Get or set whether the current property editor is in read-only mode"), DefaultValue(false)]
        public bool ReadOnlyModel {
            get { return _ReadOnlyModel; }
            set {
                if (value == _ReadOnlyModel)
                    return;

                _ReadOnlyModel = value;
                Invalidate(m_rect_title);
            }
        }
        /// <summary>
        /// Get the current scroll bar height
        /// </summary>
        [Description("Get the current scroll bar height")]
        public int ScrollOffset { get { return m_nOffsetY; } }

        #endregion

        #region protected fields ==========

        /// <summary>
        /// Author link address area
        /// </summary>
        protected Rectangle m_rect_link;
        /// <summary>
        /// View help button area
        /// </summary>
        protected Rectangle m_rect_help;
        /// <summary>
        /// editor title area
        /// </summary>
        protected Rectangle m_rect_title;
        /// <summary>
        /// Panel Toggle Button Area
        /// </summary>
        protected Rectangle m_rect_switch;

        /// <summary>
        /// The vertical scroll offset used by the control during drawing
        /// </summary>
        protected int m_nOffsetY;
        /// <summary>
        /// Saved info panel vertical scroll offset
        /// </summary>
        protected int m_nInfoOffsetY;
        /// <summary>
        /// Saved properties panel vertical scroll offset
        /// </summary>
        protected int m_nPropertyOffsetY;

        /// <summary>
        /// The total height of the drawing area used by the control during drawing
        /// </summary>
        protected int m_nVHeight;
        /// <summary>
        /// The total height required by the saved info panel
        /// </summary>
        protected int m_nInfoVHeight;
        /// <summary>
        /// The total height required by the saved property panel
        /// </summary>
        protected int m_nPropertyVHeight;
        /// <summary>
        /// Key display required horizontal width in information panel
        /// </summary>
        protected int m_nInfoLeft;

        #endregion

        private Type m_type;
        private string[] m_KeysString = new string[] { "author", "Mail", "Link", "view help" };

        private int m_nTitleHeight = 20;
        private int m_item_height = 30;
        private Color m_clr_item_1 = Color.FromArgb(10, 0, 0, 0);
        private Color m_clr_item_2 = Color.FromArgb(10, 255, 255, 255);
        //All attribute lists are saved in this List
        private List<STNodePropertyDescriptor> m_lst_item = new List<STNodePropertyDescriptor>();

        private STNodePropertyDescriptor m_item_hover;        //The option currently being hovered over by the mouse
        private STNodePropertyDescriptor m_item_hover_value;  //The current value area is hovered over by the mouse
        private STNodePropertyDescriptor m_item_down_value;   //The option that the current value area is clicked by the mouse
        private STNodePropertyDescriptor m_item_selected;     //currently selected option
        private STNodeAttribute m_node_attribute;             //Node parameter information
        private bool m_b_hover_switch;                        //Whether the mouse is hovering over the panel toggle button
        private bool m_b_current_draw_info;                   //The information panel is currently drawn

        private Point m_pt_move;                              //The real-time coordinates of the mouse on the control
        private Point m_pt_down;                              //The coordinates of the last mouse click on the control
        private string m_str_err;                             //When set, draw error messages
        private string m_str_desc;                            //When set, draw description information

        private Pen m_pen;
        private SolidBrush m_brush;
        private StringFormat m_sf;
        private DrawingTools m_dt;

        /// <summary>
        /// Construct a node property editor
        /// </summary>
        public STNodePropertyGrid() {
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.ResizeRedraw, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);

            m_pen = new Pen(Color.Black, 1);
            m_brush = new SolidBrush(Color.Black);
            m_sf = new StringFormat
            {
                LineAlignment = StringAlignment.Center,
                FormatFlags = StringFormatFlags.NoWrap
            };
            m_dt.Pen = m_pen;
            m_dt.SolidBrush = m_brush;

            ForeColor = Color.White;
            BackColor = Color.FromArgb(255, 35, 35, 35);

            MinimumSize = new Size(120, 50);
            Size = new Size(200, 150);
        }

        #region private method ==========

        private List<STNodePropertyDescriptor> GetProperties(STNode node) {
            List<STNodePropertyDescriptor> lst = new List<STNodePropertyDescriptor>();

            if (node == null)
                return lst;

            Type t = node.GetType();

            foreach (var p in t.GetProperties()) {
                var attrs = p.GetCustomAttributes(true);

                foreach (var a in attrs) {
                    if (!(a is STNodePropertyAttribute))
                        continue;

                    var attr = a as STNodePropertyAttribute;
                    object obj = Activator.CreateInstance(attr.DescriptorType);

                    if (!(obj is STNodePropertyDescriptor))
                        throw new ArgumentException("[STNodePropertyAttribute.DescriptorType] parameter value must be [STNodePropertyDescriptor] or its subclass type");

                    var desc = (STNodePropertyDescriptor)Activator.CreateInstance(attr.DescriptorType);
                    desc.Node = node;
                    desc.Name = attr.Name;
                    desc.Description = attr.Description;
                    desc.PropertyInfo = p;
                    desc.Control = this;
                    lst.Add(desc);
                }
            }

            return lst;
        }

        private STNodeAttribute GetNodeAttribute(STNode node) {
            if (node == null)
                return null;

            Type t = node.GetType();

            foreach (var v in t.GetCustomAttributes(true)) {
                if (!(v is STNodeAttribute))
                    continue;

                return (STNodeAttribute)v;
            }

            return null;
        }

        private void SetItemRectangle() {
            int nw_p = 0, nw_h = 0;

            using (Graphics g = CreateGraphics()) {
                foreach (var v in m_lst_item) {
                    SizeF szf = g.MeasureString(v.Name, Font);

                    if (szf.Width > nw_p)
                        nw_p = (int)Math.Ceiling(szf.Width);
                }

                for (int i = 0; i < m_KeysString.Length - 1; i++) {
                    SizeF szf = g.MeasureString(m_KeysString[i], Font);

                    if (szf.Width > nw_h)
                        nw_h = (int)Math.Ceiling(szf.Width);
                }

                nw_p += 5; nw_h += 5;
                nw_p = Math.Min(nw_p, Width >> 1);
                m_nInfoLeft = Math.Min(nw_h, Width >> 1);

                int nTitleHeight = _ShowTitle ? m_nTitleHeight : 0;

                for (int i = 0; i < m_lst_item.Count; i++) {
                    STNodePropertyDescriptor item = m_lst_item[i];
                    Rectangle rect = new Rectangle(0, i * m_item_height + nTitleHeight, Width, m_item_height);
                    item.Rectangle = rect;
                    rect.Width = nw_p;
                    item.RectangleL = rect;
                    rect.X = rect.Right;
                    rect.Width = Width - rect.Left - 1;
                    rect.Inflate(-4, -4);
                    item.RectangleR = rect;
                    item.OnSetItemLocation();
                }

                m_nPropertyVHeight = m_lst_item.Count * m_item_height;

                if (_ShowTitle)
                    m_nPropertyVHeight += m_nTitleHeight;
            }
        }

        #endregion

        #region override ==========

        /// <summary>
        /// Occurs when the control is repainted
        /// </summary>
        /// <param name="e">event parameters</param>
        protected override void OnPaint(PaintEventArgs e) {
            base.OnPaint(e);
            Graphics g = e.Graphics;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
            m_dt.Graphics = g;

            m_nOffsetY = m_b_current_draw_info ? m_nInfoOffsetY : m_nPropertyOffsetY;
            g.TranslateTransform(0, m_nOffsetY);

            if (m_b_current_draw_info) {
                m_nVHeight = m_nInfoVHeight;
                OnDrawInfo(m_dt);
            } else {
                m_nVHeight = m_nPropertyVHeight;

                for (int i = 0; i < m_lst_item.Count; i++)
                    OnDrawPropertyItem(m_dt, m_lst_item[i], i);
            }

            g.ResetTransform();

            if (_ShowTitle)
                OnDrawTitle(m_dt);

            m_sf.FormatFlags = 0;

            if (!string.IsNullOrEmpty(m_str_err))
                OnDrawErrorInfo(m_dt);

            if (!string.IsNullOrEmpty(m_str_desc))
                OnDrawDescription(m_dt);
        }

        /// <summary>
        /// Occurs when the mouse moves over the control
        /// </summary>
        /// <param name="e">event parameters</param>
        protected override void OnMouseMove(MouseEventArgs e) {
            base.OnMouseMove(e);
            m_pt_move = e.Location;
            bool bHover = _ShowTitle && m_rect_switch.Contains(e.Location);

            if (bHover != m_b_hover_switch) {
                m_b_hover_switch = bHover;
                Invalidate(m_rect_switch);
            }

            Point pt = new Point(e.X, e.Y - (int)m_nOffsetY);
            MouseEventArgs mea = new MouseEventArgs(e.Button, e.Clicks, pt.X, pt.Y, e.Delta);

            if (m_b_current_draw_info)
                OnProcessHelpMouseMove(mea);
            else
                OnProcessPropertyMouseMove(mea);
        }

        /// <summary>
        /// Occurs when the mouse is clicked on the control
        /// </summary>
        /// <param name="e">event parameters</param>
        protected override void OnMouseDown(MouseEventArgs e) {
            base.OnMouseDown(e);
            m_pt_down = e.Location;
            Focus();
            bool bRedraw = false;

            if (m_str_err != null) {
                bRedraw = true;
                m_str_err = null;
            }

            if (_ShowTitle) {
                if (m_rect_switch.Contains(e.Location)) {
                    if (m_node_attribute == null || m_lst_item.Count == 0)
                        return;

                    m_b_current_draw_info = !m_b_current_draw_info;
                    Invalidate();
                    return;
                } else if (m_rect_title.Contains(e.Location)) {
                    return;
                }
            }

            if (_ShowTitle && m_rect_switch.Contains(e.Location)) {
                if (m_node_attribute == null || m_lst_item.Count == 0)
                    return;

                m_b_current_draw_info = !m_b_current_draw_info;
                Invalidate();
                return;
            }

            Point pt = new Point(e.X, e.Y - (int)m_nOffsetY);
            MouseEventArgs mea = new MouseEventArgs(e.Button, e.Clicks, pt.X, pt.Y, e.Delta);

            if (m_b_current_draw_info)
                OnProcessInfoMouseDown(mea);
            else
                OnProcessPropertyMouseDown(mea);

            if (bRedraw)
                Invalidate();
        }

        /// <summary>
        /// Occurs when the mouse is lifted over the control
        /// </summary>
        /// <param name="e">event parameters</param>
        protected override void OnMouseUp(MouseEventArgs e) {
            base.OnMouseUp(e);
            m_str_desc = null;

            if (m_item_down_value != null && !_ReadOnlyModel) {
                Point pt = new Point(e.X, e.Y - (int)m_nOffsetY);
                MouseEventArgs mea = new MouseEventArgs(e.Button, e.Clicks, pt.X, pt.Y, e.Delta);
                m_item_down_value.OnMouseUp(mea);

                if (m_pt_down == e.Location && !_ReadOnlyModel)
                    m_item_down_value.OnMouseClick(mea);
            }

            m_item_down_value = null;
            Invalidate();
        }

        /// <summary>
        /// Occurs when the mouse leaves the control
        /// </summary>
        /// <param name="e">event parameters</param>
        protected override void OnMouseLeave(EventArgs e) {
            base.OnMouseLeave(e);
            m_b_hover_switch = false;

            if (m_item_hover_value != null && !_ReadOnlyModel)
                m_item_hover_value.OnMouseLeave(e);

            m_item_hover = null;
            Invalidate();
        }

        /// <summary>
        /// Occurs when the mouse scrolls the wheel on the control
        /// </summary>
        /// <param name="e">event parameters</param>
        protected override void OnMouseWheel(MouseEventArgs e) {
            base.OnMouseWheel(e);

            if (e.Delta > 0) {
                if (m_nOffsetY == 0)
                    return;

                m_nOffsetY += m_item_height;

                if (m_nOffsetY > 0)
                    m_nOffsetY = 0;
            } else {
                if (Height - m_nOffsetY >= m_nVHeight)
                    return;

                m_nOffsetY -= m_item_height;
            }

            if (m_b_current_draw_info)
                m_nInfoOffsetY = m_nOffsetY;
            else
                m_nPropertyOffsetY = m_nOffsetY;

            Invalidate();
        }

        /// <summary>
        /// Occurs when the control size changes
        /// </summary>
        /// <param name="e">event parameters</param>
        protected override void OnResize(EventArgs e) {
            base.OnResize(e);
            m_rect_title.Width = Width;
            m_rect_title.Height = m_nTitleHeight;

            if (_ShowTitle)
                m_rect_switch = new Rectangle(Width - m_nTitleHeight + 2, 2, m_nTitleHeight - 4, m_nTitleHeight - 4);

            if (_STNode != null)
                SetItemRectangle();
        }

        #endregion

        #region virtual method ==========

        /// <summary>
        /// Occurs when drawing attribute options
        /// </summary>
        /// <param name="dt">drawing tool</param>
        /// <param name="item">target attribute option descriptor</param>
        /// <param name="nIndex">index of options</param>
        protected virtual void OnDrawPropertyItem(DrawingTools dt, STNodePropertyDescriptor item, int nIndex) {
            Graphics g = dt.Graphics;
            m_brush.Color = (nIndex % 2 == 0) ? m_clr_item_1 : m_clr_item_2;
            g.FillRectangle(m_brush, item.Rectangle);

            if (item == m_item_hover || item == m_item_selected) {
                m_brush.Color = _ItemHoverColor;
                g.FillRectangle(m_brush, item.Rectangle);
            }

            if (m_item_selected == item) {
                g.FillRectangle(m_brush, item.Rectangle.X, item.Rectangle.Y, 5, item.Rectangle.Height);

                if (_AutoColor && _STNode != null)
                    m_brush.Color = _STNode.TitleColor;
                else
                    m_brush.Color = _ItemSelectedColor;

                g.FillRectangle(m_brush, item.Rectangle.X, item.Rectangle.Y + 4, 5, item.Rectangle.Height - 8);
            }

            m_sf.Alignment = StringAlignment.Far;
            m_brush.Color = ForeColor;
            g.DrawString(item.Name, Font, m_brush, item.RectangleL, m_sf);

            item.OnDrawValueRectangle(m_dt);

            if (_ReadOnlyModel) {
                m_brush.Color = Color.FromArgb(125, 125, 125, 125);
                g.FillRectangle(m_brush, item.RectangleR);
                m_pen.Color = ForeColor;
                //g.DrawLine(m_pen,
                //    item.RectangleR.Left - 2, item.RectangleR.Top + item.RectangleR.Height / 2,
                //    item.RectangleR.Right + 1, item.RectangleR.Top + item.RectangleR.Height / 2);
            }
        }

        /// <summary>
        /// draw property window title
        /// </summary>
        /// <param name="dt">drawing tool</param>
        protected virtual void OnDrawTitle(DrawingTools dt) {
            Graphics g = dt.Graphics;

            if (_AutoColor)
                m_brush.Color = _STNode == null ? _TitleColor : _STNode.TitleColor;
            else
                m_brush.Color = _TitleColor;

            g.FillRectangle(m_brush, m_rect_title);
            m_brush.Color = _STNode == null ? ForeColor : _STNode.ForeColor;
            m_sf.Alignment = StringAlignment.Center;
            g.DrawString(_STNode == null ? Text : _STNode.Title, Font, m_brush, m_rect_title, m_sf);

            if (_ReadOnlyModel) {
                m_brush.Color = ForeColor;
                g.FillRectangle(dt.SolidBrush, 4, 5, 2, 4);
                g.FillRectangle(dt.SolidBrush, 6, 5, 2, 2);
                g.FillRectangle(dt.SolidBrush, 8, 5, 2, 4);
                g.FillRectangle(dt.SolidBrush, 3, 9, 8, 6);
            }

            //Whether to draw the panel switch button
            if (m_node_attribute == null || m_lst_item.Count == 0)
                return;

            if (m_b_hover_switch) {
                m_brush.Color = BackColor;
                g.FillRectangle(m_brush, m_rect_switch);
            }

            m_pen.Color = _STNode == null ? ForeColor : _STNode.ForeColor;
            m_brush.Color = m_pen.Color;
            int nT1 = m_rect_switch.Top + m_rect_switch.Height / 2 - 2;
            int nT2 = m_rect_switch.Top + m_rect_switch.Height / 2 + 1;
            g.DrawRectangle(m_pen, m_rect_switch.Left, m_rect_switch.Top, m_rect_switch.Width - 1, m_rect_switch.Height - 1);

            g.DrawLines(m_pen, new Point[]{
                new Point(m_rect_switch.Left + 2, nT1), new Point(m_rect_switch.Right - 3, nT1),
                new Point(m_rect_switch.Left + 3, nT1 - 1), new Point(m_rect_switch.Right - 3, nT1 - 1)
            });
            g.DrawLines(m_pen, new Point[]{
                new Point(m_rect_switch.Left + 2, nT2), new Point(m_rect_switch.Right - 3, nT2),
                new Point(m_rect_switch.Left + 2, nT2 + 1), new Point(m_rect_switch.Right - 4, nT2 + 1),
            });

            g.FillPolygon(m_brush, new Point[]{
                new Point(m_rect_switch.Left + 2, nT1),
                new Point(m_rect_switch.Left + 7, nT1),
                new Point(m_rect_switch.Left + 7, m_rect_switch.Top  ),
            });
            g.FillPolygon(m_brush, new Point[]{
                new Point(m_rect_switch.Right - 2, nT2),
                new Point(m_rect_switch.Right - 7, nT2),
                new Point(m_rect_switch.Right - 7, m_rect_switch.Bottom - 2 ),
            });
        }

        /// <summary>
        /// Occurs when attribute description information needs to be drawn
        /// </summary>
        /// <param name="dt">drawing tool</param>
        protected virtual void OnDrawDescription(DrawingTools dt) {
            if (string.IsNullOrEmpty(m_str_desc))
                return;

            Graphics g = dt.Graphics;
            SizeF szf = g.MeasureString(m_str_desc, Font, Width - 4);
            Rectangle rect_desc = new Rectangle(0, Height - (int)szf.Height - 4, Width, (int)szf.Height + 4);
            m_brush.Color = _DescriptionColor;
            g.FillRectangle(m_brush, rect_desc);
            m_pen.Color = _DescriptionColor;
            g.DrawRectangle(m_pen, 0, rect_desc.Top, rect_desc.Width - 1, rect_desc.Height - 1);
            rect_desc.Inflate(-4, 0);
            m_brush.Color = ForeColor;
            m_sf.Alignment = StringAlignment.Near;
            g.DrawString(m_str_desc, Font, m_brush, rect_desc, m_sf);
        }

        /// <summary>
        /// Occurs when an error message needs to be drawn
        /// </summary>
        /// <param name="dt">drawing tool</param>
        protected virtual void OnDrawErrorInfo(DrawingTools dt) {
            if (string.IsNullOrEmpty(m_str_err))
                return;

            Graphics g = dt.Graphics;
            SizeF szf = g.MeasureString(m_str_err, Font, Width - 4);
            Rectangle rect_desc = new Rectangle(0, 0, Width, (int)szf.Height + 4);
            m_brush.Color = _ErrorColor;
            g.FillRectangle(m_brush, rect_desc);
            m_pen.Color = _ErrorColor;
            g.DrawRectangle(m_pen, 0, rect_desc.Top, rect_desc.Width - 1, rect_desc.Height - 1);
            rect_desc.Inflate(-4, 0);
            m_brush.Color = ForeColor;
            m_sf.Alignment = StringAlignment.Near;
            g.DrawString(m_str_err, Font, m_brush, rect_desc, m_sf);
        }

        /// <summary>
        /// Occurs when drawing node information
        /// </summary>
        /// <param name="dt">drawing tool</param>
        protected virtual void OnDrawInfo(DrawingTools dt) {
            if (m_node_attribute == null)
                return;

            var attr = m_node_attribute;
            Graphics g = dt.Graphics;
            Color clr_r = Color.FromArgb(ForeColor.A / 2, ForeColor);
            m_sf.Alignment = StringAlignment.Near;
            Rectangle rect = new Rectangle(0, _ShowTitle ? m_nTitleHeight : 0, Width, m_item_height);
            Rectangle rect_l = new Rectangle(2, rect.Top, m_nInfoLeft - 2, m_item_height);
            Rectangle rect_r = new Rectangle(m_nInfoLeft, rect.Top, Width - m_nInfoLeft, m_item_height);
            m_brush.Color = m_clr_item_2;
            g.FillRectangle(m_brush, rect);
            m_brush.Color = ForeColor;
            m_sf.FormatFlags = StringFormatFlags.NoWrap;
            m_sf.Alignment = StringAlignment.Near;
            g.DrawString(m_KeysString[0], Font, m_brush, rect_l, m_sf);          //author
            m_brush.Color = clr_r;
            g.DrawString(attr.Author, Font, m_brush, rect_r, m_sf);
            rect.Y += m_item_height; rect_l.Y += m_item_height; rect_r.Y += m_item_height;

            m_brush.Color = m_clr_item_1;
            g.FillRectangle(m_brush, rect);
            m_brush.Color = ForeColor;
            g.DrawString(m_KeysString[1], Font, m_brush, rect_l, m_sf);          //mail
            m_brush.Color = clr_r;
            g.DrawString(attr.Mail, Font, m_brush, rect_r, m_sf);
            rect.Y += m_item_height; rect_l.Y += m_item_height; rect_r.Y += m_item_height;

            m_brush.Color = m_clr_item_2;
            g.FillRectangle(m_brush, rect);
            m_brush.Color = ForeColor;
            g.DrawString(m_KeysString[2], Font, m_brush, rect_l, m_sf);          //link_key
            m_brush.Color = clr_r;
            g.DrawString(attr.Link, Font, Brushes.CornflowerBlue, rect_r, m_sf); //link

            if (!string.IsNullOrEmpty(attr.Link))
                m_rect_link = rect_r;

            //fill left
            m_brush.Color = Color.FromArgb(40, 125, 125, 125);
            g.FillRectangle(m_brush, 0, _ShowTitle ? m_nTitleHeight : 0, m_nInfoLeft - 1, m_item_height * 3);

            rect.X = 5; rect.Y += m_item_height;
            rect.Width = Width - 10;

            if (!string.IsNullOrEmpty(m_node_attribute.Description)) {
                float h = g.MeasureString(m_node_attribute.Description, Font, rect.Width).Height;
                rect.Height = (int)Math.Ceiling(h / m_item_height) * m_item_height;
                m_brush.Color = clr_r;
                m_sf.FormatFlags = 0;
                g.DrawString(m_node_attribute.Description, Font, m_brush, rect, m_sf);
            }

            m_nInfoVHeight = rect.Bottom;
            bool bHasHelp = STNodeAttribute.GetHelpMethod(m_type) != null;
            rect.X = 5; rect.Y += rect.Height;
            rect.Height = m_item_height;
            m_sf.Alignment = StringAlignment.Center;
            m_brush.Color = Color.FromArgb(125, 125, 125, 125);
            g.FillRectangle(m_brush, rect);

            if (bHasHelp)
                m_brush.Color = Color.CornflowerBlue;

            g.DrawString(m_KeysString[3], Font, m_brush, rect, m_sf);

            if (bHasHelp) {
                m_rect_help = rect;
            } else {
                int w = (int)g.MeasureString(m_KeysString[3], Font).Width + 1;
                int x = rect.X + (rect.Width - w) / 2, y = rect.Y + rect.Height / 2;
                m_pen.Color = m_brush.Color;
                g.DrawLine(m_pen, x, y, x + w, y);
            }

            m_nInfoVHeight = rect.Bottom;
        }

        /// <summary>
        /// Occurs when the mouse is clicked in the properties panel
        /// </summary>
        /// <param name="e">mouse event parameters</param>
        protected virtual void OnProcessPropertyMouseDown(MouseEventArgs e) {
            bool bRedraw = false;

            if (m_item_selected != m_item_hover) {
                m_item_selected = m_item_hover;
                bRedraw = true;
            }

            m_item_down_value = null;

            if (m_item_selected == null) {
                if (bRedraw)
                    Invalidate();

                return;
            }

            if (m_item_selected.RectangleR.Contains(e.Location)) {
                m_item_down_value = m_item_selected;

                if (!_ReadOnlyModel)
                    m_item_selected.OnMouseDown(e);
                else {
                    return;
                }
            } else if (m_item_selected.RectangleL.Contains(e.Location)) {
                m_str_desc = m_item_selected.Description;
                bRedraw = true;
            }

            if (bRedraw)
                Invalidate();
        }

        /// <summary>
        /// Occurs when the mouse is clicked on the info panel
        /// </summary>
        /// <param name="e">mouse event parameters</param>
        protected virtual void OnProcessInfoMouseDown(MouseEventArgs e) {
            try {
                if (m_rect_link.Contains(e.Location)) {
                    System.Diagnostics.Process.Start(m_node_attribute.Link);
                } else if (m_rect_help.Contains(e.Location)) {
                    STNodeAttribute.ShowHelp(m_type);
                }
            } catch (Exception ex) {
                SetErrorMessage(ex.Message);
            }
        }

        /// <summary>
        /// Occurs when the mouse is moved in the properties panel
        /// </summary>
        /// <param name="e">mouse event parameters</param>
        protected virtual void OnProcessPropertyMouseMove(MouseEventArgs e) {
            if (m_item_down_value != null) {
                m_item_down_value.OnMouseMove(e);
                return;
            }

            STNodePropertyDescriptor item = null;

            foreach (var v in m_lst_item) {
                if (v.Rectangle.Contains(e.Location)) {
                    item = v;
                    break;
                }
            }

            if (item != null) {
                if (item.RectangleR.Contains(e.Location)) {
                    if (m_item_hover_value != item) {
                        m_item_hover_value?.OnMouseLeave(e);
                        m_item_hover_value = item;
                        m_item_hover_value.OnMouseEnter(e);
                    }

                    m_item_hover_value.OnMouseMove(e);
                } else {
                    m_item_hover_value?.OnMouseLeave(e);
                }
            }

            if (m_item_hover != item) {
                m_item_hover = item;
                Invalidate();
            }
        }

        /// <summary>
        /// Occurs when the mouse is moved over the info panel
        /// </summary>
        /// <param name="e">mouse event parameters</param>
        protected virtual void OnProcessHelpMouseMove(MouseEventArgs e) {
            if (m_rect_link.Contains(e.Location) || m_rect_help.Contains(e.Location))
                Cursor = Cursors.Hand;
            else
                Cursor = Cursors.Arrow;
        }

        #endregion

        #region public ==========

        /// <summary>
        /// Set the STNode node to be displayed
        /// </summary>
        /// <param name="node">target node</param>
        public void SetNode(STNode node) {
            if (node == _STNode)
                return;

            m_nInfoOffsetY = m_nPropertyOffsetY = 0;
            m_nInfoVHeight = m_nPropertyVHeight = 0;
            m_rect_link = m_rect_help = Rectangle.Empty;
            m_str_desc = m_str_err = null;
            _STNode = node;

            if (node != null) {
                m_type = node.GetType();
                m_lst_item = GetProperties(node);
                m_node_attribute = GetNodeAttribute(node);
                SetItemRectangle();
                m_b_current_draw_info = m_lst_item.Count == 0 || _InfoFirstOnDraw;

                if (_AutoColor)
                    _ItemSelectedColor = _STNode.TitleColor;
            } else {
                m_type = null;
                m_lst_item.Clear();
                m_node_attribute = null;
            }

            Invalidate();
        }

        /// <summary>
        /// Set the display text of the key on the information page
        /// </summary>
        /// <param name="strAuthor">author</param>
        /// <param name="strMail">Mail</param>
        /// <param name="strLink">Link</param>
        /// <param name="strHelp">view help</param>
        public void SetInfoKey(string strAuthor, string strMail, string strLink, string strHelp) {
            m_KeysString = new string[] { strAuthor, strMail, strLink, strHelp };
        }

        /// <summary>
        /// Set the error message to display
        /// </summary>
        /// <param name="strText">error message</param>
        public void SetErrorMessage(string strText) {
            m_str_err = strText;
            Invalidate();
        }

        #endregion
    }
}
