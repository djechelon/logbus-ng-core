﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using It.Unina.Dis.Logbus.Filters;

namespace Filter_Tests.ExampleCustom
{
    class CloneFalseFilter
        :IFilter
    {

        #region IFilter Membri di

        bool IFilter.IsMatch(It.Unina.Dis.Logbus.SyslogMessage message)
        {
            return false;
        }

        #endregion
    }
}