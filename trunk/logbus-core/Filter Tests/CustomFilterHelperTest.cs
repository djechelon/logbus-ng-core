using It.Unina.Dis.Logbus.Filters;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Reflection;

namespace Filter_Tests
{
    
    
    /// <summary>
    ///Classe di test per CustomFilterHelperTest.
    ///Creata per contenere tutti gli unit test CustomFilterHelperTest
    ///</summary>
    [TestClass()]
    public class CustomFilterHelperTest
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
        ///Test per Item
        ///</summary>
        [TestMethod()]
        public void ItemTest1()
        {
            CustomFilterHelper_Accessor target = new CustomFilterHelper_Accessor(); // TODO: Eseguire l'inizializzazione a un valore appropriato
            string tag = string.Empty; // TODO: Eseguire l'inizializzazione a un valore appropriato
            IEnumerable<FilterParameter> parameters = null; // TODO: Eseguire l'inizializzazione a un valore appropriato
            IFilter actual;
            actual = target[tag, parameters];
            Assert.Inconclusive("Verificare la correttezza del metodo di test.");
        }

        /// <summary>
        ///Test per Item
        ///</summary>
        [TestMethod()]
        public void ItemTest()
        {
            CustomFilterHelper_Accessor target = new CustomFilterHelper_Accessor(); // TODO: Eseguire l'inizializzazione a un valore appropriato
            string tag = string.Empty; // TODO: Eseguire l'inizializzazione a un valore appropriato
            IEnumerable<FilterParameter> parameters = null; // TODO: Eseguire l'inizializzazione a un valore appropriato
            IFilter actual;
            actual = target[tag, parameters];
            Assert.Inconclusive("Verificare la correttezza del metodo di test.");
        }


        /// <summary>
        ///Test per ScanAssemblyAndRegister
        ///</summary>
        [TestMethod()]
        public void ScanAssemblyAndRegisterTest()
        {
            CustomFilterHelper_Accessor target = new CustomFilterHelper_Accessor(); // TODO: Eseguire l'inizializzazione a un valore appropriato
            Assembly to_scan = null; // TODO: Eseguire l'inizializzazione a un valore appropriato
            target.ScanAssemblyAndRegister(to_scan);
            Assert.Inconclusive("Impossibile verificare un metodo che non restituisce valori.");
        }

        /// <summary>
        ///Test per RegisterCustomFilter
        ///</summary>
        [TestMethod()]
        public void RegisterCustomFilterTest()
        {
            CustomFilterHelper_Accessor target = new CustomFilterHelper_Accessor(); // TODO: Eseguire l'inizializzazione a un valore appropriato
            string tag = string.Empty; // TODO: Eseguire l'inizializzazione a un valore appropriato
            string typeName = string.Empty; // TODO: Eseguire l'inizializzazione a un valore appropriato
            target.RegisterCustomFilter(tag, typeName);
            Assert.Inconclusive("Impossibile verificare un metodo che non restituisce valori.");
        }

        /// <summary>
        ///Test per BuildFilter
        ///</summary>
        [TestMethod()]
        public void BuildFilterTest()
        {
            CustomFilterHelper_Accessor target = new CustomFilterHelper_Accessor(); // TODO: Eseguire l'inizializzazione a un valore appropriato
            string tag = string.Empty; // TODO: Eseguire l'inizializzazione a un valore appropriato
            IEnumerable<FilterParameter> parameters = null; // TODO: Eseguire l'inizializzazione a un valore appropriato
            IFilter expected = null; // TODO: Eseguire l'inizializzazione a un valore appropriato
            IFilter actual;
            actual = target.BuildFilter(tag, parameters);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verificare la correttezza del metodo di test.");
        }

        
    }
}
