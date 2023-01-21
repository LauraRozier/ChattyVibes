using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using System.ComponentModel;
using System.Collections;
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
 * create: 2021-02-23
 * modify: 2021-03-02
 * Author: Crystal_lz
 * blog: http://st233.com
 * Gitee: https://gitee.com/DebugST
 * Github: https://github.com/DebugST
 */
namespace ST.Library.UI.NodeEditor
{
    public class STNodeTreeView : Control
    {
        private const int C_SCROLL_WIDTH = 8;
        private Color _ItemHoverColor = Color.FromArgb(50, 125, 125, 125);
        /// <summary>
        /// Get or set the background color when the property option is hovered by the mouse
        /// </summary>
        [Description("Get or set the background color when the property option is hovered by the mouse")]
        public Color ItemHoverColor {
            get { return _ItemHoverColor; }
            set { _ItemHoverColor = value; }
        }

        private Color _TitleColor = Color.FromArgb(255, 60, 60, 60);
        /// <summary>
        /// Get or set the background color of the top retrieval area
        /// </summary>
        [Description("Get or set the background color of the top retrieval area")]
        public Color TitleColor {
            get { return _TitleColor; }
            set {
                _TitleColor = value;
                Invalidate(new Rectangle(0, 0, Width, m_nItemHeight));
            }
        }

        /// <summary>
        /// Get or set the background color of the search text box
        /// </summary>
        [Description("Get or set the background color of the search text box")]
        public Color TextBoxColor {
            get { return m_tbx.BackColor; }
            set {
                m_tbx.BackColor = value;
                Invalidate(new Rectangle(0, 0, Width, m_nItemHeight));
            }
        }

        private Color _HightLightTextColor = Color.Lime;
        /// <summary>
        /// Get or set the highlighted text color when searching
        /// </summary>
        [Description("Get or set the highlighted text color when searching"), DefaultValue(typeof(Color), "Lime")]
        public Color HightLightTextColor {
            get { return _HightLightTextColor; }
            set { _HightLightTextColor = value; }
        }

        private Color _InfoButtonColor = Color.Gray;
        /// <summary>
        /// Get or set the color of the information display button If AutoColor is set, this property value cannot be set
        /// </summary>
        [Description("Get or set the color of the information display button If AutoColor is set, this property value cannot be set"), DefaultValue(typeof(Color), "Gray")]
        public Color InfoButtonColor {
            get { return _InfoButtonColor; }
            set { _InfoButtonColor = value; }
        }

        private Color _FolderCountColor = Color.FromArgb(40, 255, 255, 255);
        /// <summary>
        /// Get or set the text color of the statistics
        /// </summary>
        [Description("Get or set the text color of the statistics")]
        public Color FolderCountColor {
            get { return _FolderCountColor; }
            set { _FolderCountColor = value; }
        }

        private readonly Color _SwitchColor = Color.LightGray;

        private bool _ShowFolderCount = true;
        /// <summary>
        /// Get or set whether to count the number of STNode
        /// </summary>
        [Description("Get or set whether to count the number of STNode"), DefaultValue(typeof(Color), "LightGray")]
        public bool ShowFolderCount {
            get { return _ShowFolderCount; }
            set { _ShowFolderCount = value; }
        }

        private bool _ShowInfoButton = true;
        /// <summary>
        /// Get or set whether to display the information button
        /// </summary>
        [Description("Get or set whether to display the information button"), DefaultValue(true)]
        public bool ShowInfoButton {
            get { return _ShowInfoButton; }
            set { _ShowInfoButton = value; }
        }

        private bool _InfoPanelIsLeftLayout = true;
        /// <summary>
        /// Get or set whether the preview window is laid out to the left
        /// </summary>
        [Description("Get or set whether the preview window is laid out to the left"), DefaultValue(true)]
        public bool InfoPanelIsLeftLayout {
            get { return _InfoPanelIsLeftLayout; }
            set { _InfoPanelIsLeftLayout = value; }
        }

        private bool _AutoColor = true;
        /// <summary>
        /// Get or set the title color of the STNode corresponding to some colors in the control
        /// </summary>
        [Description("Get or set the title color of the STNode corresponding to some colors in the control"), DefaultValue(true)]
        public bool AutoColor {
            get { return _AutoColor; }
            set {
                _AutoColor = value;
                Invalidate();
            }
        }

        private readonly STNodePropertyGrid _PropertyGrid;
        /// <summary>
        /// Get the STNodePropertyGrid used when previewing the node
        /// </summary>
        [Description("Get the STNodePropertyGrid used when previewing the node"), Browsable(false)]
        public STNodePropertyGrid PropertyGrid {
            get { return _PropertyGrid; }
        }

        private readonly int m_nItemHeight = 29;

        private static readonly Type m_type_node_base = typeof(STNode);
        private static readonly char[] m_chr_splitter = new char[] { '/', '\\' };
        private STNodeTreeCollection m_items_draw;
        private readonly STNodeTreeCollection m_items_source = new STNodeTreeCollection("ROOT");
        private readonly Dictionary<Type, string> m_dic_all_type = new Dictionary<Type, string>();

