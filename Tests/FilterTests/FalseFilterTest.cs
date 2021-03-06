﻿using System;
using It.Unina.Dis.Logbus;
using It.Unina.Dis.Logbus.Filters;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Filter_Tests
{
    /// <summary>
    ///Classe di test per FalseFilterTest.
    ///Creata per contenere tutti gli unit test FalseFilterTest
    ///</summary>
    [TestClass]
    public class FalseFilterTest
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
            FalseFilter target = new FalseFilter();
            SyslogMessage message = new SyslogMessage(DateTime.Now, "logbus.unina.it", SyslogFacility.Kernel,
                                                      SyslogSeverity.Emergency, "Please don't discard me!!");

            bool expected = false;
            bool actual;
            actual = target.IsMatch(message);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///Test per Costruttore FalseFilter
        ///</summary>
        [TestMethod]
        public void FalseFilterConstructorTest()
        {
            FalseFilter target = new FalseFilter();
            Assert.IsNotNull(target);
            Assert.IsInstanceOfType(target, typeof (FalseFilter));
        }
    }
}