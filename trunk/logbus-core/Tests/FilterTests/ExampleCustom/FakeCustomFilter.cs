using It.Unina.Dis.Logbus;
using It.Unina.Dis.Logbus.Design;
using It.Unina.Dis.Logbus.Filters;

namespace Filter_Tests.ExampleCustom
{
    /// <summary>
    /// This class must be refused as filter as it doesn't implement ICustomFilter
    /// </summary>
#if FAKE_ON
    [CustomFilter("fake")]
#endif
    internal class FakeCustomFilter
        : IFilter
    {
        #region IFilter Membri di

        public bool IsMatch(SyslogMessage message)
        {
            return true;
        }

        #endregion
    }
}