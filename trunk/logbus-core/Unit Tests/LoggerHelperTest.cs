using It.Unina.Dis.Logbus.Loggers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using It.Unina.Dis.Logbus.Configuration;
using It.Unina.Dis.Logbus;
using System.Net;
using It.Unina.Dis.Logbus.Utils;
using System;

namespace Unit_Tests
{


    /// <summary>
    ///Classe di test per LoggerHelperTest.
    ///Creata per contenere tutti gli unit test LoggerHelperTest
    ///</summary>
    [TestClass()]
    public class LoggerHelperTest
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
        ///Test per CreateUdpLogger
        ///</summary>
        [TestMethod()]
        public void CreateUdpLoggerTest()
        {
            IPAddress logbus_ip = IPAddress.Parse("127.0.0.1");
            int logbus_port = 3569;

            ILog actual;
            actual = LoggerHelper.CreateUnreliableLogger(logbus_ip, logbus_port);

            Assert.IsNotNull(actual);
        }

        /// <summary>
        ///Test per CreateUdpCollector
        ///</summary>
        [TestMethod()]
        public void CreateUdpCollectorTest()
        {
            IPAddress logbus_ip = null; // TODO: Eseguire l'inizializzazione a un valore appropriato
            int logbus_port = 0; // TODO: Eseguire l'inizializzazione a un valore appropriato
            ILogCollector expected = null; // TODO: Eseguire l'inizializzazione a un valore appropriato
            ILogCollector actual;
            actual = LoggerHelper.CreateUdpCollector(logbus_ip, logbus_port);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verificare la correttezza del metodo di test.");
        }

        /// <summary>
        ///Test per CreateDefaultCollector
        ///</summary>
        [TestMethod()]
        public void CreateDefaultCollectorTest()
        {
            ILogCollector expected = null; // TODO: Eseguire l'inizializzazione a un valore appropriato
            ILogCollector actual;
            actual = LoggerHelper.CreateDefaultCollector();
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verificare la correttezza del metodo di test.");
        }

    }
}
