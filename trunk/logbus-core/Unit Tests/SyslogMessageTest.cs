using It.Unina.Dis.Logbus;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using It.Unina.Dis.Logbus.Loggers;

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

        [TestMethod()]
        public void CreateMessageTest()
        {
            ILog logger = new SimpleLogImpl(new ContextWriter() { Tc = TestContext }) { LogName = "TestLogger" };

            logger.Debug("Hell0 debug world!");
            logger.Notice("Second message");
        }

        protected class ContextWriter
            : ILogCollector
        {

            #region ILogCollector Membri di

            protected internal TestContext Tc;

            public void SubmitMessage(SyslogMessage message)
            {
                Tc.WriteLine(message.ToRfc5424String());
            }

            #endregion
        }

        /// <summary>
        ///Test per Parse
        ///</summary>
        [TestMethod()]
        public void ParseTest()
        {
            string payload = @"<34>1 2003-10-11T22:14:15.003Z mymachine.example.com su - ID47 - ’su root’ failed for lonvick on /dev/pts/8";


            SyslogMessage expected = new SyslogMessage
                                         {
                                             Facility = SyslogFacility.Security,
                                             Severity = SyslogSeverity.Critical,
                                             Timestamp = new DateTime(2003, 10, 11, 22, 14, 15, 3),
                                             Host = "mymachine.example.com",
                                             ApplicationName = "su",
                                             ProcessID = null,
                                             MessageId = "ID47",
                                             Text = @"’su root’ failed for lonvick on /dev/pts/8"
                                         }; // TODO: Eseguire l'inizializzazione a un valore appropriato

            SyslogMessage? actual = null;
            try
            {
                actual = SyslogMessage.Parse(payload);
            }
            catch (FormatException ex)
            {
                Assert.Fail("Failed parsing: {0}", ex);
            }

            Assert.IsNotNull(actual);
            Assert.AreEqual(expected, actual);

            payload = @"<165>1 2003-08-24T05:14:15.000003-07:00 192.0.2.1 myproc 8710 - - %% It’s time to make the do-nuts.";
            expected = new SyslogMessage
                           {
                               Facility = SyslogFacility.Local4,
                               Severity = SyslogSeverity.Notice,
                               Timestamp = new DateTime(2003, 08, 23, 22, 14, 15, 0),
                               Host = "192.0.2.1",
                               ApplicationName = "myproc",
                               ProcessID = "8710",
                               MessageId = null,
                               Text = @"%% It’s time to make the do-nuts."
                           };

            actual = null;

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

            payload = @"<165>1 2003-10-11T22:14:15.003Z mymachine.example.com evntslog - ID47 [exampleSDID@32473 iut=""3"" eventSource=""Application"" eventID=""1011""] An application event log entry...";
            actual = null;

            try
            {
                actual = SyslogMessage.Parse(payload);
            }
            catch (FormatException ex)
            {
                Assert.Fail("Failed parsing: {0}", ex);
            }
            Assert.IsNotNull(actual);

            payload = @"<165>1 2003-10-11T22:14:15.003Z mymachine.example.com evntslog - ID47 [exampleSDID@32473 iut=""3"" eventSource=""Application"" eventID=""1011""][examplePriority@32473 class=""high""]";
            actual = null;

            try
            {
                actual = SyslogMessage.Parse(payload);
            }
            catch (FormatException ex)
            {
                Assert.Fail("Failed parsing: {0}", ex);
            }
            Assert.IsNotNull(actual);

            // Test del formato 3164 BSD.....
            payload = @"<43>Jun 27 23:43:47 marcus syslog-ng[21655]: Connection broken to AF_INET(127.0.0.1:3588), reopening in 60 seconds";
            expected = new SyslogMessage
                           {
                               Facility = SyslogFacility.Internally,
                               Severity = SyslogSeverity.Error,
                               Timestamp = new DateTime(2010, 6, 27, 23, 43, 47, 0),
                               Host = "marcus",
                               ApplicationName = "syslog-ng",
                               ProcessID = "21655",
                               MessageId = null,
                               Text = @"Connection broken to AF_INET(127.0.0.1:3588), reopening in 60 seconds"
                           };
            actual = null;

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

            //Testing with escape sequences in Data
            //RFC says that '"' and ']' in structured data values must be escaped
            payload = @"<165>1 2003-10-11T22:14:15.003Z mymachine.example.com evntslog - ID47 [exampleSDID@32473 escapeParentesis=""Wow\]"" escapeQuotes=""\"""" moreEscape=""\\\\\\\""""] Now it works....";

            actual = null;

            try
            {
                actual = SyslogMessage.Parse(payload);
            }
            catch (FormatException ex)
            {
                Assert.Fail("Failed parsing: {0}", ex);
            }
            Assert.IsNotNull(actual);
            IDictionary<String, String> test = actual.Value.Data["exampleSDID@32473"];
            Assert.AreEqual("Wow\\]", test["escapeParentesis"]);
            Assert.AreEqual("\\\"", test["escapeQuotes"]);
            Assert.AreEqual("\\\\\\\\\\\\\\\"", test["moreEscape"]);

            /*
            //Testing with spaces and equals in Data
            payload = @"<165>1 2003-10-11T22:14:15.003Z mymachine.example.com evntslog - ID47 [exampleSDID@32473 Space=""Wow "" Equal=""a=1"" SpaceAndEquals=""---== ""] I don't think the current parser will accept this message";

            actual = null;

            try
            {
                actual = SyslogMessage.Parse(payload);
            }
            catch (FormatException ex)
            {
                Assert.Fail("Failed parsing: {0}", ex);
            }
            Assert.IsNotNull(actual);
            test = actual.Value.Data["exampleSDID@32473"];
            Assert.AreEqual("\"Wow \"", test["Space"]);
            Assert.AreEqual("\"a=1\"", test["Equal"]);
            Assert.AreEqual("\"---== \"", test["SpaceAndEquals"]);
            */

            /*Testing a message generated by Log4net SyslogAppender with SimpleLayout
            payload = @"<179>UnitTestAdapterDomain_ForC:\Users\DJ Echelon\Documents\Visual Studio 2008\Projects\Logbus-ng-core\TestResults\DJ Echelon_MONSTR 2010-07-03 19_09_28\Out\Log4test.dll: ERROR - Test error message";
            actual = null;

            try
            {
                actual = SyslogMessage.Parse(payload);
            }
            catch (FormatException ex)
            {
                Assert.Fail("Failed parsing: {0}", ex);
            }
            Assert.IsNotNull(actual);
            */

            //Testing FFDA Message
            payload = @"<134>1 2010-07-13T04:56:38Z HYPERCUBE VSTestHost 3696 FFDA [CallerData@8289 ClassName=""System.RuntimeMethodHandle"" MethodName=""_InvokeMethodFast""] ﻿﻿SST";

            actual = null;

            try
            {
                actual = SyslogMessage.Parse(payload);
            }
            catch (FormatException ex)
            {
                Assert.Fail("Failed parsing: {0}", ex);
            }
            Assert.IsNotNull(actual);
            test = actual.Value.Data["CallerData@8289"];
            Assert.AreEqual("System.RuntimeMethodHandle", test["ClassName"]);
            Assert.AreEqual("_InvokeMethodFast", test["MethodName"]);
        }

        [TestMethod()]
        public void SyslogParserTestReal()
        {
            //Just test that messages are being parset. Stop.

            using (StreamReader sr = new StreamReader(GetType().Assembly.GetManifestResourceStream("Unit_Tests.TestLogs.Syslog.base64.txt"), Encoding.GetEncoding(1252)))
            {
                while (!sr.EndOfStream)
                {
                    string base64Line = sr.ReadLine();
                    byte[] rawLog = Convert.FromBase64String(base64Line);
                    try
                    {
                        SyslogMessage.Parse(rawLog);
                    }
                    catch (FormatException ex)
                    {
                        Assert.Fail("Test failed: {0}", ex);
                    }
                }
            }
            using (StreamReader sr = new StreamReader(GetType().Assembly.GetManifestResourceStream("Unit_Tests.TestLogs.Syslog.plain.txt"), Encoding.UTF8))
            {
                while (!sr.EndOfStream)
                {
                    string rawLog = sr.ReadLine();
                    try
                    {
                        SyslogMessage.Parse(rawLog);
                    }
                    catch (FormatException ex)
                    {
                        TestContext.WriteLine("Failed parsing: {0}", rawLog);
                        Assert.Fail("Test failed: {0}", ex);
                    }
                }
            }
        }
    }
}
