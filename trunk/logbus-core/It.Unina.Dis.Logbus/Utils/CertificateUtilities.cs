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
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Web;

namespace It.Unina.Dis.Logbus.Utils
{
    /// <summary>
    /// Class for certficate-related utilities
    /// </summary>
    public static class CertificateUtilities
    {
        /// <summary>
        /// Gets the default Logbus-ng self-signed certificate
        /// </summary>
        public static X509Certificate2 DefaultCertificate
        {
            get
            {
                using (Stream stream = typeof(CertificateUtilities).Assembly.GetManifestResourceStream("It.Unina.Dis.Logbus.Security.DefaultCertificate.p12"))
                {
                    if (stream == null)
                        throw new LogbusException("Unable to find default self-signed SSL certificate");

                    byte[] payload = new byte[stream.Length];
                    stream.Read(payload, 0, (int)stream.Length);
                    return new X509Certificate2(payload);
                }
            }
        }

        /// <summary>
        /// Loads a certificate from a relative path
        /// </summary>
        /// <param name="relativePath">Relative path to certificate file</param>
        /// <returns></returns>
        public static X509Certificate2 LoadCertificate(string relativePath)
        {
            if (string.IsNullOrEmpty(relativePath))
                return DefaultCertificate;

            string abspath = Path.GetFullPath(relativePath);
            if (!File.Exists(abspath))
            {
                //Then we might have a problem :(
                if (HttpContext.Current != null) //We are running ASP.NET, use Server
                {
                    abspath = HttpContext.Current.Server.MapPath(relativePath);
                }
            }


            try
            {
                return relativePath.EndsWith(".pem") ? LoadPemCertificate(abspath) : new X509Certificate2(abspath);
            }
            catch (FileNotFoundException ex)
            {
                throw new LogbusException("Certificate file does not exist", ex);
            }
            catch (Exception ex)
            {
                throw new LogbusException("Invalid certificate path", ex);
            }
        }



        #region PEM support

        /*
         * Code taken from http://social.msdn.microsoft.com/Forums/en/csharpgeneral/thread/d7e2ccea-4bea-4f22-890b-7e48c267657f
         * */
        private static byte[] GetPem(string type, byte[] data)
        {
            string pem = Encoding.UTF8.GetString(data);
            string header = String.Format(@"-----BEGIN {0}-----", type);
            string footer = String.Format(@"-----END {0}-----", type);
            int start = pem.IndexOf(header) + header.Length;
            int end = pem.IndexOf(footer, start);
            string base64 = pem.Substring(start, (end - start));
            return Convert.FromBase64String(base64);
        }

        private static X509Certificate2 LoadPemCertificate(string filename)
        {
            using (FileStream fs = File.OpenRead(filename))
            {
                byte[] data = new byte[fs.Length];
                byte[] res = null;
                fs.Read(data, 0, data.Length);
                if (data[0] != 0x30)
                {
                    res = GetPem("RSA PRIVATE KEY", data);
                }
                X509Certificate2 x509 = new X509Certificate2(res); //Exception hit here
                return x509;
            }
        }

        #endregion
    }
}
