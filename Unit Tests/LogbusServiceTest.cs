using It.Unina.Dis.Logbus;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using It.Unina.Dis.Logbus.Configuration;
using System.Collections.Generic;
using It.Unina.Dis.Logbus.Filters;
using It.Unina.Dis.Logbus.Utils;

namespace Unit_Tests
{


    /// <summary>
    ///Classe di test per LogbusServiceTest.
    ///Creata per contenere tutti gli unit test LogbusServiceTest
    ///</summary>
    [TestClass()]
    public class LogbusServiceTest
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
        ///Test per Configuration
        ///</summary>
        [TestMethod()]
        public void ConfigurationTest()
        {
            //App.config configuration
            using (LogbusService target = new LogbusService())
            {
                target.Configure();
            }
        }



        /// <summary>
        ///Test per SubmitMessage
        ///</summary>
        [TestMethod()]
        public void SubmitMessageTest()
        {
            LogbusService target = new LogbusService(); // TODO: Eseguire l'inizializzazione a un valore appropriato
            SyslogMessage msg = new SyslogMessage(); // TODO: Eseguire l'inizializzazione a un valore appropriato
            target.SubmitMessage(msg);
            Assert.Inconclusive("Impossibile verificare un metodo che non restituisce valori.");
        }

        /// <summary>
        ///Test per GetAvailableTransports
        ///</summary>
        [TestMethod()]
        public void GetAvailableTransportsTest()
        {
            LogbusService target = new LogbusService();
            target.Configure(new LogbusCoreConfiguration());
            string[] expected = new string[] { "udp" };
            string[] actual;
            actual = target.GetAvailableTransports();
            Assert.AreEqual(1, actual.Length);
            Assert.AreEqual(expected[0], actual[0]);
        }



        /// <summary>
        ///Test per Configure
        ///</summary>
        [TestMethod()]
        public void ConfigureTest()
        {
            LogbusCoreConfiguration config = new LogbusCoreConfiguration();

            config.corefilter = new FacilityEqualsFilter() { facility = Facility.Kernel };
            config.inchannels = new InboundChannelDefinition[]
                {
                    new InboundChannelDefinition()
                    {
                         name="udp",
                         type="It.Unina.Dis.Logbus.InChannels.SyslogUdpReceiver, It.Unina.Dis.Logbus"
                    }
                };
            //Manual configuration
            using (ILogBus target = new LogbusService(config))
            {
                Assert.AreEqual(1, target.InboundChannels.Count);
                Assert.IsInstanceOfType(target.MainFilter, typeof(FacilityEqualsFilter));
            }
        }

    }
}
