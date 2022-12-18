using ST.Library.UI.NodeEditor;
using System.Drawing;
using SysColor = System.Drawing.Color;
using SysMath = System.Math;

namespace ChattyVibes.Nodes.Graphics.Color
{
    //[STNode("/Graphics/Color", "LauraRozier", "", "", "This is a color mix node")]
    internal class ColorMixNode : STNode
    {
        private const byte C_RANGE = 255;         /* Zero-based maximum value. */
        private const float C_F_RANGE = 255.0f;   /* Convenient floating-point version of range. */
        private const int C_CMP_RANGE = 254;
        private const int C_EXPANDED_RANGE = 256; /* One-based maximum value. */
        private const float C_EPS_SATURATION = 0.0005f;
        private readonly static float[] C_LUMA_COEF = new float[3] { 0.0f, 0.0f, 0.0f };

        private ColorMixType _mixType;
        [STNodeProperty("MixType", "This is MixType")]
        public ColorMixType MixType
        {
            get { return _mixType; }
            set
            {
                _mixType = value;
                m_ctrl_select.Enum = value;
                ProcessColor();
            }
        }

        private int _fac = 127;
        [STNodeProperty("Fac", "This is Fac")]
        public int Fac
        {
            get { return _fac; }
            set
            {
                if (value < 0)
                    value = 0;

                if (value > C_CMP_RANGE)
                    value = C_RANGE;

                _fac = value;
                m_ctrl_progess.Value = value;
                ProcessColor();
            }
        }

        private SysColor _color1 = SysColor.LightGray;
        [STNodeProperty("Color1", "This is color1", DescriptorType = typeof(DescriptorForColor))]
        public SysColor Color1
        {
            get { return _color1; }
            set
            {
                _color1 = value;
                m_ctrl_btn_1.BackColor = value;
                ProcessColor();
            }
        }

        private SysColor _color2 = SysColor.LightGray;
        [STNodeProperty("Color2", "This is color2", DescriptorType = typeof(DescriptorForColor))]
        public SysColor Color2
        {
            get { return _color2; }
            set
            {
                _color2 = value;
                m_ctrl_btn_2.BackColor = value;
                ProcessColor();
            }
        }

        public enum ColorMixType
        {
            /**
             * Addition mode is very simple.
             * The pixel values of the upper and lower layers are added to each other.
             * The resulting image is usually lighter.
             * The equation can result in color values greater than 255, so some of the light colors may be set to the maximum value of 255.
             */
            Add,
            /**
             * Subtract mode subtracts the pixel values of the upper layer from the pixel values of the lower layer.
             * The resulting image is normally darker.
             * You might get a lot of black or near-black in the resulting image.
             * The equation can result in negative color values, so some of the dark colors may be set to the minimum value of 0.
             */
            Subtract,
            /**
             * Multiply mode multiplies the pixel values of the upper layer with those of the layer below it and then divides the result by 255.
             * The result is usually a darker image.
             * If either layer is white, the resulting image is the same as the other layer (1 * I = I).
             * If either layer is black, the resulting image is completely black (0 * I = 0).
             */
            Multiply,
            /**
             * Screen mode inverts the values of each of the visible pixels in the two layers of the image.
             * (That is, it subtracts each of them from 255.)
             * Then it multiplies them together, divides by 255 and inverts this value again.
             * The resulting image is usually brighter, and sometimes “washed out” in appearance.
             * The exceptions to this are a black layer, which does not change the other layer, and a white layer, which results in a white image.
             * Darker colors in the image appear to be more transparent.
             */
            Screen,
            /**
             * Difference mode subtracts the pixel value of the upper layer from that of the lower layer and then takes the absolute value of the result.
             * No matter what the original two layers look like, the result looks rather odd.
             * You can use it to invert elements of an image.
             */
            Difference,
            /**
             * Darken only mode compares each component of each pixel in the upper layer with the corresponding one in the lower layer and uses the
             * smaller value in the resulting image.
             * Completely white layers have no effect on the final image and completely black layers result in a black image.
             */
            Darken,
            /**
             * Lighten only mode compares each component of each pixel in the upper layer with the corresponding one in the lower layer and uses the
             * larger value in the resulting image.
             * Completely black layers have no effect on the final image and completely white layers result in a white image.
             */
            Lighten,
            /**
             * Overlay mode inverts the pixel value of the lower layer, multiplies it by two times the pixel value of the upper layer, adds that to
             * the original pixel value of the lower layer, divides by 255, and then multiplies by the pixel value of the original lower layer and
             * divides by 255 again.
             * It darkens the image, but not as much as with “Multiply” mode.
             */
            Overlay,
            /**
             * Dodge mode multiplies the pixel value of the lower layer by 256, then divides that by the inverse of the pixel value of the top layer.
             * The resulting image is usually lighter, but some colors may be inverted.
             */
            ColorDodge,
            /**
             * Burn mode inverts the pixel value of the lower layer, multiplies it by 256, divides that by one plus the pixel value of the upper layer,
             * then inverts the result.
             * It tends to make the image darker, somewhat similar to “Multiply” mode.
             */
            ColorBurn,
            /** HSV TODO */
            Hue,
            /** HSV TODO */
            Saturation,
            /** HSV TODO */
            Value,
            /** HSV TODO */
            Color,
            /**
             * Soft light is not related to “Hard light” in anything but the name, but it does tend to make the edges softer and the colors not so bright.
             * It is similar to “Overlay” mode.
             * In some versions of GIMP, “Overlay” mode and “Soft light” mode are identical.
             */
            SoftLight,
            /** Contrast TODO */
            LinearLight
        }

