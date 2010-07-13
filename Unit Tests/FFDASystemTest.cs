using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using It.Unina.Dis.Logbus;
using It.Unina.Dis.Logbus.Configuration;
using System.Threading;
using It.Unina.Dis.Logbus.Utils;
using It.Unina.Dis.Logbus.Loggers;
using It.Unina.Dis.Logbus.Filters;
using System.Xml.Serialization;
using System.Xml;

namespace Unit_Tests
{
    /// <summary>
    /// Descrizione del riepilogo per FFDASystemTest
    /// </summary>
    [TestClass]
    public class FFDASystemTest
    {
        public FFDASystemTest()
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

        int messages = 0;

        [TestMethod]
        public void FFDATest()
        {
            AndFilter ffda_filter = new AndFilter();
            ffda_filter.filter = new FilterBase[]
            {
                new FacilityEqualsFilter() { facility = Facility.Local0 },
                new PropertyFilter() { comparison = ComparisonOperator.eq, propertyName = Property.MessageID, value = "FFDA" }
            };

            StringBuilder markup = new StringBuilder();
            new XmlSerializer(typeof(FilterBase)).Serialize(XmlTextWriter.Create(markup, new XmlWriterSettings() { Indent = true }), ffda_filter, ffda_filter.xmlns);

            TestContext.WriteLine("{0}", markup.ToString());

            const int SEND_PORT = 5427;

            //Init Logbus
            LogbusCoreConfiguration core_config = new LogbusCoreConfiguration();
            core_config.corefilter = ffda_filter;
            InboundChannelDefinition in_ch = new InboundChannelDefinition();
            in_ch.name = "ffda";
            in_ch.type = "SyslogUdpReceiver";
            in_ch.param = new KeyValuePair[]
            {
                new KeyValuePair(){
                     name = "port",
                     value = SEND_PORT.ToString()
                }
            };
            core_config.inchannels = new InboundChannelDefinition[] { in_ch };

            using (ILogBus logbus = new LogbusService(core_config))
            {

                logbus.MessageReceived += new SyslogMessageEventHandler(logbus_MessageReceived);
                logbus.Start();

                //Init FFDA
                LogbusSourceConfiguration source_config = new LogbusSourceConfiguration();
                LoggerDefinition udp_def = new LoggerDefinition()
                {
                    type = "SyslogUdpLogger",
                    param = new KeyValuePair[] { new KeyValuePair() { name = "port", value = SEND_PORT.ToString() }, new KeyValuePair() { name = "ip", value = "127.0.0.1" } }
                };
                source_config.logger = new LoggerDefinition[] { udp_def };
                LoggerHelper.Configuration = source_config;

                //Send what we want: 2 FFDA messages
                FFDALogger logger = LoggerHelper.CreateFFDALogger();
                logger.LogSST();
                logger.LogSEN();

                //Send junk
                ILog junk_log = LoggerHelper.CreateDefaultLogger();
                junk_log.Debug("Hello");
                junk_log.Error("Junk error");

                Thread.Sleep(1000);
                logbus.Stop();
            }

            Assert.AreEqual(2, messages);

        }

        void logbus_MessageReceived(object sender, SyslogMessageEventArgs e)
        {
            messages++;
            TestContext.WriteLine("Got message: {0}", e.Message);
        }
    }
}
