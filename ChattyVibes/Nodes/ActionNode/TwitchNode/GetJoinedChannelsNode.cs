using ST.Library.UI.NodeEditor;
using TwitchLib.Client;

namespace ChattyVibes.Nodes.ActionNode.TwitchNode
{
    [STNode("/Actions/Twitch", "LauraRozier", "", "", "Twitch GetJoinedChannels node")]
    internal sealed class GetJoinedChannelsNode : ActionNode
    {
        private STNodeOption m_op_out;

        protected override void OnFlowTrigger()
        {
            if (MainForm.ChatState != ConnectionState.Connected)
                return;

            MainForm.TwitchQueue?.Enqueue(new Queues.QueuedTwitchTaskHandler(SendCommand), null);
        }

        private void SendCommand(TwitchClient client, object data)
        {
            if (!(client?.IsConnected ?? false))
                return;

            var channels = client.JoinedChannels;
            int len = channels.Count;
            string[] result = new string[len];

            for (int i = 0; i < len; i++)
                result[i] = channels[i].Channel;

            m_op_out.TransferData(result);
            Trigger();
        }

        protected override void OnCreate()
        {
            _direction = FlowDirection.ManualBoth;
            base.OnCreate();
            Title = "Get Joined Channels";

            m_op_out = OutputOptions.Add("Channels", typeof(string[]), false);

            m_op_out.TransferData(new string[0]);
        }
    }
}