        private NodeSelectEnumBox m_ctrl_select;
        private NodeProgress m_ctrl_progess;
        private NodeColorButton m_ctrl_btn_1;
        private NodeColorButton m_ctrl_btn_2;

        private STNodeOption m_in_fac;
        private STNodeOption m_in_color1;
        private STNodeOption m_in_color2;
        private STNodeOption m_out_color;

        protected override void OnCreate()
        {
            base.OnCreate();
            TitleColor = SysColor.FromArgb(200, FrmBindingGraphs.C_COLOR_COLOR);
            Title = "Mix RGB";
            AutoSize = false;
            Width = 140;
            Height = 127;

            InputOptions.Add(STNodeOption.Empty);
            InputOptions.Add(STNodeOption.Empty);
            m_in_fac = InputOptions.Add("", typeof(float), true);
            m_in_color1 = InputOptions.Add("Color1", typeof(SysColor), true);
            m_in_color2 = InputOptions.Add("Color2", typeof(SysColor), true);
            m_out_color = OutputOptions.Add("Color", typeof(SysColor), true);

            m_in_fac.DataTransfer += new STNodeOptionEventHandler(InNumDataTransfer);
            m_in_color1.DataTransfer += new STNodeOptionEventHandler(InColorDataTransfer);
            m_in_color2.DataTransfer += new STNodeOptionEventHandler(InColorDataTransfer);

            m_out_color.TransferData(SysColor.LightGray);

            m_ctrl_progess = new NodeProgress
            {
                Text = "Fac",
                DisplayRectangle = new Rectangle(10, 42, 120, 16),
                MinValue = 0,
                MaxValue = C_RANGE,
                Value = 127
            };
            m_ctrl_progess.ValueChanged += (s, e) =>
            {
                _fac = m_ctrl_progess.Value;
                ProcessColor();
            };
            Controls.Add(m_ctrl_progess);

            m_ctrl_btn_1 = new NodeColorButton
            {
                Text = "",
                BackColor = _color1,
                DisplayRectangle = new Rectangle(80, 62, 50, 16)
            };
            m_ctrl_btn_1.ValueChanged += (s, e) =>
            {
                _color1 = m_ctrl_btn_1.BackColor;
                ProcessColor();
            };
            Controls.Add(m_ctrl_btn_1);

            m_ctrl_btn_2 = new NodeColorButton
            {
                Text = "",
                BackColor = _color2,
                DisplayRectangle = new Rectangle(80, 82, 50, 16)
            };
            m_ctrl_btn_2.ValueChanged += (s, e) =>
            {
                _color2 = m_ctrl_btn_2.BackColor;
                ProcessColor();
            };
            Controls.Add(m_ctrl_btn_2);

            m_ctrl_select = new NodeSelectEnumBox
            {
                DisplayRectangle = new Rectangle(10, 21, 120, 18),
                Enum = _mixType
            };
            m_ctrl_select.ValueChanged += (s, e) =>
            {
                _mixType = (ColorMixType)m_ctrl_select.Enum;
                ProcessColor();
            };
            Controls.Add(m_ctrl_select);
        }

