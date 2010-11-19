using System;
using System.Threading;
using It.Unina.Dis.Logbus.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace UnitTests
{


    /// <summary>
    ///Classe di test per FastFifoQueueTest.
    ///Creata per contenere tutti gli unit test FastFifoQueueTest
    ///</summary>
    [TestClass()]
    public class FastFifoQueueTest
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

        private int _errors;

        [TestMethod()]
        public void ConcurrencyTest()
        {
            const int size = 3;
            _errors = 0;
            IFifoQueue<object> queue = new FastFifoQueue<object>(2048);
            Thread.CurrentThread.Priority = ThreadPriority.AboveNormal;
            Thread[] producers = new Thread[size], consumers = new Thread[size];

            for (int i = 0; i < size; i++)
            {
                producers[i] = new Thread(LoopProducer) { Priority = ThreadPriority.BelowNormal };
                consumers[i] = new Thread(LoopConsumer) { Priority = ThreadPriority.BelowNormal };
                producers[i].Start(queue);
                consumers[i].Start(queue);
            }

            Thread.Sleep(new TimeSpan(0, 0, 1, 0));

            for (int i = 0; i < size; i++)
            {
                producers[i].Abort();
                consumers[i].Abort();
            }

            Assert.AreEqual(0, _errors);
        }

        private void LoopProducer(object queue)
        {
            long writes = 0;
            try
            {
                IFifoQueue<object> q = (IFifoQueue<object>)queue;
                while (true)
                {
                    q.Enqueue(new object());
                    writes++;
                    Thread.Sleep(0);
                }
            }
            catch (ThreadAbortException)
            { TestContext.WriteLine("Wrote {0} elements", writes); }
        }

        private void LoopConsumer(object queue)
        {
            long reads = 0;
            try
            {
                IFifoQueue<object> q = (IFifoQueue<object>)queue;
                while (true)
                {
                    object item = q.Dequeue();
                    if (item == null) Interlocked.Increment(ref _errors);
                    else reads++;
                    Thread.Sleep(0);
                }
            }
            catch (ThreadAbortException)
            { TestContext.WriteLine("Read {0} elements", reads); }

        }


        [TestMethod]
        public void CounterTest()
        {
            int locked = int.MinValue;
            int size = 512;
            int j = 1;
            for (long i = 0; i < Math.Pow(2, 33); i++)
            {
                int temp = Interlocked.Increment(ref locked);
                temp %= size;
                if (temp < 0) temp += size;
                if (i == 0) TestContext.WriteLine("Starting from {0}", temp);

                Assert.AreEqual(j, temp);
                j++;
                if (j == size) j = 0;
            }
        }
    }
}