        private readonly Pen m_pen = new Pen(Color.Black);
        private readonly SolidBrush m_brush = new SolidBrush(Color.White);
        private readonly StringFormat m_sf = new StringFormat { LineAlignment = StringAlignment.Center };
        private DrawingTools m_dt;
        private readonly Color m_clr_item_1 = Color.FromArgb(10, 0, 0, 0);// Color.FromArgb(255, 40, 40, 40);
        private readonly Color m_clr_item_2 = Color.FromArgb(10, 255, 255, 255);// Color.FromArgb(255, 50, 50, 50);

        private int m_nOffsetY;                         //The vertical height that needs to be offset when the control is drawn
        private int m_nSourceOffsetY;                   //The vertical height that needs to be offset when drawing the source data
        private int m_nSearchOffsetY;                   //The vertical height that needs to be offset when drawing retrieved data
        private int m_nVHeight;                         //The total height required by the content in the control

        private Rectangle m_thumbRect = new Rectangle(0, 0, C_SCROLL_WIDTH, 0);
        private decimal m_scrollJump;
        private bool m_thumbActive = false;
        private Point m_previousMousePos = default;

        private bool m_bHoverInfo;                      //Whether the current mouse is hovering over the information display button
        private STNodeTreeCollection m_item_hover;      //The tree node currently hovered over by the mouse
        private Point m_pt_control;                     //The coordinates of the mouse on the control
        private Point m_pt_offsety;                     //The coordinates of the mouse on the control after the hammer is offset
        private Rectangle m_rect_clear;                 //Clear the search button area

        private string m_str_search;                    //retrieved text
        private readonly TextBox m_tbx = new TextBox
        {
            Left = 6,
            BackColor = Color.FromArgb(255, 30, 30, 30),
            BorderStyle = BorderStyle.None,
            MaxLength = 20
        }; //search text box

        /// <summary>
        /// Construct a STNode tree control
        /// </summary>
        public STNodeTreeView() {
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.ResizeRedraw, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);

            MinimumSize = new Size(100, 60);
            Size = new Size(200, 150);
            ForeColor = Color.FromArgb(255, 220, 220, 220);
            BackColor = Color.FromArgb(255, 35, 35, 35);

            m_items_draw = m_items_source;

            m_dt.Pen = m_pen;
            m_dt.SolidBrush = m_brush;

            m_tbx.ForeColor = ForeColor;
            m_tbx.TextChanged += new EventHandler(Tbx_TextChanged);
            Controls.Add(m_tbx);
            AllowDrop = true;

            _PropertyGrid = new STNodePropertyGrid();
        }

        #region private method ==========

        private void Tbx_TextChanged(object sender, EventArgs e) {
            m_str_search = m_tbx.Text.Trim().ToLower();
            m_nSearchOffsetY = 0;

            if (m_str_search == string.Empty) {
                m_items_draw = m_items_source;
                Invalidate();
                return;
            }

            m_items_draw = m_items_source.Copy();
            Search(m_items_draw, new Stack<string>(), m_str_search);
            Invalidate();
        }

        private bool Search(STNodeTreeCollection items, Stack<string> stack, string strText) {
            bool bFound = false;
            int nCounter = 0;

            foreach (STNodeTreeCollection v in items) {
                if (v.NameLower.IndexOf(strText) != -1) {
                    v.IsOpen = bFound = true;
                } else {
                    if (!Search(v, stack, strText)) {
                        stack.Push(v.Name);
                        nCounter++;
                    } else {
                        v.IsOpen = bFound = true;
                    }
                }
            }

            for (int i = 0; i < nCounter; i++)
                items.Remove(stack.Pop(), false);

            return bFound;
        }

        private bool AddSTNode(Type stNodeType, STNodeTreeCollection items, string strLibName, bool bShowException) {
            if (m_dic_all_type.ContainsKey(stNodeType) || stNodeType == null)
                return false;

            if (!stNodeType.IsSubclassOf(m_type_node_base)) {
                if (bShowException)
                    throw new ArgumentException("Unsupported type [" + stNodeType.FullName + "] [stNodeType] parameter value must be the type of [STNode] subclass");
                else return false;
            }

            var attr = GetNodeAttribute(stNodeType);

            if (attr == null) {
                if (bShowException)
                    throw new InvalidOperationException("Types of[" + stNodeType.FullName + "] is not marked by [STNodeAttribute]");

                else return false;
            }

            string strPath = string.Empty;
            items.STNodeCount++;

            if (!string.IsNullOrEmpty(attr.Path)) {
                var strKeys = attr.Path.Split(m_chr_splitter);

                for (int i = 0; i < strKeys.Length; i++) {
                    items = items.Add(strKeys[i]);
                    items.STNodeCount++;
                    strPath += "/" + strKeys[i];
                }
            }

            try {
                STNode node = (STNode)Activator.CreateInstance(stNodeType);
                STNodeTreeCollection stt = new STNodeTreeCollection(node.Title)
                {
                    Path = (strLibName + "/" + attr.Path).Trim('/'),
                    STNodeType = stNodeType,
                    STNodeTypeColor = node.TitleColor
                };
                items[stt.Name] = stt;
                m_dic_all_type.Add(stNodeType, stt.Path);
                Invalidate();
            } catch (Exception ex) {
                if (bShowException)
                    throw ex;

                return false;
            }

            return true;
        }

