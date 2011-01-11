using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using It.Unina.Dis.Logbus.Configuration;
using It.Unina.Dis.Logbus.Filters;
using System.IO;
using System.Xml.Serialization;
using It.Unina.Dis.Logbus;
using System.Threading;
using System.Net.Sockets;
using System.Net;
using System.Globalization;

namespace UnitTests
{
    /// <summary>
    /// Descrizione del riepilogo per LogbusSystemTest
    /// </summary>
    [TestClass]
    public class LogbusSystemTest
    {
        public LogbusSystemTest()
        {
            //
            // TODO: aggiungere qui la logica del costruttore
            //
        }

        private TestContext testContextInstance;

        /// <summary>
        ///Ottiene o imposta il contesto del test che fornisce
        ///le informazioni e le funzionalità per l'esecuzione del test corrente.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Attributi di test aggiuntivi
        //
        // È possibile utilizzare i seguenti attributi aggiuntivi per la scrittura dei test:
        //
        // Utilizzare ClassInitialize per eseguire il codice prima di eseguire il primo test della classe
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Utilizzare ClassCleanup per eseguire il codice dopo l'esecuzione di tutti i test della classe
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Utilizzare TestInitialize per eseguire il codice prima di eseguire ciascun test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Utilizzare TestCleanup per eseguire il codice dopo l'esecuzione di ciascun test
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        /// <summary>
        /// 6 audit messages, of which:
        /// 2 Warning
        /// 1 Emergency
        /// 1 Error
        /// 1 Info
        /// 1 Debug
        /// </summary>
        public SyslogMessage[] MessagesToTest
        {
            get
            {
                SyslogMessage[] ret = new SyslogMessage[10];

                ret[0] = new SyslogMessage()
                {
                    ApplicationName = "UUT1",
                    Facility = SyslogFacility.Audit,
                    Severity = SyslogSeverity.Info,
                    Timestamp = DateTime.Now,
                    Host = "localhost",
                    Text = "Sample Syslog test from UUT1 application, Audit Info"
                };

                ret[1] = new SyslogMessage()
                {
                    ApplicationName = "su",
                    Facility = SyslogFacility.Security,

                    Severity = SyslogSeverity.Warning,
                    Text = "Failed login",
                    Host = "localhost",
                    Timestamp = DateTime.Now
                };

                ret[2] = new SyslogMessage()
                {
                    ApplicationName = "postfix",
                    Facility = SyslogFacility.Mail,
                    Severity = SyslogSeverity.Alert,
                    Text = "Spam detected",
                    Host = "localhost",
                    Timestamp = DateTime.Now
                };

                ret[3] = new SyslogMessage()
                {
                    ApplicationName = "auditd",
                    Facility = SyslogFacility.Audit,
                    Severity = SyslogSeverity.Warning,
                    Text = "Audit warning",
                    Host = "localhost",
                    Timestamp = DateTime.Now
                };

                ret[4] = new SyslogMessage()
                {
                    ApplicationName = "auditd",
                    Facility = SyslogFacility.Audit,
                    Severity = SyslogSeverity.Emergency,
                    Text = "System is blowing up!",
                    Host = "localhost",
                    Timestamp = DateTime.Now
                };

                ret[5] = new SyslogMessage()
                {
                    ApplicationName = "smartd",
                    Facility = SyslogFacility.System,
                    Severity = SyslogSeverity.Notice,
                    Text = "Hard disks are OK",
                    Host = "localhost",
                    Timestamp = DateTime.Now
                };

                ret[6] = new SyslogMessage()
                {
                    ApplicationName = "auditd",
                    Facility = SyslogFacility.Audit,
                    Severity = SyslogSeverity.Error,
                    Text = "Auditing error",
                    Host = "localhost",
                    Timestamp = DateTime.Now
                };

                ret[7] = new SyslogMessage()
                {
                    ApplicationName = "auditd",
                    Facility = SyslogFacility.Audit,
                    Severity = SyslogSeverity.Info,
                    Text = "auditd start",
                    Host = "localhost",
                    Timestamp = DateTime.Now
                };

                ret[8] = new SyslogMessage()
                {
                    ApplicationName = "init",
                    Facility = SyslogFacility.Kernel,
                    Severity = SyslogSeverity.Info,
                    Text = "Runlevel 5 reached",
                    Host = "localhost",
                    Timestamp = DateTime.Now
                };

                ret[9] = new SyslogMessage()
                {
                    ApplicationName = "wozzup",
                    Facility = SyslogFacility.Audit,
                    Severity = SyslogSeverity.Debug,
                    Text = "Transaction started",
                    Host = "localhost",
                    Timestamp = DateTime.Now
                };

                return ret;
            }
        }

