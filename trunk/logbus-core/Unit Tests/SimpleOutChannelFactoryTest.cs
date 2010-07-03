﻿using It.Unina.Dis.Logbus.OutChannels;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using It.Unina.Dis.Logbus;
using It.Unina.Dis.Logbus.Filters;

namespace Unit_Tests
{
    
    
    /// <summary>
    ///Classe di test per SimpleOutChannelFactoryTest.
    ///Creata per contenere tutti gli unit test SimpleOutChannelFactoryTest
    ///</summary>
    [TestClass()]
    public class SimpleOutChannelFactoryTest
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
        ///Test per It.Unina.Dis.Logbus.IOutboundChannelFactory.CreateChannel
        ///</summary>
        [TestMethod()]
        [DeploymentItem("It.Unina.Dis.Logbus.dll")]
        public void CreateChannelTest()
        {
            IOutboundChannelFactory target = new SimpleOutChannelFactory(); // TODO: Eseguire l'inizializzazione a un valore appropriato
            string name = string.Empty; // TODO: Eseguire l'inizializzazione a un valore appropriato
            string description = string.Empty; // TODO: Eseguire l'inizializzazione a un valore appropriato
            IFilter filter = null; // TODO: Eseguire l'inizializzazione a un valore appropriato
            IOutboundChannel expected = null; // TODO: Eseguire l'inizializzazione a un valore appropriato
            IOutboundChannel actual;
            actual = target.CreateChannel(name, description, filter);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verificare la correttezza del metodo di test.");
        }

        /// <summary>
        ///Test per Costruttore SimpleOutChannelFactory
        ///</summary>
        [TestMethod()]
        public void SimpleOutChannelFactoryConstructorTest()
        {
            SimpleOutChannelFactory target = new SimpleOutChannelFactory();
            Assert.Inconclusive("TODO: Implementare il codice per la verifica della destinazione");
        }
    }
}
