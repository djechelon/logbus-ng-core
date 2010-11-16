using It.Unina.Dis.Logbus;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using It.Unina.Dis.Logbus.Filters;
using It.Unina.Dis.Logbus.OutChannels;

namespace UnitTests
{
    
    
    /// <summary>
    ///Classe di test per IOutboundChannelTest.
    ///Creata per contenere tutti gli unit test IOutboundChannelTest
    ///</summary>
    [TestClass()]
    public class IOutboundChannelTest
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
        ///Test per TransportFactoryHelper
        ///</summary>
        [TestMethod()]
        public void TransportFactoryHelperTest()
        {
            IOutboundChannel target = CreateIOutboundChannel(); // TODO: Eseguire l'inizializzazione a un valore appropriato
            ITransportFactoryHelper expected = null; // TODO: Eseguire l'inizializzazione a un valore appropriato
            target.TransportFactoryHelper = expected;
            Assert.Inconclusive("Impossibile verificare proprietà in sola scrittura.");
        }

        /// <summary>
        ///Test per SubscribedClients
        ///</summary>
        [TestMethod()]
        public void SubscribedClientsTest()
        {
            IOutboundChannel target = CreateIOutboundChannel(); // TODO: Eseguire l'inizializzazione a un valore appropriato
            int actual;
            actual = target.SubscribedClients;
            Assert.Inconclusive("Verificare la correttezza del metodo di test.");
        }

        /// <summary>
        ///Test per UnsubscribeClient
        ///</summary>
        [TestMethod()]
        public void UnsubscribeClientTest()
        {
            IOutboundChannel target = CreateIOutboundChannel(); // TODO: Eseguire l'inizializzazione a un valore appropriato
            string clientId = string.Empty; // TODO: Eseguire l'inizializzazione a un valore appropriato
            target.UnsubscribeClient(clientId);
            Assert.Inconclusive("Impossibile verificare un metodo che non restituisce valori.");
        }

        /// <summary>
        ///Test per SubscribeClient
        ///</summary>
        [TestMethod()]
        public void SubscribeClientTest()
        {
            IOutboundChannel target = CreateIOutboundChannel(); // TODO: Eseguire l'inizializzazione a un valore appropriato
            string transportId = string.Empty; // TODO: Eseguire l'inizializzazione a un valore appropriato
            IEnumerable<KeyValuePair<string, string>> inputInstructions = null; // TODO: Eseguire l'inizializzazione a un valore appropriato
            IEnumerable<KeyValuePair<string, string>> outputInstructions = null; // TODO: Eseguire l'inizializzazione a un valore appropriato
            IEnumerable<KeyValuePair<string, string>> outputInstructionsExpected = null; // TODO: Eseguire l'inizializzazione a un valore appropriato
            string expected = string.Empty; // TODO: Eseguire l'inizializzazione a un valore appropriato
            string actual;
            actual = target.SubscribeClient(transportId, inputInstructions, out outputInstructions);
            Assert.AreEqual(outputInstructionsExpected, outputInstructions);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verificare la correttezza del metodo di test.");
        }

        /// <summary>
        ///Test per Stop
        ///</summary>
        [TestMethod()]
        public void StopTest()
        {
            IOutboundChannel target = CreateIOutboundChannel(); // TODO: Eseguire l'inizializzazione a un valore appropriato
            target.Stop();
            Assert.Inconclusive("Impossibile verificare un metodo che non restituisce valori.");
        }

        /// <summary>
        ///Test per Start
        ///</summary>
        [TestMethod()]
        public void StartTest()
        {
            IOutboundChannel target = CreateIOutboundChannel(); // TODO: Eseguire l'inizializzazione a un valore appropriato
            target.Start();
            Assert.Inconclusive("Impossibile verificare un metodo che non restituisce valori.");
        }

        internal virtual IOutboundChannel CreateIOutboundChannel()
        {
            // TODO: creare l'istanza di una classe concreta appropriata.
            IOutboundChannel target = ((IOutboundChannelFactory)new SimpleOutChannelFactory()).CreateChannel("dummy", "Channel for tests", new TrueFilter());
            return target;
        }

        /// <summary>
        ///Test per RefreshClient
        ///</summary>
        [TestMethod()]
        public void RefreshClientTest()
        {
            IOutboundChannel target = CreateIOutboundChannel(); // TODO: Eseguire l'inizializzazione a un valore appropriato
            string clientId = string.Empty; // TODO: Eseguire l'inizializzazione a un valore appropriato
            target.RefreshClient(clientId);
            Assert.Inconclusive("Impossibile verificare un metodo che non restituisce valori.");
        }
    }
}
