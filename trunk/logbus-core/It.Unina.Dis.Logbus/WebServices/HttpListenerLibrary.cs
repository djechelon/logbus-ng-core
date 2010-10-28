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

/*
 *  This file comes from code example at http://msdn.microsoft.com/en-us/magazine/cc163879.aspx 
 *  "Service Station: Run ASMX without IIS", written by Aaron Skonnard
 * 
 *  Article and code copyright Microsoft Corporation. Imported in Logbus-ng according to EULA
 *  Original code edited
*/

#region Using directives

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Net;
using System.Threading;
using System.Web;
using System.Web.Hosting;

#endregion

namespace It.Unina.Dis.Logbus.WebServices
{
    internal class HttpListenerController
    {
        private Thread _pump;
        private bool _listening;
        private string _virtualDir;
        private string _physicalDir;
        private string[] _prefixes;
        private HttpListenerWrapper _listener;

        public HttpListenerController(string[] prefixes, string vdir, string pdir)
        {
            _prefixes = prefixes;
            _virtualDir = vdir;
            _physicalDir = pdir;
        }

        public void Start()
        {
            _listening = true;

            _listener = (HttpListenerWrapper) ApplicationHost.CreateApplicationHost(
                typeof (HttpListenerWrapper), _virtualDir, _physicalDir);
            _listener.Configure(_prefixes, _virtualDir, _physicalDir);
            _listener.Start();

            _pump = new Thread(Pump);
            _pump.Start();
        }

        public void Stop()
        {
            _listening = false;

            Process.GetCurrentProcess().Kill();
            _pump.Abort();
            _pump.Join();
        }

        private void Pump()
        {
            try
            {
                while (_listening)
                    _listener.ProcessRequest();
            }
            catch (Exception)
            {
                Stop();
                throw;
                /*EventLog myLog = new EventLog();
                myLog.Source = "HttpListenerController";
                if (null != ex.InnerException)
                    myLog.WriteEntry(ex.InnerException.ToString(), EventLogEntryType.Error);
                else
                    myLog.WriteEntry(ex.ToString(), EventLogEntryType.Error);
                 */
            }
        }

        public AppDomain Domain
        {
            get { return _listener.Domain; }
        }
    }

    internal class HttpListenerWrapper : MarshalByRefObject
    {
        private HttpListener _listener;
        private string _virtualDir;
        private string _physicalDir;

        public void Configure(string[] prefixes, string vdir, string pdir)
        {
            _virtualDir = vdir;
            _physicalDir = pdir;
            _listener = new HttpListener();

            foreach (string prefix in prefixes)
                _listener.Prefixes.Add(prefix);
        }

        public void Start()
        {
            _listener.Start();
        }

        public void Stop()
        {
            _listener.Stop();
        }

        public void ProcessRequest()
        {
            HttpListenerContext ctx = _listener.GetContext();
            HttpListenerWorkerRequest workerRequest =
                new HttpListenerWorkerRequest(ctx, _virtualDir, _physicalDir);
            HttpRuntime.ProcessRequest(workerRequest);
        }

        public AppDomain Domain
        {
            get { return Thread.GetDomain(); }
        }
    }

    internal class HttpListenerWorkerRequest : HttpWorkerRequest
    {
        private readonly HttpListenerContext _context;
        private readonly string _virtualDir;
        private readonly string _physicalDir;

        public HttpListenerWorkerRequest(
            HttpListenerContext context, string vdir, string pdir)
        {
            if (null == context)
                throw new ArgumentNullException("context");
            if (null == vdir || vdir.Equals(""))
                throw new ArgumentException("vdir");
            if (null == pdir || pdir.Equals(""))
                throw new ArgumentException("pdir");

            _context = context;
            _virtualDir = vdir;
            _physicalDir = pdir;
        }

        // required overrides (abstract)
        public override void EndOfRequest()
        {
            _context.Response.OutputStream.Close();
            _context.Response.Close();
            //_context.Close();
        }

        public override void FlushResponse(bool finalFlush)
        {
            _context.Response.OutputStream.Flush();
        }

        public override string GetHttpVerbName()
        {
            return _context.Request.HttpMethod;
        }

        public override string GetHttpVersion()
        {
            return string.Format("HTTP/{0}.{1}",
                                 _context.Request.ProtocolVersion.Major,
                                 _context.Request.ProtocolVersion.Minor);
        }

