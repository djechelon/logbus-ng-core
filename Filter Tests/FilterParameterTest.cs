using It.Unina.Dis.Logbus.Filters;
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace Filter_Tests
{
    
    
    /// <summary>
    ///Classe di test per FilterParameterTest.
    ///Creata per contenere tutti gli unit test FilterParameterTest
    ///</summary>
    [TestClass()]
    public class FilterParameterTest
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
            FilterParameter target = new FilterParameter(); // TODO: Eseguire l'inizializzazione a un valore appropriato
            object expected = null; // TODO: Eseguire l'inizializzazione a un valore appropriato
            object actual;
            target.value = expected;
            actual = target.value;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verificare la correttezza del metodo di test.");
        }

        /// <summary>
        ///Test per name
        ///</summary>
        [TestMethod()]
        public void nameTest()
        {
            FilterParameter target = new FilterParameter(); // TODO: Eseguire l'inizializzazione a un valore appropriato
            string expected = string.Empty; // TODO: Eseguire l'inizializzazione a un valore appropriato
            string actual;
            target.name = expected;
            actual = target.name;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verificare la correttezza del metodo di test.");
        }

        /// <summary>
        ///Test per RaisePropertyChanged
        ///</summary>
        [TestMethod()]
        [DeploymentItem("It.Unina.Dis.Logbus.dll")]
        public void RaisePropertyChangedTest()
        {
            FilterParameter_Accessor target = new FilterParameter_Accessor(); // TODO: Eseguire l'inizializzazione a un valore appropriato
            string propertyName = string.Empty; // TODO: Eseguire l'inizializzazione a un valore appropriato
            target.RaisePropertyChanged(propertyName);
            Assert.Inconclusive("Impossibile verificare un metodo che non restituisce valori.");
        }

        /// <summary>
        ///Test per Costruttore FilterParameter
        ///</summary>
        [TestMethod()]
        public void FilterParameterConstructorTest()
        {
            FilterParameter target = new FilterParameter();
            Assert.Inconclusive("TODO: Implementare il codice per la verifica della destinazione");
        }
    }
}
