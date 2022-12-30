using ST.Library.UI.NodeEditor;
using System;
using TwitchLib.Client;
using TwitchLib.Client.Extensions;

namespace ChattyVibes.Nodes.ActionNode.TwitchNode
{
    [STNode("/Actions/Twitch", "LauraRozier", "", "", "Twitch TimeoutUser node")]
    internal sealed class TimeoutUserNode : ActionNode
    {
        private string _channel = string.Empty;
        [STNodeProperty("Channel", "The channel to time the user out for.")]
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
        [STNodeProperty("Username", "The name of the user to time out.")]
        public string Username
        {
            get { return _username; }
            set
            {
                _username = value;
                Invalidate();
            }
        }
        private float _minutes = 1.0f;
        [STNodeProperty("Minutes", "The timeout duration as a minute fraction.")]
        public float Minutes
        {
            get { return _minutes; }
            set
            {
                _minutes = value;
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
        private STNodeOption m_op_username_in;
        private STNodeOption m_op_minutes_in;
        private STNodeOption m_op_message_in;

        private struct MsgData
        {
            public string Channel { get; set; }
            public string Username { get; set; }
            public float Minutes { get; set; }
            public string Message { get; set; }
        }

        protected override void OnFlowTrigger()
        {
            if (MainForm.ChatState != ConnectionState.Connected)
                return;

            MainForm.TwitchQueue?.Enqueue(
                new Queues.QueuedTwitchTaskHandler(SendCommand),
                new MsgData {
                    Channel = _channel,
                    Username = _username,
                    Minutes = _minutes,
                    Message = _message
                }
            );
        }

        private void SendCommand(TwitchClient client, object data)
        {
            if (!(client?.IsConnected ?? false))
                return;

            MsgData dataObj = (MsgData)data;
            client.TimeoutUser(
                dataObj.Channel,
                dataObj.Username,
                TimeSpan.FromMinutes(dataObj.Minutes),
                dataObj.Message
            );
        }

        protected override void OnCreate()
        {
            base.OnCreate();
            Title = "Timeout User";

            m_op_channel_in = InputOptions.Add("Channel", typeof(string), false);
            m_op_username_in = InputOptions.Add("Username", typeof(string), false);
            m_op_minutes_in = InputOptions.Add("Minutes", typeof(float), false);
            m_op_message_in = InputOptions.Add("Message", typeof(string), false);

            m_op_channel_in.DataTransfer += new STNodeOptionEventHandler(m_op_DataTransfer);
            m_op_username_in.DataTransfer += new STNodeOptionEventHandler(m_op_DataTransfer);
            m_op_minutes_in.DataTransfer += new STNodeOptionEventHandler(m_op_DataTransfer);
            m_op_message_in.DataTransfer += new STNodeOptionEventHandler(m_op_DataTransfer);
        }

        private void m_op_DataTransfer(object sender, STNodeOptionEventArgs e)
        {
            if (e.Status == ConnectionStatus.Connected && e.TargetOption.Data != null)
            {
                if (sender == m_op_channel_in)
                    Channel = (string)e.TargetOption.Data;
                else if (sender == m_op_username_in)
                    Username = (string)e.TargetOption.Data;
                else if (sender == m_op_minutes_in)
                    Minutes = (float)e.TargetOption.Data;
                else
                    Message = (string)e.TargetOption.Data;
            }
            else
            {
                if (sender == m_op_channel_in)
                    Channel = string.Empty;
                else if (sender == m_op_username_in)
                    Username = string.Empty;
                else if (sender == m_op_minutes_in)
                    Minutes = 1.0f;
                else
                    Message = string.Empty;
            }
        }
    }
}