        public override string GetLocalAddress()
        {
            return _context.Request.LocalEndPoint.Address.ToString();
        }

        public override int GetLocalPort()
        {
            return _context.Request.LocalEndPoint.Port;
        }

        public override string GetQueryString()
        {
            string queryString = "";
            string rawUrl = _context.Request.RawUrl;
            int index = rawUrl.IndexOf('?');
            if (index != -1)
                queryString = rawUrl.Substring(index + 1);
            return queryString;
        }

        public override string GetRawUrl()
        {
            return _context.Request.RawUrl;
        }

        public override string GetRemoteAddress()
        {
            return _context.Request.RemoteEndPoint.Address.ToString();
        }

        public override int GetRemotePort()
        {
            return _context.Request.RemoteEndPoint.Port;
        }

        public override string GetUriPath()
        {
            return _context.Request.Url.LocalPath;
        }

        public override void SendKnownResponseHeader(int index, string value)
        {
            _context.Response.Headers[
                GetKnownResponseHeaderName(index)] = value;
        }

        public override void SendResponseFromMemory(byte[] data, int length)
        {
            _context.Response.OutputStream.Write(data, 0, length);
        }

        public override void SendStatus(int statusCode, string statusDescription)
        {
            _context.Response.StatusCode = statusCode;
            _context.Response.StatusDescription = statusDescription;
        }

        public override void SendUnknownResponseHeader(string name, string value)
        {
            _context.Response.Headers[name] = value;
        }

        public override void SendResponseFromFile(
            IntPtr handle, long offset, long length)
        {
        }

        public override void SendResponseFromFile(
            string filename, long offset, long length)
        {
        }

        // additional overrides
        public override void CloseConnection()
        {
            //_context.Close();
        }

        public override string GetAppPath()
        {
            return _virtualDir;
        }

        public override string GetAppPathTranslated()
        {
            return _physicalDir;
        }

        public override int ReadEntityBody(byte[] buffer, int size)
        {
            return _context.Request.InputStream.Read(buffer, 0, size);
        }

        public override string GetUnknownRequestHeader(string name)
        {
            return _context.Request.Headers[name];
        }

        public override string[][] GetUnknownRequestHeaders()
        {
            string[][] unknownRequestHeaders;
            NameValueCollection headers = _context.Request.Headers;
            int count = headers.Count;
            List<string[]> headerPairs = new List<string[]>(count);
            for (int i = 0; i < count; i++)
            {
                string headerName = headers.GetKey(i);
                if (GetKnownRequestHeaderIndex(headerName) == -1)
                {
                    string headerValue = headers.Get(i);
                    headerPairs.Add(new[] {headerName, headerValue});
                }
            }
            unknownRequestHeaders = headerPairs.ToArray();
            return unknownRequestHeaders;
        }

        public override string GetKnownRequestHeader(int index)
        {
            switch (index)
            {
                case HeaderUserAgent:
                    return _context.Request.UserAgent;
                default:
                    return _context.Request.Headers[GetKnownRequestHeaderName(index)];
            }
        }

        public override string GetServerVariable(string name)
        {
            // TODO: vet this list
            switch (name)
            {
                case "HTTPS":
                    return _context.Request.IsSecureConnection ? "on" : "off";
                case "HTTP_USER_AGENT":
                    return _context.Request.Headers["UserAgent"];
                default:
                    return null;
            }
        }

        public override string GetFilePath()
        {
            // TODO: this is a hack
            string s = _context.Request.Url.LocalPath;
            if (s.IndexOf(".aspx") != -1)
                s = s.Substring(0, s.IndexOf(".aspx") + 5);
            else if (s.IndexOf(".asmx") != -1)
                s = s.Substring(0, s.IndexOf(".asmx") + 5);
            return s;
        }

        public override string GetFilePathTranslated()
        {
            string s = GetFilePath();
            s = s.Substring(_virtualDir.Length);
            s = s.Replace('/', '\\');
            return _physicalDir + s;
        }

        public override string GetPathInfo()
        {
            string s1 = GetFilePath();
            string s2 = _context.Request.Url.LocalPath;
            return s1.Length == s2.Length ? "" : s2.Substring(s1.Length);
        }
    }
}