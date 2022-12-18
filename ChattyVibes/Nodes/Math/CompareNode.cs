using ST.Library.UI.NodeEditor;
using System;
using System.ComponentModel.DataAnnotations;
using System.Drawing;

namespace ChattyVibes.Nodes.Math
{
    internal abstract class CompareNode<T> : STNode where T : struct, IComparable, IComparable<T>
    {
        public enum CompareMode
        {
            [Display(Name = "==")]
            Equal,
            [Display(Name = "!=")]
            NotEqual,
            [Display(Name = "<")]
            Lower,
            [Display(Name = "<=")]
            LowerOrEqual,
            [Display(Name = ">")]
            Greater,
            [Display(Name = ">=")]
            GreaterOrEqual
        }

        protected T _defaultVal;
        protected T _aVal;
        protected T _bVal;
        private CompareMode _mode = CompareMode.Equal;
        [STNodeProperty("Mode", "The comparison mode")]
        public CompareMode Mode
        {
            get { return _mode; }
            set
            {
                _mode = value;
                m_ctrl_select.Enum = value;
                PerformComparison();
            }
        }

        private NodeSelectEnumBox m_ctrl_select;

        private STNodeOption m_in_A;
        private STNodeOption m_in_B;
        private STNodeOption m_out;

        protected override void OnCreate()
        {
            base.OnCreate();
            AutoSize = false;
            Width = 120;
            Height = 85;

            m_in_A = InputOptions.Add("A", typeof(T), true);
            m_in_B = InputOptions.Add("B", typeof(T), true);
            m_out = OutputOptions.Add("", typeof(bool), false);

            m_in_A.DataTransfer += new STNodeOptionEventHandler(m_op_DataTransfer);
            m_in_B.DataTransfer += new STNodeOptionEventHandler(m_op_DataTransfer);

            m_ctrl_select = new NodeSelectEnumBox
            {
                DisplayRectangle = new Rectangle(10, 42, 60, 18),
                Enum = _mode
            };
            m_ctrl_select.ValueChanged += (s, e) =>
            {
                _mode = (CompareMode)m_ctrl_select.Enum;
                PerformComparison();
            };
            Controls.Add(m_ctrl_select);

            PerformComparison();
        }

        private void m_op_DataTransfer(object sender, STNodeOptionEventArgs e)
        {
            if (e.Status == ConnectionStatus.Connected && e.TargetOption.Data != null)
            {
                if (sender == m_in_A)
                    _aVal = (T)e.TargetOption.Data;
                else
                    _bVal = (T)e.TargetOption.Data;
            }
            else
            {
                if (sender == m_in_A)
                    _aVal = _defaultVal;
                else
                    _bVal = _defaultVal;
            }

            PerformComparison();
        }

        private void PerformComparison()
        {
            bool result = false;

            switch (_mode)
            {
                case CompareMode.Equal:
                    {
                        result = _aVal.CompareTo(_bVal) == 0;
                        break;
                    }
                case CompareMode.NotEqual:
                    {
                        result = _aVal.CompareTo(_bVal) != 0;
                        break;
                    }
                case CompareMode.Lower:
                    {
                        result = _aVal.CompareTo(_bVal) < 0;
                        break;
                    }
                case CompareMode.LowerOrEqual:
                    {
                        result = _aVal.CompareTo(_bVal) <= 0;
                        break;
                    }
                case CompareMode.Greater:
                    {
                        result = _aVal.CompareTo(_bVal) > 0;
                        break;
                    }
                case CompareMode.GreaterOrEqual:
                    {
                        result = _aVal.CompareTo(_bVal) >= 0;
                        break;
                    }
            }

            SetOptionText(m_out, result ? "True" : "False");
            m_out.TransferData(result);
        }
    }
}
