using ST.Library.UI.NodeEditor;

namespace ChattyVibes.Nodes.Math.Float
{
    [STNode("/Math/Float", "LauraRozier", "", "", "This node clamps the number within set values")]
    internal class FloatClampNode : FloatNode
    {
        private float m_nNum = 0.0f;
        private float m_nMin = 0.0f;
        [STNodeProperty("Min", "The minimum value")]
        public float Min
        {
            get { return m_nMin; }
            set
            {
                m_nMin = value;
                ProcessResult();
            }
        }
        private float m_nMax = 1.0f;
        [STNodeProperty("Max", "The maximum value")]
        public float Max
        {
            get { return m_nMax; }
            set
            {
                m_nMax = value;
                ProcessResult();
            }
        }

        private STNodeOption m_in_num;
        private STNodeOption m_in_min;
        private STNodeOption m_in_max;
        private STNodeOption m_out;

        protected override void OnCreate()
        {
            base.OnCreate();
            Title = "Float Clamp";

            m_in_num = InputOptions.Add("", typeof(float), true);
            m_in_min = InputOptions.Add("Min", typeof(float), true);
            m_in_max = InputOptions.Add("Max", typeof(float), true);
            m_out = OutputOptions.Add("", typeof(float), false);

            m_in_num.DataTransfer += new STNodeOptionEventHandler(m_in_DataTransfer);
            m_in_min.DataTransfer += new STNodeOptionEventHandler(m_in_DataTransfer);
            m_in_max.DataTransfer += new STNodeOptionEventHandler(m_in_DataTransfer);

            ProcessResult();
        }

        private void m_in_DataTransfer(object sender, STNodeOptionEventArgs e)
        {
            if (e.Status == ConnectionStatus.Connected && e.TargetOption.Data != null)
            {
                if (sender == m_in_num)
                    m_nNum = (float)e.TargetOption.Data;
                else if (sender == m_in_min)
                    m_nMin = (float)e.TargetOption.Data;
                else
                    m_nMax = (float)e.TargetOption.Data;
            }
            else
            {
                if (sender == m_in_num)
                    m_nNum = 0;
                else if (sender == m_in_min)
                    m_nMin = 0;
                else
                    m_nMax = 100;
            }

            ProcessResult();
        }

        private void ProcessResult()
        {
            float result = m_nNum.Clamp(m_nMin, m_nMax);
            SetOptionText(m_in_num, m_nNum.ToString());
            SetOptionText(m_in_min, $"Min {m_nMin}");
            SetOptionText(m_in_max, $"Max {m_nMax}");
            SetOptionText(m_out, result.ToString());
            m_out.TransferData(result);
        }
    }
}
