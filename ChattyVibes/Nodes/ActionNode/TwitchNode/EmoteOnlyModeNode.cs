using ST.Library.UI.NodeEditor;
using TwitchLib.Client;
using TwitchLib.Client.Extensions;

namespace ChattyVibes.Nodes.ActionNode.TwitchNode
{
    [STNode("/Actions/Twitch", "LauraRozier", "", "", "Twitch EmoteOnlyMode node")]
    internal sealed class EmoteOnlyModeNode : ActionNode
    {
        private string _channel = string.Empty;
        [STNodeProperty("Channel", "The name of the channel to change the EmoteOnly mode of.")]
        public string Channel
        {
            get { return _channel; }
            set
            {
                _channel = value;
                Invalidate();
            }
        }
        private bool _enabled = true;
        [STNodeProperty("Enabled", "Wether to enable or disable EmoteOnly mode.")]
        public bool Enabled
        {
            get { return _enabled; }
            set
            {
                _enabled = value;
                Invalidate();
            }
        }

        private STNodeOption m_op_channel_in;
        private STNodeOption m_op_ena_in;

        private struct MsgData
        {
            public string Channel { get; set; }
            public bool Enabled { get; set; }
        }

        protected override void OnFlowTrigger()
        {
            if (MainForm.ChatState != ConnectionState.Connected)
                return;

            MainForm.TwitchQueue?.Enqueue(
                new Queues.QueuedTwitchTaskHandler(SendCommand),
                new MsgData { Channel = _channel, Enabled = _enabled }
            );
        }

        private void SendCommand(TwitchClient client, object data)
        {
            if (!(client?.IsConnected ?? false))
                return;

            MsgData dataObj = (MsgData)data;

            try
            {
                if (client.GetJoinedChannel(dataObj.Channel) != default)
                {
                    if (dataObj.Enabled)
                        client.EmoteOnlyOn(dataObj.Channel);
                    else
                        client.EmoteOnlyOff(dataObj.Channel);
                }
            } catch { }
        }

        protected override void OnCreate()
        {
            base.OnCreate();
            Title = "Emote Only Mode";

            m_op_channel_in = InputOptions.Add("Channel", typeof(string), false);
            m_op_ena_in = InputOptions.Add("Enabled", typeof(bool), false);

            m_op_channel_in.DataTransfer += new STNodeOptionEventHandler(m_op_DataTransfer);
            m_op_ena_in.DataTransfer += new STNodeOptionEventHandler(m_op_DataTransfer);
        }

        private void m_op_DataTransfer(object sender, STNodeOptionEventArgs e)
        {
            if (e.Status == ConnectionStatus.Connected && e.TargetOption.Data != null)
            {
                if (sender == m_op_channel_in)
                    Channel = (string)e.TargetOption.Data;
                else
                    Enabled = (bool)e.TargetOption.Data;
            }
            else
            {
                if (sender == m_op_channel_in)
                    Channel = string.Empty;
                else
                    Enabled = true;
            }
        }
    }
}
