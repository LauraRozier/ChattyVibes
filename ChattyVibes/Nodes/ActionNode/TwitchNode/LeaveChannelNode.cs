using ST.Library.UI.NodeEditor;
using TwitchLib.Client;

namespace ChattyVibes.Nodes.ActionNode.TwitchNode
{
    [STNode("/Actions/Twitch", "LauraRozier", "", "", "Twitch LeaveChannel node")]
    internal sealed class LeaveChannelNode : ActionNode
    {
        private string _channelName = string.Empty;
        [STNodeProperty("ChannelName", "The name of the channel to leave.")]
        public string ChannelName
        {
            get { return _channelName; }
            set
            {
                _channelName = value;
                Invalidate();
            }
        }

        private STNodeOption m_op_in;

        private struct MsgData
        {
            public string ChannelName { get; set; }
        }

        protected override void OnFlowTrigger()
        {
            if (MainForm._chatState != ConnectionState.Connected)
                return;

            MainForm.TwitchQueue?.Enqueue(
                new Queues.QueuedTwitchTaskHandler(SendCommand),
                new MsgData { ChannelName = _channelName }
            );
        }

        private void SendCommand(TwitchClient client, object data)
        {
            if (!(client?.IsConnected ?? false))
                return;

            MsgData dataObj = (MsgData)data;

            try
            {
                if (client.GetJoinedChannel(dataObj.ChannelName) != null)
                    client.LeaveChannel(dataObj.ChannelName);
            } catch { }
        }

        protected override void OnCreate()
        {
            base.OnCreate();
            Title = "Leave Channel";

            m_op_in = InputOptions.Add("Channel Name", typeof(string), false);

            m_op_in.DataTransfer += new STNodeOptionEventHandler(m_op_DataTransfer);
        }

        private void m_op_DataTransfer(object sender, STNodeOptionEventArgs e)
        {
            if (e.Status == ConnectionStatus.Connected && e.TargetOption.Data != null)
                ChannelName = (string)e.TargetOption.Data;
            else
                ChannelName = string.Empty;
        }
    }
}
