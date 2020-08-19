using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using PcapDotNet.Core;
using PcapDotNet.Packets;
using System.Threading;
using System.Runtime.InteropServices;


namespace ROAMapper
{
    static class Program
    {

        static PacketHandler _eventHandler;
        static PhotonPacketHandler photonPacketHandler;

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool AllocConsole();

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        /// 
        [STAThread]
        static void Main()
        {
           // AllocConsole();

            // Basic support for Npcap. Needs to be able to support other installations (ie: installed on D: drive)
            var npcapDirectory = @"C:\Windows\System32\Npcap";
            Environment.SetEnvironmentVariable("PATH", Environment.GetEnvironmentVariable("PATH") + ";" + npcapDirectory);

            WorldMap.LoadMap();
            PlayerState.Load();

            _eventHandler = new PacketHandler();
            photonPacketHandler = new PhotonPacketHandler(_eventHandler);
            Thread t = new Thread(() => createListener());
            t.Start();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainWindow());                                          
     
        }

        private static void createListener()
        {
            IList<LivePacketDevice> allDevices = LivePacketDevice.AllLocalMachine;
            if (allDevices.Count == 0)
            {
                MessageBox.Show("No interfaces found! Make sure WinPcap is installed.");
                return;
            }
            // Print the list
            for (int i = 0; i != allDevices.Count; ++i)
            {
                LivePacketDevice device = allDevices[i];

                if (device.Description != null)
                    Console.WriteLine(" (" + device.Description + ")");
                else
                    Console.WriteLine(" (No description available)");
            }

            foreach (PacketDevice selectedDevice in allDevices.ToList())
            {
                // Open the device
                Thread t = new Thread(() =>
                {
                    using (PacketCommunicator communicator =
                           selectedDevice.Open(65536,                                  
                                               PacketDeviceOpenAttributes.Promiscuous, 
                                               1000))                                  
                    {

                        // Compile the filter
                        using (BerkeleyPacketFilter filter = communicator.CreateFilter("ip and udp and port 5056")) // Restrict to only photon event port
                        {
                            // Set the filter
                            communicator.SetFilter(filter);
                        }

                        Console.WriteLine("Listening on " + selectedDevice.Description + "...");

                        // start the capture
                        communicator.ReceivePackets(0, photonPacketHandler.PacketHandler);

                    }
                });
                // Make sure thread closes on application exit
                t.IsBackground = true;
                t.Start();
            }

        }
    }
}