        void ProcessColor()
        {
            if (_fac == 0)
            {
                m_out_color.TransferData(_color1);
                Invalidate();
                return;
            }

            SysColor outColor = SysColor.Empty;
            int tmpR, tmpG, tmpB, tmpA;
            int mfac = C_RANGE - _fac;

            switch (_mixType)
            {
                case ColorMixType.Add:
                    {
                        tmpA = _color1.A + (int)SysMath.Round((_fac * _color2.A) / (float)C_RANGE);
                        tmpR = _color1.R + (int)SysMath.Round((_fac * _color2.R) / (float)C_RANGE);
                        tmpG = _color1.G + (int)SysMath.Round((_fac * _color2.G) / (float)C_RANGE);
                        tmpB = _color1.B + (int)SysMath.Round((_fac * _color2.B) / (float)C_RANGE);
                        outColor = SysColor.FromArgb(
                            (tmpA > C_CMP_RANGE) ? C_RANGE : tmpA,
                            (tmpR > C_CMP_RANGE) ? C_RANGE : tmpR,
                            (tmpG > C_CMP_RANGE) ? C_RANGE : tmpG,
                            (tmpB > C_CMP_RANGE) ? C_RANGE : tmpB
                        );
                        break;
                    }
                case ColorMixType.Subtract:
                    {
                        tmpA = _color1.A - (int)SysMath.Round((_fac * _color2.A) / (float)C_RANGE);
                        tmpR = _color1.R - (int)SysMath.Round((_fac * _color2.R) / (float)C_RANGE);
                        tmpG = _color1.G - (int)SysMath.Round((_fac * _color2.G) / (float)C_RANGE);
                        tmpB = _color1.B - (int)SysMath.Round((_fac * _color2.B) / (float)C_RANGE);
                        outColor = SysColor.FromArgb(
                            (tmpA < 0) ? 0 : tmpA,
                            (tmpR < 0) ? 0 : tmpR,
                            (tmpG < 0) ? 0 : tmpG,
                            (tmpB < 0) ? 0 : tmpB
                        );
                        break;
                    }
                case ColorMixType.Multiply:
                    {
                        tmpA = (int)SysMath.Round((mfac * _color1.A * C_RANGE + _fac * _color2.A * _color1.A) / (float)(C_RANGE * C_RANGE));
                        tmpR = (int)SysMath.Round((mfac * _color1.R * C_RANGE + _fac * _color2.R * _color1.R) / (float)(C_RANGE * C_RANGE));
                        tmpG = (int)SysMath.Round((mfac * _color1.G * C_RANGE + _fac * _color2.G * _color1.G) / (float)(C_RANGE * C_RANGE));
                        tmpB = (int)SysMath.Round((mfac * _color1.B * C_RANGE + _fac * _color2.B * _color1.B) / (float)(C_RANGE * C_RANGE));
                        outColor = SysColor.FromArgb(tmpA, tmpR, tmpG, tmpB);
                        break;
                    }
                case ColorMixType.Screen:
                    {
                        tmpA = (int)SysMath.Max(C_RANGE - (((C_RANGE - _color1.A) * (C_RANGE - _color2.A)) / C_RANGE), 0.0f);
                        tmpR = (int)SysMath.Max(C_RANGE - (((C_RANGE - _color1.R) * (C_RANGE - _color2.R)) / C_RANGE), 0.0f);
                        tmpG = (int)SysMath.Max(C_RANGE - (((C_RANGE - _color1.G) * (C_RANGE - _color2.G)) / C_RANGE), 0.0f);
                        tmpB = (int)SysMath.Max(C_RANGE - (((C_RANGE - _color1.B) * (C_RANGE - _color2.B)) / C_RANGE), 0.0f);
                        outColor = SysColor.FromArgb(
                            (mfac * _color1.A + tmpA * _fac) / C_RANGE,
                            (mfac * _color1.R + tmpR * _fac) / C_RANGE,
                            (mfac * _color1.G + tmpG * _fac) / C_RANGE,
                            (mfac * _color1.B + tmpB * _fac) / C_RANGE
                        );
                        break;
                    }
                case ColorMixType.Difference:
                    {
                        tmpA = SysMath.Abs(_color1.A - _color2.A);
                        tmpR = SysMath.Abs(_color1.R - _color2.R);
                        tmpG = SysMath.Abs(_color1.G - _color2.G);
                        tmpB = SysMath.Abs(_color1.B - _color2.B);
                        outColor = SysColor.FromArgb(
                            (mfac * _color1.A + tmpA * _fac) / C_RANGE,
                            (mfac * _color1.R + tmpR * _fac) / C_RANGE,
                            (mfac * _color1.G + tmpG * _fac) / C_RANGE,
                            (mfac * _color1.B + tmpB * _fac) / C_RANGE
                        );
                        break;
                    }
                case ColorMixType.Darken:
                    {
                        if (_fac >= C_RANGE)
                        {
                            outColor = SysColor.FromArgb(_color2.ToArgb());
                            break;
                        }

                        /* See if we're lighter, if so mix, else don't do anything.  If the paint color is lighter then the original, ignore */
                        if (GetLuminance(_color1) < GetLuminance(_color2))
                        {
                            outColor = SysColor.FromArgb(_color1.ToArgb());
                            break;
                        }

                        tmpA = (int)SysMath.Round((mfac * _color1.A + _fac * _color2.A) / (float)C_RANGE);
                        tmpR = (int)SysMath.Round((mfac * _color1.R + _fac * _color2.R) / (float)C_RANGE);
                        tmpG = (int)SysMath.Round((mfac * _color1.G + _fac * _color2.G) / (float)C_RANGE);
                        tmpB = (int)SysMath.Round((mfac * _color1.B + _fac * _color2.B) / (float)C_RANGE);
                        outColor = SysColor.FromArgb(tmpA, tmpR, tmpG, tmpB);
                        break;
                    }
                case ColorMixType.Lighten:
                    {
                        if (_fac >= C_RANGE)
                        {
                            outColor = SysColor.FromArgb(_color2.ToArgb());
                            break;
                        }

                        /* See if we're lighter, if so mix, else don't do anything.  If the paint color is darker then the original, ignore */
                        if (GetLuminance(_color1) > GetLuminance(_color2))
                        {
                            outColor = SysColor.FromArgb(_color1.ToArgb());
                            break;
                        }

                        tmpA = (int)SysMath.Round((mfac * _color1.A + _fac * _color2.A) / (float)C_RANGE);
                        tmpR = (int)SysMath.Round((mfac * _color1.R + _fac * _color2.R) / (float)C_RANGE);
                        tmpG = (int)SysMath.Round((mfac * _color1.G + _fac * _color2.G) / (float)C_RANGE);
                        tmpB = (int)SysMath.Round((mfac * _color1.B + _fac * _color2.B) / (float)C_RANGE);
                        outColor = SysColor.FromArgb(tmpA, tmpR, tmpG, tmpB);
                        break;
                    }
                case ColorMixType.Overlay:
                    {
                        int ProcChannel(int a, int b)
                        {
                            int tmp = (a > (C_RANGE / 2))
                                ? C_RANGE - ((C_RANGE - 2 * (a - (C_RANGE / 2))) * (C_RANGE - b) / C_RANGE)
                                : (2 * b * a) / C_EXPANDED_RANGE;
                            return (int)SysMath.Min((mfac * a + tmp * _fac) / C_RANGE, (float)C_RANGE);
                        }

                        tmpA = ProcChannel(_color1.A, _color2.A);
                        tmpR = ProcChannel(_color1.R, _color2.R);
                        tmpG = ProcChannel(_color1.G, _color2.G);
                        tmpB = ProcChannel(_color1.B, _color2.B);
                        outColor = SysColor.FromArgb(tmpA, tmpR, tmpG, tmpB);
                        break;
                    }
                case ColorMixType.ColorDodge:
                    {
                        int dodgefac = (int)(C_RANGE * 0.885f); /* ~225/255 */
                        tmpA = (_color2.A == C_RANGE) ? C_RANGE
                            : (int)SysMath.Min((_color1.A * dodgefac) / (C_RANGE - _color2.A), (float)C_RANGE);
                        tmpR = (_color2.R == C_RANGE) ? C_RANGE
                            : (int)SysMath.Min((_color1.R * dodgefac) / (C_RANGE - _color2.R), (float)C_RANGE);
                        tmpG = (_color2.G == C_RANGE) ? C_RANGE
                            : (int)SysMath.Min((_color1.G * dodgefac) / (C_RANGE - _color2.G), (float)C_RANGE);
                        tmpB = (_color2.B == C_RANGE) ? C_RANGE
                            : (int)SysMath.Min((_color1.B * dodgefac) / (C_RANGE - _color2.B), (float)C_RANGE);
                        outColor = SysColor.FromArgb(
                            (mfac * _color1.A + tmpA * _fac) / C_RANGE,
                            (mfac * _color1.R + tmpR * _fac) / C_RANGE,
                            (mfac * _color1.G + tmpG * _fac) / C_RANGE,
                            (mfac * _color1.B + tmpB * _fac) / C_RANGE
                        );
                        break;
                    }
                case ColorMixType.ColorBurn:
                    {
                        int ProcChannel(int a, int b)
                        {
                            int tmp = (b == 0) ? 0
                                : SysMath.Max(C_RANGE - ((C_RANGE - a) * C_RANGE) / b, 0);
                            return (int)((tmp * _fac + a * mfac) / C_RANGE);
                        }

                        tmpR = ProcChannel(_color1.R, _color2.R);
                        tmpG = ProcChannel(_color1.G, _color2.G);
                        tmpB = ProcChannel(_color1.B, _color2.B);
                        outColor = SysColor.FromArgb(_color1.A, tmpR, tmpG, tmpB);
                        break;
                    }
                case ColorMixType.Hue:
                    {
                        float h1 = 0, s1 = 0, v1 = 0;
                        float h2 = 0, s2 = 0, v2 = 0;
                        float r = 0, g = 0, b = 0;

                        RgbToHsv(
                            _color1.R / C_F_RANGE, _color1.G / C_F_RANGE, _color1.B / C_F_RANGE,
                            ref h1, ref s1, ref v1
                        );
                        RgbToHsv(
                            _color2.R / C_F_RANGE, _color2.G / C_F_RANGE, _color2.B / C_F_RANGE,
                            ref h2, ref s2,  ref v2
                        );

                        h1 = h2;

                        HsvToRgb(h1, s1, v1, ref r, ref g, ref b);
                        outColor = SysColor.FromArgb(
                            ((int)(_color2.A)     * _fac + mfac * _color1.A) / C_RANGE,
                            ((int)(r * C_F_RANGE) * _fac + mfac * _color1.R) / C_RANGE,
                            ((int)(g * C_F_RANGE) * _fac + mfac * _color1.G) / C_RANGE,
                            ((int)(b * C_F_RANGE) * _fac + mfac * _color1.B) / C_RANGE
                        );
                        break;
                    }
                case ColorMixType.Saturation:
                    {
                        float h1 = 0, s1 = 0, v1 = 0;
                        float h2 = 0, s2 = 0, v2 = 0;
                        float r = 0, g = 0, b = 0;

                        RgbToHsv(
                            _color1.R / C_F_RANGE, _color1.G / C_F_RANGE, _color1.B / C_F_RANGE,
                            ref h1, ref s1, ref v1
                        );
                        RgbToHsv(
                            _color2.R / C_F_RANGE, _color2.G / C_F_RANGE, _color2.B / C_F_RANGE,
                            ref h2, ref s2, ref v2
                        );

                        if (s1 > C_EPS_SATURATION)
                            s1 = s2;

                        HsvToRgb(h1, s1, v1, ref r, ref g, ref b);
                        outColor = SysColor.FromArgb(
                            _color1.A,
                            ((int)(r * C_F_RANGE) * _fac + mfac * _color1.R) / C_RANGE,
                            ((int)(g * C_F_RANGE) * _fac + mfac * _color1.G) / C_RANGE,
                            ((int)(b * C_F_RANGE) * _fac + mfac * _color1.B) / C_RANGE
                        );
                        break;
                    }
                case ColorMixType.Value:
                    {
                        float h1 = 0, s1 = 0, v1 = 0;
                        float h2 = 0, s2 = 0, v2 = 0;
                        float r = 0, g = 0, b = 0;

                        RgbToHsv(
                            _color1.R / C_F_RANGE, _color1.G / C_F_RANGE, _color1.B / C_F_RANGE,
                            ref h1, ref s1, ref v1
                        );
                        RgbToHsv(
                            _color2.R / C_F_RANGE, _color2.G / C_F_RANGE, _color2.B / C_F_RANGE,
                            ref h2, ref s2, ref v2
                        );

                        v1 = v2;

                        HsvToRgb(h1, s1, v1, ref r, ref g, ref b);
                        outColor = SysColor.FromArgb(
                            ((int)(_color2.A)     * _fac + mfac * _color1.A) / C_RANGE,
                            ((int)(r * C_F_RANGE) * _fac + mfac * _color1.R) / C_RANGE,
                            ((int)(g * C_F_RANGE) * _fac + mfac * _color1.G) / C_RANGE,
                            ((int)(b * C_F_RANGE) * _fac + mfac * _color1.B) / C_RANGE
                        );
                        break;
                    }
                case ColorMixType.Color:
                    {
                        float h1 = 0, s1 = 0, v1 = 0;
                        float h2 = 0, s2 = 0, v2 = 0;
                        float r = 0, g = 0, b = 0;

                        RgbToHsv(
                            _color1.R / C_F_RANGE, _color1.G / C_F_RANGE, _color1.B / C_F_RANGE,
                            ref h1, ref s1, ref v1
                        );
                        RgbToHsv(
                            _color2.R / C_F_RANGE, _color2.G / C_F_RANGE, _color2.B / C_F_RANGE,
                            ref h2, ref s2, ref v2
                        );

                        h1 = h2;
                        s1 = s2;

                        HsvToRgb(h1, s1, v1, ref r, ref g, ref b);
                        outColor = SysColor.FromArgb(
                            _color1.A,
                            ((int)(r * C_F_RANGE) * _fac + _color1.R * mfac) / C_RANGE,
                            ((int)(g * C_F_RANGE) * _fac + _color1.G * mfac) / C_RANGE,
                            ((int)(b * C_F_RANGE) * _fac + _color1.B * mfac) / C_RANGE
                        );
                        break;
                    }
                case ColorMixType.SoftLight:
                    {
                        int add = (int)SysMath.Round(C_RANGE / 4.0f);

                        int ProcChannel(int a, int b)
                        {
                            int tmp = (a < (C_RANGE / 2))
                                ? ((2 * ((b / 2) + add)) * a) / C_RANGE
                                : C_RANGE - (2 * (C_RANGE - ((b / 2) + add)) * (C_RANGE - a) / C_RANGE);
                            return (int)((tmp * _fac + a * mfac) / C_RANGE);
                        }

                        tmpA = ProcChannel(_color1.A, _color2.A);
                        tmpR = ProcChannel(_color1.R, _color2.R);
                        tmpG = ProcChannel(_color1.G, _color2.G);
                        tmpB = ProcChannel(_color1.B, _color2.B);
                        outColor = SysColor.FromArgb(tmpA, tmpR, tmpG, tmpB);
                        break;
                    }
                case ColorMixType.LinearLight:
                    {
                        int cmp = C_RANGE / 2;

                        int ProcChannel(int a, int b)
                        {
                            int tmp = (b > cmp)
                                ? SysMath.Min(a + 2 * (b - cmp), C_RANGE)
                                : SysMath.Max(a + 2 * b - C_RANGE, 0);
                            return (int)((tmp * _fac + a * mfac) / C_RANGE);
                        }

                        tmpA = ProcChannel(_color1.A, _color2.A);
                        tmpR = ProcChannel(_color1.R, _color2.R);
                        tmpG = ProcChannel(_color1.G, _color2.G);
                        tmpB = ProcChannel(_color1.B, _color2.B);
                        outColor = SysColor.FromArgb(tmpA, tmpR, tmpG, tmpB);
                        break;
                    }
            }

            m_out_color.TransferData(outColor);
            Invalidate();
        }

