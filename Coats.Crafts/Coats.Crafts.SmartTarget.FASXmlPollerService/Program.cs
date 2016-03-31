using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;

namespace Coats.Crafts.SmartTarget.FASXmlPollerService
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            log4net.Config.XmlConfigurator.Configure();

            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[] 
			{ 
				new FASXmlPoller() 
			};
            ServiceBase.Run(ServicesToRun);
        }
    }
}
