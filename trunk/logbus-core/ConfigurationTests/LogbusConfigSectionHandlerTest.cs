using It.Unina.Dis.Logbus.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Configuration;
using System.Xml;
using System.Configuration;

namespace ConfigurationTests
{
    
    
    /// <summary>
    ///Classe di test per LogbusConfigSectionHandlerTest.
    ///Creata per contenere tutti gli unit test LogbusConfigSectionHandlerTest
    ///</summary>
    [TestClass()]
    public class LogbusConfigSectionHandlerTest
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
        ///Test per Costruttore LogbusConfigSectionHandler
        ///</summary>
        [TestMethod()]
        public void LogbusConfigSectionHandlerConstructorTest()
        {
            LogbusConfigSectionHandler target = new LogbusConfigSectionHandler();
            Assert.Inconclusive("TODO: Implementare il codice per la verifica della destinazione");

        }

        /// <summary>
        ///Test per System.Configuration.IConfigurationSectionHandler.Create
        ///</summary>
        [TestMethod()]
        [DeploymentItem("It.Unina.Dis.Logbus.dll")]
        public void CreateTest()
        {
            IConfigurationSectionHandler target = new LogbusConfigSectionHandler(); // TODO: Eseguire l'inizializzazione a un valore appropriato
            object parent = null; // TODO: Eseguire l'inizializzazione a un valore appropriato
            object configContext = null; // TODO: Eseguire l'inizializzazione a un valore appropriato
            XmlNode section = null; // TODO: Eseguire l'inizializzazione a un valore appropriato
            object expected = null; // TODO: Eseguire l'inizializzazione a un valore appropriato
            object actual;
            actual = target.Create(parent, configContext, section);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verificare la correttezza del metodo di test.");
        }
    }
}
