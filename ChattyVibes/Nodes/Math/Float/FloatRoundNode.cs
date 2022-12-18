using ST.Library.UI.NodeEditor;
using System;
using System.Drawing;
using System.Globalization;
using SysMath = System.Math;

namespace ChattyVibes.Nodes.Math.Float
{
    [STNode("/Math/Float", "LauraRozier", "", "", "Rounds a value to a specified number of fractional digits, and uses the specified rounding convention for midpoint values.")]
    internal class FloatRoundNode : FloatNode
    {
        private float _val = 0.0f;
        private int _digits = 2;
        [STNodeProperty("Digits", "The number of fractional digits in the return value.")]
        public int Digits
        {
            get { return _digits; }
            set
            {
                _digits = value;
                ProcessResult();
            }
        }
        private MidpointRounding _mode = MidpointRounding.ToEven;
        [STNodeProperty("Mode", "Specification for how to round value if it is midway between two other numbers.")]
        public MidpointRounding Mode
        {
            get { return _mode; }
            set
            {
                _mode = value;
                m_ctrl_select.Enum = value;
                ProcessResult();
            }
        }

        private NodeSelectEnumBox m_ctrl_select;

        private STNodeOption m_in_val;
        private STNodeOption m_in_digits;
        private STNodeOption m_out;

        protected override void OnCreate()
        {
            base.OnCreate();
            Title = "Float Round";
            AutoSize = false;
            Width = 180;
            Height = 84;

            m_in_val = InputOptions.Add("0", typeof(float), true);
            m_in_digits = InputOptions.Add("0", typeof(float), true);
            m_out = OutputOptions.Add("0", typeof(float), false);

            m_in_val.DataTransfer += new STNodeOptionEventHandler(m_in_DataTransfer);
            m_in_digits.DataTransfer += new STNodeOptionEventHandler(m_in_DataTransfer);

            m_ctrl_select = new NodeSelectEnumBox
            {
                DisplayRectangle = new Rectangle(10, 42, 120, 18),
                Enum = _mode
            };
            m_ctrl_select.ValueChanged += (s, e) =>
            {
                _mode = (MidpointRounding)m_ctrl_select.Enum;
                ProcessResult();
            };
            Controls.Add(m_ctrl_select);

            ProcessResult();
        }

        private void m_in_DataTransfer(object sender, STNodeOptionEventArgs e)
        {
            if (e.Status == ConnectionStatus.Connected && e.TargetOption.Data != null)
            {
                if (sender == m_in_val)
                    _val = (float)e.TargetOption.Data;
                else
                    _digits = (int)e.TargetOption.Data;
            }
            else
            {
                if (sender == m_in_val)
                    _val = 0.0f;
                else
                    _digits = 2;
            }

            ProcessResult();
        }

        private void ProcessResult()
        {
            float result = (float)SysMath.Round(_val, _digits, _mode);
            SetOptionText(m_in_val, _val.ToString("G", CultureInfo.InvariantCulture));
            SetOptionText(m_in_digits, _digits.ToString());
            SetOptionText(m_out, result.ToString("G", CultureInfo.InvariantCulture));
            m_out.TransferData(result);
        }
    }
}
