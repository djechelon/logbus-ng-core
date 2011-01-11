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
using It.Unina.Dis.Logbus;
using It.Unina.Dis.Logbus.WebServices;
using System.Threading;

namespace LogbusDaemon
{
    class Program
    {
        private static ILogBus _logbus;

        static void Main(string[] args)
        {
            Console.CancelKeyPress += new ConsoleCancelEventHandler(Console_CancelKeyPress);

            Console.WriteLine("Starting the Logbus-ng daemon. You MUST be running this program as Administrator");

            try
            {
                _logbus = LogbusSingletonHelper.Instance;

                _logbus.Start();

                Console.WriteLine("Logbus is started");

                Thread.Sleep(Timeout.Infinite);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Something went wrong...");
                Console.WriteLine(ex);
                Environment.Exit(1);
            }
        }

        static void Console_CancelKeyPress(object sender, ConsoleCancelEventArgs e)
        {
            
            Console.WriteLine("Logbus is shutting down");
            try
            {
                _logbus.Dispose();
            }
            catch { }
        }

    }
}
