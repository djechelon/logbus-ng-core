using It.Unina.Dis.Logbus.Api;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using It.Unina.Dis.Logbus.Configuration;
using System;
using It.Unina.Dis.Logbus;
using System.Threading;
using System.Net;
using It.Unina.Dis.Logbus.Filters;
using System.Collections.Generic;
using System.Net.Sockets;
namespace Unit_Tests
{


    /// <summary>
    ///Classe di test per ILogTest.
    ///Creata per contenere tutti gli unit test ILogTest
    ///</summary>
    [TestClass()]
    public class ILogTest
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

        private AutoResetEvent step1, step2, finish;
        private int SOURCE_PORT = 3535, MONITOR_PORT=3636;
        [TestMethod()]
        public void LogbusTest()
        {
            step1 = new AutoResetEvent(false);
            step2 = new AutoResetEvent(false);
            finish = new AutoResetEvent(false);

            try
            {
                

                LogbusCoreConfiguration config = new LogbusCoreConfiguration();
                config.inchannels = new InboundChannelDefinition[1];
                config.inchannels[0] = new InboundChannelDefinition();
                config.inchannels[0].name = "udp";
                config.inchannels[0].type = "It.Unina.Dis.Logbus.InChannels.SyslogUdpReceiver, It.Unina.Dis.Logbus";
                config.inchannels[0].param = new KeyValuePair[1];
                config.inchannels[0].param[0] =
                    new KeyValuePair()
                    {
                        name = "port",
                        value = SOURCE_PORT.ToString()
                    };

                using (ILogBus service = new LogbusService(config))
                {
                    service.Start();
                    
                    new Thread(thread_Client).Start(service);
                    step1.WaitOne();

                    new Thread(thread_Source).Start();
                    step2.WaitOne();

                    finish.WaitOne();
                }
            }
            catch (Exception ex)
            {
                Assert.Fail("Test failed: {0}", ex);
            }
        }

        private void thread_Source()
        {
            try
            {
                ILog logger = LoggerHelper.CreateUdpLogger(IPAddress.Loopback, SOURCE_PORT);

                logger.Warning("Hello warn!");

                step2.Set();
            }
            catch (Exception ex) { Assert.Fail("Test failed: {0}", ex); }
        }

        private void thread_Client(object Logbus)
        {
            try
            {
                ILogBus ctrl = Logbus as ILogBus;
                ctrl.CreateChannel("simple", "Simple", new TrueFilter(), "Very simple channel", 0);
                Dictionary<string, string> input;
                input = new Dictionary<string, string>();
                IEnumerable<KeyValuePair<string, string>> output;
                input.Add("ip", "127.0.0.1");
                input.Add("port", MONITOR_PORT.ToString());
                string clientid = ctrl.SubscribeClient("simple", "udp", input, out output);
                TestContext.WriteLine("Client ID obtained by logbus: {0}", clientid);

                //Go ahead and send
                step1.Set();

                
                IPEndPoint remote_ep = new IPEndPoint(IPAddress.Any, 0);
                using (UdpClient client = new UdpClient(MONITOR_PORT))
                {
                    byte[] payload = client.Receive(ref remote_ep);
                    SyslogMessage msg = SyslogMessage.Parse(payload);
                    TestContext.WriteLine("Message: {0}", msg);
                }   

                ctrl.UnsubscribeClient(clientid);
                finish.Set();
            }
            catch (Exception ex) { Assert.Fail("Test failed: {0}", ex); }
        }


    }
}
