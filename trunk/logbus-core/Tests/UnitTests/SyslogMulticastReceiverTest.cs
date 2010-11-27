using System;
using It.Unina.Dis.Logbus.Collectors;
using It.Unina.Dis.Logbus.InChannels;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net;
using System.Threading;
using It.Unina.Dis.Logbus;
using System.Text;
using System.Net.Sockets;
using It.Unina.Dis.Logbus.Loggers;

namespace UnitTests
{


    /// <summary>
    ///Classe di test per SyslogMulticastReceiverTest.
    ///Creata per contenere tutti gli unit test SyslogMulticastReceiverTest
    ///</summary>
    [TestClass()]
    public class SyslogMulticastReceiverTest
    {


        private TestContext testContextInstance;

        /// <summary>
        ///Ottiene o imposta il contesto dei test, che fornisce
        ///funzionalità e informazioni sull'esecuzione dei test corrente.
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
        //Durante la scrittura dei test è possibile utilizzare i seguenti attributi aggiuntivi:
        //
        //Utilizzare ClassInitialize per eseguire il codice prima di eseguire il primo test della classe
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Utilizzare ClassCleanup per eseguire il codice dopo l'esecuzione di tutti i test di una classe
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Utilizzare TestInitialize per eseguire il codice prima di eseguire ciascun test
        //
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //Utilizzare TestCleanup per eseguire il codice dopo l'esecuzione di ciascun test
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion


        /// <summary>
        ///Test per MulticastGroup
        ///</summary>
        [TestMethod()]
        public void MulticastGroupTest()
        {
            using (SyslogMulticastReceiver target = new SyslogMulticastReceiver())
            {
                target.MulticastGroup = new IPAddress(new byte[] { 236, 2, 6, 86 });

                try
                {
                    //Error expected
                    target.MulticastGroup = new IPAddress(new byte[] { 192, 168, 1, 1 });
                }
                catch { }
            }
        }

        [TestMethod]
        public void TestMulticast()
        {
            AutoResetEvent complete = new AutoResetEvent(false);
            using (SyslogMulticastReceiver target = new SyslogMulticastReceiver())
            {
                target.Log = new SimpleLogImpl(new NullCollector());
                target.MulticastGroup = new IPAddress(new byte[] { 236, 13, 2, 86 });

                target.Start();
                target.MessageReceived += delegate(object sender, SyslogMessageEventArgs e)
                                              {
                                                  if (e.Message.MessageId == "UNIT_TEST") complete.Set();
                                              };
                target.ParseError += delegate {
                                             Assert.Fail("Error decoding Syslog message");
                                         };

                target.Error += delegate {
                                        Assert.Fail("Error in test object");
                                    };

                SyslogMessage msg = new SyslogMessage("localhost", SyslogFacility.Local0, SyslogSeverity.Notice, "Hello")
                                        {
                                            MessageId = "UNIT_TEST"
                                        };
                byte[] payload = Encoding.UTF8.GetBytes(msg.ToRfc5424String());

                using (UdpClient client = new UdpClient { MulticastLoopback = false })
                {
                    client.Connect(target.MulticastGroup, target.Port);
                    client.Send(payload, payload.Length);
                }

                if (!complete.WaitOne(3000)) Assert.Fail("Multicast listener didn't receive the Syslog datagram");
            }
        }

    }
}
