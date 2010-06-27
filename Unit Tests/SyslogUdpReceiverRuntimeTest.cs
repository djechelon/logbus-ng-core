using It.Unina.Dis.Logbus.InChannels;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System;
using It.Unina.Dis.Logbus;
using System.Threading;
using System.IO;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.Resources;
using System.Reflection;

namespace Unit_Tests
{
    /// <summary>
    /// This test simulates real-world usage of SyslogUdpListener by submitting Syslog messages via a real UDP channel
    /// </summary>
    [TestClass()]
    public class SyslogUdpReceiverRuntimeTest
    {

        private const int port = 37845;

        private Thread injector_thread;
        private AutoResetEvent test_finished;
        private List<SyslogMessage> messages_to_test;
        private int logs_sent = 0, logs_received = 0;

        [TestInitialize()]
        public void Init()
        {
            injector_thread = new Thread(Injector_Code);
            injector_thread.IsBackground = true;
            test_finished = new AutoResetEvent(false);

            //Add messages
            //messages_to_test = new List<SyslogMessage>();

        }

        [TestMethod()]
        public void RuntimeTest()
        {
            using (SyslogUdpReceiver target = new SyslogUdpReceiver())
            {
                target.Configuration["port"] = port.ToString();
                target.Configuration["ip"] = "127.0.0.1";
                target.Start();
                target.MessageReceived += new EventHandler<SyslogMessageEventArgs>(target_MessageReceived);

                injector_thread.Start();
                test_finished.WaitOne();

                target.Stop();

                //See if the number of sent logs matches the number of received logs
                //Don't actually check correct parsing, just that they are parsed
                Assert.AreEqual(logs_received, logs_sent);
            }
        }

        void target_MessageReceived(object sender, SyslogMessageEventArgs e)
        {
            logs_received += 1;
        }


        private void Injector_Code()
        {
            try
            {
                //The injector will basically read Syslog messages, encoded in raw base64 form and collected with the proper tool by djechelon (or equivalent)
                //The collector tool can be found at http://logbus-ng.svn.sourceforge.net/viewvc/logbus-ng/developers/djechelon/SyslogCollector/

                IPEndPoint endpoint = new IPEndPoint(IPAddress.Loopback, port);

                using (StreamReader sr = new StreamReader(GetType().Assembly.GetManifestResourceStream("Unit_Tests.TestLogs.Syslog.base64.txt"), Encoding.GetEncoding(1252)))
                {
                    using (UdpClient client = new UdpClient())
                        while (!sr.EndOfStream)
                        {
                            string base64line = sr.ReadLine();
                            byte[] raw_log = Convert.FromBase64String(base64line);
                            client.Send(raw_log, raw_log.Length, endpoint);
                            logs_sent += 1;
                        }
                }

            }
            finally
            {
                test_finished.Set();
            }
        }

    }
}
