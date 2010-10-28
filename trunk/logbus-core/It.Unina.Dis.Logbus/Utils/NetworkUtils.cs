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

            IPAddress preferred_v4 = null, preferred_v6 = null, ret = null;
            bool wan_found = false;

            for (int i = 0; i < a.Length; i++)
            {
                if (a[i].AddressFamily == AddressFamily.InterNetwork)
                {
                    if (a[i].ToString().Contains("localhost"))
                        continue;
                    else if (a[i].GetAddressBytes()[0] == 0 ||
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
                    else if (a[i].GetAddressBytes()[0] == 255)
                        //No network interface should return this
                        continue;
                    else if (a[i].GetAddressBytes()[0] == 127)
                        //Local address. We don't want to use it
                        continue;
                    else if ((a[i].GetAddressBytes()[0] == 192 && a[i].GetAddressBytes()[1] == 168) ||
                             a[i].GetAddressBytes()[0] == 169 && a[i].GetAddressBytes()[1] == 254 ||
                             a[i].GetAddressBytes()[0] == 10)
                    {
                        //Local-scope address. While we don't like these, they could be our only choice
                        if (preferred_v4 == null && !wan_found)
                        {
                            preferred_v4 = a[i];
                            wan_found = true;
                        }
                    }
                    else if (!wan_found && preferred_v4 == null)
                        preferred_v4 = a[i];
                }
                else if (a[i].AddressFamily == AddressFamily.InterNetworkV6)
                    if (a[i].IsIPv6LinkLocal)
                        //We dont' like link local addresses
                        continue;
                    else if (preferred_v6 == null) preferred_v6 = a[i];
            }

            //Workaround: we must currently prefer IPv4 over IPv6
            if (preferred_v4 != null) ret = preferred_v4;
            if (preferred_v6 != null && ret == null) ret = preferred_v6;

            if (ret != null) return ret;
            throw new LogbusException("Unable to determine the IP address of current host");
        }
    }
}