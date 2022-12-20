using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;

namespace ST.Library.UI.NodeEditor
{
    /// <summary>
    /// STNode node attribute characteristics
    /// Used to describe STNode node attribute information and behavior on the attribute editor
    /// </summary>
    public class STNodePropertyAttribute : Attribute
    {
        private string _Name;
        /// <summary>
        /// Gets the name of the attribute that needs to be displayed on the attribute editor
        /// </summary>
        public string Name {
            get { return _Name; }
        }

        private string _Description;
        /// <summary>
        /// Gets the description that the attribute needs to display on the attribute editor
        /// </summary>
        public string Description {
            get { return _Description; }
        }

        private Type _ConverterType = typeof(STNodePropertyDescriptor);
        /// <summary>
        /// Get property descriptor type
        /// </summary>
        public Type DescriptorType {
            get { return _ConverterType; }
            set { _ConverterType = value; }
        }

        /// <summary>
        /// Constructs a STNode attribute property
        /// </summary>
        /// <param name="strKey">name to be displayed</param>
        /// <param name="strDesc">Descriptive information to be displayed</param>
        public STNodePropertyAttribute(string strKey, string strDesc) {
            _Name = strKey;
            _Description = strDesc;
        }
        //private Type m_descriptor_type_base = typeof(STNodePropertyDescriptor);
    }

    /// <summary>
    /// STNode property descriptor
    /// Used to determine how the property's value will be interacted with on the property editor and
    /// how the property value will be drawn and interacted with on the property editor
    /// </summary>
    public class STNodePropertyDescriptor
    {
        /// <summary>
        /// Get the target node
        /// </summary>
        public STNode Node { get; internal set; }
        /// <summary>
        /// Get the node property editor control to which it belongs
        /// </summary>
        public STNodePropertyGrid Control { get; internal set; }
        /// <summary>
        /// Get the region where the option is located
        /// </summary>
        public Rectangle Rectangle { get; internal set; }
        /// <summary>
        /// Get the region where the option name is located
        /// </summary>
        public Rectangle RectangleL { get; internal set; }
        /// <summary>
        /// Get the region where the option value is located
        /// </summary>
        public Rectangle RectangleR { get; internal set; }
        /// <summary>
        /// Get the displayed name of the option
        /// </summary>
        public string Name { get; internal set; }
        /// <summary>
        /// Get the description information corresponding to the attribute
        /// </summary>
        public string Description { get; internal set; }
        /// <summary>
        /// Get attribute information
        /// </summary>
        public PropertyInfo PropertyInfo { get; internal set; }

        private static Type m_t_int = typeof(int);
        private static Type m_t_uint = typeof(uint);
        private static Type m_t_float = typeof(float);
        private static Type m_t_double = typeof(double);
        private static Type m_t_char = typeof(char);
        private static Type m_t_string = typeof(string);
        private static Type m_t_bool = typeof(bool);

        private StringFormat m_sf;

        /// <summary>
        /// construct a descriptor
        /// </summary>
        public STNodePropertyDescriptor() {
            m_sf = new StringFormat
            {
                LineAlignment = StringAlignment.Center,
                FormatFlags = StringFormatFlags.NoWrap
            };
        }

        /// <summary>
        /// Occurs when determining the position of the STNode property on the property editor
        /// </summary>
        protected internal virtual void OnSetItemLocation() { }

