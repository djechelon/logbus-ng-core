using System;
using It.Unina.Dis.Logbus;
using It.Unina.Dis.Logbus.Filters;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Filter_Tests
{
    /// <summary>
    ///Classe di test per FacilityEqualsFilterTest.
    ///Creata per contenere tutti gli unit test FacilityEqualsFilterTest
    ///</summary>
    [TestClass]
    public class FacilityEqualsFilterTest
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
            Array severities = Enum.GetValues(typeof (SyslogSeverity));

            for (int i = 0; i < Enum.GetValues(typeof (SyslogFacility)).Length; i++)
                for (int j = 0; j < Enum.GetValues(typeof (SyslogFacility)).Length; j++)
                {
                    SyslogFacility to_set = (SyslogFacility) Enum.GetValues(typeof (SyslogFacility)).GetValue(i);
                    SyslogFacility to_match = (SyslogFacility) Enum.GetValues(typeof (SyslogFacility)).GetValue(j);

                    FacilityEqualsFilter target = new FacilityEqualsFilter {facility = to_match};
                    //Random SyslogSeverity
                    SyslogMessage message = new SyslogMessage(null, "logbus.dis.unina.it", to_set, SyslogSeverity.Info,
                                                              "Hello people!");

                    bool expected = i == j;

                    bool actual;
                    actual = target.IsMatch(message);

                    Assert.AreEqual(expected, actual);
                }
        }

        /// <summary>
        ///Test per Costruttore FacilityEqualsFilter
        ///</summary>
        [TestMethod]
        public void FacilityEqualsFilterConstructorTest()
        {
            FacilityEqualsFilter target = new FacilityEqualsFilter
                                              {
                                                  facility = SyslogFacility.Clock2
                                              };
            Assert.IsNotNull(target);
            Assert.IsInstanceOfType(target, typeof (FacilityEqualsFilter));
            Assert.AreEqual(target.facility, SyslogFacility.Clock2);
        }
    }
}