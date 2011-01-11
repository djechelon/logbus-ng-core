using System;
using System.Text;
using It.Unina.Dis.Logbus.Configuration;
using System.Xml.Serialization;
using System.IO;

namespace TestAppConfig
{
    class ConfigSerializer
    {
        public static void Main(string[] args)
        {
            LogbusClientConfiguration config = new LogbusClientConfiguration
                                                   {
                                                       endpoint = new LogbusEndpointDefinition
                                                                      {
                                                                          basePath = "http://localhost:8065",
                                                                          suffix = ".asmx"
                                                                      }
                                                   };
            XmlSerializer seria = new XmlSerializer(typeof(LogbusClientConfiguration), "http://www.dis.unina.it/logbus-ng/configuration/2.0");
            seria.Serialize(Console.Out, config, config.xmlns);
            if (File.Exists("output.txt")) File.Delete("output.txt");
            using (StreamWriter sw = new StreamWriter(new FileStream("output.txt", FileMode.CreateNew), Encoding.UTF8))
            {
                seria.Serialize(sw, config, config.xmlns);
            }


            Console.WriteLine();

            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
        }
    }
}
