using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using It.Unina.Dis.Logbus;

namespace Unit_Tests.TestClasses
{
    /// <summary>
    /// OK for NotImplementedException. This is a test
    /// </summary>
    public class TestTransportFactory
        :IOutboundTransportFactory
    {
        #region IOutboundTransportFactory Membri di

        public IOutboundTransport CreateTransport()
        {
            throw new NotImplementedException();
        }

        public string GetConfigurationParameter(string key)
        {
            throw new NotImplementedException();
        }

        public void SetConfigurationParameter(string key, string value)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<KeyValuePair<string, string>> Configuration
        {
            set { throw new NotImplementedException(); }
        }

        #endregion
    }
}
