using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using It.Unina.Dis.Logbus.Filters;

namespace Filter_Tests.ExampleCustom
{
    /// <summary>
    /// A crazy example filter.
    /// Checks if a given char is in the given index of the Text part of Syslog message
    /// </summary>
    [It.Unina.Dis.Logbus.Design.CustomFilter("char")]
    class CharInMessageFilter
        : ICustomFilter
    {
        #region ICustomFilter Membri di

        public IEnumerable<FilterParameter> Configuration
        {
            set { throw new NotImplementedException(); }
        }

        #endregion

        #region IFilter Membri di

        public bool IsMatch(It.Unina.Dis.Logbus.SyslogMessage message)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
