using ChattyVibes.Events;
using ST.Library.UI.NodeEditor;
using TwitchLib.Client;
using TwitchLib.Client.Events;
using TwitchLib.Client.Extensions;

namespace ChattyVibes.Nodes.ActionNode.TwitchNode
{
    [STNode("/Actions/Twitch", "LauraRozier", "", "", "Twitch GetModerators node")]
    internal sealed class GetModeratorsNode : ActionNode
    {
        private string _channel = string.Empty;
        [STNodeProperty("Channel", "The name of the channel.")]
        public string Channel
        {
            get { return _channel; }
            set
            {
                _channel = value;
                Invalidate();
            }
        }

        private volatile string _curChannel;

        private STNodeOption m_op_in;
        private STNodeOption m_op_out;

        protected override void OnFlowTrigger()
        {
            if (MainForm.ChatState != ConnectionState.Connected)
                return;

            MainForm.TwitchQueue?.Enqueue(new Queues.QueuedTwitchTaskHandler(SendCommand), _channel);
        }

        private void SendCommand(TwitchClient client, object data)
        {
            if (!(client?.IsConnected ?? false))
                return;

            _curChannel = (string)data;
            client.GetChannelModerators(_curChannel);
        }

        protected override void OnCreate()
        {
            _direction = FlowDirection.ManualBoth;
            base.OnCreate();
            Title = "Get Moderators";

            m_op_in = InputOptions.Add("Channel", typeof(string), false);
            m_op_out = OutputOptions.Add("Moderators", typeof(string[]), false);

            m_op_in.DataTransfer += new STNodeOptionEventHandler(m_op_DataTransfer);

            m_op_out.TransferData(new string[0]);

            ((TwitchOnModeratorsReceivedEvent)MainForm.EventFactory.GetEvent(EventType.TwitchOnModeratorsReceived)).RaiseEvent +=
                OnEventNode_RaiseEvent;
        }

        ~GetModeratorsNode()
        {
            ((TwitchOnModeratorsReceivedEvent)MainForm.EventFactory.GetEvent(EventType.TwitchOnModeratorsReceived)).RaiseEvent -=
                OnEventNode_RaiseEvent;
        }

        private void m_op_DataTransfer(object sender, STNodeOptionEventArgs e)
        {
            if (e.Status == ConnectionStatus.Connected && e.TargetOption.Data != null)
                Channel = (string)e.TargetOption.Data;
            else
                Channel = string.Empty;
        }

        private void OnEventNode_RaiseEvent(object sender, OnModeratorsReceivedArgs e)
        {
            if (!e.Channel.Equals(_curChannel))
                return;

            m_op_out.TransferData(e.Moderators.ToArray());
            Trigger();
        }
    }
}
