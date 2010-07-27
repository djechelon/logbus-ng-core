/*
 *                  Logbus-ng project
 *    ©2010 Logbus Reasearch Team - Some rights reserved
 *
 *  Created by:
 *      Vittorio Alfieri - vitty85@users.sourceforge.net
 *      Antonio Anzivino - djechelon@users.sourceforge.net
 *
 *  Based on the research project "Logbus" by
 *
 *  Dipartimento di Informatica e Sistemistica
 *  University of Naples "Federico II"
 *  via Claudio, 21
 *  80121 Naples, Italy
 *
 *  Software is distributed under Microsoft Reciprocal License
 *  Documentation under Creative Commons 3.0 BY-SA License
*/

using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
namespace It.Unina.Dis.Logbus
{
    public class FileLogger : ILogCollector, IConfigurable
    {

        private string filePath;

        #region ILogCollector Membri di
        void ILogCollector.SubmitMessage(SyslogMessage message)
        {
            using (TextWriter tw = new StreamWriter(File.Open(filePath, FileMode.Append), Encoding.UTF8))
            {
                tw.WriteLine(message.ToRfc5424String());
            }
        }

        #endregion

        #region IConfigurable Membri di

        public string GetConfigurationParameter(string key)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentNullException("key", "Key cannot be null");
            switch (key)
            {
                case "filePath":
                    return filePath;
                default:
                    {
                        NotSupportedException ex = new NotSupportedException("Invalid key");
                        ex.Data.Add("key", key);
                        throw ex;
                    }
            }
        }

        public void SetConfigurationParameter(string key, string value)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentNullException("key", "Key cannot be null");
            if (string.IsNullOrEmpty(value))
                throw new ArgumentNullException("value", "Value cannot be null");
            switch (key)
            {

                case "filePath":
                    {
                        if (string.IsNullOrEmpty(value))
                            throw new ArgumentNullException("value");
                        try
                        {
                            new System.IO.FileInfo(value);
                            filePath = value;
                        }
                        catch (Exception ex)
                        {
                            throw new LogbusException("File path error", ex);
                        }

                        break;
                    }

                default:


                    throw new NotSupportedException("Invalid key");

            }
        }

        public System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<string, string>> Configuration
        {
            set
            {
                foreach (KeyValuePair<string, string> kvp in value)
                    SetConfigurationParameter(kvp.Key, kvp.Value);
            }
        }

        #endregion

    }
}

