using ST.Library.UI.NodeEditor;
using System;
using TwitchLib.Client;
using TwitchLib.Client.Extensions;

namespace ChattyVibes.Nodes.ActionNode.TwitchNode
{
    [STNode("/Actions/Twitch", "LauraRozier", "", "", "Twitch SlowMode node")]
    internal sealed class SlowModeNode : ActionNode
    {
        private string _channel = string.Empty;
        [STNodeProperty("Channel", "The name of the channel to change the Slow mode of.")]
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
        [STNodeProperty("Enabled", "Wether to enable or disable Slow mode.")]
        public bool Enabled
        {
            get { return _enabled; }
            set
            {
                _enabled = value;
                Invalidate();
            }
        }
        private int _days = 0;
        [STNodeProperty("Days", "The amount of days they need to be following.")]
        public int Days
        {
            get { return _days; }
            set
            {
                _days = value;
                Invalidate();
            }
        }
        private int _hours = 0;
        [STNodeProperty("Hours", "The amount of days they need to be following.")]
        public int Hours
        {
            get { return _hours; }
            set
            {
                _hours = value;
                Invalidate();
            }
        }
        private int _minutes = 30;
        [STNodeProperty("Minutes", "The amount of days they need to be following.")]
        public int Minutes
        {
            get { return _minutes; }
            set
            {
                _minutes = value;
                Invalidate();
            }
        }

        private STNodeOption m_op_channel_in;
        private STNodeOption m_op_ena_in;
        private STNodeOption m_op_days_in;
        private STNodeOption m_op_hours_in;
        private STNodeOption m_op_minutes_in;

        private struct MsgData
        {
            public string Channel { get; set; }
            public bool Enabled { get; set; }
            public int Days { get; set; }
            public int Hours { get; set; }
            public int Minutes { get; set; }
        }

        protected override void OnFlowTrigger()
        {
            if (MainForm.ChatState != ConnectionState.Connected)
                return;

            MainForm.TwitchQueue?.Enqueue(
                new Queues.QueuedTwitchTaskHandler(SendCommand),
                new MsgData
                {
                    Channel = _channel,
                    Enabled = _enabled,
                    Days = _days,
                    Hours = _hours,
                    Minutes = _minutes
                }
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
                        client.SlowModeOn(dataObj.Channel, new TimeSpan(dataObj.Days, dataObj.Hours, dataObj.Minutes, 0));
                    else
                        client.SlowModeOff(dataObj.Channel);
                }
            } catch { }
        }

        protected override void OnCreate()
        {
            base.OnCreate();
            Title = "Slow Mode";

            m_op_channel_in = InputOptions.Add("Channel", typeof(string), false);
            m_op_ena_in = InputOptions.Add("Enabled", typeof(bool), false);
            m_op_days_in = InputOptions.Add("Days (<= 1)", typeof(int), false);
            m_op_hours_in = InputOptions.Add("Hours", typeof(int), false);
            m_op_minutes_in = InputOptions.Add("Minutes", typeof(int), false);

            m_op_channel_in.DataTransfer += new STNodeOptionEventHandler(m_op_DataTransfer);
            m_op_ena_in.DataTransfer += new STNodeOptionEventHandler(m_op_DataTransfer);
            m_op_days_in.DataTransfer += new STNodeOptionEventHandler(m_op_DataTransfer);
            m_op_hours_in.DataTransfer += new STNodeOptionEventHandler(m_op_DataTransfer);
            m_op_minutes_in.DataTransfer += new STNodeOptionEventHandler(m_op_DataTransfer);
        }

        private void m_op_DataTransfer(object sender, STNodeOptionEventArgs e)
        {
            if (e.Status == ConnectionStatus.Connected && e.TargetOption.Data != null)
            {
                if (sender == m_op_channel_in)
                    Channel = (string)e.TargetOption.Data;
                else if (sender == m_op_ena_in)
                    Enabled = (bool)e.TargetOption.Data;
                else if (sender == m_op_days_in)
                    Days = (int)e.TargetOption.Data;
                else if (sender == m_op_hours_in)
                    Hours = (int)e.TargetOption.Data;
                else
                    Minutes = (int)e.TargetOption.Data;
            }
            else
            {
                if (sender == m_op_channel_in)
                    Channel = string.Empty;
                else if (sender == m_op_ena_in)
                    Enabled = true;
                else if (sender == m_op_days_in)
                    Days = 0;
                else if (sender == m_op_hours_in)
                    Hours = 0;
                else
                    Minutes = 30;
            }
        }
    }
}
