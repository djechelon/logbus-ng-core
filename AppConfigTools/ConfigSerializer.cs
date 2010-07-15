using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using It.Unina.Dis.Logbus.Configuration;
using It.Unina.Dis.Logbus.Filters;
using System.Xml.Serialization;
using System.Diagnostics;
using System.IO;
using System.Xml;

namespace TestAppConfig
{
    class ConfigSerializer
    {
        public static void Main(string[] args)
        {
            LogbusCoreConfiguration config = new LogbusCoreConfiguration();

            config.corefilter = new FacilityEqualsFilter() { facility = Facility.Security };


            XmlSerializer seria = new XmlSerializer(typeof(LogbusCoreConfiguration), "http://www.dis.unina.it/logbus-ng/configuration");
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
