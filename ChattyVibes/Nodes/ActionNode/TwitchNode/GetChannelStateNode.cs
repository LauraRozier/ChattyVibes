using ST.Library.UI.NodeEditor;
using System;
using TwitchLib.Client;

namespace ChattyVibes.Nodes.ActionNode.TwitchNode
{
    [STNode("/Actions/Twitch", "LauraRozier", "", "", "Twitch GetChannelState node")]
    internal sealed class GetChannelStateNode : ActionNode
    {
        private string _channel = string.Empty;
        [STNodeProperty("Channel", "The name of the channel to change the EmoteOnly mode of.")]
        public string Channel
        {
            get { return _channel; }
            set
            {
                _channel = value;
                Invalidate();
            }
        }

        private STNodeOption m_op_channel_in;
        private STNodeOption m_op_lang_out;
        private STNodeOption m_op_room_id_out;
        private STNodeOption m_op_rituals_out;
        private STNodeOption m_op_mercury_out;
        private STNodeOption m_op_r9k_out;
        private STNodeOption m_op_sub_only_out;
        private STNodeOption m_op_emote_only_out;
        private STNodeOption m_op_followers_only_out;
        private STNodeOption m_op_slow_mode_out;

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
                var channel = client.GetJoinedChannel(dataObj.Channel);

                if (channel == default)
                {
                    Trigger();
                    return;
                }

                m_op_lang_out.TransferData(channel.ChannelState.BroadcasterLanguage);
                m_op_room_id_out.TransferData(channel.ChannelState.RoomId);
                m_op_rituals_out.TransferData(channel.ChannelState.Rituals ?? false);
                m_op_mercury_out.TransferData(channel.ChannelState.Mercury);
                m_op_r9k_out.TransferData(channel.ChannelState.R9K ?? false);
                m_op_slow_mode_out.TransferData(channel.ChannelState.SlowMode ?? -1);
                m_op_emote_only_out.TransferData(channel.ChannelState.EmoteOnly ?? false);
                m_op_followers_only_out.TransferData((float)(channel.ChannelState.FollowersOnly ?? TimeSpan.FromMinutes(-1)).TotalMinutes);
                m_op_sub_only_out.TransferData(channel.ChannelState.SubOnly ?? false);
            }
            catch
            {
                m_op_lang_out.TransferData(string.Empty);
                m_op_room_id_out.TransferData(string.Empty);
                m_op_rituals_out.TransferData(false);
                m_op_mercury_out.TransferData(false);
                m_op_r9k_out.TransferData(false);
                m_op_slow_mode_out.TransferData(-1);
                m_op_emote_only_out.TransferData(false);
                m_op_followers_only_out.TransferData(TimeSpan.FromMinutes(-1));
                m_op_sub_only_out.TransferData(false);
            }

            Trigger();
        }

        protected override void OnCreate()
        {
            _direction = FlowDirection.ManualBoth;
            base.OnCreate();
            Title = "Get Channel State";

            m_op_channel_in = InputOptions.Add("Channel", typeof(string), true);
            m_op_lang_out = OutputOptions.Add("Broadcaster Language", typeof(string), false);
            m_op_room_id_out = OutputOptions.Add("Room ID", typeof(string), false);
            m_op_rituals_out = OutputOptions.Add("Rituals (Deprecated)", typeof(bool), false);
            m_op_mercury_out = OutputOptions.Add("Mercury (Deprecated)", typeof(bool), false);
            m_op_r9k_out = OutputOptions.Add("R9K (Spam Mitigation)", typeof(bool), false);
            m_op_slow_mode_out = OutputOptions.Add("Slow Mode", typeof(int), false);
            m_op_emote_only_out = OutputOptions.Add("Emote Only", typeof(bool), false);
            m_op_followers_only_out = OutputOptions.Add("Followers Only", typeof(float), false);
            m_op_sub_only_out = OutputOptions.Add("Sub Only", typeof(bool), false);

            m_op_channel_in.DataTransfer += new STNodeOptionEventHandler(m_op_DataTransfer);

            m_op_lang_out.TransferData(string.Empty);
            m_op_room_id_out.TransferData(string.Empty);
            m_op_rituals_out.TransferData(false);
            m_op_mercury_out.TransferData(false);
            m_op_r9k_out.TransferData(false);
            m_op_slow_mode_out.TransferData(-1);
            m_op_emote_only_out.TransferData(false);
            m_op_followers_only_out.TransferData(TimeSpan.FromMinutes(-1));
            m_op_sub_only_out.TransferData(false);
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