        /// <summary>
        /// Converts a property value as a string to a value of the property's target type
        /// By default, only int float double string bool and Array of the above types are supported
        /// If the target type is not in the above, please rewrite this function to convert it yourself
        /// </summary>
        /// <param name="strText">property value as a string</param>
        /// <returns>The value of the property's true target type</returns>
        protected internal virtual object GetValueFromString(string strText) {
            Type t = PropertyInfo.PropertyType;

            if (t == m_t_int)
                return int.Parse(strText);

            if (t == m_t_uint)
                return uint.Parse(strText);

            if (t == m_t_float)
                return float.Parse(strText);

            if (t == m_t_double)
                return double.Parse(strText);

            if (t == m_t_char)
                return strText[0];

            if (t == m_t_string)
                return strText;

            if (t == m_t_bool)
                return bool.Parse(strText);

            if (t.IsEnum)
                return Enum.Parse(t, strText);

            if (t.IsArray) {
                var t_1 = t.GetElementType();

                if (t_1 == m_t_string)
                    return strText.Split(',');

                string[] strs = strText.Trim(new char[] { ' ', ',' }).Split(',');//add other place trim()

                if (t_1 == m_t_int)
                {
                    int[] arr = new int[strs.Length];

                    for (int i = 0; i < strs.Length; i++)
                        arr[i] = int.Parse(strs[i].Trim());

                    return arr;
                }

                if (t_1 == m_t_uint) {
                    uint[] arr = new uint[strs.Length];

                    for (int i = 0; i < strs.Length; i++)
                        arr[i] = uint.Parse(strs[i].Trim());

                    return arr;
                }

                if (t_1 == m_t_float) {
                    float[] arr = new float[strs.Length];

                    for (int i = 0; i < strs.Length; i++)
                        arr[i] = float.Parse(strs[i].Trim());

                    return arr;
                }

                if (t_1 == m_t_double) {
                    double[] arr = new double[strs.Length];

                    for (int i = 0; i < strs.Length; i++)
                        arr[i] = double.Parse(strs[i].Trim());

                    return arr;
                }

                if (t_1 == m_t_char) {
                    char[] arr = new char[strs.Length];

                    for (int i = 0; i < strs.Length; i++)
                        arr[i] = char.Parse(strs[i].Trim());

                    return arr;
                }

                if (t_1 == m_t_bool) {
                    bool[] arr = new bool[strs.Length];

                    for (int i = 0; i < strs.Length; i++)
                        arr[i] = bool.Parse(strs[i].Trim());

                    return arr;
                }
            }

            throw new InvalidCastException("Could not complete [string] to [" + t.FullName + "] Conversion Please overload [STNodePropertyDescriptor.GetValueFromString(string)]");
        }

        /// <summary>
        /// Converts the value of the property's target type to a value in string form
        /// Perform ToString() operation on type value by default
        /// If special processing is required, please rewrite this function to convert it yourself
        /// </summary>
        /// <returns>The string form of the attribute value</returns>
        protected internal virtual string GetStringFromValue() {
            var v = PropertyInfo.GetValue(Node, null);
            var t = PropertyInfo.PropertyType;

            if (v == null)
                return null;

            if (t.IsArray) {
                List<string> lst = new List<string>();

                foreach (var item in (Array)v)
                    lst.Add(item.ToString());

                return string.Join(",", lst.ToArray());
            }

            return v.ToString();
        }

        /// <summary>
        /// Converts a property value in binary form to a value of the property target type used to restore property values from data in a file store
        /// Convert it to a string by default and then call GetValueFromString(string)
        /// This function corresponds to GetBytesFromValue() If the function needs to be rewritten, the two functions should be rewritten together
        /// </summary>
        /// <param name="byData">binary data</param>
        /// <returns>The value of the property's true target type</returns>
        protected internal virtual object GetValueFromBytes(byte[] byData) {
            if (byData == null)
                return null;

            string strText = Encoding.UTF8.GetString(byData);
            return GetValueFromString(strText);
        }

        /// <summary>
        /// Convert the value of the attribute target type to a value in binary form, which is called when storing the file
        /// By default calls GetStringFromValue() and then converts the string to binary data
        /// If special processing is required, please rewrite this function to convert by yourself and rewrite GetValueFromBytes()
        /// </summary>
        /// <returns>The binary form of the attribute value</returns>
        protected internal virtual byte[] GetBytesFromValue() {
            string strText = GetStringFromValue();

            if (strText == null)
                return null;

            return Encoding.UTF8.GetBytes(strText);
        }

        /// <summary>
        /// This function corresponds to System.Reflection.PropertyInfo.GetValue()
        /// </summary>
        /// <param name="index">optional index value for indexed properties for non-indexed properties this value should be null</param>
        /// <returns>attribute value</returns>
        protected internal virtual object GetValue(object[] index) {
            return PropertyInfo.GetValue(Node, index);
        }

        /// <summary>
        /// This function corresponds to System.Reflection.PropertyInfo.SetValue()
        /// </summary>
        /// <param name="value">The attribute value that needs to be set</param>
        protected internal virtual void SetValue(object value) {
            PropertyInfo.SetValue(Node, value, null);
        }

        /// <summary>
        /// This function corresponds to System.Reflection.PropertyInfo.SetValue()
        /// GetValueFromString(strValue) will be processed by default before calling
        /// </summary>
        /// <param name="strValue">The value of the attribute string that needs to be set</param>
        protected internal virtual void SetValue(string strValue) {
            PropertyInfo.SetValue(Node, GetValueFromString(strValue), null);
        }

        /// <summary>
        /// This function corresponds to System.Reflection.PropertyInfo.SetValue()
        /// GetValueFromBytes(byte[]) will be processed by default before calling
        /// </summary>
        /// <param name="byData">The attribute binary data that needs to be set</param>
        protected internal virtual void SetValue(byte[] byData) {
            PropertyInfo.SetValue(Node, GetValueFromBytes(byData), null);
        }