        private STNodeTreeCollection AddAssemblyPrivate(string strFile) {
            strFile = System.IO.Path.GetFullPath(strFile);
            var asm = Assembly.LoadFrom(strFile);
            STNodeTreeCollection items = new STNodeTreeCollection(System.IO.Path.GetFileNameWithoutExtension(strFile));

            foreach (var v in asm.GetTypes()) {
                if (v.IsAbstract)
                    continue;

                if (v.IsSubclassOf(m_type_node_base))
                    AddSTNode(v, items, items.Name, false);
            }

            return items;
        }

        private STNodeAttribute GetNodeAttribute(Type stNodeType) {
            if (stNodeType == null)
                return null;

            foreach (var v in stNodeType.GetCustomAttributes(true)) {
                if (!(v is STNodeAttribute))
                    continue;

                return (STNodeAttribute)v;
            }

            return null;
        }

        private STNodeTreeCollection FindItemByPoint(STNodeTreeCollection items, Point pt) {
            foreach (STNodeTreeCollection t in items) {
                if (t.DisplayRectangle.Contains(pt))
                    return t;

                if (t.IsOpen) {
                    var n = FindItemByPoint(t, pt);

                    if (n != null)
                        return n;
                }
            }

            return null;
        }

        #endregion

        #region overide method ==========

        protected override void OnCreateControl() {
            base.OnCreateControl();
            m_tbx.Top = (m_nItemHeight - m_tbx.Height) / 2;
        }

        protected override void OnResize(EventArgs e) {
            base.OnResize(e);
            m_tbx.Width = Width - 29;
            m_rect_clear = new Rectangle(Width - 20, 9, 12, 12);
        }

        protected override void OnPaint(PaintEventArgs e) {
            base.OnPaint(e);
            m_nOffsetY = string.IsNullOrEmpty(m_str_search) ? m_nSourceOffsetY : m_nSearchOffsetY;
            Graphics g = e.Graphics;
            m_dt.Graphics = g;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
            g.TranslateTransform(0, m_nOffsetY);
            int nCounter = 0;

            foreach (STNodeTreeCollection v in m_items_draw)
                nCounter = OnStartDrawItem(m_dt, v, nCounter, 0);

            m_nVHeight = (nCounter + 1) * m_nItemHeight;

            foreach (STNodeTreeCollection v in m_items_draw)
                OnDrawSwitch(m_dt, v);

            g.ResetTransform();
            OnDrawSearch(m_dt);

            OnDrawScrollbar(m_dt);
        }

        protected override void OnMouseMove(MouseEventArgs e) {
            base.OnMouseMove(e);
            bool bRedraw = false;
            m_pt_offsety = m_pt_control = e.Location;
            m_pt_offsety.Y -= m_nOffsetY;

            if (!string.IsNullOrEmpty(m_str_search) && m_rect_clear.Contains(e.Location))
                Cursor = Cursors.Hand;
            else if (m_thumbRect.Contains(e.Location) || m_thumbActive)
                Cursor = Cursors.NoMoveVert;
            else
                Cursor = Cursors.Arrow;

            var node = FindItemByPoint(m_items_draw, m_pt_offsety);

            if (m_item_hover != node) {
                m_item_hover = node;
                bRedraw = true;
            }

            if (node != null) {
                bool bHoverInfo = node.InfoRectangle.Contains(m_pt_offsety);

                if (bHoverInfo != m_bHoverInfo) {
                    m_bHoverInfo = bHoverInfo;
                    bRedraw = true;
                }
            }

            if (m_thumbActive)
            {
                int diff = m_previousMousePos.Y - e.Location.Y;

                if (Math.Abs(diff) >= (m_nItemHeight / m_scrollJump))
                {
                    PerformScroll(diff);
                    m_previousMousePos = e.Location;
                }
            }

            if (bRedraw)
                Invalidate();
        }

