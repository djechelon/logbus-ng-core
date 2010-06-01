using It.Unina.Dis.Logbus.Filters;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using It.Unina.Dis.Logbus;

namespace Filter_Tests
{
    
    
    /// <summary>
    ///Classe di test per PropertyFilterTest.
    ///Creata per contenere tutti gli unit test PropertyFilterTest
    ///</summary>
    [TestClass()]
    public class PropertyFilterTest
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
        ///Test per value
        ///</summary>
        [TestMethod()]
        public void valueTest()
        {
            PropertyFilter target = new PropertyFilter(); // TODO: Eseguire l'inizializzazione a un valore appropriato
            string expected = string.Empty; // TODO: Eseguire l'inizializzazione a un valore appropriato
            string actual;
            target.value = expected;
            actual = target.value;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verificare la correttezza del metodo di test.");
        }

        /// <summary>
        ///Test per propertyName
        ///</summary>
        [TestMethod()]
        public void propertyNameTest()
        {
            PropertyFilter target = new PropertyFilter(); // TODO: Eseguire l'inizializzazione a un valore appropriato
            Property expected = new Property(); // TODO: Eseguire l'inizializzazione a un valore appropriato
            Property actual;
            target.propertyName = expected;
            actual = target.propertyName;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verificare la correttezza del metodo di test.");
        }

        /// <summary>
        ///Test per comparison
        ///</summary>
        [TestMethod()]
        public void comparisonTest()
        {
            PropertyFilter target = new PropertyFilter(); // TODO: Eseguire l'inizializzazione a un valore appropriato
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
            PropertyFilter target = new PropertyFilter(); // TODO: Eseguire l'inizializzazione a un valore appropriato
            SyslogMessage message = new SyslogMessage(); // TODO: Eseguire l'inizializzazione a un valore appropriato
            bool expected = false; // TODO: Eseguire l'inizializzazione a un valore appropriato
            bool actual;
            actual = target.IsMatch(message);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verificare la correttezza del metodo di test.");
        }

        /// <summary>
        ///Test per Costruttore PropertyFilter
        ///</summary>
        [TestMethod()]
        public void PropertyFilterConstructorTest()
        {
            PropertyFilter target = new PropertyFilter();
            Assert.Inconclusive("TODO: Implementare il codice per la verifica della destinazione");
        }
    }
}