        /// <summary>
        /// This function corresponds to System.Reflection.PropertyInfo.SetValue()
        /// </summary>
        /// <param name="value">The attribute value that needs to be set</param>
        /// <param name="index">optional index value for indexed properties for non-indexed properties this value should be null</param>
        protected internal virtual void SetValue(object value, object[] index) {
            PropertyInfo.SetValue(Node, value, index);
        }

        /// <summary>
        /// This function corresponds to System.Reflection.PropertyInfo.SetValue()
        /// GetValueFromString(strValue) will be processed by default before calling
        /// </summary>
        /// <param name="strValue">The value of the attribute string that needs to be set</param>
        /// <param name="index">optional index value for indexed properties for non-indexed properties this value should be null</param>
        protected internal virtual void SetValue(string strValue, object[] index) {
            PropertyInfo.SetValue(Node, GetValueFromString(strValue), index);
        }

        /// <summary>
        /// This function corresponds to System.Reflection.PropertyInfo.SetValue()
        /// GetValueFromBytes(byte[]) will be processed by default before calling
        /// </summary>
        /// <param name="byData">The attribute binary data that needs to be set</param>
        /// <param name="index">optional index value for indexed properties for non-indexed properties this value should be null</param>
        protected internal virtual void SetValue(byte[] byData, object[] index) {
            PropertyInfo.SetValue(Node, GetValueFromBytes(byData), index);
        }

        /// <summary>
        /// Occurs when an error occurs while setting a property value
        /// </summary>
        /// <param name="ex">exception information</param>
        protected internal virtual void OnSetValueError(Exception ex) {
            Control.SetErrorMessage(ex.Message);
        }

        /// <summary>
        /// Occurs when painting the area where the property's value is located on the property editor
        /// </summary>
        /// <param name="dt">drawing tool</param>
        protected internal virtual void OnDrawValueRectangle(DrawingTools dt) {
            Graphics g = dt.Graphics;
            SolidBrush brush = dt.SolidBrush;
            STNodePropertyGrid ctrl = Control;
            //STNodePropertyItem item = _PropertyItem;
            brush.Color = ctrl.ItemValueBackColor;

            g.FillRectangle(brush, RectangleR);
            Rectangle rect = RectangleR;
            rect.Width--;
            rect.Height--;
            brush.Color = Control.ForeColor;
            g.DrawString(GetStringFromValue(), ctrl.Font, brush, RectangleR, m_sf);

            if (PropertyInfo.PropertyType.IsEnum || PropertyInfo.PropertyType == m_t_bool)
                g.FillPolygon(Brushes.Gray, new Point[]{
                        new Point(rect.Right - 13, rect.Top + rect.Height / 2 - 2),
                        new Point(rect.Right - 4, rect.Top + rect.Height / 2 - 2),
                        new Point(rect.Right - 9, rect.Top + rect.Height / 2 + 3)
                    });
        }

        /// <summary>
        /// Occurs when the mouse enters the area where the property value is located
        /// </summary>
        /// <param name="e">event parameters</param>
        protected internal virtual void OnMouseEnter(EventArgs e) { }

        /// <summary>
        /// Occurs when the mouse clicks on the area where the attribute value is located
        /// </summary>
        /// <param name="e">event parameters</param>
        protected internal virtual void OnMouseDown(MouseEventArgs e) { }

        /// <summary>
        /// Occurs when the mouse moves over the area where the property value is located
        /// </summary>
        /// <param name="e">event parameters</param>
        protected internal virtual void OnMouseMove(MouseEventArgs e) { }

        /// <summary>
        /// Occurs when the mouse is lifted over the area where the property value is located
        /// </summary>
        /// <param name="e">event parameters</param>
        protected internal virtual void OnMouseUp(MouseEventArgs e) { }

        /// <summary>
        /// Occurs when the mouse leaves the area where the property value is located
        /// </summary>
        /// <param name="e">event parameters</param>
        protected internal virtual void OnMouseLeave(EventArgs e) { }

        /// <summary>
        /// Occurs when the mouse clicks on the area where the attribute value is located
        /// </summary>
        /// <param name="e">event parameters</param>
        protected internal virtual void OnMouseClick(MouseEventArgs e) {
            Type t = PropertyInfo.PropertyType;

            if (t == m_t_bool || t.IsEnum) {
                new FrmSTNodePropertySelect(this).Show(Control);
                return;
            }

            Rectangle rect = Control.RectangleToScreen(RectangleR);
            new FrmSTNodePropertyInput(this).Show(Control);
        }

        /// <summary>
        /// redraw options area
        /// </summary>
        public void Invalidate() {
            Rectangle rect = Rectangle;
            rect.X -= Control.ScrollOffset;
            Control.Invalidate(rect);
        }
    }
}
