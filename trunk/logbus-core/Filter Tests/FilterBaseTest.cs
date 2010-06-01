using It.Unina.Dis.Logbus.Filters;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using It.Unina.Dis.Logbus;

namespace Filter_Tests
{
    
    
    /// <summary>
    ///Classe di test per FilterBaseTest.
    ///Creata per contenere tutti gli unit test FilterBaseTest
    ///</summary>
    [TestClass()]
    public class FilterBaseTest
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


        internal virtual FilterBase_Accessor CreateFilterBase_Accessor()
        {
            // TODO: creare l'istanza di una classe concreta appropriata.
            FilterBase_Accessor target = null;
            return target;
        }

        /// <summary>
        ///Test per RaisePropertyChanged
        ///</summary>
        [TestMethod()]
        [DeploymentItem("It.Unina.Dis.Logbus.dll")]
        public void RaisePropertyChangedTest()
        {
            PrivateObject param0 = null; // TODO: Eseguire l'inizializzazione a un valore appropriato
            FilterBase_Accessor target = new FilterBase_Accessor(param0); // TODO: Eseguire l'inizializzazione a un valore appropriato
            string propertyName = string.Empty; // TODO: Eseguire l'inizializzazione a un valore appropriato
            target.RaisePropertyChanged(propertyName);
            Assert.Inconclusive("Impossibile verificare un metodo che non restituisce valori.");
        }

        internal virtual FilterBase CreateFilterBase()
        {
            // TODO: creare l'istanza di una classe concreta appropriata.
            FilterBase target = null;
            return target;
        }

        /// <summary>
        ///Test per IsMatch
        ///</summary>
        [TestMethod()]
        public void IsMatchTest()
        {
            FilterBase target = CreateFilterBase(); // TODO: Eseguire l'inizializzazione a un valore appropriato
            SyslogMessage message = new SyslogMessage(); // TODO: Eseguire l'inizializzazione a un valore appropriato
            bool expected = false; // TODO: Eseguire l'inizializzazione a un valore appropriato
            bool actual;
            actual = target.IsMatch(message);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verificare la correttezza del metodo di test.");
        }
    }
}
