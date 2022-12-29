using ST.Library.UI.NodeEditor;
using TwitchLib.Client;

namespace ChattyVibes.Nodes.ActionNode.TwitchNode
{
    [STNode("/Actions/Twitch", "LauraRozier", "", "", "Twitch SendWhisper node")]
    internal sealed class SendWhisperNode : ActionNode
    {
        private string _username = string.Empty;
        [STNodeProperty("Username", "The name of the user to send the whisper to.")]
        public string Username
        {
            get { return _username; }
            set
            {
                _username = value;
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

        private STNodeOption m_op_username_in;
        private STNodeOption m_op_message_in;

        private struct MsgData
        {
            public string Username { get; set; }
            public string Message { get; set; }
        }

        protected override void OnFlowTrigger()
        {
            if (MainForm.ChatState != ConnectionState.Connected)
                return;

            MainForm.TwitchQueue?.Enqueue(
                new Queues.QueuedTwitchTaskHandler(SendCommand),
                new MsgData { Username = _username, Message = _message }
            );
        }

        private void SendCommand(TwitchClient client, object data)
        {
            if (!(client?.IsConnected ?? false))
                return;

            MsgData dataObj = (MsgData)data;
            client.SendWhisper(dataObj.Username, dataObj.Message);
        }

        protected override void OnCreate()
        {
            base.OnCreate();
            Title = "Send Whisper";

            m_op_username_in = InputOptions.Add("Username", typeof(string), false);
            m_op_message_in = InputOptions.Add("Message", typeof(string), false);

            m_op_username_in.DataTransfer += new STNodeOptionEventHandler(m_op_DataTransfer);
            m_op_message_in.DataTransfer += new STNodeOptionEventHandler(m_op_DataTransfer);
        }

        private void m_op_DataTransfer(object sender, STNodeOptionEventArgs e)
        {
            if (e.Status == ConnectionStatus.Connected && e.TargetOption.Data != null)
            {
                if (sender == m_op_username_in)
                    Username = (string)e.TargetOption.Data;
                else
                    Message = (string)e.TargetOption.Data;
            }
            else
            {
                if (sender == m_op_username_in)
                    Username = string.Empty;
                else
                    Message = string.Empty;
            }
        }
    }
}
