using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ROAMapper
{
    public static class Settings
    {
        public static string ROA_MAPPER_API = "https://albion.reznok.com/api/";
        public static string clientID = "";
        private static DateTime _lastJoinReceived;

        public static string clientVersion = "1.3.1";

        // Must receive a join within 3 seconds to be valid to send ROA data
        private const int JOIN_BUFFER = 3;

        public static void Load()
        {
            try
            {
                clientID = System.IO.File.ReadAllText("client_id.txt");
                ConfirmClientID(clientID);
                PacketHandlerEvents.PlayerJoinEvent += onJoinReceived;
            }

            catch
            {
                 ShowInputDialog(ref clientID);
                 ConfirmClientID(clientID);

            }
        }

        private static void onJoinReceived(object sender, EventArgs e)
        {
            _lastJoinReceived = DateTime.Now;
        }

        public static bool RecentlyJoinedNewMap()
        {

            return (DateTime.Now - _lastJoinReceived).Seconds <= JOIN_BUFFER;
        }


        private static bool ConfirmClientID(string clientID)
        {
            // POST the ROA updates

            string url = Settings.ROA_MAPPER_API + "client/confirm/" + clientID;

            var client = new WebClient();
            Uri uri = new Uri(url);
            var servicePoint = ServicePointManager.FindServicePoint(uri);
            servicePoint.Expect100Continue = false;

            try
            {
                var success = client.DownloadString(url);
                System.IO.File.WriteAllText(@"client_id.txt", clientID);
                return true;
            }
            catch
            {
                MessageBox.Show("Invalid Client ID");
                Application.Exit();
            }
            return false;
        }

      
        private static DialogResult ShowInputDialog(ref string input)
        {
            System.Drawing.Size size = new System.Drawing.Size(400, 70);
            Form inputBox = new Form();

            inputBox.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            inputBox.ClientSize = size;
            inputBox.Text = "Enter Client ID";

            System.Windows.Forms.TextBox textBox = new TextBox();
            textBox.Size = new System.Drawing.Size(size.Width - 10, 23);
            textBox.Location = new System.Drawing.Point(5, 5);
            textBox.Text = input;
            inputBox.Controls.Add(textBox);

            Button okButton = new Button();
            okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            okButton.Name = "okButton";
            okButton.Size = new System.Drawing.Size(75, 23);
            okButton.Text = "&OK";
            okButton.Location = new System.Drawing.Point(size.Width - 80 - 80, 39);
            inputBox.Controls.Add(okButton);

            Button cancelButton = new Button();
            cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            cancelButton.Name = "cancelButton";
            cancelButton.Size = new System.Drawing.Size(75, 23);
            cancelButton.Text = "&Cancel";
            cancelButton.Location = new System.Drawing.Point(size.Width - 80, 39);
            inputBox.Controls.Add(cancelButton);

            inputBox.AcceptButton = okButton;
            inputBox.CancelButton = cancelButton;

            DialogResult result = inputBox.ShowDialog();
            input = textBox.Text;
            return result;
        }
    }

}
