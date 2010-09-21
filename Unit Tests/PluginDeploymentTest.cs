using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using It.Unina.Dis.Logbus;
using System.Web.Services;
using It.Unina.Dis.Logbus.Configuration;
using It.Unina.Dis.Logbus.WebServices;
using System.Web.Services.Protocols;
using System.Web.Services.Description;
using System.Threading;

namespace Unit_Tests
{
    /// <summary>
    /// Descrizione del riepilogo per PluginDeploymentTest
    /// </summary>
    [TestClass]
    public class PluginDeploymentTest
    {
        public PluginDeploymentTest()
        {
            //
            // TODO: aggiungere qui la logica del costruttore
            //
        }

        private TestContext testContextInstance;

        /// <summary>
        ///Ottiene o imposta il contesto del test che fornisce
        ///le informazioni e le funzionalità per l'esecuzione del test corrente.
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
        // È possibile utilizzare i seguenti attributi aggiuntivi per la scrittura dei test:
        //
        // Utilizzare ClassInitialize per eseguire il codice prima di eseguire il primo test della classe
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Utilizzare ClassCleanup per eseguire il codice dopo l'esecuzione di tutti i test della classe
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Utilizzare TestInitialize per eseguire il codice prima di eseguire ciascun test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Utilizzare TestCleanup per eseguire il codice dopo l'esecuzione di ciascun test
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        [TestMethod]
        public void TestDeployAsmx()
        {
            try
            {
                LogbusCoreConfiguration config = new LogbusCoreConfiguration();
                PluginDefinition plugDef = new PluginDefinition() { type = typeof(DummyPlugin).AssemblyQualifiedName };
                config.plugins = new PluginDefinition[] { plugDef };
                using (LogbusService logbus = new LogbusService())
                {
                    logbus.Configure(config);
                    logbus.Start();

                    WebServiceActivator.Start(logbus, 8065);

                    //Test HTTP request
                    DummyStub stub = new DummyStub()
                    {
                        Url = "http://localhost:8065/Dummy.asmx"
                    };

                    Assert.AreEqual<int>(5, stub.Sum(3, 2));
                }
            }
            catch (Exception ex)
            {
                Thread.Sleep(Timeout.Infinite);
                Assert.Fail("Test failed: {0}", ex.ToString());
            }
        }

        protected class DummyPlugin : IPlugin
        {
            #region IPlugin Membri di

            public void Register(ILogBus logbus)
            { }

            public void Unregister()
            { }

            public string Name
            {
                get { return "Dummy"; }
            }

            public WsdlSkeletonDefinition[] GetWsdlSkeletons()
            {
                WsdlSkeletonDefinition def = new WsdlSkeletonDefinition()
                    {
                        UrlFileName = "Dummy",
                        SkeletonType = typeof(DummySkeleton)
                    };
                return new WsdlSkeletonDefinition[] { def };
            }

            public MarshalByRefObject GetPluginRoot()
            {
                return null;
            }

            #endregion

            #region ILogSupport Membri di

            public It.Unina.Dis.Logbus.Loggers.ILog Log
            {
                get;
                set;
            }

            #endregion

            #region IDisposable Membri di

            public void Dispose()
            { }

            #endregion

            protected class DummySkeleton : WebService
            {
                [WebMethod()]
                [SoapDocumentMethodAttribute("urn:#Sum", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Bare)]
                public int Sum(int a, int b)
                {
                    return a + b;
                }
            }
        }

        [System.Web.Services.WebServiceBindingAttribute(Name = "Dummy", Namespace = "http://www.dis.unina.it/logbus-ng/dummy")]
        protected class DummyStub : SoapHttpClientProtocol
        {
            public DummyStub() : base() { }

            [SoapDocumentMethodAttribute("urn:#Sum", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Bare)]
            public int Sum(int a, int b)
            {
                return (int)(base.Invoke("Sum", new object[] { a, b })[0]);
            }
        }
    }
}
