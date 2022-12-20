using Buttplug;
using ST.Library.UI.NodeEditor;
using System.Threading.Tasks;

namespace ChattyVibes.Nodes.ActionNode.IntifaceNode
{
    [STNode("/Actions/Intiface", "LauraRozier", "", "", "Intiface SendLinearCommand node")]
    internal sealed class SendLinearCommandNode : BaseActionNode
    {
        private const int C_DIR_CHANGE_DELAY = 50;

        private uint _deviceId = 0u;
        [STNodeProperty("Device ID", "The ID of the device to send the command to.")]
        public uint DeviceId
        {
            get { return _deviceId; }
            set
            {
                _deviceId = value;
                Invalidate();
            }
        }
        private int _strokeCount = 5;
        [STNodeProperty("Stroke Count", "The amount of strokes to perform.")]
        public int StrokeCount
        {
            get { return _strokeCount; }
            set
            {
                _strokeCount = value;
                Invalidate();
            }
        }
        private uint _downTime = 200u;
        [STNodeProperty("Down Travel Time in ms", "The duration of the downwards motion.")]
        public uint DownTime
        {
            get { return _downTime; }
            set
            {
                _downTime = value;
                Invalidate();
            }
        }
        private uint _upTime = 200u;
        [STNodeProperty("Up Travel Time in ms", "The duration of the upwards motion.")]
        public uint UpTime
        {
            get { return _upTime; }
            set
            {
                _upTime = value;
                Invalidate();
            }
        }
        private float _downPos = 0.0f;
        [STNodeProperty("DownPos (0.0 to Up-0.1)", "The max downwards position.")]
        public float DownPos
        {
            get { return _downPos; }
            set
            {
                _downPos = value.Clamp(0.0f, _upPos - 0.1f);
                Invalidate();
            }
        }
        private float _upPos = 1.0f;
        [STNodeProperty("UpPos (Down+0.1 to 1.0)", "The max upwards position.")]
        public float UpPos
        {
            get { return _upPos; }
            set
            {
                _upPos = value.Clamp(_downPos + 0.1f, 1.0f);
                Invalidate();
            }
        }

        private STNodeOption m_op_deviceId_in;
        private STNodeOption m_op_strokeCount_in;
        private STNodeOption m_op_downTime_in;
        private STNodeOption m_op_upTime_in;
        private STNodeOption m_op_downPos_in;
        private STNodeOption m_op_upPos_in;

        private struct MsgData
        {
            public int StrokeCount { get; set; }
            public uint DownTime { get; set; }
            public uint UpTime { get; set; }
            public float DownPos { get; set; }
            public float UpPos { get; set; }
        }

        protected override void OnFlowTrigger()
        {
            if (MainForm._plugState != ConnectionState.Connected || !MainForm.ButtplugQueues.ContainsKey(_deviceId))
                return;

            MainForm.ButtplugQueues[_deviceId].Enqueue(
                new Queues.QueuedTaskHandler(SendCommand),
                new MsgData
                {
                    StrokeCount = _strokeCount,
                    DownTime = _downTime,
                    UpTime = _upTime,
                    DownPos = _downPos,
                    UpPos = _upPos
                }
            );
        }

        private async Task SendCommand(ButtplugClientDevice device, object data)
        {
            if (!device.AllowedMessages.ContainsKey(ServerMessage.Types.MessageAttributeType.LinearCmd))
                return;

            MsgData dataObj = (MsgData)data;

            for (int i = 0; i < dataObj.StrokeCount; i++)
            {
                await device.SendLinearCmd(dataObj.DownTime, dataObj.DownPos);
                await Task.Delay((int)dataObj.DownTime + C_DIR_CHANGE_DELAY);
                await device.SendLinearCmd(dataObj.UpTime, dataObj.UpPos);
                await Task.Delay((int)dataObj.UpTime + C_DIR_CHANGE_DELAY);
            }
        }

        protected override void OnCreate()
        {
            base.OnCreate();
            Title = "Send Linear Command";

            m_op_deviceId_in = InputOptions.Add("Device ID", typeof(uint), false);
            m_op_strokeCount_in = InputOptions.Add("Stroke Count", typeof(int), false);
            m_op_downTime_in = InputOptions.Add("Down Travel Time", typeof(uint), false);
            m_op_upTime_in = InputOptions.Add("Up Travel Time", typeof(uint), false);
            m_op_downPos_in = InputOptions.Add("DownPos (0.0 to UpPos-0.1)", typeof(float), false);
            m_op_upPos_in = InputOptions.Add("UpPos (DownPos+0.1 to 1.0)", typeof(float), false);

            m_op_deviceId_in.DataTransfer += new STNodeOptionEventHandler(m_op_DataTransfer);
            m_op_strokeCount_in.DataTransfer += new STNodeOptionEventHandler(m_op_DataTransfer);
            m_op_downTime_in.DataTransfer += new STNodeOptionEventHandler(m_op_DataTransfer);
            m_op_upTime_in.DataTransfer += new STNodeOptionEventHandler(m_op_DataTransfer);
            m_op_downPos_in.DataTransfer += new STNodeOptionEventHandler(m_op_DataTransfer);
            m_op_upPos_in.DataTransfer += new STNodeOptionEventHandler(m_op_DataTransfer);
        }

        private void m_op_DataTransfer(object sender, STNodeOptionEventArgs e)
        {
            if (e.Status == ConnectionStatus.Connected && e.TargetOption.Data != null)
            {
                if (sender == m_op_deviceId_in)
                    DeviceId = (uint)e.TargetOption.Data;
                else if (sender == m_op_strokeCount_in)
                    StrokeCount = (int)e.TargetOption.Data;
                else if (sender == m_op_downTime_in)
                    DownTime = (uint)e.TargetOption.Data;
                else if (sender == m_op_upTime_in)
                    UpTime = (uint)e.TargetOption.Data;
                else if (sender == m_op_downPos_in)
                    DownPos = (float)e.TargetOption.Data;
                else
                    UpPos = (float)e.TargetOption.Data;
            }
            else
            {
                if (sender == m_op_deviceId_in)
                    DeviceId = 0u;
                else if (sender == m_op_strokeCount_in)
                    StrokeCount = 5;
                else if (sender == m_op_downTime_in)
                    DownTime = 200u;
                else if (sender == m_op_upTime_in)
                    UpTime = 200u;
                else if (sender == m_op_downPos_in)
                    DownPos = 0.0f;
                else
                    UpPos = 1.0f;
            }
        }
    }
}
