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

using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;
using System.Net;
using System.IO;
using System;
using System.Globalization;
using System.Net.Security;
using System.Security.Authentication;
using System.Text;
using System.Security.Cryptography.X509Certificates;
namespace It.Unina.Dis.Logbus.InChannels
{
    public sealed class SyslogTlsReceiver
        : ReceiverBase
    {
        /// <summary>
        /// Default port for TLS transport as defined in RFC 5425
        /// </summary>
        public const int TLS_PORT = 6154;

        /// <summary>
        /// Number of worker threads concurrently listening for datagrams
        /// </summary>
        public new const int WORKER_THREADS = 4;

        private Dictionary<string, string> config = new Dictionary<string, string>();
        private TcpListener listener;
        private string certificate_path;

        private Thread[] listener_threads;

        private List<TcpClient> clients = new List<TcpClient>();

        /// <summary>
        /// Port to listen on
        /// </summary>
        public int Port
        {
            get;
            set;
        }

        /// <summary>
        /// Interface to listen on
        /// </summary>
        public string IpAddress
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the SSL certificate for the current server
        /// </summary>
        public X509Certificate2 Certificate
        {
            get;
            set;
        }

        /// <summary>
        /// Implements ReceiverBase.OnStart
        /// </summary>
        protected override void OnStart()
        {
            if (Port == 0)
            {
                Port = TLS_PORT;
            }

            IPEndPoint local_ep;
            if (IpAddress == null) local_ep = new IPEndPoint(IPAddress.Any, Port);
            else local_ep = new IPEndPoint(IPAddress.Parse(IpAddress), Port);

            if (Certificate == null) throw new InvalidOperationException("Certificate not specified");

            try
            {
                listener = new TcpListener(local_ep);
                listener.Start();
            }
            catch (IOException ex)
            {
                throw new LogbusException("Cannot start TLS/TCP listener", ex);
            }

            listener_threads = new Thread[WORKER_THREADS];
            for (int i = 0; i < WORKER_THREADS; i++)
            {
                listener_threads[i] = new Thread(ListenerLoop);
                listener_threads[i].Name = string.Format("SyslogTlsReceiver[{1}].ListenerLoop[{0}]", i, Name);
                listener_threads[i].IsBackground = true;
                listener_threads[i].Start();
            }
        }

        /// <summary>
        /// Implements ReceiverBase.OnStart
        /// </summary>
        protected override void OnStop()
        {
            lock (clients)
                foreach (TcpClient client in clients)
                {
                    try
                    {
                        client.Close();
                    }
                    catch { }
                }

            try
            {
                listener.Stop();
                for (int i = 0; i < WORKER_THREADS; i++)
                    listener_threads[i].Join();
                listener_threads = null;
            }
            catch (Exception) { } //Really nothing?
        }

        #region Configuration
        /// <summary>
        /// Implements IConfigurable.GetConfigurationParameter
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public override string GetConfigurationParameter(string key)
        {
            if (Disposed) throw new ObjectDisposedException(GetType().FullName);
            switch (key)
            {
                case "ip":
                    return IpAddress;
                case "port":
                    return Port.ToString(CultureInfo.InvariantCulture);
                case "certificate":
                    return certificate_path;
                default:
                    {
                        throw new NotSupportedException("Configuration parameter is not supported");
                    }
            }
        }

        /// <summary>
        /// Implements IConfigurable.SetConfigurationParameter
        /// </summary>
        public override void SetConfigurationParameter(string key, string value)
        {
            if (Disposed) throw new ObjectDisposedException(GetType().FullName);
            switch (key)
            {
                case "ip":
                    {
                        IpAddress = value;
                        break;
                    }
                case "port":
                    {
                        Port = int.Parse(value);
                        break;
                    }
                case "certificate":
                    {
                        certificate_path = value;
                        LoadCertificate(value);
                        break;
                    }
                default:
                    {
                        throw new NotSupportedException("Configuration parameter is not supported");
                    }
            }
        }

        private void LoadCertificate(string path)
        {
            try
            {
                if (path.EndsWith(".pem"))
                {
                    Certificate = LoadPemCertificate(path);
                    return;
                }
                else //Should be .p12
                {
                    Certificate = new X509Certificate2(path);
                }
            }
            catch (Exception ex)
            {
                Log.Error("Unable to load X.509 certificate for TLS listener");
                Log.Debug(string.Format("Exception message: {0}", ex.Message));
                throw;
            }
        }

        /// <summary>
        /// Implements IConfigurable.Configuration
        /// </summary>
        public override IEnumerable<KeyValuePair<string, string>> Configuration
        {
            set
            {
                if (Disposed) throw new ObjectDisposedException(GetType().FullName);
                foreach (KeyValuePair<string, string> kvp in value)
                    SetConfigurationParameter(kvp.Key, kvp.Value);
            }
        }
        #endregion

        /// <summary>
        /// Accepts clients
        /// </summary>
        private void ListenerLoop()
        {
            while (Running)
                try
                {
                    TcpClient client = listener.AcceptTcpClient();
                    Thread client_thread = new Thread(ProcessClient);
                    client_thread.Name = string.Format("TlsListener.ProcessClient[{0}]", client.Client.RemoteEndPoint.ToString());
                    client_thread.IsBackground = true;
                    client_thread.Start(client);
                }
                catch (SocketException) { }
        }

        /// <summary>
        /// Threads spawned after connection use this loop
        /// </summary>
        /// <param name="clientObj"></param>
        private void ProcessClient(object clientObj)
        {
            using (TcpClient client = (TcpClient)clientObj)
            {
                lock (clients) clients.Add(client);
                // A client has connected. Create the 
                // SslStream using the client's network stream.
                using (SslStream sslStream = new SslStream(client.GetStream(), false))
                    // Authenticate the server but don't require the client to authenticate.
                    try
                    {
                        sslStream.AuthenticateAsServer(Certificate, false, SslProtocols.Tls, true);

                        sslStream.ReadTimeout = 3600000; //1 hour

                        using (StreamReader sr = new StreamReader(sslStream, Encoding.UTF8, true))
                            while (true)
                            {
                                StringBuilder sb = new StringBuilder();
                                char next_char;
                                do
                                {
                                    next_char = (char)sr.Read();
                                    if (char.IsDigit(next_char)) sb.Append(next_char);
                                    else if (next_char == ' ') break;
                                    else throw new FormatException("Invalid TLS encoding of Syslog message");
                                } while (true);

                                int char_len = int.Parse(sb.ToString(), CultureInfo.InvariantCulture);

                                char[] buffer = new char[char_len];
                                if (sr.Read(buffer, 0, char_len) != char_len)
                                {
                                    throw new FormatException("Invalid TLS encoding of Syslog message");
                                }

                                ForwardMessage(SyslogMessage.Parse(new string(buffer)));
                            }

                    }
                    catch { return; }
                    finally
                    {
                        lock (clients) clients.Remove(client);
                    }
            }

        }

        #region PEM support

        /*
         * Code taken from http://social.msdn.microsoft.com/Forums/en/csharpgeneral/thread/d7e2ccea-4bea-4f22-890b-7e48c267657f
         * */
        private byte[] GetPem(string type, byte[] data)
        {
            string pem = Encoding.UTF8.GetString(data);
            string header = String.Format(@"-----BEGIN {0}-----", type);
            string footer = String.Format(@"-----END {0}-----", type);
            int start = pem.IndexOf(header) + header.Length;
            int end = pem.IndexOf(footer, start);
            string base64 = pem.Substring(start, (end - start));
            return Convert.FromBase64String(base64);
        }

        private X509Certificate2 LoadPemCertificate(string filename)
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
