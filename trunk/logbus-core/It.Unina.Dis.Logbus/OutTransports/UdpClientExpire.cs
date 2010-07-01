using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;

namespace It.Unina.Dis.Logbus.OutTransports
{
    class UdpClientExpire
    {
        public UdpClient Client
        {
            get;
            set;
        }

        public DateTime? LastRefresh
        {
            get;
            set;
        }
    }
}
