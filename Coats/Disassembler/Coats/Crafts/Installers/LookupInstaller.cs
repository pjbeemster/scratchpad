namespace Coats.Crafts.Installers
{
    using Castle.MicroKernel.Registration;
    using Castle.MicroKernel.SubSystems.Configuration;
    using Castle.Windsor;
    using GeoIP;
    using System;
    using System.Configuration;
    using System.IO;

    public class LookupInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            string str = ConfigurationManager.AppSettings["GeoData"];
            string data = AppDomain.CurrentDomain.GetData("DataDirectory") as string;
            string path = Path.Combine(data, str);
            if (!File.Exists(path))
            {
                throw new FieldAccessException(string.Format("The file {0} is not there!", path));
            }
            container.Register(new IRegistration[] { Component.For<ILookupService>().ImplementedBy<LookupService>().DependsOn(new Dependency[] { Dependency.OnValue("databaseFile", path), Dependency.OnValue("options", LookupService.GEOIP_MEMORY_CACHE) }) });
        }
    }
}

