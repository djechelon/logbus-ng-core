using It.Unina.Dis.Logbus.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using It.Unina.Dis.Logbus.Filters;

namespace ConfigurationTests
{
    
    
    /// <summary>
    ///Classe di test per LogbusConfigurationTest.
    ///Creata per contenere tutti gli unit test LogbusConfigurationTest
    ///</summary>
    [TestClass()]
    public class LogbusConfigurationTest
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
        ///Test per Costruttore LogbusConfiguration
        ///</summary>
        [TestMethod()]
        public void LogbusConfigurationConstructorTest()
        {
            LogbusConfiguration target = new LogbusConfiguration();
            Assert.Inconclusive("TODO: Implementare il codice per la verifica della destinazione");
        }

        /// <summary>
        ///Test per corefilter
        ///</summary>
        [TestMethod()]
        public void corefilterTest()
        {
            LogbusConfiguration target = new LogbusConfiguration(); // TODO: Eseguire l'inizializzazione a un valore appropriato
            FilterBase expected = null; // TODO: Eseguire l'inizializzazione a un valore appropriato
            FilterBase actual;
            target.corefilter = expected;
            actual = target.corefilter;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verificare la correttezza del metodo di test.");
        }

        /// <summary>
        ///Test per customfilters
        ///</summary>
        [TestMethod()]
        public void customfiltersTest()
        {
            LogbusConfiguration target = new LogbusConfiguration(); // TODO: Eseguire l'inizializzazione a un valore appropriato
            customfilters expected = null; // TODO: Eseguire l'inizializzazione a un valore appropriato
            customfilters actual;
            target.customfilters = expected;
            actual = target.customfilters;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verificare la correttezza del metodo di test.");
        }

        /// <summary>
        ///Test per inchannels
        ///</summary>
        [TestMethod()]
        public void inchannelsTest()
        {
            LogbusConfiguration target = new LogbusConfiguration(); // TODO: Eseguire l'inizializzazione a un valore appropriato
            InboundChannelDefinition[] expected = null; // TODO: Eseguire l'inizializzazione a un valore appropriato
            InboundChannelDefinition[] actual;
            target.inchannels = expected;
            actual = target.inchannels;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verificare la correttezza del metodo di test.");
        }

        /// <summary>
        ///Test per outtransports
        ///</summary>
        [TestMethod()]
        public void outtransportsTest()
        {
            LogbusConfiguration target = new LogbusConfiguration(); // TODO: Eseguire l'inizializzazione a un valore appropriato
            OutputTransportsConfiguration expected = null; // TODO: Eseguire l'inizializzazione a un valore appropriato
            OutputTransportsConfiguration actual;
            target.outtransports = expected;
            actual = target.outtransports;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verificare la correttezza del metodo di test.");
        }
    }
}
