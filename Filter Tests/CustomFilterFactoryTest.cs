using It.Unina.Dis.Logbus.Filters;
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace Filter_Tests
{
    
    
    /// <summary>
    ///Classe di test per CustomFilterFactoryTest.
    ///Creata per contenere tutti gli unit test CustomFilterFactoryTest
    ///</summary>
    [TestClass()]
    public class CustomFilterFactoryTest
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
        ///Test per Costruttore CustomFilterFactory
        ///</summary>
        [TestMethod()]
        [DeploymentItem("It.Unina.Dis.Logbus.dll")]
        public void CustomFilterFactoryConstructorTest()
        {
            CustomFilterFactory_Accessor target = new CustomFilterFactory_Accessor();
            Assert.Inconclusive("TODO: Implementare il codice per la verifica della destinazione");
        }
    }
}
