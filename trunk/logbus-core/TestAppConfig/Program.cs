using System;
using System.Configuration;
using System.Text;

namespace TestAppConfig
{
    class Program
    {
        static void Main(string[] args)
        {
            object section = ConfigurationManager.GetSection("logbus");

            Console.WriteLine(section);
            Console.ReadLine();
        }
    }
}
