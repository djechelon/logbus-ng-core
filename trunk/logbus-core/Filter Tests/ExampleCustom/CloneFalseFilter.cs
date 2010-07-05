using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using It.Unina.Dis.Logbus.Filters;
using It.Unina.Dis.Logbus.Design;

namespace Filter_Tests.ExampleCustom
{
    [CustomFilterAttribute("clonefalse")]
    class CloneFalseFilter
        : ICustomFilter
    {

        #region IFilter Membri di

        public bool IsMatch(It.Unina.Dis.Logbus.SyslogMessage message)
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
