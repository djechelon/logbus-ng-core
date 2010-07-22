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
using System.Threading;
using It.Unina.Dis.Logbus.Loggers;

namespace RandomMessagePump
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.CancelKeyPress += new ConsoleCancelEventHandler(Console_CancelKeyPress);
            Console.WriteLine("This program is going to submit random log messages to Logbus-ng.");
            Console.WriteLine("You should have already started Logbus daemon, otherwise you will lose messages.");
            Console.WriteLine("Press ENTER when you are ready to begin...");

            Console.ReadLine();

            Console.WriteLine();
            Console.WriteLine("Let's start! Random messages are going to be sent to Logbus-ng. Press CTRL+C when you are finished.");

            try
            {
                ILog the_log = LoggerHelper.CreateDefaultLogger();

                while (true)
                {
                    Random rand = new Random();
                    Thread.Sleep(rand.Next(5000, 15000)); //Random sleep between 5 and 15 seconds

                    switch (rand.Next(0, 10))
                    {
                        case 0:
                            {
                                the_log.Debug("A debug message");
                                break;
                            }
                        case 1:
                            {
                                the_log.Notice("A notice message");
                                break;
                            }
                        case 2:
                            {
                                the_log.Info("An informational message");
                                break;
                            }
                        case 3:
                            {
                                the_log.Warning("A warning message");
                                break;
                            }
                        case 4:
                            {
                                the_log.Error("An error message");
                                break;
                            }
                        case 5:
                            {
                                the_log.Alert("An alert message");
                                break;
                            }
                        case 6:
                            {
                                the_log.Critical("A critical message");
                                break;
                            }
                        case 7:
                            {
                                the_log.Emergency("An emergency message");
                                break;
                            }
                        default:
                            {
                                Console.WriteLine("Not logging this time");
                                break;
                            }
                    }
                }
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
            Environment.Exit(0);
        }

    }
}
