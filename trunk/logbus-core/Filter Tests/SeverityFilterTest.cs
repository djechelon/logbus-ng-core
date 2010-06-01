using It.Unina.Dis.Logbus.Filters;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using It.Unina.Dis.Logbus;

namespace Filter_Tests
{
    
    
    /// <summary>
    ///Classe di test per SeverityFilterTest.
    ///Creata per contenere tutti gli unit test SeverityFilterTest
    ///</summary>
    [TestClass()]
    public class SeverityFilterTest
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
        ///Test per severity
        ///</summary>
        [TestMethod()]
        public void severityTest()
        {
            SeverityFilter target = new SeverityFilter(); // TODO: Eseguire l'inizializzazione a un valore appropriato
            Severity expected = new Severity(); // TODO: Eseguire l'inizializzazione a un valore appropriato
            Severity actual;
            target.severity = expected;
            actual = target.severity;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verificare la correttezza del metodo di test.");
        }

        /// <summary>
        ///Test per comparison
        ///</summary>
        [TestMethod()]
        public void comparisonTest()
        {
            SeverityFilter target = new SeverityFilter(); // TODO: Eseguire l'inizializzazione a un valore appropriato
            ComparisonOperator expected = new ComparisonOperator(); // TODO: Eseguire l'inizializzazione a un valore appropriato
            ComparisonOperator actual;
            target.comparison = expected;
            actual = target.comparison;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verificare la correttezza del metodo di test.");
        }

        /// <summary>
        ///Test per IsMatch
        ///</summary>
        [TestMethod()]
        public void IsMatchTest()
        {
            SeverityFilter target = new SeverityFilter(); // TODO: Eseguire l'inizializzazione a un valore appropriato
            SyslogMessage message = new SyslogMessage(); // TODO: Eseguire l'inizializzazione a un valore appropriato
            bool expected = false; // TODO: Eseguire l'inizializzazione a un valore appropriato
            bool actual;
            actual = target.IsMatch(message);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verificare la correttezza del metodo di test.");
        }

        /// <summary>
        ///Test per Costruttore SeverityFilter
        ///</summary>
        [TestMethod()]
        public void SeverityFilterConstructorTest()
        {
            SeverityFilter target = new SeverityFilter();
            Assert.Inconclusive("TODO: Implementare il codice per la verifica della destinazione");
        }
    }
}
