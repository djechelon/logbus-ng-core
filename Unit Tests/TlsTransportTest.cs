using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using It.Unina.Dis.Logbus.Filters;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using It.Unina.Dis.Logbus;
using It.Unina.Dis.Logbus.Configuration;
using It.Unina.Dis.Logbus.Clients;
using It.Unina.Dis.Logbus.WebServices;
using It.Unina.Dis.Logbus.Loggers;
using System.Threading;

namespace Unit_Tests
{
    /// <summary>
    /// Descrizione del riepilogo per TlsTransportTest
    /// </summary>
    [TestClass]
    public class TlsTransportTest
    {
        public TlsTransportTest()
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

        [TestInitialize()]
        public void MyTestInitialize()
        {
            LogbusCoreConfiguration coreConfig = new LogbusCoreConfiguration
                                                 {
                                                     inchannels = new[]
                                                                      {
                                                                          new InboundChannelDefinition(){type = "SyslogTlsReceiver"}
                                                 }
                                                 };

            ConfigurationHelper.CoreConfiguration = coreConfig;
            LogbusClientConfiguration clientConfig = new LogbusClientConfiguration
                                                         {
                                                             endpoint = new LogbusEndpointDefinition { managementUrl = "http://localhost:8065/LogbusManagement.asmx", subscriptionUrl = "http://localhost:8065/LogbusSubscription.asmx" }
                                                         };

            ConfigurationHelper.ClientConfiguration = clientConfig;

            LogbusLoggerConfiguration loggerConfig = new LogbusLoggerConfiguration()
                                                         {
                                                             collector = new[]
                                                                             {
                                                                                 new LogbusCollectorDefinition
                                                                                     {
                                                                                         id = "tls", type = "SyslogTlsCollector",
                                                                                         param =
                                                                                             new[]{new KeyValuePair{name = "host", value = "localhost"}}
                                                                                     }
                                                                             },
                                                             defaultcollector = "tls"
                                                         };

            ConfigurationHelper.SourceConfiguration = loggerConfig;
        }

        private AutoResetEvent _success;

        [TestMethod]
        public void TestTlsClient()
        {
            _success = new AutoResetEvent(false);
            using (ILogBus logbus = LogbusSingletonHelper.Instance)
            {
                logbus.Start();
                WebServiceActivator.Start(logbus, 8065);

                using (ILogClient client = ClientHelper.CreateReliableClient(new TrueFilter()))
                {
                    client.Start();
                    client.MessageReceived += client_MessageReceived;

                    ILog logger = LoggerHelper.GetLogger();
                    logger.Notice("Hello K.I.T.T.!");

                    Assert.IsTrue(_success.WaitOne(10000));
                }

            }
        }

        void client_MessageReceived(object sender, SyslogMessageEventArgs e)
        {
            _success.Set();
        }

    }
}
