﻿using It.Unina.Dis.Logbus.Filters;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using It.Unina.Dis.Logbus;

namespace Filter_Tests
{
    
    
    /// <summary>
    ///Classe di test per OrFilterTest.
    ///Creata per contenere tutti gli unit test OrFilterTest
    ///</summary>
    [TestClass()]
    public class OrFilterTest
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
        ///Test per filter
        ///</summary>
        [TestMethod()]
        public void filterTest()
        {
            OrFilter target = new OrFilter();
            FilterBase
                f1 = new TrueFilter(),
                f2 = new NotFilter() { filter = new FalseFilter() },
                f3 = new MessageRegexMatchFilter() { pattern = "FFDA" },
                f4 = new FacilityEqualsFilter() { facility = Facility.Security };

            target.filter = new FilterBase[] { f1, f2, f3, f4 };

            Assert.AreEqual(target.filter[0], f1);
            Assert.IsInstanceOfType(target.filter[1], typeof(NotFilter));
            Assert.IsInstanceOfType(target.filter[3], typeof(FacilityEqualsFilter));
            Assert.AreEqual(((FacilityEqualsFilter)target.filter[3]).facility, Facility.Security);
        }

        /// <summary>
        ///Test per IsMatch
        ///</summary>
        [TestMethod()]
        public void IsMatchTest()
        {
            {
                FilterBase f1 = new FacilityEqualsFilter()
                {
                    facility = Facility.Ftp
                },
                f2 = new MessageRegexMatchFilter()
                {
                    pattern = "^FFDA"
                };

                OrFilter target = new OrFilter();
                target.filter = new FilterBase[] { f1, f2 };


                SyslogMessage message = new SyslogMessage()
                {
                    Facility = SyslogFacility.Internally,
                    Severity = SyslogSeverity.Error,
                    Text = "FFDA WOW!"
                };


                bool expected = true;
                bool actual;
                actual = target.IsMatch(message);
                Assert.AreEqual(expected, actual);
            }

            {
                FilterBase f1 = new FacilityEqualsFilter()
                {
                    facility = Facility.Ftp
                },
                f2 = new MessageRegexMatchFilter()
                {
                    pattern = "^FFDA"
                };

                OrFilter target = new OrFilter();
                target.filter = new FilterBase[] { f1, f2 };


                SyslogMessage message = new SyslogMessage()
                {
                    Facility = SyslogFacility.Ftp,
                    Severity = SyslogSeverity.Error,
                    Text = "Nobody!"
                };


                bool expected = true;
                bool actual;
                actual = target.IsMatch(message);
                Assert.AreEqual(expected, actual);
            }

            {
                string msg = @"<34>1 2003-10-11T22:14:15.003Z mymachine.example.com su - ID47 - ’su root’ failed for lonvick on /dev/pts/8";
                FilterBase f1 = new FacilityEqualsFilter()
                {
                    facility = Facility.Security
                },
                f2 = new MessageRegexMatchFilter()
                {
                    //Ok, that should be the real UNIX user pattern but let's assume only letters and numbers here :)
                    pattern = "’su root’ failed for [a-zA-Z0-9]"
                },
                f3 = new SeverityFilter()
                {
                    comparison = ComparisonOperator.neq,
                    severity = Severity.Emergency
                };

                OrFilter target = new OrFilter();
                target.filter = new FilterBase[] { f1, f2, f3 };


                SyslogMessage message = SyslogMessage.Parse(msg);


                bool expected = true;
                bool actual;
                actual = target.IsMatch(message);
                Assert.AreEqual(expected, actual);
            }
        }

        /// <summary>
        ///Test per Costruttore OrFilter
        ///</summary>
        [TestMethod()]
        public void OrFilterConstructorTest()
        {
            OrFilter target = new OrFilter();
            Assert.IsNotNull(target);
        }
    }
}