        #region Test 1

        [TestInitialize()]
        public void Init()
        {
            try
            {
                core_configuration = new LogbusServerConfiguration[2];

                {
                    core_configuration[0] = new LogbusServerConfiguration();

                    //Only messages from Audit
                    core_configuration[0].corefilter = new FacilityEqualsFilter() { facility = SyslogFacility.Audit };


                    //Listening on UDP 3140 port
                    core_configuration[0].inchannels = new InboundChannelDefinition[1];
                    KeyValuePair[] prm = new KeyValuePair[1];
                    prm[0] = new KeyValuePair()
                    {
                        name = "port",
                        value = t1_source_port.ToString()
                    };
                    core_configuration[0].inchannels[0] = new InboundChannelDefinition()
                    {
                        type = "It.Unina.Dis.Logbus.InChannels.SyslogUdpReceiver, It.Unina.Dis.Logbus",
                        param = prm
                    };
                }

                {
                    core_configuration[1] = new LogbusServerConfiguration();
                    core_configuration[1].inchannels = new InboundChannelDefinition[1];

                    KeyValuePair[] prm = new KeyValuePair[1];
                    prm[0] = new KeyValuePair()
                    {
                        name = "port",
                        value = t2_source_port.ToString()
                    };
                    core_configuration[1].inchannels[0] = new InboundChannelDefinition()
                    {
                        type = "It.Unina.Dis.Logbus.InChannels.SyslogUdpReceiver, It.Unina.Dis.Logbus",
                        param = prm
                    };
                }

                TestContext.WriteLine("Core configuration is rendered by the following markup");
                using (MemoryStream ms = new MemoryStream())
                {
                    new XmlSerializer(core_configuration[0].GetType()).Serialize(ms, core_configuration[0], core_configuration[0].xmlns);
                    ms.Flush();
                    ms.Seek(0, SeekOrigin.Begin);
                    TestContext.WriteLine(Encoding.Default.GetString(ms.ToArray()));
                }
                TestContext.WriteLine("");

                
                
            }
            catch (Exception ex) { Assert.Fail("Test failed: {0}", ex); }
        }

        private LogbusServerConfiguration[] core_configuration;

        private AutoResetEvent t1_step1, t1_step2, t1_finish;
        private const int t1_source_port = 3140, t1_client_port = 3141;

        [TestMethod]
        public void Test1()
        {
            try
            {
                t1_step1 = new AutoResetEvent(false);
                t1_step2 = new AutoResetEvent(false);
                t1_finish = new AutoResetEvent(false);

                Thread source, client;
                source = new Thread(t1_thread_Source);
                client = new Thread(t1_thread_Client);
                using (ILogBus logbus_service = new LogbusService(core_configuration[0]))
                {
                    logbus_service.Start();
                    client.Start(logbus_service);

                    //Wait for client to be synchronized
                    t1_step1.WaitOne();

                    //Prepare source
                    source.Start();
                    //Wait for source to send all the messages
                    t1_step2.WaitOne();

                    //Wait for finish
                    t1_finish.WaitOne();
                }
            }
            catch (Exception ex) { Assert.Fail("Test failed: {0}", ex); }
        }

        private void t1_thread_Source()
        {
            try
            {
                UdpClient client = new UdpClient();
                IPEndPoint remote_ep = new IPEndPoint(IPAddress.Loopback, t1_source_port);
                foreach (SyslogMessage msg in MessagesToTest)
                {
                    byte[] payload = Encoding.UTF8.GetBytes(msg.ToRfc5424String());
                    client.Send(payload, payload.Length, remote_ep);
                    Thread.Sleep(50);
                }
                t1_step2.Set();
            }
            catch (Exception ex) { Assert.Fail("Test failed: {0}", ex); }
        }

