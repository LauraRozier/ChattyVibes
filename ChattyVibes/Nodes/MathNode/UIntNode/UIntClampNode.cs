using ST.Library.UI.NodeEditor;

namespace ChattyVibes.Nodes.MathNode.UIntNode
{
    [STNode("/Math/UInt", "LauraRozier", "", "", "This node clamps the number within set values")]
    internal class UIntClampNode : BaseUIntNode
    {
        private uint m_nNum = 0u;
        private uint m_nMin = 0u;
        [STNodeProperty("Min", "The minimum value")]
        public uint Min
        {
            get { return m_nMin; }
            set
            {
                m_nMin = value;
                ProcessResult();
            }
        }
        private uint m_nMax = 100u;
        [STNodeProperty("Max", "The maximum value")]
        public uint Max
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
            Title = "UInt Clamp";

            m_in_num = InputOptions.Add("", typeof(uint), true);
            m_in_min = InputOptions.Add("Min", typeof(uint), true);
            m_in_max = InputOptions.Add("Max", typeof(uint), true);
            m_out = OutputOptions.Add("", typeof(uint), false);

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
                    m_nNum = (uint)e.TargetOption.Data;
                else if (sender == m_in_min)
                    m_nMin = (uint)e.TargetOption.Data;
                else
                    m_nMax = (uint)e.TargetOption.Data;
            }
            else
            {
                if (sender == m_in_num)
                    m_nNum = 0u;
                else if (sender == m_in_min)
                    m_nMin = 0u;
                else
                    m_nMax = 100u;
            }

            ProcessResult();
        }

        private void ProcessResult()
        {
            uint result = m_nNum.Clamp(m_nMin, m_nMax);
            SetOptionText(m_in_num, m_nNum.ToString());
            SetOptionText(m_in_min, $"Min {m_nMin}");
            SetOptionText(m_in_max, $"Max {m_nMax}");
            SetOptionText(m_out, result.ToString());
            m_out.TransferData(result);
        }
    }
}
