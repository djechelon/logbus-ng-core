using It.Unina.Dis.Logbus;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace Unit_Tests
{
    
    
    /// <summary>
    ///Classe di test per SyslogMessageTest.
    ///Creata per contenere tutti gli unit test SyslogMessageTest
    ///</summary>
    [TestClass()]
    public class SyslogMessageTest
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
        ///Test per Version
        ///</summary>
        [TestMethod()]
        public void VersionTest()
        {
            SyslogMessage target = new SyslogMessage(); // TODO: Eseguire l'inizializzazione a un valore appropriato
            int expected = 0; // TODO: Eseguire l'inizializzazione a un valore appropriato
            int actual;
            target.Version = expected;
            actual = target.Version;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verificare la correttezza del metodo di test.");
        }

        /// <summary>
        ///Test per Timestamp
        ///</summary>
        [TestMethod()]
        public void TimestampTest()
        {
            SyslogMessage target = new SyslogMessage(); // TODO: Eseguire l'inizializzazione a un valore appropriato
            Nullable<DateTime> expected = new Nullable<DateTime>(); // TODO: Eseguire l'inizializzazione a un valore appropriato
            Nullable<DateTime> actual;
            target.Timestamp = expected;
            actual = target.Timestamp;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verificare la correttezza del metodo di test.");
        }

        /// <summary>
        ///Test per Text
        ///</summary>
        [TestMethod()]
        public void TextTest()
        {
            SyslogMessage target = new SyslogMessage(); // TODO: Eseguire l'inizializzazione a un valore appropriato
            string expected = string.Empty; // TODO: Eseguire l'inizializzazione a un valore appropriato
            string actual;
            target.Text = expected;
            actual = target.Text;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verificare la correttezza del metodo di test.");
        }

        /// <summary>
        ///Test per Severity
        ///</summary>
        [TestMethod()]
        public void SeverityTest()
        {
            SyslogMessage target = new SyslogMessage(); // TODO: Eseguire l'inizializzazione a un valore appropriato
            SyslogSeverity expected = new SyslogSeverity(); // TODO: Eseguire l'inizializzazione a un valore appropriato
            SyslogSeverity actual;
            target.Severity = expected;
            actual = target.Severity;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verificare la correttezza del metodo di test.");
        }

        /// <summary>
        ///Test per ProcessID
        ///</summary>
        [TestMethod()]
        public void ProcessIDTest()
        {
            SyslogMessage target = new SyslogMessage(); // TODO: Eseguire l'inizializzazione a un valore appropriato
            string expected = string.Empty; // TODO: Eseguire l'inizializzazione a un valore appropriato
            string actual;
            target.ProcessID = expected;
            actual = target.ProcessID;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verificare la correttezza del metodo di test.");
        }

        /// <summary>
        ///Test per MessageId
        ///</summary>
        [TestMethod()]
        public void MessageIdTest()
        {
            SyslogMessage target = new SyslogMessage(); // TODO: Eseguire l'inizializzazione a un valore appropriato
            string expected = string.Empty; // TODO: Eseguire l'inizializzazione a un valore appropriato
            string actual;
            target.MessageId = expected;
            actual = target.MessageId;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verificare la correttezza del metodo di test.");
        }

        /// <summary>
        ///Test per Host
        ///</summary>
        [TestMethod()]
        public void HostTest()
        {
            SyslogMessage target = new SyslogMessage(); // TODO: Eseguire l'inizializzazione a un valore appropriato
            string expected = string.Empty; // TODO: Eseguire l'inizializzazione a un valore appropriato
            string actual;
            target.Host = expected;
            actual = target.Host;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verificare la correttezza del metodo di test.");
        }

        /// <summary>
        ///Test per Facility
        ///</summary>
        [TestMethod()]
        public void FacilityTest()
        {
            SyslogMessage target = new SyslogMessage(); // TODO: Eseguire l'inizializzazione a un valore appropriato
            SyslogFacility expected = new SyslogFacility(); // TODO: Eseguire l'inizializzazione a un valore appropriato
            SyslogFacility actual;
            target.Facility = expected;
            actual = target.Facility;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verificare la correttezza del metodo di test.");
        }

        /// <summary>
        ///Test per Data
        ///</summary>
        [TestMethod()]
        public void DataTest()
        {
            SyslogMessage target = new SyslogMessage(); // TODO: Eseguire l'inizializzazione a un valore appropriato
            IDictionary<string, IDictionary<string, string>> expected = null; // TODO: Eseguire l'inizializzazione a un valore appropriato
            IDictionary<string, IDictionary<string, string>> actual;
            target.Data = expected;
            actual = target.Data;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verificare la correttezza del metodo di test.");
        }

        /// <summary>
        ///Test per ApplicationName
        ///</summary>
        [TestMethod()]
        public void ApplicationNameTest()
        {
            SyslogMessage target = new SyslogMessage(); // TODO: Eseguire l'inizializzazione a un valore appropriato
            string expected = string.Empty; // TODO: Eseguire l'inizializzazione a un valore appropriato
            string actual;
            target.ApplicationName = expected;
            actual = target.ApplicationName;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verificare la correttezza del metodo di test.");
        }

        /// <summary>
        ///Test per ToString
        ///</summary>
        [TestMethod()]
        public void ToStringTest()
        {
            SyslogMessage target = new SyslogMessage(); // TODO: Eseguire l'inizializzazione a un valore appropriato
            string expected = string.Empty; // TODO: Eseguire l'inizializzazione a un valore appropriato
            string actual;
            actual = target.ToString();
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verificare la correttezza del metodo di test.");
        }

        /// <summary>
        ///Test per ToByteArray
        ///</summary>
        [TestMethod()]
        public void ToByteArrayTest()
        {
            SyslogMessage target = new SyslogMessage(); // TODO: Eseguire l'inizializzazione a un valore appropriato
            byte[] expected = null; // TODO: Eseguire l'inizializzazione a un valore appropriato
            byte[] actual;
            actual = target.ToByteArray();
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verificare la correttezza del metodo di test.");
        }

        /// <summary>
        ///Test per Parse
        ///</summary>
        [TestMethod()]
        public void ParseTest1()
        {
            byte[] payload = null; // TODO: Eseguire l'inizializzazione a un valore appropriato
            SyslogMessage expected = new SyslogMessage(); // TODO: Eseguire l'inizializzazione a un valore appropriato
            SyslogMessage actual;
            actual = SyslogMessage.Parse(payload);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verificare la correttezza del metodo di test.");
        }

        /// <summary>
        ///Test per Parse
        ///</summary>
        [TestMethod()]
        public void ParseTest()
        {
            string payload = @"<34>1 2003-10-11T22:14:15.003Z mymachine.example.com su - ID47 - ’su root’ failed for lonvick on /dev/pts/8";

            
            SyslogMessage expected = new SyslogMessage(); // TODO: Eseguire l'inizializzazione a un valore appropriato
            expected.Facility = SyslogFacility.Security;
            expected.Severity = SyslogSeverity.Critical;
            expected.Version = 1;
            expected.Timestamp = new DateTime(2003, 10, 11, 22, 14, 15, 3);
            expected.Host = "mymachine.example.com";
            expected.ApplicationName = "su";
            expected.ProcessID = null;
            expected.MessageId = "ID47";
            expected.Text = @"’su root’ failed for lonvick on /dev/pts/8";

            SyslogMessage? actual = null;
            try
            {
                actual = SyslogMessage.Parse(payload);
            }
            catch (FormatException ex)
            {
                Assert.Fail("Failed parsing", ex);
            }

            Assert.IsNotNull(actual);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///Test per Costruttore SyslogMessage
        ///</summary>
        [TestMethod()]
        public void SyslogMessageConstructorTest()
        {
            Nullable<DateTime> timestamp = new Nullable<DateTime>(); // TODO: Eseguire l'inizializzazione a un valore appropriato
            string host = string.Empty; // TODO: Eseguire l'inizializzazione a un valore appropriato
            SyslogFacility facility = new SyslogFacility(); // TODO: Eseguire l'inizializzazione a un valore appropriato
            SyslogSeverity level = new SyslogSeverity(); // TODO: Eseguire l'inizializzazione a un valore appropriato
            string text = string.Empty; // TODO: Eseguire l'inizializzazione a un valore appropriato
            IDictionary<string, IDictionary<string, string>> data = null; // TODO: Eseguire l'inizializzazione a un valore appropriato
            SyslogMessage target = new SyslogMessage(timestamp, host, facility, level, text, data);
            Assert.Inconclusive("TODO: Implementare il codice per la verifica della destinazione");
        }
    }
}
