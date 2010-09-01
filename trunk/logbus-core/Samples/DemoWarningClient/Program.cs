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
using It.Unina.Dis.Logbus.Filters;
using It.Unina.Dis.Logbus.Clients;
using System.Threading;
using It.Unina.Dis.Logbus;

namespace DemoWarningClient
{
    class Program
    {
        private static ILogClient logclient;

        static void Main(string[] args)
        {
            SeverityFilter warning_filter = new SeverityFilter()
            {
                comparison = ComparisonOperator.geq,
                severity = Severity.Warning
            };

            Console.WriteLine("This program is going to listen for all messages with severity >= Warning.");
            Console.WriteLine("All received messages will be written on this console in their native format.");

            Console.WriteLine();
            Console.WriteLine("You must start Logbus daemon prior to running this program.");
            Console.WriteLine("Please press ENTER when you are sure that Logbus daemon is already running...");
            Console.ReadLine();

            Console.CancelKeyPress += new ConsoleCancelEventHandler(Console_CancelKeyPress);

            try
            {
                logclient = ClientHelper.CreateDefaultClient(warning_filter);
                logclient.MessageReceived += new EventHandler<SyslogMessageEventArgs>(logclient_MessageReceived);

                logclient.Start();
                Console.WriteLine("Listener is started. Press CTRL+C to exit when you are finished");

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
            //Shut down
            if (logclient != null)
                logclient.Dispose();
            Console.WriteLine("Program shutting down");
            Environment.Exit(0);
        }

        static void logclient_MessageReceived(object sender, It.Unina.Dis.Logbus.SyslogMessageEventArgs e)
        {
            Console.WriteLine(e.Message.ToString());
        }
    }
}
