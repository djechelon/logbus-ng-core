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

            config.corefilter = new FacilityEqualsFilter() { facility = Facility.Kernel };

            XmlSerializer seria = new XmlSerializer(typeof(LogbusCoreConfiguration));
            seria.Serialize(Console.Out, config);
            using (FileStream fs = new FileStream("output.txt", FileMode.CreateNew))
            {
                seria.Serialize(fs, config);
            }


            Console.WriteLine();

            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
        }
    }
}
