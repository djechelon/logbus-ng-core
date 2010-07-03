using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using It.Unina.Dis.Logbus.Filters;

namespace Filter_Tests.ExampleCustom
{
    class CloneFalseFilter
        : ICustomFilter
    {

        #region IFilter Membri di

        bool IFilter.IsMatch(It.Unina.Dis.Logbus.SyslogMessage message)
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
