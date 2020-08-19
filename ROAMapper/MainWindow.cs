using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;

namespace ROAMapper
{
    public partial class MainWindow : Form
    {
        private static System.Timers.Timer statusTimer;

        public MainWindow()
        {
            InitializeComponent();
            
            this.Text += " v" + Settings.clientVersion;

        }

        private void MainWindow_Load(object sender, EventArgs e)
        {
            // Check for status every 15 seconds
            statusTimer = new System.Timers.Timer(15000);
            statusTimer.Elapsed += onStatusTimerExpired;

            PacketHandlerEvents.PlayerJoinEvent += onPlayerJoinEvent;

            // These events indicate packets are being read
            PacketHandlerEvents.TimeSyncEvent += onPacketSuccessfullyRead;
            PacketHandlerEvents.PlayerJoinEvent += onPacketSuccessfullyRead;

            Settings.Load();
            lblClientID.Text = Settings.clientID.Remove(Settings.clientID.Length - 12, 12) + "************";
        }

        private void MainWindow_Resize(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                Hide();
                notifyIcon1.Visible = true;
            }
        }


        private void onPlayerJoinEvent(object sender, PlayerJoinEventArgs e)
        {
            lblCurrentZone.Text = WorldMap.GetMapDisplayName(e.mapID);
        }

        private void onPacketSuccessfullyRead(object sender, EventArgs e)
        {
            lblStatus.Text = "Green";

            statusTimer.Stop();
            statusTimer.Start();
        }

        private void onStatusTimerExpired(object sender, ElapsedEventArgs e)
        {
            lblStatus.Text = "Red";
        }
        

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            Show();
            this.WindowState = FormWindowState.Normal;
            notifyIcon1.Visible = false;
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }
    }
}
