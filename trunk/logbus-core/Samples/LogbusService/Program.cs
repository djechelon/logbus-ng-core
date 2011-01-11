using System.ServiceProcess;
using It.Unina.Dis.Logbus.Services;

namespace LogbusService
{
    static class Program
    {
        /// <summary>
        /// Punto di ingresso principale dell'applicazione.
        /// </summary>
        static void Main()
        {
            ServiceBase.Run(new LogbusDaemon());
        }
    }
}
