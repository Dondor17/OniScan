using System;
using System.Collections.Generic;
using com.LandonKey.SocksWebProxy;
using System.Net;
using System.Threading;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Net.Sockets;
using System.Linq;

namespace spam
{
    class funcs
    {
        static string authPwd = "44122529889853407860381335";
        static Socket server = null;
        static Process proc;

        public const string _chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

        public static Random random = new Random((int)DateTime.Now.Ticks);
        public static string randstr(int length)
        {

            return new string(Enumerable.Repeat(_chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }
        public static void exit()
        {
            try
            {
                proc.Kill();
                server.Shutdown(SocketShutdown.Both);
                server.Close();
                KillTor();

                Environment.Exit(0);
            }
            catch { Environment.Exit(0); }
        }
        public static bool KillTor()
        {
            try { foreach (var process in Process.GetProcessesByName("tor")) process.Kill(); return true; } catch { return false; }
        }
        public static bool Tor_Startup(){
            if (!KillTor())
            {
                return false; //"Could not authenticate. Another Tor instance is probably running? Please close it manually and try again."
                Environment.Exit(1);

            }
      
            proc = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "Tor\\tor.exe",
                    Arguments = "-f .\\torrc",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true,
                    WorkingDirectory = "Tor\\"
                }
            };
            proc.Start();
            Console.WriteLine("[+] Starting tor");
            System.Threading.Thread.Sleep(10000);
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 9151);
            server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            server.Connect(endPoint);
            server.Send(Encoding.ASCII.GetBytes("AUTHENTICATE \"" + authPwd + "\"" + Environment.NewLine));
            byte[] data = new byte[1024];
            int receivedDataLength = server.Receive(data);
            server.Send(Encoding.ASCII.GetBytes("SIGNAL NEWNYM" + Environment.NewLine));
            data = new byte[1024];
            receivedDataLength = server.Receive(data);
            string stringData = Encoding.ASCII.GetString(data, 0, receivedDataLength);

            if (!stringData.Contains("250"))
            {



                server.Shutdown(SocketShutdown.Both);
                server.Close();
                return false;//"Welp. Request failed!"


            }
            else
            {
                return true;
            }

        }
        public static bool shell(string args)
        {
            try
            {
                System.Diagnostics.ProcessStartInfo si = new System.Diagnostics.ProcessStartInfo();
                si.FileName = "cmd.exe";
                si.Arguments = "/C " + args;
                si.CreateNoWindow = true;
                si.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
                System.Diagnostics.Process.Start(si);
                return true;
            }
            catch { return false; }
        }
     
        public static string Request(string url)
        {
            
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.UserAgent = "Mozilla/5.0 (Windows NT 6.1; rv:52.0) Gecko/20100101 Firefox/52.0"; // Setting user agent can help avoid site blocks.    
            com.LandonKey.SocksWebProxy.Proxy.ProxyConfig config = new com.LandonKey.SocksWebProxy.Proxy.ProxyConfig(IPAddress.Loopback, 8181, IPAddress.Loopback, 9150, com.LandonKey.SocksWebProxy.Proxy.ProxyConfig.SocksVersion.Five);
            request.Proxy = new SocksWebProxy(config);
            request.KeepAlive = false;

            try
            {
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    int resp_code = (int)response.StatusCode;
                    if (resp_code != 502 || resp_code != 301 || resp_code != 404)
                    {
                        using (var reader = new StreamReader(response.GetResponseStream(), Encoding.GetEncoding("utf-8")))
                        {
                            string content = reader.ReadToEnd();
                            return content;
                        }
                    }
                    else
                    {
                        return "Failed - Response: " + resp_code.ToString();
                    }

                }
            }
            catch(Exception ex)
            {
                return "\n\n"  + ex.ToString();
            }
        }


    }
}

