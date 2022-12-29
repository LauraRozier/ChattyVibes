using ST.Library.UI.NodeEditor;
using TwitchLib.Client;
using TwitchLib.Client.Extensions;

namespace ChattyVibes.Nodes.ActionNode.TwitchNode
{
    [STNode("/Actions/Twitch", "LauraRozier", "", "", "Twitch Raid node")]
    internal sealed class RaidNode : ActionNode
    {
        private string _channel = string.Empty;
        [STNodeProperty("Channel", "The name of the channel to start the raid in.")]
        public string Channel
        {
            get { return _channel; }
            set
            {
                _channel = value;
                Invalidate();
            }
        }
        private string _targetChannel = string.Empty;
        [STNodeProperty("Target Channel", "The channel to target the raid towards.")]
        public string TargetChannel
        {
            get { return _targetChannel; }
            set
            {
                _targetChannel = value;
                Invalidate();
            }
        }

        private STNodeOption m_op_channel_in;
        private STNodeOption m_op_targetChannel_in;

        private struct MsgData
        {
            public string Channel { get; set; }
            public string TargetChannel { get; set; }
        }

        protected override void OnFlowTrigger()
        {
            if (MainForm.ChatState != ConnectionState.Connected)
                return;

            MainForm.TwitchQueue?.Enqueue(
                new Queues.QueuedTwitchTaskHandler(SendCommand),
                new MsgData { Channel = _channel, TargetChannel = _targetChannel }
            );
        }

        private void SendCommand(TwitchClient client, object data)
        {
            if (!(client?.IsConnected ?? false))
                return;

            MsgData dataObj = (MsgData)data;
            client.Raid(dataObj.Channel, dataObj.TargetChannel);
        }

        protected override void OnCreate()
        {
            base.OnCreate();
            Title = "Raid";

            m_op_channel_in = InputOptions.Add("Channel", typeof(string), false);
            m_op_targetChannel_in = InputOptions.Add("Target Channel", typeof(string), false);

            m_op_channel_in.DataTransfer += new STNodeOptionEventHandler(m_op_DataTransfer);
            m_op_targetChannel_in.DataTransfer += new STNodeOptionEventHandler(m_op_DataTransfer);
        }

        private void m_op_DataTransfer(object sender, STNodeOptionEventArgs e)
        {
            if (e.Status == ConnectionStatus.Connected && e.TargetOption.Data != null)
            {
                if (sender == m_op_channel_in)
                    Channel = (string)e.TargetOption.Data;
                else
                    TargetChannel = (string)e.TargetOption.Data;
            }
            else
            {
                if (sender == m_op_channel_in)
                    Channel = string.Empty;
                else
                    TargetChannel = string.Empty;
            }
        }
    }
}
