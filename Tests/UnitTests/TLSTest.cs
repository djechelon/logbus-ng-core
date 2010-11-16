using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using It.Unina.Dis.Logbus;
using It.Unina.Dis.Logbus.Configuration;
using It.Unina.Dis.Logbus.Filters;
using It.Unina.Dis.Logbus.Loggers;
using System.Threading;
using System.IO;

namespace UnitTests
{
    /// <summary>
    /// Descrizione del riepilogo per TLSTest
    /// </summary>
    [TestClass]
    public class TLSTest
    {

        private ILogBus logbus;
        private LogbusCoreConfiguration core_config;
        private LogbusLoggerConfiguration source_config;

        private ILog logger;

        public TLSTest()
        {
            //
            // TODO: aggiungere qui la logica del costruttore
            //

            core_config = new LogbusCoreConfiguration();
            core_config.corefilter = new TrueFilter();
            core_config.inchannels = new InboundChannelDefinition[1];
            core_config.inchannels[0] = new InboundChannelDefinition
                                            {
                type = "SyslogTlsReceiver"
            };
            core_config.inchannels[0].param = new KeyValuePair[1];
            core_config.inchannels[0].param[0] = new KeyValuePair { name = "certificate", value = @"C:\\logbus.p12" };


            source_config = new LogbusLoggerConfiguration();

            source_config.logger = new LoggerDefinition[1];
            source_config.logger[0] = new LoggerDefinition
                                          {
                type = "SyslogTlsCollector",
                name = "tls",
            };
            source_config.logger[0].param = new KeyValuePair[]{
                new KeyValuePair() { name="host", value="localhost" }
            };

            ConfigurationHelper.CoreConfiguration = core_config;
            ConfigurationHelper.SourceConfiguration = source_config;
        }

        ~TLSTest()
        {
            ConfigurationHelper.ClientConfiguration = null;
            ConfigurationHelper.CoreConfiguration = null;
            ConfigurationHelper.SourceConfiguration = null;
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

        private AutoResetEvent testSuccess = new AutoResetEvent(false);

        [TestMethod]
        public void TestTlsLogger()
        {
            try
            {
                if (!File.Exists(@"C:\\logbus.p12")) Assert.Inconclusive("Please copy first logbus.p12 to C:\\ path");

                logbus = LogbusSingletonHelper.Instance;
                logger = LoggerHelper.CreateDefaultLogger();

                logbus.Start();
                logbus.MessageReceived += logbus_MessageReceived;

                logger.Info("TLS test");

                Assert.IsTrue(testSuccess.WaitOne(5000));
            }
            finally
            {
                logbus.Dispose();
                logger = null;
            }
        }

        void logbus_MessageReceived(object sender, SyslogMessageEventArgs e)
        {
            testSuccess.Set();
        }
    }
}