        private static byte GetLuminance(SysColor rgb) =>
            GetLuminance(new byte[3] { rgb.R, rgb.G, rgb.B });

        private static byte GetLuminance(byte[] rgb)
        {
            float[] rgbf = new float[3];
            RgbUcharToFloat(ref rgbf, rgb);
            float val = DotV3V3(C_LUMA_COEF, rgbf);
            return UnitFloatToUcharClamp(val);
        }

        private static float DotV3V3(float[] a, float[] b) =>
            a[0] * b[0] + a[1] * b[1] + a[2] * b[2];

        private static byte UnitFloatToUcharClamp(float val) =>
            (byte)((val <= 0.0f) ? 0 : ((val > (1.0f - 0.5f / 255.0f)) ? 255 : ((255.0f * val) + 0.5f)));

        private static void RgbUcharToFloat(ref float[] r_col, byte[] col_ub)
        {
            r_col[0] = ((float)col_ub[0]) * (1.0f / 255.0f);
            r_col[1] = ((float)col_ub[1]) * (1.0f / 255.0f);
            r_col[2] = ((float)col_ub[2]) * (1.0f / 255.0f);
        }

        private static void RgbToHsv(float r, float g, float b, ref float r_h, ref float r_s, ref float r_v)
        {
            float k = 0.0f;
            float chroma, min_gb;

            if (g < b)
            {
                (b, g) = (g, b);
                k = -1.0f;
            }

            min_gb = b;

            if (r < g)
            {
                (r, g) = (g, r);
                k = -2.0f / 6.0f - k;
                min_gb = SysMath.Min(g, b);
            }

            chroma = r - min_gb;

            r_h = SysMath.Abs(k + (g - b) / (6.0f * chroma + 1e-20f));
            r_s = chroma / (r + 1e-20f);
            r_v = r;
        }

