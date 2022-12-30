using ST.Library.UI.NodeEditor;
using TwitchLib.Client;
using TwitchLib.Client.Extensions;

namespace ChattyVibes.Nodes.ActionNode.TwitchNode
{
    [STNode("/Actions/Twitch", "LauraRozier", "", "", "Twitch DeleteMessage node")]
    internal sealed class DeleteMessageNode : ActionNode
    {
        private string _channel = string.Empty;
        [STNodeProperty("Channel", "The name of the channel to remove the message from.")]
        public string Channel
        {
            get { return _channel; }
            set
            {
                _channel = value;
                Invalidate();
            }
        }
        private string _messageId = string.Empty;
        [STNodeProperty("Message ID", "The message ID to remove.")]
        public string MessageId
        {
            get { return _messageId; }
            set
            {
                _messageId = value;
                Invalidate();
            }
        }

        private STNodeOption m_op_channel_in;
        private STNodeOption m_op_message_in;

        private struct MsgData
        {
            public string Channel { get; set; }
            public string MessageId { get; set; }
        }

        protected override void OnFlowTrigger()
        {
            if (MainForm.ChatState != ConnectionState.Connected)
                return;

            MainForm.TwitchQueue?.Enqueue(
                new Queues.QueuedTwitchTaskHandler(SendCommand),
                new MsgData { Channel = _channel, MessageId = _messageId }
            );
        }

        private void SendCommand(TwitchClient client, object data)
        {
            if (!(client?.IsConnected ?? false))
                return;

            MsgData dataObj = (MsgData)data;
            client.DeleteMessage(dataObj.Channel, dataObj.MessageId);
        }

        protected override void OnCreate()
        {
            base.OnCreate();
            Title = "Delete Message";

            m_op_channel_in = InputOptions.Add("Channel", typeof(string), false);
            m_op_message_in = InputOptions.Add("Message ID", typeof(string), false);

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
                    MessageId = (string)e.TargetOption.Data;
            }
            else
            {
                if (sender == m_op_channel_in)
                    Channel = string.Empty;
                else
                    MessageId = string.Empty;
            }
        }
    }
}
