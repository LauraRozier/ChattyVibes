using Buttplug;
using ST.Library.UI.NodeEditor;
using System.Threading.Tasks;

namespace ChattyVibes.Nodes.ActionNode.IntifaceNode
{
    [STNode("/Actions/Intiface", "LauraRozier", "", "", "Intiface SendVibrateCommand node")]
    internal sealed class SendVibeCommandNode : BaseActionNode
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
        [STNodeProperty("Level (0.0-1.0)", "The strength level of the vibrations.")]
        public float Level
        {
            get { return _level; }
            set
            {
                _level = value.Clamp(0.0f, 1.0f);
                Invalidate();
            }
        }
        private int _duration = 1000;
        [STNodeProperty("Duration in ms", "The duration of the vibrations.")]
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
        private STNodeOption m_op_duration_in;

        private struct MsgData
        {
            public float Level { get; set; }
            public int Duration { get; set; }
        }

        protected override void OnFlowTrigger()
        {
            if (MainForm._plugState != ConnectionState.Connected || !MainForm.ButtplugQueues.ContainsKey(_deviceId))
                return;

            MainForm.ButtplugQueues[_deviceId].Enqueue(
                new Queues.QueuedPlugTaskHandler(SendCommand),
                new MsgData { Level = _level, Duration = _duration }
            );
        }

        private async Task SendCommand(ButtplugClientDevice device, object data)
        {
            if (!device.AllowedMessages.ContainsKey(ServerMessage.Types.MessageAttributeType.VibrateCmd))
                return;

            MsgData dataObj = (MsgData)data;

            await device.SendVibrateCmd(dataObj.Level);
            await Task.Delay(dataObj.Duration);
            await device.SendVibrateCmd(0);
        }

        protected override void OnCreate()
        {
            base.OnCreate();
            Title = "Send Vibrate Command";

            m_op_deviceId_in = InputOptions.Add("Device ID", typeof(uint), false);
            m_op_level_in = InputOptions.Add("Level (0.0-1.0)", typeof(float), false);
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
                else
                    Duration = (int)e.TargetOption.Data;
            }
            else
            {
                if (sender == m_op_deviceId_in)
                    DeviceId = 0u;
                else if (sender == m_op_level_in)
                    Level = 0.5f;
                else
                    Duration = 1000;
            }
        }
    }
}
