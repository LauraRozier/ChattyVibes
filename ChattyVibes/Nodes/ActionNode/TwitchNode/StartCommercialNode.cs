using ST.Library.UI.NodeEditor;
using System.Drawing;
using TwitchLib.Client;
using TwitchLib.Client.Enums;
using TwitchLib.Client.Extensions;

namespace ChattyVibes.Nodes.ActionNode.TwitchNode
{
    [STNode("/Actions/Twitch", "LauraRozier", "", "", "Twitch StartCommercial node")]
    internal sealed class StartCommercialNode : ActionNode
    {
        private string _channel = string.Empty;
        [STNodeProperty("Channel", "The name of the channel to start the commercial for.")]
        public string Channel
        {
            get { return _channel; }
            set
            {
                _channel = value;
                Invalidate();
            }
        }
        private CommercialLength _duration = CommercialLength.Seconds60;
        [STNodeProperty("Duration", "The duration of the commercial.")]
        public CommercialLength Duration
        {
            get { return _duration; }
            set
            {
                _duration = value;
                m_ctrl_select.Enum = value;
                Invalidate();
            }
        }

        private NodeSelectEnumBox m_ctrl_select;

        private STNodeOption m_op_channel_in;
        private STNodeOption m_op_duration_in;

        private struct MsgData
        {
            public string Channel { get; set; }
            public CommercialLength Duration { get; set; }
        }

        protected override void OnFlowTrigger()
        {
            if (MainForm.ChatState != ConnectionState.Connected)
                return;

            MainForm.TwitchQueue?.Enqueue(
                new Queues.QueuedTwitchTaskHandler(SendCommand),
                new MsgData { Channel = _channel, Duration = _duration }
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
                    client.StartCommercial(dataObj.Channel, dataObj.Duration);
            }
            catch { }
        }

        protected override void OnCreate()
        {
            base.OnCreate();
            Title = "Start Commercial Chat";

            m_op_channel_in = InputOptions.Add("Channel", typeof(string), false);
            m_op_duration_in = InputOptions.Add(string.Empty, typeof(CommercialLength), false);

            m_op_channel_in.DataTransfer += new STNodeOptionEventHandler(m_op_DataTransfer);
            m_op_duration_in.DataTransfer += new STNodeOptionEventHandler(m_op_DataTransfer);

            m_ctrl_select = new NodeSelectEnumBox
            {
                DisplayRectangle = new Rectangle(10, 41, 120, 18),
                Enum = _duration
            };
            m_ctrl_select.ValueChanged += (s, e) =>
            {
                _duration = (CommercialLength)m_ctrl_select.Enum;
                Invalidate();
            };
            Controls.Add(m_ctrl_select);
        }

        private void m_op_DataTransfer(object sender, STNodeOptionEventArgs e)
        {
            if (e.Status == ConnectionStatus.Connected && e.TargetOption.Data != null)
            {
                if (sender == m_op_channel_in)
                    Channel = (string)e.TargetOption.Data;
                else
                    Duration = (CommercialLength)e.TargetOption.Data;
            }
            else
            {
                if (sender == m_op_channel_in)
                    Channel = string.Empty;
                else
                    Duration = CommercialLength.Seconds60;
            }
        }
    }
}
