using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using It.Unina.Dis.Logbus.Filters;

namespace Filter_Tests.ExampleCustom
{
    /// <summary>
    /// This class must be refused as filter as it doesn't implement ICustomFilter
    /// </summary>
    class FakeCustomFilter
        : IFilter
    {
        #region IFilter Membri di

        public bool IsMatch(It.Unina.Dis.Logbus.SyslogMessage message)
        {
            return true;
        }

        #endregion
    }
}
