using System;
using It.Unina.Dis.Logbus;
using It.Unina.Dis.Logbus.Filters;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Filter_Tests
{
    /// <summary>
    ///Classe di test per MessageRegexNotMatchFilterTest.
    ///Creata per contenere tutti gli unit test MessageRegexNotMatchFilterTest
    ///</summary>
    [TestClass]
    public class MessageRegexNotMatchFilterTest
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
            //Simply opposite to RegexMatchFilter
            {
                MessageRegexMatchFilter target = new MessageRegexMatchFilter
                                                     {
                                                         pattern = "FFDA"
                                                     };
                SyslogMessage message = new SyslogMessage(DateTime.Now, "logbus.dis.unina.it", SyslogFacility.Local6,
                                                          SyslogSeverity.Info, "FFDA SST");
                bool expected = false;
                bool actual;
                actual = target.IsMatch(message);
                Assert.AreEqual(expected, actual);
            }

            {
                MessageRegexMatchFilter target = new MessageRegexMatchFilter
                                                     {
                                                         pattern = @"^FFDA (SST|SEN|BIND|COA|EIS|EIE|RIS|RIE)"
                                                     };
                SyslogMessage message = new SyslogMessage(DateTime.Now, "logbus.dis.unina.it", SyslogFacility.Local6,
                                                          SyslogSeverity.Info, "This is FFDA SST");
                bool expected = true;
                bool actual;
                actual = target.IsMatch(message);
                Assert.AreEqual(expected, actual);
            }

            {
                MessageRegexMatchFilter target = new MessageRegexMatchFilter
                                                     {
                                                         pattern = @"^FFDA (SST|SEN|BIND|COA|EIS|EIE|RIS|RIE)"
                                                     };
                SyslogMessage message = new SyslogMessage(DateTime.Now, "logbus.dis.unina.it", SyslogFacility.Local6,
                                                          SyslogSeverity.Info, "FFDA EIS 105995");
                bool expected = false;
                bool actual;
                actual = target.IsMatch(message);
                Assert.AreEqual(expected, actual);
            }
        }

        /// <summary>
        ///Test per Costruttore MessageRegexNotMatchFilter
        ///</summary>
        [TestMethod]
        public void MessageRegexNotMatchFilterConstructorTest()
        {
            MessageRegexMatchFilter target = new MessageRegexNotMatchFilter();
            Assert.IsNotNull(target);
            Assert.IsInstanceOfType(target, typeof (MessageRegexNotMatchFilter));
            Assert.IsInstanceOfType(target, typeof (MessageRegexMatchFilter));
            Assert.IsInstanceOfType(target, typeof (FilterBase));
            Assert.IsInstanceOfType(target, typeof (IFilter));
        }
    }
}