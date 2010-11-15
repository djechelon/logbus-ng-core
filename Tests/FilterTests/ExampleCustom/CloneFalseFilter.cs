using System.Collections.Generic;
using It.Unina.Dis.Logbus;
using It.Unina.Dis.Logbus.Design;
using It.Unina.Dis.Logbus.Filters;

namespace Filter_Tests.ExampleCustom
{
    [CustomFilter("clonefalse")]
    internal class CloneFalseFilter
        : ICustomFilter
    {
        #region IFilter Membri di

        public bool IsMatch(SyslogMessage message)
        {
            return false;
        }

        #endregion

        #region ICustomFilter Membri di

        public IEnumerable<FilterParameter> Configuration
        {
            set { }
        }

        #endregion
    }
}