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
            OutputTransportsConfiguration out_cfg = new OutputTransportsConfiguration();
            config.outtransports = out_cfg;
            out_cfg.outtransport = new OutputTransportDefinition[1];
            out_cfg.outtransport[0] = new OutputTransportDefinition()
            {
                factory = "Unit_Tests.TestClasses.TestTransportFactory, Unit_Tests",
                tag = "test"
            };


            XmlSerializer seria = new XmlSerializer(config.GetType(),"http://www.dis.unina.it/logbus-ng/configuration");
            seria.Serialize(Console.Out, config);
            if (File.Exists("output.txt")) File.Delete("output.txt");
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
