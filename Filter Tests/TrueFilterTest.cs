using It.Unina.Dis.Logbus.Filters;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using It.Unina.Dis.Logbus;

namespace Filter_Tests
{
    
    
    /// <summary>
    ///Classe di test per TrueFilterTest.
    ///Creata per contenere tutti gli unit test TrueFilterTest
    ///</summary>
    [TestClass()]
    public class TrueFilterTest
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
        ///Test per IsMatch
        ///</summary>
        [TestMethod()]
        public void IsMatchTest()
        {
            TrueFilter target = new TrueFilter();
            SyslogMessage message = new SyslogMessage(null, "logbus.unina.it", SyslogFacility.Kernel, SyslogSeverity.Notice, "Don't care about me");

            bool expected = true;
            bool actual;
            actual = target.IsMatch(message);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///Test per Costruttore TrueFilter
        ///</summary>
        [TestMethod()]
        public void TrueFilterConstructorTest()
        {
            TrueFilter target = new TrueFilter();
            Assert.IsNotNull(target);
            Assert.IsInstanceOfType(target, typeof(TrueFilter));
        }
    }
}
