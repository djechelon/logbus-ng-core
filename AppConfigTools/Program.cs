using System;
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

            Console.WriteLine("Trying to retrieve \"logbus\" section from App.config: {0}", (section is LogbusCoreConfiguration) ? "success" : "fail");
            if (section is LogbusCoreConfiguration)
            {
                //OK, configuration was loaded;
                LogbusCoreConfiguration config = (LogbusCoreConfiguration)section;

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
                    Console.WriteLine("Defined inbound channel of name {0}, type {1}", inch.name, inch.type);
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
                        Console.WriteLine("Transport tag {0}, factory {2}", def.tag, def.factory);
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