        protected override void OnMouseDown(MouseEventArgs e) {
            base.OnMouseDown(e);
            Focus();

            if (!string.IsNullOrEmpty(m_str_search) && m_rect_clear.Contains(e.Location)) {
                m_tbx.Text = string.Empty;
                return;
            }

            m_pt_offsety = m_pt_control = e.Location;
            m_pt_offsety.Y -= m_nOffsetY;

            if (m_item_hover == null)
            {
                if (m_thumbRect.Contains(e.Location) && !m_thumbActive)
                {
                    m_previousMousePos = e.Location;
                    m_thumbActive = true;
                }

                return;
            }

            if (m_item_hover.SwitchRectangle.Contains(m_pt_offsety)) {
                m_item_hover.IsOpen = !m_item_hover.IsOpen;
                Invalidate();
            } else if (m_item_hover.InfoRectangle.Contains(m_pt_offsety)) {
                Rectangle rect = RectangleToScreen(m_item_hover.DisplayRectangle);
                FrmNodePreviewPanel frm = new FrmNodePreviewPanel(
                    m_item_hover.STNodeType,
                    new Point(rect.Right - m_nItemHeight, rect.Top + m_nOffsetY),
                    m_nItemHeight,
                    _InfoPanelIsLeftLayout,
                    _PropertyGrid
                ) { BackColor = BackColor };
                frm.Show(this);
            } else if (m_item_hover.STNodeType != null) {
                DataObject d = new DataObject("STNodeType", m_item_hover.STNodeType);
                DoDragDrop(d, DragDropEffects.Copy);
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            if (m_thumbActive)
                m_thumbActive = false;

            base.OnMouseUp(e);
        }

        protected override void OnMouseDoubleClick(MouseEventArgs e) {
            base.OnMouseDoubleClick(e);
            m_pt_offsety = m_pt_control = e.Location;
            m_pt_offsety.Y -= m_nOffsetY;
            STNodeTreeCollection item = FindItemByPoint(m_items_draw, m_pt_offsety);

            if (item == null || item.STNodeType != null)
                return;

            item.IsOpen = !item.IsOpen;
            Invalidate();
        }

        protected override void OnMouseLeave(EventArgs e) {
            base.OnMouseLeave(e);

            if (m_item_hover != null) {
                m_item_hover = null;
                Invalidate();
            }
        }

        protected override void OnMouseWheel(MouseEventArgs e) {
            base.OnMouseWheel(e);
            PerformScroll(e.Delta);
        }

        private void PerformScroll(int delta)
        {
            if (delta > 0)
            {
                if (m_nOffsetY == 0)
                    return;

                m_nOffsetY += m_nItemHeight;

                if (m_nOffsetY > 0)
                    m_nOffsetY = 0;
            }
            else
            {
                if (Height - m_nOffsetY >= m_nVHeight)
                    return;

                m_nOffsetY -= m_nItemHeight;
            }

            if (string.IsNullOrEmpty(m_str_search))
                m_nSourceOffsetY = m_nOffsetY;
            else
                m_nSearchOffsetY = m_nOffsetY;

            Invalidate();
        }

        #endregion

        #region protected method ==========
        /// <summary>
        /// Occurs when drawing the retrieved text area
        /// </summary>
        /// <param name="dt">drawing tool</param>
        protected virtual void OnDrawSearch(DrawingTools dt) {
            Graphics g = dt.Graphics;
            m_brush.Color = _TitleColor;
            g.FillRectangle(m_brush, 0, 0, Width, m_nItemHeight);
            m_brush.Color = m_tbx.BackColor;
            g.FillRectangle(m_brush, 5, 5, Width - 10, m_nItemHeight - 10);
            m_pen.Color = ForeColor;

            if (string.IsNullOrEmpty(m_str_search)) {
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                g.DrawEllipse(m_pen, Width - 17, 8, 8, 8);
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;
                g.DrawLine(m_pen, Width - 13, 17, Width - 13, m_nItemHeight - 9);
            } else {
                m_pen.Color = ForeColor;
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                g.DrawEllipse(m_pen, Width - 20, 9, 10, 10);
                g.DrawLine(m_pen, Width - 18, 11, Width - 12, 17);
                g.DrawLine(m_pen, Width - 12, 11, Width - 18, 17);
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;
            }
        }

        /// <summary>
        /// Occurs when drawing the scrollbar area
        /// </summary>
        /// <param name="dt">drawing tool</param>
        protected virtual void OnDrawScrollbar(DrawingTools dt)
        {
            Graphics g = dt.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;

            m_thumbRect.X = Width - C_SCROLL_WIDTH;
            m_thumbRect.Y = m_nItemHeight;
            m_thumbRect.Height = Height - m_nItemHeight;

            m_brush.Color = ControlPaint.Dark(BackColor);
            g.FillRectangle(m_brush, m_thumbRect.X, m_nItemHeight, C_SCROLL_WIDTH, m_thumbRect.Height);

            int nCounter = 0;

            foreach (STNodeTreeCollection v in m_items_draw)
                nCounter = CountScrollRegionItems(v, nCounter);

            decimal totalItemHeight = nCounter * m_nItemHeight;
            decimal scrollTrackSpace = totalItemHeight - m_thumbRect.Height;

            if (scrollTrackSpace > 1)
            {
                int barHeight = m_thumbRect.Height;
                m_thumbRect.Height = Math.Max(19, (int)Math.Ceiling(barHeight * (barHeight / totalItemHeight)));
                m_scrollJump = scrollTrackSpace / (barHeight - m_thumbRect.Height);
                m_thumbRect.Y -= (int)Math.Ceiling(m_nOffsetY / m_scrollJump);
            }

            m_brush.Color = ControlPaint.Light(BackColor);
            g.FillRectangle(m_brush, m_thumbRect.X, m_thumbRect.Y, C_SCROLL_WIDTH, m_thumbRect.Height);

            m_brush.Color = ControlPaint.Light(BackColor, 0.15f);
            int thumbCenter = m_thumbRect.Y + (m_thumbRect.Height / 2);
            g.FillRectangle(m_brush, m_thumbRect.X + 1, thumbCenter - 2, C_SCROLL_WIDTH - 2, 1);
            g.FillRectangle(m_brush, m_thumbRect.X + 1, thumbCenter,     C_SCROLL_WIDTH - 2, 1);
            g.FillRectangle(m_brush, m_thumbRect.X + 1, thumbCenter + 2, C_SCROLL_WIDTH - 2, 1);
        }

        private int CountScrollRegionItems(STNodeTreeCollection Items, int nCounter)
        {
            nCounter++;

            if (Items.STNodeType == null && Items.IsOpen)
                foreach (STNodeTreeCollection n in Items)
                    nCounter = CountScrollRegionItems(n, nCounter++);

            return nCounter;
        }

        /// <summary>
        /// Occurs when starting to draw each node of the tree node
        /// </summary>
        /// <param name="dt">drawing tool</param>
        /// <param name="Items">the collection currently to be drawn</param>
        /// <param name="nCounter">A counter of the number drawn</param>
        /// <param name="nLevel">What level of sub-collection is currently in</param>
        /// <returns>number of drawn</returns>
        protected virtual int OnStartDrawItem(DrawingTools dt, STNodeTreeCollection Items, int nCounter, int nLevel) {
            Items.DisplayRectangle = new Rectangle(0, m_nItemHeight * (nCounter + 1), Width - C_SCROLL_WIDTH, m_nItemHeight);
            Items.SwitchRectangle = new Rectangle(5 + nLevel * 10, (nCounter + 1) * m_nItemHeight, 10, m_nItemHeight);

            if (_ShowInfoButton && Items.STNodeType != null)
                Items.InfoRectangle = new Rectangle(Width - 18 - (C_SCROLL_WIDTH + 2), Items.DisplayRectangle.Top + (m_nItemHeight - 14) / 2, 14, 14);
            else
                Items.InfoRectangle = Rectangle.Empty;

            OnDrawItem(dt, Items, nCounter++, nLevel);

            if (!Items.IsOpen)
                return nCounter;

            foreach (STNodeTreeCollection n in Items) {
                if (n.STNodeType == null)
                    nCounter = OnStartDrawItem(dt, n, nCounter++, nLevel + 1);
            }

            foreach (STNodeTreeCollection n in Items) {
                if (n.STNodeType != null)
                    nCounter = OnStartDrawItem(dt, n, nCounter++, nLevel + 1);
            }

            foreach (STNodeTreeCollection v in Items)
                OnDrawSwitch(dt, v);

            return nCounter;
        }

        /// <summary>
        /// Occurs when drawing each node of the tree node
        /// </summary>
        /// <param name="dt">drawing tool</param>
        /// <param name="items">the collection currently to be drawn</param>
        /// <param name="nCounter">A counter of the number drawn</param>
        /// <param name="nLevel">What level of sub-collection is currently in</param>
        protected virtual void OnDrawItem(DrawingTools dt, STNodeTreeCollection items, int nCounter, int nLevel) {
            Graphics g = dt.Graphics;
            m_brush.Color = nCounter % 2 == 0 ? m_clr_item_1 : m_clr_item_2;
            g.FillRectangle(m_brush, items.DisplayRectangle);

            if (items == m_item_hover) {
                m_brush.Color = _ItemHoverColor;
                g.FillRectangle(m_brush, items.DisplayRectangle);
            }

            Rectangle rect = new Rectangle(45 + nLevel * 10, items.SwitchRectangle.Top, Width - 45 - (nLevel * 10) - C_SCROLL_WIDTH, m_nItemHeight);
            m_pen.Color = Color.FromArgb(100, 125, 125, 125);
            g.DrawLine(
                m_pen,
                items.SwitchRectangle.Left + 5,
                items.SwitchRectangle.Top + m_nItemHeight / 2,
                items.SwitchRectangle.Left + 19,
                items.SwitchRectangle.Top + m_nItemHeight / 2
            );

            if (nCounter != 0)
                for (int i = 0; i <= nLevel; i++)
                    g.DrawLine(
                        m_pen,
                        9 + i * 10,
                        items.SwitchRectangle.Top - m_nItemHeight / 2,
                        9 + i * 10,
                        items.SwitchRectangle.Top + m_nItemHeight / 2
                    );

            OnDrawItemText(dt, items, rect);
            OnDrawItemIcon(dt, items, rect);
        }

        /// <summary>
        /// Occurs when drawing tree node expand and close switches
        /// </summary>
        /// <param name="dt">drawing tool</param>
        /// <param name="items">the collection currently to be drawn</param>
        protected virtual void OnDrawSwitch(DrawingTools dt, STNodeTreeCollection items) {
            Graphics g = dt.Graphics;

            if (items.Count != 0) {
                m_pen.Color = _SwitchColor;
                m_brush.Color = m_pen.Color;
                int nT = items.SwitchRectangle.Y + m_nItemHeight / 2 - 4;
                g.DrawRectangle(m_pen, items.SwitchRectangle.Left, nT, 8, 8);
                g.DrawLine(m_pen, items.SwitchRectangle.Left + 1, nT + 4, items.SwitchRectangle.Right - 3, nT + 4);

                if (items.IsOpen)
                    return;

                g.DrawLine(m_pen, items.SwitchRectangle.Left + 4, nT + 1, items.SwitchRectangle.Left + 4, nT + 7);
            }
        }

        /// <summary>
        /// Occurs when the text of a tree node is drawn
        /// </summary>
        /// <param name="dt">drawing tool</param>
        /// <param name="items">the collection currently to be drawn</param>
        /// <param name="rect">The rectangular area where the text field is located</param>
        protected virtual void OnDrawItemText(DrawingTools dt, STNodeTreeCollection items, Rectangle rect) {
            Graphics g = dt.Graphics;
            rect.Width -= 20;
            m_sf.FormatFlags = StringFormatFlags.NoWrap;

            if (!string.IsNullOrEmpty(m_str_search)) {
                int nIndex = items.NameLower.IndexOf(m_str_search);

                if (nIndex != -1) {
                    CharacterRange[] chrs = { new CharacterRange(nIndex, m_str_search.Length) };//global
                    m_sf.SetMeasurableCharacterRanges(chrs);
                    Region[] regions = g.MeasureCharacterRanges(items.Name, Font, rect, m_sf);
                    g.SetClip(regions[0], System.Drawing.Drawing2D.CombineMode.Intersect);
                    m_brush.Color = _HightLightTextColor;
                    g.DrawString(items.Name, Font, m_brush, rect, m_sf);
                    g.ResetClip();
                    g.SetClip(regions[0], System.Drawing.Drawing2D.CombineMode.Exclude);
                    m_brush.Color = items.STNodeType == null ? Color.FromArgb(ForeColor.A * 1 / 2, ForeColor) : ForeColor;
                    g.DrawString(items.Name, Font, m_brush, rect, m_sf);
                    g.ResetClip();
                    return;
                }
            }

            m_brush.Color = items.STNodeType == null ? Color.FromArgb(ForeColor.A * 2 / 3, ForeColor) : ForeColor;
            g.DrawString(items.Name, Font, m_brush, rect, m_sf);
        }

        /// <summary>
        /// Occurs when the tree node icon is drawn
        /// </summary>
        /// <param name="dt">drawing tool</param>
        /// <param name="items">the collection currently to be drawn</param>
        /// <param name="rect">The rectangular area where the text field is located</param>
        protected virtual void OnDrawItemIcon(DrawingTools dt, STNodeTreeCollection items, Rectangle rect) {
            Graphics g = dt.Graphics;

            if (items.STNodeType != null) {
                m_pen.Color = _AutoColor ? items.STNodeTypeColor : Color.DarkCyan;
                m_brush.Color = Color.LightGray;
                g.DrawRectangle(m_pen, rect.Left - 15, rect.Top + m_nItemHeight / 2 - 5, 11, 10);
                g.FillRectangle(m_brush, rect.Left - 17, rect.Top + m_nItemHeight / 2 - 2, 5, 5);
                g.FillRectangle(m_brush, rect.Left - 6, rect.Top + m_nItemHeight / 2 - 2, 5, 5);

                if (m_item_hover == items && m_bHoverInfo) {
                    m_brush.Color = BackColor;
                    g.FillRectangle(m_brush, items.InfoRectangle);
                }

                m_pen.Color = _AutoColor ? items.STNodeTypeColor : _InfoButtonColor;
                m_pen.Width = 2;
                g.DrawLine(m_pen, items.InfoRectangle.X + 4, items.InfoRectangle.Y + 3, items.InfoRectangle.X + 10, items.InfoRectangle.Y + 3);
                g.DrawLine(m_pen, items.InfoRectangle.X + 4, items.InfoRectangle.Y + 6, items.InfoRectangle.X + 10, items.InfoRectangle.Y + 6);
                g.DrawLine(m_pen, items.InfoRectangle.X + 4, items.InfoRectangle.Y + 11, items.InfoRectangle.X + 10, items.InfoRectangle.Y + 11);
                g.DrawLine(m_pen, items.InfoRectangle.X + 7, items.InfoRectangle.Y + 7, items.InfoRectangle.X + 7, items.InfoRectangle.Y + 10);
                m_pen.Width = 1;
                g.DrawRectangle(m_pen, items.InfoRectangle.X, items.InfoRectangle.Y, items.InfoRectangle.Width - 1, items.InfoRectangle.Height - 1);
            } else {
                if (items.IsLibraryRoot) {
                    Rectangle rect_box = new Rectangle(rect.Left - 15, rect.Top + m_nItemHeight / 2 - 5, 11, 10);
                    g.DrawRectangle(Pens.Gray, rect_box);
                    g.DrawLine(Pens.Cyan, rect_box.X - 2, rect_box.Top, rect_box.X + 2, rect_box.Top);
                    g.DrawLine(Pens.Cyan, rect_box.X, rect_box.Y - 2, rect_box.X, rect_box.Y + 2);
                    g.DrawLine(Pens.Cyan, rect_box.Right - 2, rect_box.Bottom, rect_box.Right + 2, rect_box.Bottom);
                    g.DrawLine(Pens.Cyan, rect_box.Right, rect_box.Bottom - 2, rect_box.Right, rect_box.Bottom + 2);
                } else {
                    g.DrawRectangle(Pens.Goldenrod, new Rectangle(rect.Left - 16, rect.Top + m_nItemHeight / 2 - 6, 8, 3));
                    g.DrawRectangle(Pens.Goldenrod, new Rectangle(rect.Left - 16, rect.Top + m_nItemHeight / 2 - 3, 13, 9));
                }

                if (!_ShowFolderCount)
                    return;

                m_sf.Alignment = StringAlignment.Far;
                m_brush.Color = _FolderCountColor;
                rect.X -= 4;
                g.DrawString("[" + items.STNodeCount.ToString() + "]", Font, m_brush, rect, m_sf);
                m_sf.Alignment = StringAlignment.Near;
            }
        }

        #endregion

        #region public method ==========
        /// <summary>
        /// Retrieve the STNode in the control
        /// </summary>
        /// <param name="strText">text to retrieve</param>
        public void Search(string strText) {
            if (strText == null)
                return;

            if (strText.Trim() == string.Empty)
                return;

            m_tbx.Text = strText.Trim();
        }

        /// <summary>
        /// Add an STNode type to the control
        /// </summary>
        /// <param name="stNodeType">STNode type</param>
        /// <returns>Is it added successfully?</returns>
        public bool AddNode(Type stNodeType) =>
            AddSTNode(stNodeType, m_items_source, null, true);

        /// <summary>
        /// Add the STNode type to the control from the file
        /// </summary>
        /// <param name="strFile">Specify the file path</param>
        /// <returns>Number of successful additions</returns>
        public int LoadAssembly(string strFile) {
            strFile = System.IO.Path.GetFullPath(strFile);
            var items = AddAssemblyPrivate(strFile);

            if (items.STNodeCount == 0)
                return 0;

            items.IsLibraryRoot = true;
            items.IsOpen = true;
            m_items_source[items.Name] = items;
            return items.STNodeCount;
        }

        /// <summary>
        /// Clear all STNode types in the control
        /// </summary>
        public void Clear() {
            m_items_source.Clear();
            m_items_draw.Clear();
            m_dic_all_type.Clear();
            Invalidate();
        }

        /// <summary>
        /// Remove an STNode type from the control
        /// </summary>
        /// <param name="stNodeType">STNode type</param>
        /// <returns>Whether the removal was successful</returns>
        public bool RemoveNode(Type stNodeType) {
            if (!m_dic_all_type.ContainsKey(stNodeType))
                return false;

            string strPath = m_dic_all_type[stNodeType];
            STNodeTreeCollection items = m_items_source;

            if (!string.IsNullOrEmpty(strPath)) {
                string[] strKeys = strPath.Split(m_chr_splitter);

                for (int i = 0; i < strKeys.Length; i++) {
                    items = items[strKeys[i]];

                    if (items == null)
                        return false;
                }
            }

            try {
                STNode node = (STNode)Activator.CreateInstance(stNodeType);

                if (items[node.Title] == null)
                    return false;

                items.Remove(node.Title, true);
                m_dic_all_type.Remove(stNodeType);
            } catch {
                return false;
            }

            Invalidate();
            return true;
        }

        #endregion

        //=================================================================================================
        /// <summary>
        /// A collection of each item in the STNodeTreeView control
        /// </summary>
        protected class STNodeTreeCollection : IEnumerable
        {
            private readonly string _Name;
            /// <summary>
            /// Get the display name of the current tree node
            /// </summary>
            public string Name {
                get { return _Name; }
            }

            /// <summary>
            /// Get the lowercase string of the display name of the current tree node
            /// </summary>
            public string NameLower { get; private set; }
            /// <summary>
            /// Get the STNode type corresponding to the current tree node
            /// </summary>
            public Type STNodeType { get; internal set; }
            /// <summary>
            /// Get the parent tree node of the current tree node
            /// </summary>
            public STNodeTreeCollection Parent { get; internal set; }

            /// <summary>
            /// Get the number of STNode types owned by the current tree node
            /// </summary>
            public int STNodeCount { get; internal set; }
            /// <summary>
            /// Get the corresponding path of the current tree node corresponding to the STNode type in the tree control
            /// </summary>
            public string Path { get; internal set; }
            /// <summary>
            /// Get the current or set whether the tree node is open
            /// </summary>
            public bool IsOpen { get; set; }
            /// <summary>
            /// Get whether the current tree node is the root path node of the loaded module
            /// </summary>
            public bool IsLibraryRoot { get; internal set; }
            /// <summary>
            /// Get the display area of the current tree node in the control
            /// </summary>
            public Rectangle DisplayRectangle { get; internal set; }
            /// <summary>
            /// Get the switch button area of the current tree node in the control
            /// </summary>
            public Rectangle SwitchRectangle { get; internal set; }
            /// <summary>
            /// Get the information button area of the current tree node in the control
            /// </summary>
            public Rectangle InfoRectangle { get; internal set; }
            /// <summary>
            /// Get the title color of the current tree node corresponding to the STNode type
            /// </summary>
            public Color STNodeTypeColor { get; internal set; }
            /// <summary>
            /// Get the number of child nodes contained in the current tree node
            /// </summary>
            public int Count { get { return m_dic.Count; } }

            /// <summary>
            /// Gets or sets the collection with the specified name
            /// </summary>
            /// <param name="strKey">specify name</param>
            /// <returns>Collection</returns>
            public STNodeTreeCollection this[string strKey] {
                get {
                    if (string.IsNullOrEmpty(strKey))
                        return null;

                    if (m_dic.ContainsKey(strKey))
                        return m_dic[strKey];

                    return null;
                }
                set {
                    if (string.IsNullOrEmpty(strKey))
                        return;

                    if (value == null)
                        return;

                    if (m_dic.ContainsKey(strKey)) {
                        m_dic[strKey] = value;
                    } else {
                        m_dic.Add(strKey, value);
                    }

                    value.Parent = this;
                }
            }

            private readonly SortedDictionary<string, STNodeTreeCollection> m_dic = new SortedDictionary<string, STNodeTreeCollection>();

            /// <summary>
            /// Construct a collection of tree nodes
            /// </summary>
            /// <param name="strName">The display name of the current tree node in the control</param>
            public STNodeTreeCollection(string strName) {
                if (strName == null || strName.Trim() == string.Empty)
                    throw new ArgumentNullException("Display name cannot be empty");

                _Name = strName.Trim();
                NameLower = _Name.ToLower();
            }

            /// <summary>
            /// Add a child node to the current tree node
            /// </summary>
            /// <param name="strName">node display name</param>
            /// <returns>Added set of child nodes</returns>
            public STNodeTreeCollection Add(string strName) {
                if (!m_dic.ContainsKey(strName))
                    m_dic.Add(strName, new STNodeTreeCollection(strName) { Parent = this });

                return m_dic[strName];
            }

            /// <summary>
            /// Delete a subcollection from the current tree node
            /// </summary>
            /// <param name="strName">subcollection name</param>
            /// <param name="isAutoDelFolder">Whether to automatically clear useless nodes recursively upward</param>
            /// <returns>Is it deleted successfully?</returns>
            public bool Remove(string strName, bool isAutoDelFolder) {
                if (!m_dic.ContainsKey(strName))
                    return false;

                bool b = m_dic.Remove(strName);
                var temp = this;

                while (temp != null) {
                    temp.STNodeCount--;
                    temp = temp.Parent;
                }

                if (isAutoDelFolder && m_dic.Count == 0 && Parent != null)
                    return b && Parent.Remove(Name, isAutoDelFolder);

                return b;
            }

            /// <summary>
            /// Clear all child nodes in the current tree node
            /// </summary>
            public void Clear() { Clear(this); }

            private void Clear(STNodeTreeCollection items) {
                foreach (STNodeTreeCollection v in items)
                    v.Clear(v);

                m_dic.Clear();
            }

            /// <summary>
            /// Get an array of all names in the current tree node
            /// </summary>
            /// <returns></returns>
            public string[] GetKeys() { return m_dic.Keys.ToArray(); }

            /// <summary>
            /// Copy all data in the current tree node set
            /// </summary>
            /// <returns>copy of copy</returns>
            public STNodeTreeCollection Copy() {
                STNodeTreeCollection items = new STNodeTreeCollection("COPY");
                Copy(this, items);
                return items;
            }

            private void Copy(STNodeTreeCollection items_src, STNodeTreeCollection items_dst) {
                foreach (STNodeTreeCollection v in items_src)
                    Copy(v, items_dst.Add(v.Name));

                items_dst.Path = items_src.Path;
                items_dst.STNodeType = items_src.STNodeType;
                items_dst.IsLibraryRoot = items_src.IsLibraryRoot;
                items_dst.STNodeCount = items_src.STNodeCount;
                items_dst.STNodeTypeColor = items_src.STNodeTypeColor;
            }

            /// <summary>
            /// Returns an Array of System.Collections.IEnumerator
            /// </summary>
            /// <returns></returns>
            public IEnumerator GetEnumerator() {
                foreach (var v in m_dic.Values)
                    yield return v;
            }

            IEnumerator IEnumerable.GetEnumerator() {
                return GetEnumerator();
            }
        }
    }
}
