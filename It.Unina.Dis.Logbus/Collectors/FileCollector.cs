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
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Web;

namespace It.Unina.Dis.Logbus.Collectors
{
    /// <summary>
    /// Logs to a file
    /// Configuration parameters:
    /// <list type="System.String">
    /// <item><c>filePath</c>: path of the file to write</item>
    /// </list>
    /// </summary>
    /// <remarks>Logs are stored in RFC 5424 format</remarks>
    public class FileCollector
        : ILogCollector, IConfigurable
    {
        /// <summary>
        /// Path of the file into which store logs separated by new lines
        /// </summary>
        public string FilePath { get; set; }

        private string _absoluteFilePath;

        #region ILogCollector Membri di

        [MethodImpl(MethodImplOptions.Synchronized)]
        void ILogCollector.SubmitMessage(SyslogMessage message)
        {
            using (StreamWriter sw = File.AppendText(_absoluteFilePath))
            {
                sw.WriteLine(message.ToRfc5424String());
            }
        }

        #endregion

        #region IConfigurable Membri di

        /// <remarks/>
        public string GetConfigurationParameter(string key)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentNullException("key", "Key cannot be null");
            switch (key)
            {
                case "filePath":
                    return FilePath;
                default:
                    {
                        NotSupportedException ex = new NotSupportedException("Invalid key");
                        ex.Data.Add("key", key);
                        throw ex;
                    }
            }
        }

        /// <remarks/>
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
                            string fpath = HttpContext.Current != null
                                               ? HttpContext.Current.Server.MapPath(value)
                                               : Path.GetFullPath(value);
#pragma warning disable 642
                            using (File.AppendText(fpath)) ; //Dummy open to get IOException or SecurityException
#pragma warning restore 642
                            FilePath = value;
                            _absoluteFilePath = fpath;
                        }
                        catch (Exception ex) //Expecting IOException or SecurityException
                        {
                            throw new LogbusException("File path error", ex);
                        }

                        break;
                    }
                default:
                    throw new NotSupportedException("Configuration key not supported");
            }
        }

        /// <remarks/>
        public IEnumerable<KeyValuePair<string, string>> Configuration
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