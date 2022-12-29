﻿using Buttplug;
using ChattyVibes.Events;
using ChattyVibes.Queues;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Net;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Forms;
using TwitchLib.Client;
using TwitchLib.Client.Enums;
using TwitchLib.Client.Events;
using TwitchLib.Client.Models;
using TwitchLib.Communication.Clients;
using TwitchLib.Communication.Events;
using TwitchLib.Communication.Models;

namespace ChattyVibes
{
    public partial class MainForm : Form
    {
        private const string CTwitchRedirectUrl = "http://localhost:3000";
        private const string CTwitchClientId = "77nu3r5gyqhuzsambceccrbd9ctjdo";
        private const string CTwitchAuthStateVerify = "chattyvibes_123test";
        private string _authToken = null;
        private readonly HttpListener _server = new HttpListener();

        internal readonly Config _conf = new Config();

        internal volatile static ConnectionState PlugState = ConnectionState.NotConnected;
        private ButtplugClient _plugClient = null;
        internal ButtplugClientDevice[] PlugDevices { get { return _plugClient.Devices; } }

        internal volatile static ConnectionState ChatState = ConnectionState.NotConnected;
        private WebSocketClient _socketClient = null;
        private TwitchClient _chatClient = null;

        private readonly static Color IdleBtnColor = Color.FromArgb(24, 30, 54);
        private readonly static Color SelectedBtnColor = Color.FromArgb(46, 51, 73);

        internal readonly List<string> LogMessages = new List<string>();

        internal readonly static EventFactory EventFactory = new EventFactory();
        internal readonly static Dictionary<uint, ButtplugDeviceQueue> ButtplugQueues = new Dictionary<uint, ButtplugDeviceQueue>();
        internal static TwitchQueue TwitchQueue = null;

        internal const string GraphDir = "./Graphs";
        internal const string FraphFileExt = ".bpgraph";
        internal readonly static string GraphFilePtrn = $"*{FraphFileExt}";

        private const int WM_NCLBUTTONDOWN = 0xA1;
        private const int HT_CAPTION = 0x2;
        [DllImport("user32.dll")]
        private static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImport("user32.dll")]
        private static extern bool ReleaseCapture();
        [DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn(
            int nLeftRect,
            int nTopRect,
            int nRightRect,
            int nBottomRect,
            int nWidthEllipse,
            int nHeightEllipse
        );

        public MainForm()
        {
            InitializeComponent();
            Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, Width, Height, 25, 25));
            Config.Load(ref _conf);
            _server.Prefixes.Add($"{CTwitchRedirectUrl}/");

