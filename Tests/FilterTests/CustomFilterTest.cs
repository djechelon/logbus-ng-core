﻿using System.IO;
using System.Text;
using System.Xml.Serialization;
using Filter_Tests.ExampleCustom;
using It.Unina.Dis.Logbus;
using It.Unina.Dis.Logbus.Filters;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Filter_Tests
{
    /// <summary>
    ///Classe di test per CustomFilterTest.
    ///Creata per contenere tutti gli unit test CustomFilterTest
    ///</summary>
    [TestClass]
    public class CustomFilterTest
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

        [TestInitialize]
        public void Init()
        {
            TestContext.WriteLine("Type for CloneTrue: {0}", typeof (CloneTrueFilter).AssemblyQualifiedName);
            TestContext.WriteLine("Type for CloneFalse: {0}", typeof (CloneFalseFilter).AssemblyQualifiedName);
            TestContext.WriteLine("Type for CharInMessage: {0}", typeof (CharInMessageFilter).AssemblyQualifiedName);
            CustomFilterHelper.Instance.RegisterCustomFilter("char", typeof (CharInMessageFilter).AssemblyQualifiedName,
                                                             "Char in message");
            CustomFilterHelper.Instance.RegisterCustomFilter("clonetrue", typeof (CloneTrueFilter).AssemblyQualifiedName,
                                                             "Clone of True");
            CustomFilterHelper.Instance.RegisterCustomFilter("clonefalse",
                                                             typeof (CloneFalseFilter).AssemblyQualifiedName,
                                                             "Clone of False");
        }

        /// <summary>
        ///Test per IsMatch
        ///</summary>
        [TestMethod]
        public void IsMatchTest()
        {
            {
                CustomFilter target = new CustomFilter
                                          {
                                              name = "clonetrue",
                                              parameter = null
                                          };

                SyslogMessage message = new SyslogMessage(); //Empty is ok
                bool expected = true;
                bool actual;
                actual = target.IsMatch(message);
                Assert.AreEqual(expected, actual);
            }

            {
                CustomFilter target = new CustomFilter
                                          {
                                              name = "clonefalse",
                                              parameter = null
                                          };

                SyslogMessage message = new SyslogMessage(); //Empty is ok
                bool expected = false;
                bool actual;
                actual = target.IsMatch(message);
                Assert.AreEqual(expected, actual);
            }

            {
                CustomFilter target = new CustomFilter
                                          {
                                              name = "char",
                                              parameter = new FilterParameter[2]
                                          };
                target.parameter[0] = new FilterParameter
                                          {
                                              name = "char",
                                              value = "3"
                                          };
                target.parameter[1] = new FilterParameter
                                          {
                                              name = "index",
                                              value = "1"
                                          };

                SyslogMessage message = new SyslogMessage
                                            {
                                                Text = "H3llo world!"
                                            };

                bool expected = true;
                bool actual;
                actual = target.IsMatch(message);
                Assert.AreEqual(expected, actual);

                TestContext.WriteLine("CharInMessage is represented by the following markup (basing on FilterBase)");
                XmlSerializer seria = new XmlSerializer(typeof (FilterBase));
                using (MemoryStream ms = new MemoryStream())
                {
                    seria.Serialize(ms, target, target.xmlns);
                    ms.Flush();
                    ms.Seek(0, SeekOrigin.Begin);
                    TestContext.WriteLine(Encoding.Default.GetString(ms.ToArray()));
                }
                TestContext.WriteLine("CharInMessage is represented by the following markup (basing on FilterBase)");
                seria = new XmlSerializer(typeof (CustomFilter));
                using (MemoryStream ms = new MemoryStream())
                {
                    seria.Serialize(ms, target, target.xmlns);
                    ms.Flush();
                    ms.Seek(0, SeekOrigin.Begin);
                    TestContext.WriteLine(Encoding.Default.GetString(ms.ToArray()));
                }
            }

            {
                CustomFilter target = new CustomFilter
                                          {
                                              name = "char",
                                              parameter = new FilterParameter[2]
                                          };
                target.parameter[0] = new FilterParameter
                                          {
                                              name = "char",
                                              value = "3"
                                          };
                target.parameter[1] = new FilterParameter
                                          {
                                              name = "index",
                                              value = "1"
                                          };

                SyslogMessage message = new SyslogMessage
                                            {
                                                Text = "Hello world!"
                                            };

                bool expected = false;
                bool actual;
                actual = target.IsMatch(message);
                Assert.AreEqual(expected, actual);
            }
        }
    }
}