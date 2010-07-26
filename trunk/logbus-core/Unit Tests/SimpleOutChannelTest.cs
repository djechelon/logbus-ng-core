using It.Unina.Dis.Logbus.OutChannels;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using It.Unina.Dis.Logbus;
using System.Collections.Generic;
using System;
using It.Unina.Dis.Logbus.Filters;

namespace Unit_Tests
{
    
    
    /// <summary>
    ///Classe di test per SimpleOutChannelTest.
    ///Creata per contenere tutti gli unit test SimpleOutChannelTest
    ///</summary>
    [TestClass()]
    public class SimpleOutChannelTest
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
        [DeploymentItem("It.Unina.Dis.Logbus.dll")]
        public void TransportFactoryHelperTest()
        {
            SimpleOutChannel_Accessor target = new SimpleOutChannel_Accessor(); // TODO: Eseguire l'inizializzazione a un valore appropriato
            ITransportFactoryHelper expected = null; // TODO: Eseguire l'inizializzazione a un valore appropriato
            ITransportFactoryHelper actual;
            target.TransportFactoryHelper = expected;
            actual = target.TransportFactoryHelper;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verificare la correttezza del metodo di test.");
        }

        /// <summary>
        ///Test per It.Unina.Dis.Logbus.IOutboundChannel.SubscribedClients
        ///</summary>
        [TestMethod()]
        [DeploymentItem("It.Unina.Dis.Logbus.dll")]
        public void SubscribedClientsTest()
        {
            IOutboundChannel target = new SimpleOutChannel(); // TODO: Eseguire l'inizializzazione a un valore appropriato
            int actual;
            actual = target.SubscribedClients;
            Assert.Inconclusive("Verificare la correttezza del metodo di test.");
        }

       
        /// <summary>
        ///Test per CoalescenceWindowMillis
        ///</summary>
        [TestMethod()]
        public void CoalescenceWindowMillisTest()
        {
            SimpleOutChannel target = new SimpleOutChannel(); // TODO: Eseguire l'inizializzazione a un valore appropriato
            ulong expected = 0; // TODO: Eseguire l'inizializzazione a un valore appropriato
            ulong actual;
            target.CoalescenceWindowMillis = expected;
            actual = target.CoalescenceWindowMillis;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verificare la correttezza del metodo di test.");
        }

        /// <summary>
        ///Test per UnsubscribeClient
        ///</summary>
        [TestMethod()]
        public void UnsubscribeClientTest()
        {
            SimpleOutChannel target = new SimpleOutChannel(); // TODO: Eseguire l'inizializzazione a un valore appropriato
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
            SimpleOutChannel target = new SimpleOutChannel(); // TODO: Eseguire l'inizializzazione a un valore appropriato
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
        ///Test per RefreshClient
        ///</summary>
        [TestMethod()]
        public void RefreshClientTest()
        {
            SimpleOutChannel target = new SimpleOutChannel(); // TODO: Eseguire l'inizializzazione a un valore appropriato
            string clientId = string.Empty; // TODO: Eseguire l'inizializzazione a un valore appropriato
            target.RefreshClient(clientId);
            Assert.Inconclusive("Impossibile verificare un metodo che non restituisce valori.");
        }

        /// <summary>
        ///Test per It.Unina.Dis.Logbus.IOutboundChannel.Stop
        ///</summary>
        [TestMethod()]
        [DeploymentItem("It.Unina.Dis.Logbus.dll")]
        public void StopTest()
        {
            IOutboundChannel target = new SimpleOutChannel(); // TODO: Eseguire l'inizializzazione a un valore appropriato
            target.Stop();
            Assert.Inconclusive("Impossibile verificare un metodo che non restituisce valori.");
        }

        /// <summary>
        ///Test per It.Unina.Dis.Logbus.IOutboundChannel.Start
        ///</summary>
        [TestMethod()]
        [DeploymentItem("It.Unina.Dis.Logbus.dll")]
        public void StartTest()
        {
            IOutboundChannel target = new SimpleOutChannel(); // TODO: Eseguire l'inizializzazione a un valore appropriato
            target.Start();
            Assert.Inconclusive("Impossibile verificare un metodo che non restituisce valori.");
        }

        /// <summary>
        ///Test per It.Unina.Dis.Logbus.ILogCollector.SubmitMessage
        ///</summary>
        [TestMethod()]
        [DeploymentItem("It.Unina.Dis.Logbus.dll")]
        public void SubmitMessageTest()
        {
            ILogCollector target = new SimpleOutChannel(); // TODO: Eseguire l'inizializzazione a un valore appropriato
            SyslogMessage message = new SyslogMessage(); // TODO: Eseguire l'inizializzazione a un valore appropriato
            target.SubmitMessage(message);
            Assert.Inconclusive("Impossibile verificare un metodo che non restituisce valori.");
        }

    }
}
