using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using log4net;
using log4net.Appender;
using It.Unina.Dis.Logbus.InChannels;
using System.Threading;
using It.Unina.Dis.Logbus;

namespace Log4test
{
    /// <summary>
    /// Descrizione del riepilogo per RemoteSyslogAppenderTest
    /// </summary>
    [TestClass]
    public class RemoteSyslogAppenderTest
    {
        public RemoteSyslogAppenderTest()
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
        [ClassInitialize()]
        public static void MyClassInitialize(TestContext testContext)
        {
            log4net.Config.XmlConfigurator.Configure();
        }

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

        private ILog log;
        private AutoResetEvent rcv_event;

        [TestInitialize()]
        public void Init()
        {
            //RemoteSyslogAppender appender = new RemoteSyslogAppender();
            log = LogManager.GetLogger("ALL");
            rcv_event = new AutoResetEvent(false);
        }

        [TestMethod]
        public void TestMethod()
        {
            using (SyslogUdpReceiver recv = new SyslogUdpReceiver())
            {
                recv.ParseError += new EventHandler<ParseErrorEventArgs>(recv_ParseError);
                recv.MessageReceived += new EventHandler<SyslogMessageEventArgs>(recv_MessageReceived);
                recv.Port = 3434;
                recv.Start();
                
                log.Error("Test error message");

                if (!rcv_event.WaitOne(2000)) Assert.Fail("Timed out");

                //Nothing to assert :(
            }
        }

        void recv_ParseError(object sender, It.Unina.Dis.Logbus.ParseErrorEventArgs e)
        {
            TestContext.WriteLine("Error receiving from Log4net: {0}", e.ExceptionObject);
        }

        void recv_MessageReceived(object sender, It.Unina.Dis.Logbus.SyslogMessageEventArgs e)
        {
            TestContext.WriteLine("Received message from Log4net: {0}", e.Message.ToRfc5424String());
            rcv_event.Set();
        }
    }
}
