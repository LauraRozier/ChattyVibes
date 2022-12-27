using Buttplug;
using ST.Library.UI.NodeEditor;
using System.Threading.Tasks;

namespace ChattyVibes.Nodes.ActionNode.IntifaceNode
{
    [STNode("/Actions/Intiface", "LauraRozier", "", "", "Intiface SendRotateCommand node")]
    internal sealed class SendRotateCommandNode : ActionNode
    {
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
        private float _level = 0.5f;
        [STNodeProperty("Level (0.0-1.0)", "The strength level of the rotations.")]
        public float Level
        {
            get { return _level; }
            set
            {
                _level = value.Clamp(0.0f, 1.0f);
                Invalidate();
            }
        }
        private bool _clockwise = true;
        [STNodeProperty("Clockwise", "The direction of the rotations.")]
        public bool Clockwise
        {
            get { return _clockwise; }
            set
            {
                _clockwise = value;
                Invalidate();
            }
        }
        private int _duration = 1000;
        [STNodeProperty("Duration in ms", "The duration of the rotations.")]
        public int Duration
        {
            get { return _duration; }
            set
            {
                _duration = value;
                Invalidate();
            }
        }

        private STNodeOption m_op_deviceId_in;
        private STNodeOption m_op_level_in;
        private STNodeOption m_op_clockwise_in;
        private STNodeOption m_op_duration_in;

        private struct MsgData
        {
            public float Level { get; set; }
            public bool Clockwise { get; set; }
            public int Duration { get; set; }
        }

        protected override void OnFlowTrigger()
        {
            if (MainForm._plugState != ConnectionState.Connected || !MainForm.ButtplugQueues.ContainsKey(_deviceId))
                return;

            MainForm.ButtplugQueues[_deviceId].Enqueue(
                new Queues.QueuedPlugTaskHandler(SendCommand),
                new MsgData
                {
                    Level = _level,
                    Clockwise = _clockwise,
                    Duration = _duration,
                }
            );
        }

        private async Task SendCommand(ButtplugClientDevice device, object data)
        {
            if (!device.AllowedMessages.ContainsKey(ServerMessage.Types.MessageAttributeType.RotateCmd))
                return;

            MsgData dataObj = (MsgData)data;

            await device.SendRotateCmd(dataObj.Level, dataObj.Clockwise);
            await Task.Delay(dataObj.Duration);
            await device.SendRotateCmd(0, dataObj.Clockwise);
        }

        protected override void OnCreate()
        {
            base.OnCreate();
            Title = "Send Rotate Command";

            m_op_deviceId_in = InputOptions.Add("Device ID", typeof(uint), false);
            m_op_level_in = InputOptions.Add("Level (0.0-1.0)", typeof(float), false);
            m_op_clockwise_in = InputOptions.Add("Clockwise", typeof(bool), false);
            m_op_duration_in = InputOptions.Add("Duration in ms", typeof(int), false);

            m_op_deviceId_in.DataTransfer += new STNodeOptionEventHandler(m_op_DataTransfer);
            m_op_level_in.DataTransfer += new STNodeOptionEventHandler(m_op_DataTransfer);
            m_op_duration_in.DataTransfer += new STNodeOptionEventHandler(m_op_DataTransfer);
        }

        private void m_op_DataTransfer(object sender, STNodeOptionEventArgs e)
        {
            if (e.Status == ConnectionStatus.Connected && e.TargetOption.Data != null)
            {
                if (sender == m_op_deviceId_in)
                    DeviceId = (uint)e.TargetOption.Data;
                else if (sender == m_op_level_in)
                    Level = (float)e.TargetOption.Data;
                else if (sender == m_op_clockwise_in)
                    Clockwise = (bool)e.TargetOption.Data;
                else
                    Duration = (int)e.TargetOption.Data;
            }
            else
            {
                if (sender == m_op_deviceId_in)
                    DeviceId = 0u;
                else if (sender == m_op_level_in)
                    Level = 0.5f;
                else if (sender == m_op_clockwise_in)
                    Clockwise = true;
                else
                    Duration = 1000;
            }
        }
    }
}