        private void t1_thread_Client(object Logbus)
        {
            try
            {
                ILogBus ctrl = Logbus as ILogBus;
                ctrl.CreateChannel("simple", "Simple", new TrueFilter(), "Very simple channel", 0);
                Dictionary<string, string> input;
                input = new Dictionary<string, string>();
                IEnumerable<KeyValuePair<string, string>> output;
                input.Add("ip", "127.0.0.1");
                input.Add("port", t1_client_port.ToString());
                string clientid = ctrl.SubscribeClient("simple", "udp", input, out output);
                TestContext.WriteLine("Client ID obtained by logbus: {0}", clientid);

                //Go ahead and send
                t1_step1.Set();

                //Only 6 messages expected
                IPEndPoint remote_ep = new IPEndPoint(IPAddress.Any, 0);
                using (UdpClient client = new UdpClient(t1_client_port))
                    for (int i = 0; i < 5; i++)
                    {
                        byte[] payload = client.Receive(ref remote_ep);

                        SyslogMessage msg = SyslogMessage.Parse(payload);
                        Assert.AreEqual(SyslogFacility.Audit, msg.Facility);
                    }

                ctrl.UnsubscribeClient(clientid);
                t1_finish.Set();
            }
            catch (Exception ex) { Assert.Fail("Test failed: {0}", ex); }
        }

        #region Test 2 (latency)

        private AutoResetEvent t2_step1, t2_step2, t2_finish;
        private const int t2_source_port = 3142, t2_client_port = 3143;
        private DateTime t2_start;

        [TestMethod]
        public void Test2()
        {
            try
            {
                t2_step1 = new AutoResetEvent(false);
                t2_step2 = new AutoResetEvent(false);
                t2_finish = new AutoResetEvent(false);

                Thread source, client;
                source = new Thread(t2_thread_Source);
                client = new Thread(t2_thread_Client);
                using (ILogBus logbus_service = new LogbusService(core_configuration[1]))
                {
                    logbus_service.Start();
                    client.Start(logbus_service);

                    //Wait for client to be synchronized
                    t2_step1.WaitOne();

                    //Prepare source
                    source.Start();
                    //Wait for source to send all the messages
                    t2_step2.WaitOne();

                    //Wait for finish
                    t2_finish.WaitOne();
                    //TestContext.
                }
            }
            catch (Exception ex) { Assert.Fail("Test failed: {0}", ex); }
        }

        private void t2_thread_Source()
        {
            try
            {
                UdpClient client = new UdpClient();
                IPEndPoint remote_ep = new IPEndPoint(IPAddress.Loopback, t2_source_port);


                byte[] payload = Encoding.UTF8.GetBytes(MessagesToTest[0].ToRfc5424String());
                t2_start = DateTime.Now;
                client.Send(payload, payload.Length, remote_ep);


                t2_step2.Set();
            }
            catch (Exception ex) { Assert.Fail("Test failed: {0}", ex); }
        }

        private void t2_thread_Client(object Logbus)
        {
            try
            {
                ILogBus ctrl = Logbus as ILogBus;
                ctrl.CreateChannel("simple", "Simple", new TrueFilter(), "Very simple channel", 0);
                Dictionary<string, string> input;
                input = new Dictionary<string, string>();
                IEnumerable<KeyValuePair<string, string>> output;
                input.Add("ip", "127.0.0.1");
                input.Add("port", t2_client_port.ToString());
                string clientid = ctrl.SubscribeClient("simple", "udp", input, out output);
                TestContext.WriteLine("Client ID obtained by logbus: {0}", clientid);

                //Go ahead and send
                t2_step1.Set();

                //Only 6 messages expected
                IPEndPoint remote_ep = new IPEndPoint(IPAddress.Any, 0);
                using (UdpClient client = new UdpClient(t2_client_port))
                {
                    byte[] payload = client.Receive(ref remote_ep);
                    TestContext.WriteLine("Time occurred for a message to traverse the bus: {0} milliseconds", (DateTime.Now - t2_start).TotalMilliseconds.ToString(CultureInfo.CurrentUICulture));
                    SyslogMessage msg = SyslogMessage.Parse(payload);
                }

                ctrl.UnsubscribeClient(clientid);
                t2_finish.Set();
            }
            catch (Exception ex) { Assert.Fail("Test failed: {0}", ex); }
        }


        #endregion

    }
        #endregion
}
