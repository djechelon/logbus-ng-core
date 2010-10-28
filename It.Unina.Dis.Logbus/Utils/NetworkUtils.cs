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

using System.Net;
using System.Net.Sockets;

namespace It.Unina.Dis.Logbus.Utils
{
    internal sealed class NetworkUtils
    {
        /// <summary>
        /// Returns the IP address of the local machine according to a public-preferred criterion
        /// </summary>
        /// <returns></returns>
        /// <remarks>This method preferres public IPv4 addresses to LAN IPv4 addresses.
        /// If the machine has IPv6 connectivity, this method preferres IPv4</remarks>
        public static IPAddress GetMyIPAddress()
        {
            //All IPs available on this machine
            IPAddress[] a = Dns.GetHostAddresses(Dns.GetHostName());

            IPAddress preferredV4 = null, preferredV6 = null, ret = null;
            bool wanFound = false;

            for (int i = 0; i < a.Length; i++)
            {
                switch (a[i].AddressFamily)
                {
                    case AddressFamily.InterNetwork:
                        if (a[i].ToString().Contains("localhost"))
                            continue;
                        if (a[i].GetAddressBytes()[0] == 0 ||
                                 a[i].GetAddressBytes()[0] == 255 ||
                                 a[i].GetAddressBytes()[0] == 240 ||
                                 (a[i].GetAddressBytes()[0] == 203 && a[i].GetAddressBytes()[1] == 0 &&
                                  a[i].GetAddressBytes()[2] == 113) ||
                                 (a[i].GetAddressBytes()[0] == 198 && a[i].GetAddressBytes()[1] == 51 &&
                                  a[i].GetAddressBytes()[2] == 100) ||
                                 (a[i].GetAddressBytes()[0] == 198 && ((a[i].GetAddressBytes()[1] | 1) & 18) != 0) ||
                                 (a[i].GetAddressBytes()[0] == 192 && a[i].GetAddressBytes()[1] == 0 &&
                                  a[i].GetAddressBytes()[2] == 2) ||
                                 (a[i].GetAddressBytes()[0] == 192 && a[i].GetAddressBytes()[1] == 0 &&
                                  a[i].GetAddressBytes()[2] == 0) ||
                                 (a[i].GetAddressBytes()[0] == 198 && a[i].GetAddressBytes()[1] == 51 &&
                                  a[i].GetAddressBytes()[2] == 100))
                            //No network interface should return this
                            continue;
                        if (a[i].GetAddressBytes()[0] == 255)
                            //No network interface should return this
                            continue;
                        if (a[i].GetAddressBytes()[0] == 127)
                            //Local address. We don't want to use it
                            continue;
                        if ((a[i].GetAddressBytes()[0] == 192 && a[i].GetAddressBytes()[1] == 168) ||
                                a[i].GetAddressBytes()[0] == 169 && a[i].GetAddressBytes()[1] == 254 ||
                                a[i].GetAddressBytes()[0] == 10)
                        {
                            //Local-scope address. While we don't like these, they could be our only choice
                            if (preferredV4 == null && !wanFound)
                            {
                                preferredV4 = a[i];
                                wanFound = true;
                            }
                        }
                        else if (!wanFound && preferredV4 == null)
                            preferredV4 = a[i];
                        break;
                    case AddressFamily.InterNetworkV6:
                        if (a[i].IsIPv6LinkLocal)
                            //We dont' like link local addresses
                            continue;
                        if (preferredV6 == null) preferredV6 = a[i];
                        break;
                }
            }

            //Workaround: we must currently prefer IPv4 over IPv6
            if (preferredV4 != null) ret = preferredV4;
            if (preferredV6 != null && ret == null) ret = preferredV6;

            if (ret != null) return ret;
            throw new LogbusException("Unable to determine the IP address of current host");
        }
    }
}