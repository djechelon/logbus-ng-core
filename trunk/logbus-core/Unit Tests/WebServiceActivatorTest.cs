using It.Unina.Dis.Logbus.WebServices;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using It.Unina.Dis.Logbus;
using System.Threading;

namespace Unit_Tests
{
    
    
    /// <summary>
    ///Classe di test per WebServiceActivatorTest.
    ///Creata per contenere tutti gli unit test WebServiceActivatorTest
    ///</summary>
    [TestClass()]
    public class WebServiceActivatorTest
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
        ///Test per Stop
        ///</summary>
        [TestMethod()]
        public void StopTest()
        {
            WebServiceActivator.Stop();
            Assert.Inconclusive("Impossibile verificare un metodo che non restituisce valori.");
        }

        

        /// <summary>
        ///Test per Start
        ///</summary>
        [TestMethod()]
        public void StartTest()
        {
            ILogBus service = LogbusSingletonHelper.Instance;
            int httpPort = 8065; // TODO: Eseguire l'inizializzazione a un valore appropriato
            WebServiceActivator.Start(service, httpPort);
            Thread.Sleep(Timeout.Infinite);
        }

       
    }
}
