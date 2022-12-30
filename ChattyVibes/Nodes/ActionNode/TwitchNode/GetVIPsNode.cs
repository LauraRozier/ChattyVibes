using ChattyVibes.Events;
using ST.Library.UI.NodeEditor;
using TwitchLib.Client;
using TwitchLib.Client.Events;
using TwitchLib.Client.Extensions;

namespace ChattyVibes.Nodes.ActionNode.TwitchNode
{
    [STNode("/Actions/Twitch", "LauraRozier", "", "", "Twitch GetVIPs node")]
    internal sealed class GetVIPsNode : ActionNode
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
            client.GetVIPs(_curChannel);
        }

        protected override void OnCreate()
        {
            _direction = FlowDirection.ManualBoth;
            base.OnCreate();
            Title = "Get VIPs";

            m_op_in = InputOptions.Add("Channel", typeof(string), false);
            m_op_out = OutputOptions.Add("VIPs", typeof(string[]), false);

            m_op_in.DataTransfer += new STNodeOptionEventHandler(m_op_DataTransfer);

            m_op_out.TransferData(new string[0]);

            ((TwitchOnVIPsReceivedEvent)MainForm.EventFactory.GetEvent(EventType.TwitchOnVIPsReceived)).RaiseEvent +=
                OnEventNode_RaiseEvent;
        }

        ~GetVIPsNode()
        {
            ((TwitchOnVIPsReceivedEvent)MainForm.EventFactory.GetEvent(EventType.TwitchOnVIPsReceived)).RaiseEvent -=
                OnEventNode_RaiseEvent;
        }

        private void m_op_DataTransfer(object sender, STNodeOptionEventArgs e)
        {
            if (e.Status == ConnectionStatus.Connected && e.TargetOption.Data != null)
                Channel = (string)e.TargetOption.Data;
            else
                Channel = string.Empty;
        }

        private void OnEventNode_RaiseEvent(object sender, OnVIPsReceivedArgs e)
        {
            if (!e.Channel.Equals(_curChannel))
                return;

            m_op_out.TransferData(e.VIPs.ToArray());
            Trigger();
        }
    }
}
