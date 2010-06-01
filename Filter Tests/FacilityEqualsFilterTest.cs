using It.Unina.Dis.Logbus.Filters;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using It.Unina.Dis.Logbus;

namespace Filter_Tests
{
    
    
    /// <summary>
    ///Classe di test per FacilityEqualsFilterTest.
    ///Creata per contenere tutti gli unit test FacilityEqualsFilterTest
    ///</summary>
    [TestClass()]
    public class FacilityEqualsFilterTest
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
        ///Test per facility
        ///</summary>
        [TestMethod()]
        public void facilityTest()
        {
            FacilityEqualsFilter target = new FacilityEqualsFilter(); // TODO: Eseguire l'inizializzazione a un valore appropriato
            Facility expected = new Facility(); // TODO: Eseguire l'inizializzazione a un valore appropriato
            Facility actual;
            target.facility = expected;
            actual = target.facility;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verificare la correttezza del metodo di test.");
        }

        /// <summary>
        ///Test per IsMatch
        ///</summary>
        [TestMethod()]
        public void IsMatchTest()
        {
            FacilityEqualsFilter target = new FacilityEqualsFilter(); // TODO: Eseguire l'inizializzazione a un valore appropriato
            SyslogMessage message = new SyslogMessage(); // TODO: Eseguire l'inizializzazione a un valore appropriato
            bool expected = false; // TODO: Eseguire l'inizializzazione a un valore appropriato
            bool actual;
            actual = target.IsMatch(message);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verificare la correttezza del metodo di test.");
        }

        /// <summary>
        ///Test per Costruttore FacilityEqualsFilter
        ///</summary>
        [TestMethod()]
        public void FacilityEqualsFilterConstructorTest()
        {
            FacilityEqualsFilter target = new FacilityEqualsFilter();
            Assert.Inconclusive("TODO: Implementare il codice per la verifica della destinazione");
        }
    }
}
