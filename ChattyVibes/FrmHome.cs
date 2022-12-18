using Buttplug;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Forms;

namespace ChattyVibes
{
    public partial class FrmHome : ChildForm
    {
        private Thread _batteryWorker;

        public FrmHome()
        {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            lvDevices.BeginUpdate();
            lvDevices.Items.Clear();
            lvDevices.EndUpdate();

            tbUsername.Text = MainFrm._conf.TwitchUsername;
            tbClientId.Text = MainFrm._conf.TwitchClientId;
            tbChannel.Text  = MainFrm._conf.ChannelName;
            tbHostname.Text = MainFrm._conf.ButtplugHostname;
            tbPort.Value    = MainFrm._conf.ButtplugPort;

            /*
            _batteryWorker = new Thread(new ThreadStart(HandleBatteries)) { IsBackground = true };
            _batteryWorker.Start();
            */

            UpdateGUI();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            /*
            _batteryWorker.Abort();
            _batteryWorker.Join(5000);
            */

            Config.Save(MainFrm._conf);
            base.OnFormClosing(e);
        }

        internal void UpdateGUI()
        {
            switch (MainForm._chatState)
            {
                case ConnectionState.NotConnected:
                    {
                        btnConnectTwitch.Enabled = true;
                        btnDisconnectTwitch.Enabled = false;
                        lblTwitchStatus.Text = "Disconnected";
                        break;
                    }
                case ConnectionState.Connecting:
                    {
                        btnConnectTwitch.Enabled = false;
                        btnDisconnectTwitch.Enabled = false;
                        lblTwitchStatus.Text = "Connecting";
                        break;
                    }
                case ConnectionState.Connected:
                    {
                        btnConnectTwitch.Enabled = false;
                        btnDisconnectTwitch.Enabled = true;
                        lblTwitchStatus.Text = "Connected";
                        break;
                    }
                case ConnectionState.Disconnecting:
                    {
                        btnConnectTwitch.Enabled = false;
                        btnDisconnectTwitch.Enabled = false;
                        lblTwitchStatus.Text = "Disconnecting";
                        break;
                    }
                case ConnectionState.Error:
                    {
                        btnConnectTwitch.Enabled = true;
                        btnDisconnectTwitch.Enabled = false;
                        lblTwitchStatus.Text = "Error";
                        break;
                    }
            }

            switch (MainForm._plugState)
            {
                case ConnectionState.NotConnected:
                    {
                        btnConnectIntiface.Enabled = true;
                        btnDisconnectIntiface.Enabled = false;
                        BtnRescanDevices.Enabled = false;
                        lblIntifaceStatus.Text = "Disconnected";
                        break;
                    }
                case ConnectionState.Connecting:
                    {
                        btnConnectIntiface.Enabled = false;
                        btnDisconnectIntiface.Enabled = false;
                        BtnRescanDevices.Enabled = false;
                        lblIntifaceStatus.Text = "Connecting";
                        break;
                    }
                case ConnectionState.Connected:
                    {
                        btnConnectIntiface.Enabled = false;
                        btnDisconnectIntiface.Enabled = true;
                        BtnRescanDevices.Enabled = true;
                        lblIntifaceStatus.Text = "Connected";
                        break;
                    }
                case ConnectionState.Disconnecting:
                    {
                        btnConnectIntiface.Enabled = false;
                        btnDisconnectIntiface.Enabled = false;
                        BtnRescanDevices.Enabled = false;
                        lblIntifaceStatus.Text = "Disconnecting";
                        break;
                    }
                case ConnectionState.Error:
                    {
                        btnConnectIntiface.Enabled = true;
                        btnDisconnectIntiface.Enabled = false;
                        BtnRescanDevices.Enabled = false;
                        lblIntifaceStatus.Text = "Error";
                        break;
                    }
            }
        }

        private void HandleBatteries()
        {
            while (true)
            {
                try
                {
                    if (MainForm._plugState != ConnectionState.Connected)
                    {
                        Thread.Sleep(100);
                        continue;
                    }

                    List<DeviceBattery> results = new List<DeviceBattery>();

                    foreach (ButtplugClientDevice toy in MainFrm._plugDevices)
                    {
                        try
                        {
                            results.Add(MainFrm.SendBatteryLevelCommand(toy).GetAwaiter().GetResult());
                        }
                        catch (ButtplugDeviceException)
                        {
                            MainFrm.LogMsg($"\r\n{DateTime.UtcNow:o} - Buttplug: Tried to talk to a disconnected device.").GetAwaiter();
                        }
                    }

                    lvDevices.Invoke(new MethodInvoker(() => {
                        lvDevices.BeginUpdate();
                        lvDevices.Items.Clear();

                        foreach (DeviceBattery item in results)
                        {
                            ListViewItem newEntry = new ListViewItem { Text = item.Name };
                            newEntry.SubItems.Add(item.Level);
                            lvDevices.Items.Add(newEntry);
                        }

                        lvDevices.EndUpdate();
                    }));

                    for (int i = 0; i < 300; i++)
                        Thread.Sleep(100); // Every 30 seconds is plenty fast enough
                }
                catch (ThreadAbortException) { return; }
            }
        }

        private void tbUsername_TextChanged(object sender, EventArgs e)
        {
            if (tbUsername.Text.Length <= 0)
            {
                MainFrm._conf.TwitchUsername = "ChattyVibes";
                tbUsername.Text = "ChattyVibes";
                return;
            }

            MainFrm._conf.TwitchUsername = tbUsername.Text;
        }

        private void tbClientId_TextChanged(object sender, EventArgs e)
        {
            if (tbClientId.Text.Length <= 0)
            {
                MainFrm._conf.TwitchClientId = "77nu3r5gyqhuzsambceccrbd9ctjdo";
                tbClientId.Text = "77nu3r5gyqhuzsambceccrbd9ctjdo";
                return;
            }

            MainFrm._conf.TwitchClientId = tbClientId.Text;
        }

        private void tbChannel_TextChanged(object sender, EventArgs e)
        {
            if (tbChannel.Text.Length <= 0)
            {
                MainFrm._conf.ChannelName = "ChattyVibes";
                tbChannel.Text = "ChattyVibes";
                return;
            }

            MainFrm._conf.ChannelName = tbChannel.Text;
        }

        private void tbHostname_TextChanged(object sender, EventArgs e)
        {
            if (tbHostname.Text.Length <= 0)
            {
                MainFrm._conf.ButtplugHostname = "localhost";
                tbHostname.Text = "localhost";
                return;
            }

            MainFrm._conf.ButtplugHostname = tbHostname.Text;
        }

        private void tbPort_ValueChanged(object sender, EventArgs e)
        {
            if (tbPort.Value <= 0)
            {
                MainFrm._conf.ButtplugPort = 12345u;
                tbPort.Value = 12345u;
                return;
            }

            MainFrm._conf.ButtplugPort = (uint)tbPort.Value;
        }

        private void btnConnectTwitch_Click(object sender, EventArgs e) =>
            MainFrm.ConnectTwitch().GetAwaiter();

        private void btnDisconnectTwitch_Click(object sender, EventArgs e) =>
            MainFrm.DisconnectTwitch().GetAwaiter();

        private void btnConnectIntiface_Click(object sender, EventArgs e) =>
            MainFrm.ConnectButtplug().GetAwaiter();

        private void btnDisconnectIntiface_Click(object sender, EventArgs e) =>
            MainFrm.DisconnectButtplug().GetAwaiter();

        private void BtnRescanDevices_Click(object sender, EventArgs e) =>
            MainFrm.RescanDevices().GetAwaiter();
    }
}
