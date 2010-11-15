using System.Collections.Generic;
using It.Unina.Dis.Logbus;
using It.Unina.Dis.Logbus.Design;
using It.Unina.Dis.Logbus.Filters;

namespace Filter_Tests.ExampleCustom
{
    /// <summary>
    /// A crazy example filter.
    /// Checks if a given char is in the given index of the Text part of Syslog message
    /// </summary>
    [CustomFilter("char")]
    internal class CharInMessageFilter
        : ICustomFilter
    {
        #region ICustomFilter Membri di

        /// <summary>
        /// Parameters:
        /// char: character to find
        /// index: index where it should be
        /// </summary>
        public IEnumerable<FilterParameter> Configuration { private get; set; }

        #endregion

        #region IFilter Membri di

        public bool IsMatch(SyslogMessage message)
        {
            char ch = '\0';
            int idx = 0;
            foreach (FilterParameter param in Configuration)
            {
                //Don't handle erroneous situations: we are in test environment
                if (param.name == "char") ch = char.Parse((string) param.value);
                if (param.name == "index") idx = int.Parse((string) param.value);
            }

            return (message.Text.IndexOf(ch) == idx);
        }

        #endregion
    }
}