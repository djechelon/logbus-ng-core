using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using It.Unina.Dis.Logbus.Filters;

namespace Filter_Tests.ExampleCustom
{
    class CloneTrueFilter
        :IFilter
    {

        #region IFilter Membri di

        public bool IsMatch(It.Unina.Dis.Logbus.SyslogMessage message)
        {
            return true;
        }

        #endregion
    }
}