        private static void HsvToRgb(float h, float s, float v, ref float r_r, ref float r_g, ref float r_b)
        {
            float nr, ng, nb;

            nr = SysMath.Abs(h * 6.0f - 3.0f) - 1.0f;
            ng = 2.0f - SysMath.Abs(h * 6.0f - 2.0f);
            nb = 2.0f - SysMath.Abs(h * 6.0f - 4.0f);

            nr = nr.Clamp(0.0f, 1.0f);
            ng = ng.Clamp(0.0f, 1.0f);
            nb = nb.Clamp(0.0f, 1.0f);

            r_r = ((nr - 1.0f) * s + 1.0f) * v;
            r_g = ((ng - 1.0f) * s + 1.0f) * v;
            r_b = ((nb - 1.0f) * s + 1.0f) * v;
        }

        private void InNumDataTransfer(object sender, STNodeOptionEventArgs e)
        {
            if (e.Status == ConnectionStatus.Connected && e.TargetOption.Data != null)
                Fac = ((int)SysMath.Round((float)e.TargetOption.Data * C_RANGE)).Clamp(0, C_RANGE);
            else
                Fac = 127;
        }

        private void InColorDataTransfer(object sender, STNodeOptionEventArgs e)
        {
            if (e.Status == ConnectionStatus.Connected && e.TargetOption.Data != null)
            {
                if (sender == m_in_color1)
                    Color1 = (SysColor)e.TargetOption.Data;
                else
                    Color2 = (SysColor)e.TargetOption.Data;
            }
            else
            {
                if (sender == m_in_color1)
                    Color1 = SysColor.LightGray;
                else
                    Color2 = SysColor.LightGray;
            }
        }
    }
}
