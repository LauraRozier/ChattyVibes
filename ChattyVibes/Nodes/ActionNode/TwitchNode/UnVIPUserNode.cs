using ST.Library.UI.NodeEditor;
using TwitchLib.Client;
using TwitchLib.Client.Extensions;

namespace ChattyVibes.Nodes.ActionNode.TwitchNode
{
    [STNode("/Actions/Twitch", "LauraRozier", "", "", "Twitch UnVIPUser node")]
    internal sealed class UnVIPUserNode : ActionNode
    {
        private string _channel = string.Empty;
        [STNodeProperty("Channel", "The channel to unVIP the user for.")]
        public string Channel
        {
            get { return _channel; }
            set
            {
                _channel = value;
                Invalidate();
            }
        }
        private string _username = string.Empty;
        [STNodeProperty("Username", "The name of the user to unVIP.")]
        public string Username
        {
            get { return _username; }
            set
            {
                _username = value;
                Invalidate();
            }
        }

        private STNodeOption m_op_channel_in;
        private STNodeOption m_op_username_in;

        private struct MsgData
        {
            public string Channel { get; set; }
            public string Username { get; set; }
        }

        protected override void OnFlowTrigger()
        {
            if (MainForm.ChatState != ConnectionState.Connected)
                return;

            MainForm.TwitchQueue?.Enqueue(
                new Queues.QueuedTwitchTaskHandler(SendCommand),
                new MsgData { Channel = _channel, Username = _username }
            );
        }

        private void SendCommand(TwitchClient client, object data)
        {
            if (!(client?.IsConnected ?? false))
                return;

            MsgData dataObj = (MsgData)data;
            client.UnVIP(dataObj.Channel, dataObj.Username);
        }

        protected override void OnCreate()
        {
            base.OnCreate();
            Title = "UnVIP User";

            m_op_channel_in = InputOptions.Add("Channel", typeof(string), false);
            m_op_username_in = InputOptions.Add("Username", typeof(string), false);

            m_op_channel_in.DataTransfer += new STNodeOptionEventHandler(m_op_DataTransfer);
            m_op_username_in.DataTransfer += new STNodeOptionEventHandler(m_op_DataTransfer);
        }

        private void m_op_DataTransfer(object sender, STNodeOptionEventArgs e)
        {
            if (e.Status == ConnectionStatus.Connected && e.TargetOption.Data != null)
            {
                if (sender == m_op_channel_in)
                    Channel = (string)e.TargetOption.Data;
                else
                    Username = (string)e.TargetOption.Data;
            }
            else
            {
                if (sender == m_op_channel_in)
                    Channel = string.Empty;
                else
                    Username = string.Empty;
            }
        }
    }
}
