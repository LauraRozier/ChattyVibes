using ST.Library.UI.NodeEditor;
using System.Globalization;

namespace ChattyVibes.Nodes.NumberNode.FloatNode
{
    [STNode("/Number/Float", "LauraRozier", "", "", "Float input node")]
    internal class FloatInputNode : BaseFloatNode
    {
        private STNodeOption m_op_out;

        private float _value = 0.0f;
        [STNodeProperty("Value", "The input value")]
        public float Value
        {
            get { return _value; }
            set
            {
                _value = value;
                SetOptionText(m_op_out, _value.ToString("G", CultureInfo.InvariantCulture));
                m_op_out.TransferData(value);
            }
        }

        protected override void OnCreate()
        {
            base.OnCreate();
            Title = "Float";

            m_op_out = OutputOptions.Add("0", typeof(float), false);

            m_op_out.TransferData(_value);
        }
    }
}