            if (!Directory.Exists(GraphDir))
                Directory.CreateDirectory(GraphDir);
        }

        private void UpdateNavItem(Button btn)
        {
            pnlNavInd.Height = btn.Height;
            pnlNavInd.Top = btn.Top;
            pnlNavInd.Left = btn.Left;
            btn.BackColor = SelectedBtnColor;

            if (btn != btnHome)
                btnHome.BackColor = IdleBtnColor;

            if (btn != btnBindingGraph)
                btnBindingGraph.BackColor = IdleBtnColor;

            if (btn != btnLog)
                btnLog.BackColor = IdleBtnColor;
        }

        private void UpdateGUI()
        {
            if (pnlForm.Controls.Count > 0 && pnlForm.Controls[0] is FrmHome home)
                home.UpdateGUI();
        }

        internal async Task<DeviceBattery> SendBatteryLevelCommand(ButtplugClientDevice aToy)
        {
            var result = new DeviceBattery { Name = $"{aToy.Index} - {aToy.Name}" };

            if (aToy.AllowedMessages.ContainsKey(ServerMessage.Types.MessageAttributeType.BatteryLevelCmd))
            {
                double level = await aToy.SendBatteryLevelCmd();
                result.Level = $"{level:P0}";
            }
            else if (aToy.AllowedMessages.ContainsKey(ServerMessage.Types.MessageAttributeType.RssilevelCmd))
            {
                int level = await aToy.SendRSSIBatteryLevelCmd();
                result.Level = $"{100 + level}";
            }
            else
            {
                result.Level = "N/A";
            }

            return result;
        }

        /*
         * Form actions
         */

        private async void MainForm_Load(object sender, EventArgs e)
        {
            ButtplugFFILog.LogMessage += ButtplugFFILog_LogMessage;
            ButtplugFFILog.SetLogOptions(ButtplugLogLevel.Info, false);
            _plugClient = new ButtplugClient("ChattyVibes v" + Assembly.GetExecutingAssembly().GetName().Version.ToString());
            _plugClient.ErrorReceived += PlugClient_ErrorReceived;
            _plugClient.ServerDisconnect += PlugClient_ServerDisconnect;
            _plugClient.ScanningFinished += PlugClient_ScanningFinished;
            _plugClient.DeviceAdded += PlugClient_DeviceAdded;
            _plugClient.DeviceRemoved += PlugClient_DeviceRemoved;

            btnHome.PerformClick();
            UpdateGUI();
            await LogMsg("Setup and ready to start.");
        }

        private async void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            await EventFactory.Cleanup();

            foreach (var item in ButtplugQueues)
                await item.Value.Cleanup();

            ButtplugQueues.Clear();

            if (TwitchQueue != null)
            {
                await TwitchQueue.Cleanup();
                TwitchQueue = null;
            }

            if (_chatClient != null)
            {
                _chatClient.AutoReListenOnException = false;
                _chatClient.DisableAutoPong = true;

                if (_chatClient.IsConnected)
                {
                    foreach (var channel in _chatClient.JoinedChannels)
                        _chatClient.LeaveChannel(channel.Channel);

                    _chatClient.Disconnect();
                }

                _chatClient = null;
            }

            if (_socketClient != null)
            {
                if (_socketClient.IsConnected)
                    _socketClient.Close();

                _socketClient.Dispose();
                _socketClient = null;
            }

            if (_plugClient != null && _plugClient.Connected)
            {
                _plugClient.DisconnectAsync().Wait();
                _plugClient = null;
            }
        }

        internal async Task ConnectButtplug()
        {
            PlugState = ConnectionState.Connecting;
            UpdateGUI();
            Config.Save(_conf);

            try
            {
                await _plugClient.ConnectAsync(new ButtplugWebsocketConnectorOptions(new Uri($"ws://{_conf.ButtplugHostname}:{_conf.ButtplugPort}")));
            }
            catch (ButtplugConnectorException ex)
            {
                await LogMsg($"{DateTime.UtcNow:o} - Buttplug: Can't connect to Buttplug Server, exiting! - Message: {ex.Message}");
                PlugState = ConnectionState.Error;
                UpdateGUI();
                return;
            }
            catch (ButtplugHandshakeException ex)
            {
                await LogMsg($"{DateTime.UtcNow:o} - Buttplug: Handshake with Buttplug Server, exiting! - Message: {ex.Message}");
                PlugState = ConnectionState.Error;
                UpdateGUI();
                return;
            }

            try
            {
                await LogMsg($"{DateTime.UtcNow:o} - Buttplug: Connected, scanning for devices");
                await _plugClient.StartScanningAsync();
            }
            catch (ButtplugException ex)
            {
                await LogMsg($"{DateTime.UtcNow:o} - Buttplug: Scanning failed - Message: {ex.InnerException?.Message}");
                PlugState = ConnectionState.Error;
                await _plugClient.DisconnectAsync();
                return;
            }

            PlugState = ConnectionState.Connected;
            UpdateGUI();

            await Task.Delay(5000);

            if (_plugClient.IsScanning)
                await _plugClient.StopScanningAsync();

            await LogMsg($"{DateTime.UtcNow:o} - Buttplug: Scanning done");
        }

        internal async Task DisconnectButtplug()
        {
            PlugState = ConnectionState.Disconnecting;
            UpdateGUI();

            if (_plugClient.Connected)
                await _plugClient.DisconnectAsync();

            PlugState = ConnectionState.NotConnected;
            UpdateGUI();
        }

        internal async Task RescanDevices()
        {
            if (_plugClient == null || (!_plugClient.Connected) || _plugClient.IsScanning)
                return;

            try
            {
                await LogMsg($"{DateTime.UtcNow:o} - Buttplug: Rescanning devices");
                await _plugClient.StartScanningAsync();
            }
            catch (ButtplugException ex)
            {
                await LogMsg($"{DateTime.UtcNow:o} - Buttplug: Scanning failed - Message: {ex.Message}");
                return;
            }

            await Task.Delay(5000);

            if (_plugClient.IsScanning)
                await _plugClient.StopScanningAsync();
        }

        internal async Task ConnectTwitch()
        {
            ChatState = ConnectionState.Connecting;
            UpdateGUI();
            Config.Save(_conf);

            _socketClient = new WebSocketClient(new ClientOptions
            {
                ThrottlingPeriod = TimeSpan.FromSeconds(30),
                MessagesAllowedInPeriod = 750,
                WhisperThrottlingPeriod = TimeSpan.FromSeconds(30),
                WhispersAllowedInPeriod = 50
            });
            _chatClient = new TwitchClient(_socketClient, ClientProtocol.WebSocket);
            _chatClient.OnLog += ChatClient_OnLog;
            _chatClient.OnConnected += ChatClient_OnConnected;
            _chatClient.OnDisconnected += ChatClient_OnDisconnected;
            _chatClient.OnJoinedChannel += ChatClient_OnJoinedChannel;

            _chatClient.OnMessageReceived += ChatClient_OnMessageReceived;
            _chatClient.OnWhisperReceived += ChatClient_OnWhisperReceived;
            _chatClient.OnNewSubscriber += ChatClient_OnNewSubscriber;
            _chatClient.OnGiftedSubscription += ChatClient_OnGiftedSubscription;
            _chatClient.OnContinuedGiftedSubscription += ChatClient_OnContinuedGiftedSubscription;
            _chatClient.OnCommunitySubscription += ChatClient_OnCommunitySubscription;
            _chatClient.OnPrimePaidSubscriber += ChatClient_OnPrimePaidSubscriber;
            _chatClient.OnReSubscriber += ChatClient_OnReSubscriber;

            _authToken = null;
            _server.Start();

            Process.Start(
                "https://id.twitch.tv/oauth2/authorize" +
                "?response_type=token" +
                $"&client_id={CTwitchClientId}" +
                $"&redirect_uri={CTwitchRedirectUrl}" +
                $"&scope={HttpUtility.UrlEncode("chat:read chat:edit channel:read:subscriptions whispers:read channel:read:redemptions channel:read:hype_train channel:read:goals")}" +
                $"&state={CTwitchAuthStateVerify}" +
                "&force_verify=true"
            );

            _server.BeginGetContext(new AsyncCallback(IncomingHttpRequest), _server);

            while (_server.IsListening)
                await Task.Delay(10);

            var twitchCreds = new ConnectionCredentials(_conf.TwitchUsername, _authToken);
            _chatClient.Initialize(
                credentials: twitchCreds,
                channel: _conf.ChannelName,
                autoReListenOnExceptions: true
            );

            if (!_chatClient.Connect())
            {
                ChatState = ConnectionState.Error;
                UpdateGUI();
                return;
            }

            TwitchQueue?.Cleanup();
            TwitchQueue = new TwitchQueue(_chatClient);

            ChatState = ConnectionState.Connected;
            UpdateGUI();
        }

        internal async Task DisconnectTwitch()
        {
            ChatState = ConnectionState.Disconnecting;
            UpdateGUI();
            Stopwatch sw = Stopwatch.StartNew();

            if (TwitchQueue != null)
            {
                await TwitchQueue.Cleanup();
                TwitchQueue = null;
            }

            sw.Stop();
            await LogMsg($"DisconnectTwitch - TwitchQueue cleanup took {sw.ElapsedMilliseconds}ms");
            sw.Restart();

            if (_chatClient != null)
            {
                _chatClient.AutoReListenOnException = false;
                _chatClient.DisableAutoPong = true;

                if (_chatClient.IsConnected)
                {
                    foreach (var channel in _chatClient.JoinedChannels)
                        _chatClient.LeaveChannel(channel.Channel);

                    _chatClient.Disconnect();
                }

                _chatClient = null;
            }

            sw.Stop();
            await LogMsg($"DisconnectTwitch - _chatClient cleanup took {sw.ElapsedMilliseconds}ms");
            sw.Restart();

            if (_socketClient != null)
            {
                _socketClient.Dispose();
                _socketClient = null;
            }

            sw.Stop();
            await LogMsg($"DisconnectTwitch - _socketClient cleanup took {sw.ElapsedMilliseconds}ms");

            await Task.Delay(10);
            ChatState = ConnectionState.NotConnected;
            UpdateGUI();
        }

        private void MoveMainForm(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }

        private void BtnClose_Click(object sender, EventArgs e)
        {
            if (pnlForm.Controls.Count > 0)
                ((ChildForm)pnlForm.Controls[0]).Close();

            Close();
        }

        private void BtnHome_Click(object sender, EventArgs e)
        {
            if (pnlForm.Controls.Count > 0)
                ((ChildForm)pnlForm.Controls[0]).Close();

            lblTitle.Text = "HOME";
            UpdateNavItem((Button)sender);

            FrmHome frm = new FrmHome
            {
                MainFrm = this,
                Dock = DockStyle.Fill,
                TopLevel = false,
                TopMost = true,
                FormBorderStyle = FormBorderStyle.None
            };
            pnlForm.Controls.Clear();
            pnlForm.Controls.Add(frm);
            frm.Show();
        }

        private void BtnBindingGraph_Click(object sender, EventArgs e)
        {
            if (pnlForm.Controls.Count > 0)
                ((ChildForm)pnlForm.Controls[0]).Close();

            lblTitle.Text = "BINDING GRAPH";
            UpdateNavItem((Button)sender);

            FrmBindingGraphs frm = new FrmBindingGraphs
            {
                MainFrm = this,
                Dock = DockStyle.Fill,
                TopLevel = false,
                TopMost = true,
                FormBorderStyle = FormBorderStyle.None
            };
            pnlForm.Controls.Clear();
            pnlForm.Controls.Add(frm);
            frm.Show();
        }

        private void BtnLog_Click(object sender, EventArgs e)
        {
            if (pnlForm.Controls.Count > 0)
                ((ChildForm)pnlForm.Controls[0]).Close();

            lblTitle.Text = "LOG";
            UpdateNavItem((Button)sender);

            FrmLog frm = new FrmLog
            {
                MainFrm = this,
                Dock = DockStyle.Fill,
                TopLevel = false,
                TopMost = true,
                FormBorderStyle = FormBorderStyle.None
            };
            pnlForm.Controls.Clear();
            pnlForm.Controls.Add(frm);
            frm.Show();
        }

        private void IncomingHttpRequest(IAsyncResult result)
        {
            var httpListener = (HttpListener)result.AsyncState;
            var httpContext = httpListener.EndGetContext(result);
            // If we'd like the HTTP listener to accept more incoming requests, we'd just restart the "get context" here:
            httpListener.BeginGetContext(new AsyncCallback(IncomingAuth), httpListener);
            var httpRequest = httpContext.Request;

            // Build a response to send JS back to the browser for OAUTH Relay
            var httpResponse = httpContext.Response;
            byte[] buffer = System.Text.Encoding.UTF8.GetBytes($@"<html>
  <body>
    <b id=""auth"">Login Complete</b><br>
    <script type=""text/javascript"">
      window.onload = () => {{
        var xhr = new XMLHttpRequest();
        xhr.open('POST', '{CTwitchRedirectUrl}');
        xhr.send(window.location); // Sending the window location (the url bar) from the browser to our listener
      }};
    </script>
  </body>
</html>");

            httpResponse.ContentLength64 = buffer.Length;
            httpResponse.OutputStream.Write(buffer, 0, buffer.Length);
            httpResponse.OutputStream.Close();
        }

        private void IncomingAuth(IAsyncResult ar)
        {
            var httpListener = (HttpListener)ar.AsyncState;
            var httpContext = httpListener.EndGetContext(ar);
            var httpRequest = httpContext.Request;

            // Filter out any default requests we don't want
            if (httpRequest.Url.AbsolutePath.StartsWith("/favicon.ico"))
            {
                httpListener.BeginGetContext(new AsyncCallback(IncomingAuth), httpListener);
                return;
            }

            // This time we take an input stream from the request to receive the url
            string url;

            using (var reader = new StreamReader(httpRequest.InputStream, httpRequest.ContentEncoding))
                url = reader.ReadToEnd();

            Regex rx = new Regex(@".+#access_token=(.+)&scope.*&state=(.+)&token_type=bearer");
            var match = rx.Match(url);

            // If state doesn't match reject data
            if (match.Groups[2].Value != CTwitchAuthStateVerify)
            {
                httpListener.BeginGetContext(new AsyncCallback(IncomingAuth), httpListener);
                return;
            }

            _authToken = match.Groups[1].Value;
            httpListener.Stop();
        }

        internal async Task LogMsg(string aMsg)
        {
            Invoke(new MethodInvoker(() => {
                if (LogMessages.Count >= 1024)
                    LogMessages.RemoveRange(0, (LogMessages.Count - 1024) + 1);

                LogMessages.Add(aMsg);

                if (pnlForm.Controls.Count > 0 && pnlForm.Controls[0] is FrmLog log)
                    log.AddLogMsg(aMsg);
            }));
            await Task.Delay(1);
        }

        /*
         * Twitch actions
         */
        private async void ChatClient_OnLog(object sender, OnLogArgs e) =>
            await LogMsg($"{e.DateTime} - Twitch: {e.BotUsername} - {e.Data}");

        private async void ChatClient_OnConnected(object sender, OnConnectedArgs e)
        {
            await LogMsg($"{DateTime.UtcNow:o} - Twitch: {e.BotUsername} - Connected to Twitch");
            EventFactory.Enqueue(EventType.TwitchOnConnected, sender, e);
        }

        private async void ChatClient_OnDisconnected(object sender, OnDisconnectedEventArgs e)
        {
            await LogMsg($"{DateTime.UtcNow:o} - Twitch: Disconnected from Twitch");
            EventFactory.Enqueue(EventType.TwitchOnDisconnected, sender, e);
        }

        private async void ChatClient_OnJoinedChannel(object sender, OnJoinedChannelArgs e)
        {
            await LogMsg($"{DateTime.UtcNow:o} - Twitch: {e.BotUsername} - Joined channel {e.Channel}");
            EventFactory.Enqueue(EventType.TwitchOnJoinedChannel, sender, e);
        }

        private async void ChatClient_OnMessageReceived(object sender, OnMessageReceivedArgs e)
        {
            await LogMsg($"{DateTime.UtcNow:o} - Twitch: {e.ChatMessage.DisplayName} - Sent Message \"{e.ChatMessage.Message}\"");
            EventFactory.Enqueue(EventType.TwitchOnChatMsg, sender, e);
        }

        private async void ChatClient_OnWhisperReceived(object sender, OnWhisperReceivedArgs e)
        {
            await LogMsg($"{DateTime.UtcNow:o} - Twitch: {e.WhisperMessage.DisplayName} - Sent Whisper \"{e.WhisperMessage.Message}\"");
            EventFactory.Enqueue(EventType.TwitchOnWhisperMsg, sender, e);
        }

        private async void ChatClient_OnNewSubscriber(object sender, OnNewSubscriberArgs e)
        {
            await LogMsg($"{DateTime.UtcNow:o} - Twitch: {e.Subscriber.DisplayName} - New Sub tier {e.Subscriber.SubscriptionPlanName}");
            EventFactory.Enqueue(EventType.TwitchOnNewSub, sender, e);
        }

        private async void ChatClient_OnGiftedSubscription(object sender, OnGiftedSubscriptionArgs e)
        {
            await LogMsg($"{DateTime.UtcNow:o} - Twitch: {e.GiftedSubscription.DisplayName} - New Gift Sub \"{e.GiftedSubscription.MsgParamRecipientDisplayName}\" Tier {e.GiftedSubscription.MsgParamSubPlanName} Length {e.GiftedSubscription.MsgParamMonths}");
            EventFactory.Enqueue(EventType.TwitchOnGiftSub, sender, e);
        }

        private async void ChatClient_OnReSubscriber(object sender, OnReSubscriberArgs e)
        {
            await LogMsg($"{DateTime.UtcNow:o} - Twitch: {e.ReSubscriber.DisplayName} - Re-Sub tier {e.ReSubscriber.SubscriptionPlanName} Message \"{e.ReSubscriber.ResubMessage}\"");
            EventFactory.Enqueue(EventType.TwitchOnResub, sender, e);
        }

        private async void ChatClient_OnPrimePaidSubscriber(object sender, OnPrimePaidSubscriberArgs e)
        {
            await LogMsg($"{DateTime.UtcNow:o} - Twitch: {e.PrimePaidSubscriber.DisplayName} - Prime Subscriber");
            EventFactory.Enqueue(EventType.TwitchOnPrimeSub, sender, e);
        }

        private async void ChatClient_OnCommunitySubscription(object sender, OnCommunitySubscriptionArgs e)
        {
            await LogMsg($"{DateTime.UtcNow:o} - Twitch: {e.GiftedSubscription.DisplayName} - Community Subscriber tier {e.GiftedSubscription.MsgParamSubPlan}");
            EventFactory.Enqueue(EventType.TwitchOnCommunitySub, sender, e);
        }

        private async void ChatClient_OnContinuedGiftedSubscription(object sender, OnContinuedGiftedSubscriptionArgs e)
        {
            await LogMsg($"{DateTime.UtcNow:o} - Twitch: {e.ContinuedGiftedSubscription.DisplayName} - Continued Gifted Subscriber");
            EventFactory.Enqueue(EventType.TwitchOnContinuedGiftSub, sender, e);
        }

        private void Panel3_Paint(object sender, PaintEventArgs e)
        {
            ControlPaint.DrawBorder(
                e.Graphics, panel3.ClientRectangle,
                Color.Empty, 0, ButtonBorderStyle.None,
                Color.Empty, 0, ButtonBorderStyle.None,
                Color.Empty, 0, ButtonBorderStyle.None,
                Color.FromArgb(24, 30, 54), 2, ButtonBorderStyle.Solid
            );
        }

        /*
         * Buttplug.io actions
         */
        private async void ButtplugFFILog_LogMessage(object sender, string e) =>
            await LogMsg($"{e}");

        private async void PlugClient_ErrorReceived(object sender, ButtplugExceptionEventArgs e) =>
            await LogMsg($"{DateTime.UtcNow:o} - Buttplug: Error received from the server.  Message: {e.Exception.Message}");

        private async void PlugClient_ServerDisconnect(object sender, EventArgs e)
        {
            await LogMsg($"{DateTime.UtcNow:o} - Buttplug: Disconnected from the server");
            PlugState = ConnectionState.NotConnected;

            Invoke((MethodInvoker)async delegate {
                foreach (var item in ButtplugQueues)
                    await item.Value.Cleanup();

                ButtplugQueues.Clear();
                UpdateGUI();
            });
        }

        private async void PlugClient_ScanningFinished(object sender, EventArgs e) =>
            await LogMsg($"{DateTime.UtcNow:o} - Buttplug: Finished scanning for devices");

        private async void PlugClient_DeviceAdded(object sender, DeviceAddedEventArgs e)
        {
            string logMsg = $"\r\n{DateTime.UtcNow:o} - Buttplug: New device \"{e.Device.Name}\" (idx {e.Device.Index}) supports these messages:";

            foreach (var msgInfo in e.Device.AllowedMessages)
            {
                logMsg += $"\r\n  - {msgInfo.Key}";

                if (msgInfo.Value.FeatureCount != 0)
                    logMsg += $"\r\n    - Features: {msgInfo.Value.FeatureCount}";
            }

            await LogMsg(logMsg);
            ButtplugQueues.Add(e.Device.Index, new ButtplugDeviceQueue(e.Device));
            EventFactory.Enqueue(EventType.ButtplugDeviceAdded, sender, e);
        }

        private async void PlugClient_DeviceRemoved(object sender, DeviceRemovedEventArgs e)
        {
            await LogMsg($"{DateTime.UtcNow:o} - Buttplug: Device \"{e.Device.Name}\" (idx {e.Device.Index}) disconnected");
            EventFactory.Enqueue(EventType.ButtplugDeviceRemoved, sender, e);

            if (ButtplugQueues.ContainsKey(e.Device.Index))
            {
                await ButtplugQueues[e.Device.Index].Cleanup();
                ButtplugQueues.Remove(e.Device.Index);
            }
        }
    }
}
