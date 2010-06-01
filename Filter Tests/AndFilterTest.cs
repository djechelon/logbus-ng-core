using It.Unina.Dis.Logbus.Filters;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using It.Unina.Dis.Logbus;

namespace Filter_Tests
{
    
    
    /// <summary>
    ///Classe di test per AndFilterTest.
    ///Creata per contenere tutti gli unit test AndFilterTest
    ///</summary>
    [TestClass()]
    public class AndFilterTest
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
        ///Test per filter
        ///</summary>
        [TestMethod()]
        public void filterTest()
        {
            AndFilter target = new AndFilter(); // TODO: Eseguire l'inizializzazione a un valore appropriato
            FilterBase[] expected = null; // TODO: Eseguire l'inizializzazione a un valore appropriato
            FilterBase[] actual;
            target.filter = expected;
            actual = target.filter;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verificare la correttezza del metodo di test.");
        }

        /// <summary>
        ///Test per IsMatch
        ///</summary>
        [TestMethod()]
        public void IsMatchTest()
        {
            AndFilter target = new AndFilter(); // TODO: Eseguire l'inizializzazione a un valore appropriato
            SyslogMessage message = new SyslogMessage(); // TODO: Eseguire l'inizializzazione a un valore appropriato
            bool expected = false; // TODO: Eseguire l'inizializzazione a un valore appropriato
            bool actual;
            actual = target.IsMatch(message);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verificare la correttezza del metodo di test.");
        }

        /// <summary>
        ///Test per Costruttore AndFilter
        ///</summary>
        [TestMethod()]
        public void AndFilterConstructorTest()
        {
            AndFilter target = new AndFilter();
            Assert.Inconclusive("TODO: Implementare il codice per la verifica della destinazione");
        }
    }
}
