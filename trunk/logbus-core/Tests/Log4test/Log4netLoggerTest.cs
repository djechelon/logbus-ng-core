using It.Unina.Dis.Logbus.log4net;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using It.Unina.Dis.Logbus;
using System.Collections.Generic;
using It.Unina.Dis.Logbus.Loggers;
using System.IO;
using System;

namespace Log4test
{
    
    
    /// <summary>
    ///Classe di test per Log4netLoggerTest.
    ///Creata per contenere tutti gli unit test Log4netLoggerTest
    ///</summary>
    [TestClass()]
    public class Log4netLoggerTest
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
        ///Test per It.Unina.Dis.Logbus.ILogCollector.SubmitMessage
        ///</summary>
        [TestMethod()]
        [DeploymentItem("It.Unina.Dis.Logbus.Extensions.dll")]
        public void SubmitMessageTest()
        {
            log4net.Config.XmlConfigurator.Configure();
            ILog logger = LoggerHelper.CreateLoggerByName("log4net");

            logger.Info("Info message");
            Assert.IsTrue(File.Exists("output.txt"));
        }

        
    }
}
