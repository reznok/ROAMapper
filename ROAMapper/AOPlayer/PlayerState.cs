using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ROAMapper
{
    public static class PlayerState
    {

        public static string currentMapID;
        public static long currentTime;

        public static void Load()
        {
            PacketHandlerEvents.PlayerJoinEvent += onJoinEvent;
            PacketHandlerEvents.TimeSyncEvent += onTimeSyncEvent;

            Console.WriteLine("PlayerState Loaded");
        }

        class ClientLocation
        {
            public string clientID;
            public string location;
        }

        private static void onTimeSyncEvent(object sender, TimeSyncEventArgs e)
        {
            currentTime = e.time;
        }
        private static void onJoinEvent(object sender, PlayerJoinEventArgs e)
        {
            currentMapID = e.mapID;

            ClientLocation c = new ClientLocation();
            c.clientID = Settings.clientID;
            c.location = currentMapID;

            string url = Settings.ROA_MAPPER_API + "client/location";

            var client = new WebClient();
            client.Headers[HttpRequestHeader.ContentType] = "application/json";
            Uri uri = new Uri(url);
            var servicePoint = ServicePointManager.FindServicePoint(uri);
            servicePoint.Expect100Continue = false;

            client.UploadStringAsync(uri, Newtonsoft.Json.JsonConvert.SerializeObject(c));

        }
    }
}
