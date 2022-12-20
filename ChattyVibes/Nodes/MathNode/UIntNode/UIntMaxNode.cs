using ST.Library.UI.NodeEditor;
using System;

namespace ChattyVibes.Nodes.MathNode.UIntNode
{
    [STNode("/Math/UInt", "LauraRozier", "", "", "Returns the larger of two numbers.")]
    internal class UIntMaxNode : BaseUIntNode
    {
        private uint _aVal = 0u;
        [STNodeProperty("A", "A value")]
        public uint Min
        {
            get { return _aVal; }
            set
            {
                _aVal = value;
                ProcessResult();
            }
        }
        private uint _bVal = 0u;
        [STNodeProperty("B", "B value")]
        public uint Max
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
            Title = "UInt Max";

            m_in_A = InputOptions.Add("A", typeof(uint), true);
            m_in_B = InputOptions.Add("B", typeof(uint), true);
            m_out = OutputOptions.Add("", typeof(uint), false);

            m_in_A.DataTransfer += new STNodeOptionEventHandler(m_in_DataTransfer);
            m_in_B.DataTransfer += new STNodeOptionEventHandler(m_in_DataTransfer);

            ProcessResult();
        }

        private void m_in_DataTransfer(object sender, STNodeOptionEventArgs e)
        {
            if (e.Status == ConnectionStatus.Connected && e.TargetOption.Data != null)
            {
                if (sender == m_in_A)
                    _aVal = (uint)e.TargetOption.Data;
                else
                    _bVal = (uint)e.TargetOption.Data;
            }
            else
            {
                if (sender == m_in_A)
                    _aVal = 0;
                else
                    _bVal = 0;
            }

            ProcessResult();
        }

        private void ProcessResult()
        {
            uint result = Math.Max(_aVal, _bVal);
            SetOptionText(m_in_A, $"A {_aVal}");
            SetOptionText(m_in_B, $"B {_bVal}");
            SetOptionText(m_out, result.ToString());
            m_out.TransferData(result);
        }
    }
}
