using ST.Library.UI.NodeEditor;
using TwitchLib.Client;

namespace ChattyVibes.Nodes.ActionNode.TwitchNode
{
    [STNode("/Actions/Twitch", "LauraRozier", "", "", "Twitch JoinChannel node")]
    internal sealed class JoinChannelNode : ActionNode
    {
        private string _channel = string.Empty;
        [STNodeProperty("Channel", "The name of the channel to join.")]
        public string Channel
        {
            get { return _channel; }
            set
            {
                _channel = value;
                Invalidate();
            }
        }

        private STNodeOption m_op_in;

        private struct MsgData
        {
            public string Channel { get; set; }
        }

        protected override void OnFlowTrigger()
        {
            if (MainForm.ChatState != ConnectionState.Connected)
                return;

            MainForm.TwitchQueue?.Enqueue(
                new Queues.QueuedTwitchTaskHandler(SendCommand),
                new MsgData { Channel = _channel }
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
                    return;
            } catch { }
            
            client.JoinChannel(dataObj.Channel);
        }

        protected override void OnCreate()
        {
            base.OnCreate();
            Title = "Join Channel";

            m_op_in = InputOptions.Add("Channel", typeof(string), false);

            m_op_in.DataTransfer += new STNodeOptionEventHandler(m_op_DataTransfer);
        }

        private void m_op_DataTransfer(object sender, STNodeOptionEventArgs e)
        {
            if (e.Status == ConnectionStatus.Connected && e.TargetOption.Data != null)
                Channel = (string)e.TargetOption.Data;
            else
                Channel = string.Empty;
        }
    }
}
