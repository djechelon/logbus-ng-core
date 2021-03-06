﻿using System;
using System.Configuration;
using System.Text;
using It.Unina.Dis.Logbus.Configuration;

namespace TestAppConfig
{
    class Program
    {
        static void Main(string[] args)
        {
            object section = ConfigurationManager.GetSection("logbus-source");

            Console.WriteLine("Trying to retrieve \"logbus\" section from App.config: {0}", (section is LogbusServerConfiguration) ? "success" : "fail");
            if (section is LogbusServerConfiguration)
            {
                //OK, configuration was loaded;
                LogbusServerConfiguration config = (LogbusServerConfiguration)section;

                Console.WriteLine("Parsing core filter");
                if (config.corefilter == null)
                {
                    Console.WriteLine("Core filter is not specified (always true)");
                }
                else
                {
                    Console.WriteLine("Root filter is {0}", config.corefilter.GetType().FullName);
                }
                Console.WriteLine("Core filter parsing end");

                Console.WriteLine();

                Console.WriteLine("Parsing Inbound channels");
                foreach (InboundChannelDefinition inch in config.inchannels)
                {
                    Console.WriteLine("Defined inbound channel of type {0}", inch.type);
                    foreach (KeyValuePair param in inch.param)
                    {
                        Console.WriteLine("Defined parameter {0}={1}", param.name, param.value);
                    }
                }
                Console.WriteLine("Inbound channels end");

                Console.WriteLine();

                Console.WriteLine("Parsing outbound transports");
                if (config.outtransports != null)
                {
                    Console.WriteLine("Factory is {0}", config.outtransports.factory);
                    foreach (OutputTransportDefinition def in config.outtransports.outtransport)
                    {
                        Console.WriteLine("Transport factory {0}", def.factory);
                    }
                    foreach (AssemblyToScan ass in config.outtransports.scanassembly)
                    {
                        Console.WriteLine("Will scan assembly {0} in {1}", ass.assembly, (string.IsNullOrEmpty(ass.codebase)) ? "default paths" : ass.codebase);
                    }
                }
            }
            else
            {
                Console.WriteLine("Sorry, pal");
            }

            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
        }
    }
}
