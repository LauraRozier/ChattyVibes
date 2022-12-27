using ST.Library.UI.NodeEditor;
using TwitchLib.Client;

namespace ChattyVibes.Nodes.ActionNode.TwitchNode
{
    [STNode("/Actions/Twitch", "LauraRozier", "", "", "Twitch SendMessage node")]
    internal sealed class SendMessageNode : ActionNode
    {
        private string _channel = string.Empty;
        [STNodeProperty("Channel", "The channel to send the message to.")]
        public string Channel
        {
            get { return _channel; }
            set
            {
                _channel = value;
                Invalidate();
            }
        }
        private string _message = string.Empty;
        [STNodeProperty("Message", "The message to send.")]
        public string Message
        {
            get { return _message; }
            set
            {
                _message = value;
                Invalidate();
            }
        }

        private STNodeOption m_op_channel_in;
        private STNodeOption m_op_message_in;

        private struct MsgData
        {
            public string Channel { get; set; }
            public string Message { get; set; }
        }

        protected override void OnFlowTrigger()
        {
            if (MainForm._chatState != ConnectionState.Connected)
                return;

            MainForm.TwitchQueue?.Enqueue(
                new Queues.QueuedTwitchTaskHandler(SendCommand),
                new MsgData { Channel = _channel, Message = _message }
            );
        }

        private void SendCommand(TwitchClient client, object data)
        {
            if (!(client?.IsConnected ?? false))
                return;

            MsgData dataObj = (MsgData)data;
            client.SendMessage(dataObj.Channel, dataObj.Message);
        }

        protected override void OnCreate()
        {
            base.OnCreate();
            Title = "Send Message";

            m_op_channel_in = InputOptions.Add("Channel", typeof(string), false);
            m_op_message_in = InputOptions.Add("Message", typeof(string), false);

            m_op_channel_in.DataTransfer += new STNodeOptionEventHandler(m_op_DataTransfer);
            m_op_message_in.DataTransfer += new STNodeOptionEventHandler(m_op_DataTransfer);
        }

        private void m_op_DataTransfer(object sender, STNodeOptionEventArgs e)
        {
            if (e.Status == ConnectionStatus.Connected && e.TargetOption.Data != null)
            {
                if (sender == m_op_channel_in)
                    Channel = (string)e.TargetOption.Data;
                else
                    Message = (string)e.TargetOption.Data;
            }
            else
            {
                if (sender == m_op_channel_in)
                    Channel = string.Empty;
                else
                    Message = string.Empty;
            }
        }
    }
}
