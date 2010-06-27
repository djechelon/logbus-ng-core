using It.Unina.Dis.Logbus.InChannels;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System;
using It.Unina.Dis.Logbus;

namespace Unit_Tests
{


    /// <summary>
    ///Classe di test per SyslogUdpReceiverTest.
    ///Creata per contenere tutti gli unit test SyslogUdpReceiverTest
    ///</summary>
    [TestClass()]
    public class SyslogUdpReceiverTest
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
        ///Test per Configuration
        ///</summary>
        [TestMethod()]
        public void ConfigurationTest()
        {
            //Test 1: empty configuration
            using (SyslogUdpReceiver target = new SyslogUdpReceiver())
            {

                try
                {
                    target.Start();
                    Assert.Fail("Target not initialized but started");
                }
                catch (LogbusException)
                {
                    //OK
                }
                catch (Exception)
                {
                    Assert.Fail("I expected InvalidOperationException");
                }
            }

            //Test 2: bad data
            using (SyslogUdpReceiver target = new SyslogUdpReceiver())
            {

                try
                {
                    target.Configuration["port"] = "HELL0";
                    target.Configuration["ip"] = "656.33.21.0";
                    //target.Configuration.Add(new KeyValuePair<string, string>("port", "37888"));
                    //target.Configuration.Add(new KeyValuePair<string, string>("ip", "127.0.0.1"));
                    target.Start();
                    Assert.Fail("It should never have started");
                }
                catch (LogbusException)
                {
                    //OK
                }
                catch (Exception ex)
                {
                    Assert.Fail("Listener should have started... [{0}]", ex.ToString());
                }
            }

            //Test 3: IP and port filled (it should start)
            using (SyslogUdpReceiver target = new SyslogUdpReceiver())
            {

                try
                {
                    target.Configuration["port"] = "37888";
                    target.Configuration["ip"] = "127.0.0.1";
                    //target.Configuration.Add(new KeyValuePair<string, string>("port", "37888"));
                    //target.Configuration.Add(new KeyValuePair<string, string>("ip", "127.0.0.1"));
                    target.Start();
                    target.Stop();
                }
                catch (Exception ex)
                {
                    Assert.Fail("Listener should have started... [{0}]", ex.ToString());
                }
            }


            //Test 4: start & stop twice
            using (SyslogUdpReceiver target = new SyslogUdpReceiver())
            {

                try
                {
                    target.Configuration["port"] = "37888";
                    target.Configuration["ip"] = "127.0.0.1";
                    //target.Configuration.Add(new KeyValuePair<string, string>("port", "37888"));
                    //target.Configuration.Add(new KeyValuePair<string, string>("ip", "127.0.0.1"));
                    target.Start();
                    target.Stop();
                    target.Start();
                    target.Stop();
                }
                catch (Exception ex)
                {
                    Assert.Fail("It shouldn't fail... [{0}]", ex.ToString());
                }
            }
        }

        /// <summary>
        ///Test per Stop
        ///</summary>
        [TestMethod()]
        public void StopTest()
        {
            //Test 1: stop but not started
            try
            {
                using (SyslogUdpReceiver target = new SyslogUdpReceiver())
                {
                    //target.Configuration["port"] = "37889";
                    //target.Configuration["ip"] = "127.0.0.1";
                    //target.Start();
                    target.Stop();
                }
                Assert.Fail("I expected InvalidOperationException");
            }
            catch (InvalidOperationException)
            {
                //OK
            }
            catch (Exception ex)
            {
                Assert.Fail("Something went wrong: [{0}]", ex);
            }


            //Test 2: start and stop
            try
            {
                using (SyslogUdpReceiver target = new SyslogUdpReceiver())
                {
                    target.Configuration["port"] = "37889";
                    target.Configuration["ip"] = "127.0.0.1";
                    target.Start();
                    target.Stop();
                }
                //OK
            }
            catch (Exception ex)
            {
                Assert.Fail("Something went wrong: [{0}]", ex);
            }
        }

        /// <summary>
        ///Test per Start
        ///</summary>
        [TestMethod()]
        public void StartTest()
        {
            //Test 1: double start
            using (SyslogUdpReceiver target = new SyslogUdpReceiver())
            {
                try
                {
                    target.Configuration["port"] = "37888";
                    target.Configuration["ip"] = "127.0.0.1";
                    //target.Configuration.Add(new KeyValuePair<string, string>("port", "37888"));
                    //target.Configuration.Add(new KeyValuePair<string, string>("ip", "127.0.0.1"));
                    target.Start();
                    target.Start();
                    Assert.Fail("It should have thrown LogbusException");
                }
                catch (LogbusException)
                {
                    //OK
                }
                catch (Exception)
                {
                    Assert.Fail("I expected LogbusException");
                }
            }

            //OK test
            using (SyslogUdpReceiver target = new SyslogUdpReceiver())
            {
                target.Configuration["port"] = "37889";
                target.Configuration["ip"] = "127.0.0.1";
                target.Start();
            }
            //OK
        }

        /// <summary>
        ///Test per Costruttore SyslogUdpReceiver
        ///</summary>
        [TestMethod()]
        public void SyslogUdpReceiverConstructorTest()
        {
            SyslogUdpReceiver target = new SyslogUdpReceiver();
            Assert.IsNotNull(target);
            Assert.IsInstanceOfType(target, typeof(IInboundChannel));
        }


        /// <summary>
        ///Test per IpAddress
        ///</summary>
        [TestMethod()]
        public void IpAddressTest()
        {
            using (SyslogUdpReceiver target = new SyslogUdpReceiver())
            {
                string expected = "127.0.0.1";
                string actual;
                target.IpAddress = expected;
                actual = target.IpAddress;
                Assert.AreEqual(expected, actual);
                Assert.AreEqual(expected, target.Configuration["ip"]);
            }
        }

        /// <summary>
        ///Test per Port
        ///</summary>
        [TestMethod()]
        public void PortTest()
        {
            using (SyslogUdpReceiver target = new SyslogUdpReceiver())
            {
                int expected = 8526;
                int actual;
                target.Port = expected;
                actual = target.Port;
                Assert.AreEqual(expected, actual);
                Assert.AreEqual(expected.ToString(), target.Configuration["port"]);
            }
        }

    }
}
