using ST.Library.UI.NodeEditor;

namespace ChattyVibes.Nodes.MathNode.IntNode
{
    [STNode("/Math/Int", "LauraRozier", "", "", "This node clamps the number within set values")]
    internal class IntClampNode : Nodes.IntNode
    {
        private int m_nNum = 0;
        private int m_nMin = 0;
        [STNodeProperty("Min", "The minimum value")]
        public int Min
        {
            get { return m_nMin; }
            set
            {
                m_nMin = value;
                ProcessResult();
            }
        }
        private int m_nMax = 100;
        [STNodeProperty("Max", "The maximum value")]
        public int Max
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
            Title = "Int Clamp";

            m_in_num = InputOptions.Add(string.Empty, typeof(int), true);
            m_in_min = InputOptions.Add("Min", typeof(int), true);
            m_in_max = InputOptions.Add("Max", typeof(int), true);
            m_out = OutputOptions.Add(string.Empty, typeof(int), false);

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
                    m_nNum = (int)e.TargetOption.Data;
                else if (sender == m_in_min)
                    m_nMin = (int)e.TargetOption.Data;
                else
                    m_nMax = (int)e.TargetOption.Data;
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
            int result = m_nNum.Clamp(m_nMin, m_nMax);
            SetOptionText(m_in_num, m_nNum.ToString());
            SetOptionText(m_in_min, $"Min {m_nMin}");
            SetOptionText(m_in_max, $"Max {m_nMax}");
            SetOptionText(m_out, result.ToString());
            m_out.TransferData(result);
        }
    }
}
