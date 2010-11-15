using It.Unina.Dis.Logbus;
using It.Unina.Dis.Logbus.Filters;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Filter_Tests
{
    /// <summary>
    ///Classe di test per NotFilterTest.
    ///Creata per contenere tutti gli unit test NotFilterTest
    ///</summary>
    [TestClass]
    public class NotFilterTest
    {
        /// <summary>
        ///Ottiene o imposta il contesto dei test, che fornisce
        ///funzionalità e informazioni sull'esecuzione dei test corrente.
        ///</summary>
        public TestContext TestContext { get; set; }

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
        [TestMethod]
        public void IsMatchTest()
        {
            NotFilter target = new NotFilter(); // TODO: Eseguire l'inizializzazione a un valore appropriato
            SyslogMessage message = new SyslogMessage(null, "logbus.dis.unina.it", SyslogFacility.Kernel,
                                                      SyslogSeverity.Info, "Hello people!");
            target.filter = new FacilityEqualsFilter {facility = SyslogFacility.Kernel};
            bool expected = false;
            bool actual = target.IsMatch(message);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///Test per Costruttore NotFilter
        ///</summary>
        [TestMethod]
        public void NotFilterConstructorTest()
        {
            NotFilter target = new NotFilter();
            Assert.IsNotNull(target);
        }
    }
}