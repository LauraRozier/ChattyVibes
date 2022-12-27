using ST.Library.UI.NodeEditor;
using TwitchLib.Client;

namespace ChattyVibes.Nodes.ActionNode.TwitchNode
{
    [STNode("/Actions/Twitch", "LauraRozier", "", "", "Twitch JoinChannel node")]
    internal sealed class JoinChannelNode : ActionNode
    {
        private string _channelName = string.Empty;
        [STNodeProperty("ChannelName", "The name of the channel to join.")]
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
                    return;
            } catch { }
            
            client.JoinChannel(dataObj.ChannelName);
        }

        protected override void OnCreate()
        {
            base.OnCreate();
            Title = "Join Channel";

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
