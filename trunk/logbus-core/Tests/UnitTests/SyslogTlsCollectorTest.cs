using System.Threading;
using It.Unina.Dis.Logbus.Collectors;
using It.Unina.Dis.Logbus.InChannels;
using It.Unina.Dis.Logbus.Loggers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using It.Unina.Dis.Logbus;
using System.Collections.Generic;

namespace UnitTests
{
    /// <summary>
    ///Classe di test per SyslogTlsCollectorTest.
    ///Creata per contenere tutti gli unit test SyslogTlsCollectorTest
    ///</summary>
    [TestClass()]
    public class SyslogTlsCollectorTest
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

        private AutoResetEvent _messageReceived = new AutoResetEvent(false);
        /// <summary>
        ///Test per It.Unina.Dis.Logbus.ILogCollector.SubmitMessage
        ///</summary>
        [TestMethod()]
        [DeploymentItem("It.Unina.Dis.Logbus.dll")]
        public void SubmitMessageTest()
        {
            using (SyslogTlsReceiver receiver = new SyslogTlsReceiver { Port = 7614, Log = new SimpleLogImpl(new NullCollector()) })
            {
                receiver.MessageReceived += (sender, e) => _messageReceived.Set();
                receiver.Start();
                using (SyslogTlsCollector target = new SyslogTlsCollector
                                                  {
                                                      Configuration =
                                                          new Dictionary<string, string> { { "host", "localhost" }, { "port", "7614" } }
                                                  })
                {
                    SyslogMessage message = new SyslogMessage("localhost", SyslogFacility.Local0, SyslogSeverity.Critical,
                                                              "Hello log!");


                    ((ILogCollector)target).SubmitMessage(message);
                    if (!_messageReceived.WaitOne(5000)) Assert.Fail("Waiting too much...");
                }
            }
        }


        /// <summary>
        ///Test per GetConfigurationParameter
        ///</summary>
        [TestMethod()]
        public void GetConfigurationParameterTest()
        {
            SyslogTlsCollector target = new SyslogTlsCollector() { Configuration = new Dictionary<string, string> { { "host", "myhost.mydomain.com" } } }; // TODO: Eseguire l'inizializzazione a un valore appropriato
            string key = "host"; // TODO: Eseguire l'inizializzazione a un valore appropriato
            string expected = "myhost.mydomain.com"; // TODO: Eseguire l'inizializzazione a un valore appropriato
            string actual;
            actual = target.GetConfigurationParameter(key);
            Assert.AreEqual(expected, actual);
        }
    }
}
