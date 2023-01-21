using ST.Library.UI.NodeEditor;
using System;
using System.Globalization;

namespace ChattyVibes.Nodes.MathNode.FloatNode
{
    [STNode("/Math/Float", "LauraRozier", "", "", "Returns the smaller of two numbers.")]
    internal class FloatMinNode : Nodes.FloatNode
    {
        private float _aVal = 0.0f;
        [STNodeProperty("A", "A value")]
        public float Min
        {
            get { return _aVal; }
            set
            {
                _aVal = value;
                ProcessResult();
            }
        }
        private float _bVal = 0.0f;
        [STNodeProperty("B", "B value")]
        public float Max
        {
            get { return _bVal; }
            set
            {
                _bVal = value;
                ProcessResult();
            }
        }

        private STNodeOption m_in_A;
        private STNodeOption m_in_B;
        private STNodeOption m_out;

        protected override void OnCreate()
        {
            base.OnCreate();
            Title = "Float Min";

            m_in_A = InputOptions.Add("A", typeof(float), true);
            m_in_B = InputOptions.Add("B", typeof(float), true);
            m_out = OutputOptions.Add("0", typeof(float), false);

            m_in_A.DataTransfer += new STNodeOptionEventHandler(m_in_DataTransfer);
            m_in_B.DataTransfer += new STNodeOptionEventHandler(m_in_DataTransfer);
            m_out.TransferData(_aVal);
        }

        void m_in_DataTransfer(object sender, STNodeOptionEventArgs e)
        {
            if (e.Status == ConnectionStatus.Connected && e.TargetOption.Data != null)
            {
                if (sender == m_in_A)
                    _aVal = (float)e.TargetOption.Data;
                else
                    _bVal = (float)e.TargetOption.Data;
            }
            else
            {
                if (sender == m_in_A)
                    _aVal = 0.0f;
                else
                    _bVal = 0.0f;
            }

            ProcessResult();
        }

        private void ProcessResult()
        {
            float result = Math.Min(_aVal, _bVal);
            SetOptionText(m_in_A, "A " + _aVal.ToString("G", CultureInfo.InvariantCulture));
            SetOptionText(m_in_B, "B " + _bVal.ToString("G", CultureInfo.InvariantCulture));
            SetOptionText(m_out, result.ToString("G", CultureInfo.InvariantCulture));
            m_out.TransferData(result);
        }
    }
}
