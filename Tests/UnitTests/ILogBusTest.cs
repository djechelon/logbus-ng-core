using It.Unina.Dis.Logbus;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using It.Unina.Dis.Logbus.Filters;
using It.Unina.Dis.Logbus.Configuration;
using System.Threading;
using It.Unina.Dis.Logbus.InChannels;
using System.Net;
using System.IO;
using System.Net.Sockets;
using System;
using System.Text;

namespace Unit_Tests
{


    /// <summary>
    ///Classe di test per ILogBusTest.
    ///Creata per contenere tutti gli unit test ILogBusTest
    ///</summary>
    [TestClass()]
    public class ILogBusTest
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
        ///Test per Stop
        ///</summary>
        [TestMethod()]
        public void StopTest()
        {
            ILogBus target = CreateILogBus();
            target.Stop();
            Assert.Inconclusive("Impossibile verificare un metodo che non restituisce valori.");
        }

        internal virtual ILogBus CreateILogBus()
        {
            // TODO: creare l'istanza di una classe concreta appropriata.
            LogbusService target = new LogbusService();

            LogbusCoreConfiguration config = new LogbusCoreConfiguration()
            {
                corefilter = new FacilityEqualsFilter() { facility = SyslogFacility.Security },
                inchannels = new InboundChannelDefinition[]
                  {
                      new InboundChannelDefinition(){
                           type="It.Unina.Dis.Logbus.InChannels.SyslogUdpReceiver, It.Unina.Dis.Logbus"
                      }
                  }
            };

            target.Configuration = config;
            target.Configure();

            return target;
        }

        /// <summary>
        ///Test per Start
        ///</summary>
        [TestMethod()]
        public void StartTest()
        {
            using (ILogBus target = CreateILogBus())
            {
                target.Start();
            }
        }

        #region RuntimeTest

        private int messages_to_match = 0, messages_matched = 0;
        private Thread injector_thread;
        private AutoResetEvent inject_finish;

        [TestMethod()]
        public void RuntimeTest()
        {
            using (ILogBus target = CreateILogBus())
            {
                inject_finish = new AutoResetEvent(false);
                injector_thread = new Thread(Injector);
                injector_thread.IsBackground = true;


                target.Started += new System.EventHandler(target_Started);
                target.Stopped += new System.EventHandler(target_Stopped);
                target.MessageReceived += new EventHandler<SyslogMessageEventArgs>(target_MessageReceived);

                TestContext.WriteLine("Starting Logbus instance");
                target.Start();
                injector_thread.Start();

                TestContext.WriteLine("Waiting for test completion");
                inject_finish.WaitOne();
                target.Stop();
                injector_thread.Join();

                TestContext.WriteLine("Test finished");

                Assert.AreEqual(messages_to_match, messages_matched);
            }
        }

        void target_MessageReceived(object sender, SyslogMessageEventArgs e)
        {
            messages_matched += 1;
        }

        void target_Stopped(object sender, System.EventArgs e)
        {
            TestContext.WriteLine("Logbus stopped");
        }

        void target_Started(object sender, System.EventArgs e)
        {
            TestContext.WriteLine("Logbus started");
        }

        private void Injector()
        {
            try
            {
                //The injector will basically read Syslog messages, encoded in raw base64 form and collected with the proper tool by djechelon (or equivalent)
                //The collector tool can be found at http://logbus-ng.svn.sourceforge.net/viewvc/logbus-ng/developers/djechelon/SyslogCollector/

                IPEndPoint endpoint = new IPEndPoint(IPAddress.Loopback, SyslogUdpReceiver.DEFAULT_PORT);

                using (StreamReader sr = new StreamReader(GetType().Assembly.GetManifestResourceStream("Unit_Tests.TestLogs.Syslog.base64.txt"), Encoding.GetEncoding(1252)))
                {
                    using (UdpClient client = new UdpClient())
                        while (!sr.EndOfStream)
                        {
                            string base64line = sr.ReadLine();
                            byte[] raw_log = Convert.FromBase64String(base64line);

                            //See from here if it matches filter
                            SyslogMessage msg = SyslogMessage.Parse(raw_log);
                            if (msg.Facility == SyslogFacility.Security) messages_to_match += 1;

                            client.Send(raw_log, raw_log.Length, endpoint);

                            //Wait a sec... otherwise UDP channel would be flooded and log message missed
                            //Timeout can be adjusted or replaced with a semaphore
                            Thread.Sleep(10);
                        }
                }

            }
            finally
            {
                inject_finish.Set();
            }
        }
        #endregion
    }
}
