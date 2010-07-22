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
        private static ILogBus logbus;

        static void Main(string[] args)
        {
            Console.CancelKeyPress += new ConsoleCancelEventHandler(Console_CancelKeyPress);

            Console.WriteLine("Starting the Logbus-ng daemon. You MUST be running this program as Administrator");

            try
            {
                logbus = LogbusSingletonHelper.Instance;
                logbus.MessageReceived += new SyslogMessageEventHandler(logbus_MessageReceived);

                logbus.Start();
                WebServiceActivator.Start(logbus, 8065);

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

        static void logbus_MessageReceived(object sender, SyslogMessageEventArgs e)
        {
            Console.WriteLine("Received message from: {0}, severity: {1}", e.Message.Host, e.Message.Severity);
        }

        static void Console_CancelKeyPress(object sender, ConsoleCancelEventArgs e)
        {
            
            Console.WriteLine("Logbus is shutting down");
            try
            {
                WebServiceActivator.Stop();
                logbus.Dispose();
            }
            catch { }
        }

    }
}
